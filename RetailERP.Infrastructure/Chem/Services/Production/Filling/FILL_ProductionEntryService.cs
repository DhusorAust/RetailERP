using RetailERP.Core.Chem.Production.Compounding;
using RetailERP.Core.Chem.Production.Filling;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager; 
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.Marketing
{   
    public class FILL_ProductionEntryService : IFILL_ProductionEntryService
    {
        private readonly DapperContext<Prod_FILL_ProductionMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public FILL_ProductionEntryService(DapperContext<Prod_FILL_ProductionMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        private string CommonQuery = $@"
            -- Entry Person
            SELECT 
	            CAST(ei.EmployeeID AS VARCHAR) id, CONCAT(ei.EmployeeNo, '-', ei.EmployeeName) [text],
	            ei.DepartmentID As valueA, d.DepartmentName As valueD, ei.SectionID As valueB, s.SectionName As valueE, ei.SubSectionID As valueC, 
	            ss.SubSectionName As valueF 
            FROM 
	            ERPDB..emp_EmploymentInfo ei
	            Left Join ERPDB..bas_Department d On d.DepartmentID = ei.DepartmentID
	            Left Join ERPDB..bas_Section s On s.SectionID = ei.SectionID
	            Left Join ERPDB..bas_SubSection ss On ss.SubSectionID = ei.SubSectionID 
            Where 
	            ei.EmpStatusID = 1  AND ei.UnitID = 2
            ORDER BY CONCAT(ei.EmployeeNo, '-', ei.EmployeeName); 

            Select ShiftID As id, ShiftName As [text] From bas_Shift Where IsActive = 1;  

            -- Child
            Select PackagingTypeID As id, PackagingName As [text] From bas_PackagingType Where IsActive = 1;
             -- UOM
            Select UOMID As [id], UOM As [text] From bas_UOM Where IsActive = 1; ";
        public async Task<GridEntity<Prod_FILL_ProductionMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "";

            if (status == Status.PENDING)
            {
                sql = $@"
                    Select  
                        prm.ProductionMasterID, prm.POMasterID, prm.POChildID, pm.PONo, pm.PODate, prm.ProductionDate, 
                        brm.ChemicalCode, brm.BuyerRef, cprm.ProductName AS FinishedProductName,
                        pc.FinishedProductDescription, prm.BatchNo, prm.LotNo, (ei.EmployeeNo + '-' + ei.EmployeeName) AS ProductionByName,
                        prm.ProductionQty, ot.OrderType, om.OrderMode  
                    From 
	                    prod_CMPD_ProductionMaster prm 
	                    Inner join mkt_POMaster pm on pm.POMasterID = prm.POMasterID
	                    Inner Join mkt_POChild pc On pc.POMasterID = pm.POMasterID AND pc.POChildID = prm.POChildID
	                    Inner Join mkt_BuyerRefMaster brm On brm.BuyerRefMasterID = pc.BuyerRefMasterID
	                    Inner Join bas_CS_ProductMaster cprm On cprm.ProductID = pc.FinishedProductID
	                    Left Join ERPDB..emp_EmploymentInfo ei On ei.EmployeeID = prm.ProductionBy
	                    Left Join bas_OrderType ot On ot.OrderTypeID = pm.OrderTypeID
	                    Left Join bas_OrderMode om On om.OrderModeID = pm.OrderModeID  
	                    LEFT JOIN prod_FILL_ProductionMaster fill on prm.ProductionMasterID=fill.CMPD_ProductionMasterID

                    Where 
	                    pm.IsActive = 1
	                    AND pc.IsDeleted = 0
	                    AND brm.IsActive = 1 
                        AND prm.IsActive = 1
                        AND prm.Reject = 0
                        And prm.Approved = 1  
                        and fill.ProductionMasterID is null ";

                orderBy = "ProductionMasterID Desc";
            }
            else if (status == Status.EDIT)
            {
                condition = $@" And A.Approved = 0 And A.Reject = 0 ";
            }
            else if (status == Status.PROPOSED_FOR_APPROVAL)
            {
                condition = $@"  And A.Approved = 0 And A.Reject = 0 ";
            }
            else if (status == Status.APPROVED)
            {
                condition = $@" And A.Approved = 1 And A.Reject = 0 ";
            }
            else if (status == Status.REJECT)
            {
                condition = $@" A.Reject = 1 ";
            }
            else if (status == Status.ALL)
            {
                condition = $@" ";
            }
            if (status != Status.PENDING)
            {
                sql = $@"
                Select 
                        A.*,pm.PONo, pm.PODate, 
                        brm.ChemicalCode, brm.BuyerRef, cprm.ProductName AS FinishedProductName,
                        pc.FinishedProductDescription, (ei.EmployeeNo + '-' + ei.EmployeeName) AS ProductionByName,ot.OrderType, om.OrderMode  

                from prod_FILL_ProductionMaster A
                     INNER JOIN prod_CMPD_ProductionMaster prm on A.CMPD_ProductionMasterID=prm.ProductionMasterID
                     Inner join mkt_POMaster pm on pm.POMasterID = prm.POMasterID
                     Inner Join mkt_POChild pc On pc.POMasterID = pm.POMasterID AND pc.POChildID = prm.POChildID
                     Inner Join mkt_BuyerRefMaster brm On brm.BuyerRefMasterID = pc.BuyerRefMasterID
                     Inner Join bas_CS_ProductMaster cprm On cprm.ProductID = pc.FinishedProductID
                     Left Join ERPDB..emp_EmploymentInfo ei On ei.EmployeeID = A.ProductionBy
                     Left Join bas_OrderType ot On ot.OrderTypeID = pm.OrderTypeID
                     Left Join bas_OrderMode om On om.OrderModeID = pm.OrderModeID  
	             where A.IsActive = 1 {condition} ";

                orderBy = "ProductionMasterID Desc";
            }
            return await _context.GetKendoGridDataAsync<Prod_FILL_ProductionMaster>(options, sql, orderBy, "");
        }
        public async Task<Prod_FILL_ProductionMaster> GetNew(int id)
        {
            var query = $@"  
                -- Master
                Select 
	                prm.LotNo, prm.BatchNo, pm.PONo, cpm.ProductName As FinishedProductName, pc.FinishedProductDescription, pc.POQty,
	                brm.ChemicalCode, brm.BuyerRef, pc.SizeGram, c.ColorName, u.UOM, b.BrandName As BuyerName, pk.PackagingName,
	                prm.ProductionMasterID CMPD_ProductionMasterID,prm.ProductionQty
                From 
	                prod_CMPD_ProductionMaster prm
	                Inner Join mkt_POMaster pm On pm.POMasterID = prm.POMasterID
	                Inner Join mkt_POChild pc On pc.POMasterID = pm.POMasterID AND pc.POChildID = prm.POChildID
	                Inner Join bas_CS_ProductMaster cpm On cpm.ProductID = pc.FinishedProductID
	                Inner Join mkt_BuyerRefMaster brm On brm.BuyerRefMasterID = pc.BuyerRefMasterID
	                Inner Join mkt_BuyerRefColorInfo brc On brc.BuyerRefColorID = pc.BuyerRefColorID
	                Inner Join bas_Color c On c.ColorID = brc.ColorID
	                Inner Join bas_UOM u On u.UOMID = pc.UOMID
	                Inner Join bas_Brand b On b.BrandID = brm.BuyerID 
	                Inner Join bas_PackagingType pk On pk.PackagingTypeID = pc.PackagingTypeID 
                Where 
	                pm.IsActive = 1
	                AND pc.IsDeleted = 0
	                AND brm.IsActive = 1
	                AND prm.ProductionMasterID  = {id}

                {CommonQuery} ";

            Prod_FILL_ProductionMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Prod_FILL_ProductionMaster>(); 
                if(data != null)
                {
                    data.ProductionByList = records.Read<DropDownList>().ToList();
                    data.ShiftList = records.Read<DropDownList>().ToList();
                    data.PackagingTypeList = records.Read<DropDownList>().ToList();
                    data.UOMList = records.Read<DropDownList>().ToList(); 
                }
                
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
        }
        public async Task<Prod_FILL_ProductionMaster> GetEdit(int id)
        {
            var query = $@"
                -- Master
                 Select 
	                prm.*, pm.PONo, cpm.ProductName As FinishedProductName, pc.FinishedProductDescription, pc.POQty,
	                brm.ChemicalCode, brm.BuyerRef, pc.SizeGram, c.ColorName, u.UOM, b.BrandName As BuyerName, pk.PackagingName 
                From 
	                prod_FILL_ProductionMaster prm
	                Inner Join prod_CMPD_ProductionMaster cmp On prm.CMPD_ProductionMasterID = cmp.ProductionMasterID
	                Inner Join mkt_POMaster pm On pm.POMasterID = cmp.POMasterID
	                Inner Join mkt_POChild pc On pc.POMasterID = pm.POMasterID AND pc.POChildID = cmp.POChildID
	                Inner Join bas_CS_ProductMaster cpm On cpm.ProductID = pc.FinishedProductID
	                Inner Join mkt_BuyerRefMaster brm On brm.BuyerRefMasterID = pc.BuyerRefMasterID
	                Inner Join mkt_BuyerRefColorInfo brc On brc.BuyerRefColorID = pc.BuyerRefColorID
	                Inner Join bas_Color c On c.ColorID = brc.ColorID
	                Inner Join bas_UOM u On u.UOMID = pc.UOMID
	                Inner Join bas_Brand b On b.BrandID = brm.BuyerID 
	                Inner Join bas_PackagingType pk On pk.PackagingTypeID = pc.PackagingTypeID 
                Where 
	                pm.IsActive = 1
	                AND pc.IsDeleted = 0
	                AND brm.IsActive = 1
	                AND prm.ProductionMasterID = {id} ;

                -- Childs
                Select 
                     a.*,c.UOM,pc.PackagingName
                From   prod_FILL_ProductionChild a
                       INNER join bas_PackagingType pc on A.PackagingTypeID=pc.PackagingTypeID
                       left join bas_UOM c on a.UOMID=c.UOMID 
                Where a.ProductionMasterID={id}

                {CommonQuery};  ";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Prod_FILL_ProductionMaster data = await records.ReadFirstOrDefaultAsync<Prod_FILL_ProductionMaster>();
                data.Childs = records.Read<Prod_FILL_ProductionChild>().ToList();

                if (data != null)
                {
                    data.ProductionByList = records.Read<DropDownList>().ToList();
                    data.ShiftList = records.Read<DropDownList>().ToList();
                    data.PackagingTypeList = records.Read<DropDownList>().ToList();
                    data.UOMList = records.Read<DropDownList>().ToList();

                    foreach (Prod_FILL_ProductionChild items in data.Childs)
                    {
                        items.objPackagingTypeList.id = items.PackagingTypeID;
                        items.objPackagingTypeList.text = items.PackagingName;

                        items.objUOMList.id = items.UOMID;
                        items.objUOMList.text = items.UOM;
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
        }
        public async Task<Prod_FILL_ProductionMaster> SaveAsync(Prod_FILL_ProductionMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_Prod_FILL_ProductionChild> TTList = GetChilds(entity.Childs, entity);
                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);

                parameters.Add("@ProductionMasterID", entity.ProductionMasterID);
                parameters.Add("@ProductionDate", entity.ProductionDate);
                parameters.Add("@StartTime", entity.StartTime);
                parameters.Add("@EndTime", entity.EndTime);
                parameters.Add("@LotNo", entity.LotNo); 
                parameters.Add("@BatchNo", entity.BatchNo);
                parameters.Add("@ProductionBy", entity.ProductionBy);
                parameters.Add("@ProductionQty", entity.ProductionQty);
                parameters.Add("@ShiftID", entity.ShiftID);
                parameters.Add("@CMPD_ProductionMasterID", entity.CMPD_ProductionMasterID);
                parameters.Add("@Remarks", entity.Remarks);  
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@ReviseNo", entity.ReviseNo);
                parameters.Add("@ReviseReason", entity.ReviseReason);
                parameters.Add("@Reject", entity.Reject);
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@UserBy", entity.UserBy);

                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@Message", dbType: DbType.String, size: -1, direction: ParameterDirection.Output);

                entity.NoofRows = await _context.ExecuteAsync("spSet_Prod_FILL_Production", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");
                entity.Message = parameters.Get<string>("@Message");
                if (entity.NoofRows > 0 && entity.ResultId > 0)
                {
                    entity.Message = "Save operation successful.";
                }
                else
                {
                    entity.Message = entity.Message + "Save operation failed.";
                }
            }
            catch (Exception ex)
            {
                entity.Message = ex.InnerException.ToString();
            }
            finally
            {
                parameters = null;
            }
            return entity;
        }
        public List<TT_Prod_FILL_ProductionChild> GetChilds(List<Prod_FILL_ProductionChild> Childs, Prod_FILL_ProductionMaster entity)
        {
            List<TT_Prod_FILL_ProductionChild> TTList = new List<TT_Prod_FILL_ProductionChild>();
            int Counter = 0;
            foreach (Prod_FILL_ProductionChild item in Childs)
            {
                ++Counter;
                TT_Prod_FILL_ProductionChild objItem = new TT_Prod_FILL_ProductionChild();

                objItem.ProductionChildID = item.ProductionChildID;
                objItem.ProductionMasterID = item.ProductionMasterID;
                objItem.PackagingTypeID = item.objPackagingTypeList.id;
                objItem.SizeGram = item.SizeGram;
                objItem.UOMID = item.objUOMList.id;
                objItem.FillingQty = item.FillingQty;
                objItem.Remarks = item.Remarks;
                objItem.RowSLNo = item.RowSLNo;
                objItem.IsDeleted = item.IsDeleted;

                TTList.Add(objItem);
            }
            return TTList;
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
