using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using RetailERP.Core.Chem.Production.Compounding;
using RetailERP.Core.Chem.Production.Packaging;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using Dapper;
using Microsoft.Data.SqlClient;

namespace RetailERP.Infrastructure.Chem.Services.Marketing
{
    public class PACK_ProductionEntryService : IPACK_ProductionEntryService
    {
        private readonly DapperContext<Prod_PACK_ProductionMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public PACK_ProductionEntryService(DapperContext<Prod_PACK_ProductionMaster> context)
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
	            {DBSource.PDLHRM}..emp_EmploymentInfo ei
	            Left Join {DBSource.PDLHRM}..bas_Department d On d.DepartmentID = ei.DepartmentID
	            Left Join {DBSource.PDLHRM}..bas_Section s On s.SectionID = ei.SectionID
	            Left Join {DBSource.PDLHRM}..bas_SubSection ss On ss.SubSectionID = ei.SubSectionID 
            Where 
	            ei.EmpStatusID = 1  AND ei.UnitID = 2
            ORDER BY CONCAT(ei.EmployeeNo, '-', ei.EmployeeName); 

            Select ShiftID As id, ShiftName As [text] From bas_Shift Where IsActive = 1; 

            -- Child
            Select 
	            pc.ProductCategoryID As valueA, pc.ProductCategoryName As valueB, pt.ProductTypeID As valueC, 
	            pt.ProductTypeName As valueD, pm.ProductID As id, pm.ProductName As [text]
            From  
				bas_CS_ProductMaster pm 
	            Inner Join bas_CS_ProductCategory pc On pc.ProductCategoryID = pm.ProductCategoryID  
	            Inner Join bas_CS_ProductType pt On pt.ProductTypeID = pm.ProductTypeID
            Where 
	            pm.IsActive = 1 AND pm.Approved = 1; 

            -- UOM
            Select UOMID As [id], UOM As [text] From bas_UOM Where IsActive = 1; 

            Select PackagingTypeID id, PackagingName [text] from bas_PackagingType where IsActive=1";
        public async Task<GridEntity<Prod_PACK_ProductionMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "";

            if (status == Status.PENDING)
            {
                sql = $@"
                        Select    
                            A.ProductionMasterID, prm.POMasterID, prm.POChildID, pm.PONo, pm.PODate, prm.ProductionDate, 
                            brm.ChemicalCode, brm.BuyerRef, cprm.ProductName AS FinishedProductName,
                            pc.FinishedProductDescription, prm.BatchNo, prm.LotNo, (ei.EmployeeNo + '-' + ei.EmployeeName) AS ProductionByName,
                            prm.ProductionQty, ot.OrderType, om.OrderMode  
                        From 
	                        prod_FILL_ProductionMaster A
	                        INNER JOIN prod_CMPD_ProductionMaster prm  on A.CMPD_ProductionMasterID=prm.ProductionMasterID and prm.IsActive = 1 and  prm.Reject = 0
	                        Inner join mkt_POMaster pm on pm.POMasterID = prm.POMasterID and pm.IsActive = 1 
	                        Inner Join mkt_POChild pc On pc.POMasterID = pm.POMasterID AND pc.POChildID = prm.POChildID AND pc.IsDeleted = 0
	                        Inner Join mkt_BuyerRefMaster brm On brm.BuyerRefMasterID = pc.BuyerRefMasterID AND brm.IsActive = 1 
	                        Inner Join bas_CS_ProductMaster cprm On cprm.ProductID = pc.FinishedProductID
	                        Left Join ERPDB..emp_EmploymentInfo ei On ei.EmployeeID = prm.ProductionBy
	                        Left Join bas_OrderType ot On ot.OrderTypeID = pm.OrderTypeID
	                        Left Join bas_OrderMode om On om.OrderModeID = pm.OrderModeID  
	                        LEFT JOIN prod_PACK_ProductionMaster fill on A.ProductionMasterID=fill.FILL_ProductionMasterID

                        Where    A.Approved = 1   
	                        and fill.ProductionMasterID is null
                        ";

                orderBy = "ProductionMasterID Desc";
            }
            else if (status == Status.EDIT)
            {
                condition = $@" And A.Approved = 0 ";
            }
            else if (status == Status.APPROVED)
            {
                condition = $@" And A.Approved = 1 ";
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

                from Prod_PACK_ProductionMaster A
                    INNER JOIN prod_FILL_ProductionMaster B on A.FILL_ProductionMasterID=B.ProductionMasterID
                    INNER JOIN prod_CMPD_ProductionMaster prm on B.CMPD_ProductionMasterID=prm.ProductionMasterID
                    Inner join mkt_POMaster pm on pm.POMasterID = prm.POMasterID
                    Inner Join mkt_POChild pc On pc.POMasterID = pm.POMasterID AND pc.POChildID = prm.POChildID
                    Inner Join mkt_BuyerRefMaster brm On brm.BuyerRefMasterID = pc.BuyerRefMasterID
                    Inner Join bas_CS_ProductMaster cprm On cprm.ProductID = pc.FinishedProductID
                    Left Join ERPDB..emp_EmploymentInfo ei On ei.EmployeeID = A.ProductionBy
                    Left Join bas_OrderType ot On ot.OrderTypeID = pm.OrderTypeID
                    Left Join bas_OrderMode om On om.OrderModeID = pm.OrderModeID  
                where 1=1 {condition} ";

                orderBy = "ProductionMasterID Desc";
            }
            return await _context.GetKendoGridDataAsync<Prod_PACK_ProductionMaster>(options, sql, orderBy, "");
        }
        public async Task<Prod_PACK_ProductionMaster> GetNew(int id)
        {
            var query =
                $@"  
                -- Master 
                Select 
	                A.ProductionMasterID as FILL_ProductionMasterID,prm.LotNo, prm.BatchNo, pm.PONo, cpm.ProductName As FinishedProductName, pc.FinishedProductDescription, pc.POQty,
	                brm.ChemicalCode, brm.BuyerRef, pc.SizeGram, c.ColorName, u.UOM, b.BrandName As BuyerName, pk.PackagingName,
	                prm.ProductionMasterID CMPD_ProductionMasterID,prm.ProductionQty
                From 
                    prod_FILL_ProductionMaster A
	                INNER JOIN prod_CMPD_ProductionMaster prm on A.CMPD_ProductionMasterID=prm.ProductionMasterID
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
	                AND A.ProductionMasterID  = {id} ; 

                Select 
                     c.UOM,pc.PackagingName,A.UOMID,A.PackagingTypeID,A.FillingQty,A.SizeGram
                From   prod_FILL_ProductionChild a
                    INNER join bas_PackagingType pc on A.PackagingTypeID=pc.PackagingTypeID
                    left join bas_UOM c on a.UOMID=c.UOMID 
                Where a.ProductionMasterID={id};
                {CommonQuery} ";

            Prod_PACK_ProductionMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Prod_PACK_ProductionMaster>();
                data.Childs = records.Read<Prod_PACK_ProductionChild>().ToList();

                if (data != null)
                {
                    data.ProductionByList = records.Read<DropDownList>().ToList();
                    data.ShiftList = records.Read<DropDownList>().ToList();
                    data.ProductList = records.Read<DropDownList>().ToList();
                    data.UOMList = records.Read<DropDownList>().ToList();
                    data.PackagingTypeList = records.Read<DropDownList>().ToList();
                }

                foreach (Prod_PACK_ProductionChild items in data.Childs)
                {
                    items.objUOMList.id = items.UOMID;
                    items.objUOMList.text = items.UOM;
                    items.objPackagingTypeList.id = items.PackagingTypeID;
                    items.objPackagingTypeList.text = items.PackagingName;
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
        public async Task<Prod_PACK_ProductionMaster> GetEdit(int id)
        {
            var query =
               $@"
                -- Master
               Select 
	                A.*, pm.PONo, cpm.ProductName As FinishedProductName, pc.FinishedProductDescription, pc.POQty,
	                brm.ChemicalCode, brm.BuyerRef, pc.SizeGram, c.ColorName, u.UOM, b.BrandName As BuyerName, pk.PackagingName 
                From 
	                Prod_PACK_ProductionMaster  A 
	                INNER JOIN prod_FILL_ProductionMaster prm on A.FILL_ProductionMasterID=prm.ProductionMasterID
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
	                pm.IsActive = 1 AND pc.IsDeleted = 0 AND brm.IsActive = 1
	                AND A.ProductionMasterID = {id} ;

                -- Childs
                Select 
                     a.*,c.UOM,pc.PackagingName
                From   Prod_PACK_ProductionChild a
                       INNER join bas_PackagingType pc on A.PackagingTypeID=pc.PackagingTypeID
                       left join bas_UOM c on a.UOMID=c.UOMID 
                Where a.ProductionMasterID={id};

                {CommonQuery}; 
 
            ";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Prod_PACK_ProductionMaster data = await records.ReadFirstOrDefaultAsync<Prod_PACK_ProductionMaster>();
                data.Childs = records.Read<Prod_PACK_ProductionChild>().ToList();

                if (data != null)
                {
                    data.ProductionByList = records.Read<DropDownList>().ToList();
                    data.ShiftList = records.Read<DropDownList>().ToList();
                    data.ProductList = records.Read<DropDownList>().ToList();
                    data.UOMList = records.Read<DropDownList>().ToList();
                    data.PackagingTypeList = records.Read<DropDownList>().ToList();

                    foreach (Prod_PACK_ProductionChild items in data.Childs)
                    {
                        items.objUOMList.id = items.UOMID;
                        items.objUOMList.text = items.UOM;
                        items.objPackagingTypeList.id = items.PackagingTypeID;
                        items.objPackagingTypeList.text = items.PackagingName;
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
        public async Task<Prod_PACK_ProductionMaster> SaveAsync(Prod_PACK_ProductionMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_Prod_PACK_ProductionChild> TTList = GetChilds(entity.Childs, entity);
                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@ProductionMasterID", entity.ProductionMasterID);
                parameters.Add("@ProductionDate", entity.ProductionDate);
                parameters.Add("@ProductionBy", entity.ProductionBy);
                parameters.Add("@LotNo", entity.LotNo);
                parameters.Add("@BatchNo", entity.BatchNo);
                parameters.Add("@ProductionQty", entity.ProductionQty);
                parameters.Add("@ShiftID", entity.ShiftID);
                parameters.Add("@FILL_ProductionMasterID", entity.FILL_ProductionMasterID);
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
                entity.NoofRows = await _context.ExecuteAsync("spSet_Prod_PACK_Production", parameters, 30, CommandType.StoredProcedure);
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
        public List<TT_Prod_PACK_ProductionChild> GetChilds(List<Prod_PACK_ProductionChild> Childs, Prod_PACK_ProductionMaster entity)
        {
            List<TT_Prod_PACK_ProductionChild> TTList = new List<TT_Prod_PACK_ProductionChild>();
            int Counter = 0;
            foreach (Prod_PACK_ProductionChild item in Childs)
            {
                ++Counter;
                TT_Prod_PACK_ProductionChild objItem = new TT_Prod_PACK_ProductionChild();

                objItem.ProductionChildID = item.ProductionChildID;
                objItem.ProductionMasterID = item.ProductionMasterID;
                objItem.FillProductionMasterID = item.FillProductionMasterID;
                objItem.FillProductionChildID = item.FillProductionChildID;
                objItem.PackagingTypeID = item.objPackagingTypeList.id;
                objItem.SizeGram = item.SizeGram;
                objItem.UOMID = item.objUOMList.id;
                objItem.FillingQty = item.FillingQty;
                objItem.PackQty = item.PackQty;
                objItem.RejectQty = item.RejectQty;
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
