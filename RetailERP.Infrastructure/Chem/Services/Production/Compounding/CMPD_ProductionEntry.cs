using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Production.Compounding;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using Dapper;
using Microsoft.Data.SqlClient;

namespace RetailERP.Infrastructure.Chem.Services.Marketing
{
    public class CMPD_ProductionEntry : ICMPD_ProductionEntry
    {
        private readonly DapperContext<Prod_CMPD_ProductionMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public CMPD_ProductionEntry(DapperContext<Prod_CMPD_ProductionMaster> context)
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
            Select UOMID As [id], UOM As [text] From bas_UOM Where IsActive = 1; ";
        public async Task<GridEntity<Prod_CMPD_ProductionMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "";

            if (status == Status.PENDING)
            {
                sql = $@"
                    SELECT  
                        pm.POMasterID, pc.POChildID, pm.PONo, pm.PODate, brm.ChemicalCode, brm.BuyerRef,  
                        cprm.ProductName AS FinishedProductName, pc.FinishedProductDescription,
                        ei.EmployeeNo + '-' + ei.EmployeeName AS POByName, ot.OrderType, om.OrderMode,
                        pc.POQty, ISNULL(pa.ProductionQty, 0) AS ProductionQty,
                        ISNULL(pc.POQty, 0) - ISNULL(pa.ProductionQty, 0) AS BalanceQty
                    FROM 
                        mkt_POMaster pm
                        Inner Join mkt_POChild pc ON pc.POMasterID = pm.POMasterID
                        Inner Join mkt_BuyerRefMaster brm ON brm.BuyerRefMasterID = pc.BuyerRefMasterID
                        Inner Join bas_PaymentTerms pt ON pt.PaymentTermsID = pm.PaymentTermsID
                        Inner Join bas_Incoterm i ON i.IncotermID = pm.IncotermID
                        Inner Join bas_CS_ProductMaster cprm ON cprm.ProductID = pc.FinishedProductID
                        Left Join ERPDB..emp_EmploymentInfo ei ON ei.EmployeeID = pm.POBy
                        Left Join bas_OrderType ot ON ot.OrderTypeID = pm.OrderTypeID
                        Left Join bas_OrderMode om ON om.OrderModeID = pm.OrderModeID
                        Left Join 
                        (
                            SELECT POChildID, SUM(ISNULL(ProductionQty, 0)) AS ProductionQty
                            FROM prod_CMPD_ProductionMaster
                            Where IsActive=1 AND Reject = 0
                            GROUP BY POChildID
                        ) pa 
                        ON pa.POChildID = pc.POChildID
                    WHERE 
                        pm.IsActive = 1
                        AND pc.IsDeleted = 0
                        AND brm.IsActive = 1
                        AND pm.Reject = 0
                        AND ISNULL(pa.ProductionQty, 0) < ISNULL(pc.POQty, 0); ";

                orderBy = "POMasterID Desc";
            }
            else if (status == Status.EDIT)
            {
                condition = $@" And prm.Approved = 0 ";
            }
            else if (status == Status.APPROVED)
            {
                condition = $@" And prm.Approved = 1 ";
            }
            else if (status == Status.ALL)
            {
                condition = $@" ";
            }
            if (status != Status.PENDING)
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
                    Where 
	                    pm.IsActive = 1
	                    AND pc.IsDeleted = 0
	                    AND brm.IsActive = 1 
                        AND prm.IsActive = 1
                        AND prm.Reject = 0
                    
                    {condition} ";

                orderBy = "ProductionMasterID Desc";
            }
            return await _context.GetKendoGridDataAsync<Prod_CMPD_ProductionMaster>(options, sql, orderBy, "");
        }
        public async Task<Prod_CMPD_ProductionMaster> GetNew(int id)
        {
            var query =
                $@"  
                -- Master 
                Select 
	                pm.POMasterID, pm.PONo, cpm.ProductName As FinishedProductName, brm.ChemicalCode, pc.FinishedProductDescription, 
                    pc.SizeGram, brm.BuyerRef, c.ColorName, u.UOM, b.BuyerName, pk.PackagingName, 
                    Sum(Isnull(pc.POQty, 0))POQty,pc.POChildID   
                From 
	                mkt_POMaster pm
	                Inner Join mkt_POChild pc On pc.POMasterID = pm.POMasterID
	                Inner Join bas_CS_ProductMaster cpm On cpm.ProductID = pc.FinishedProductID
	                Inner Join bas_UOM u On u.UOMID = pc.UOMID
	                Inner Join bas_PackagingType pk On pk.PackagingTypeID = pc.PackagingTypeID
	                -- Buyer Ref
	                Inner Join mkt_BuyerRefMaster brm On brm.BuyerRefMasterID = pc.BuyerRefMasterID
	                Inner Join mkt_BuyerRefColorInfo brc On brc.BuyerRefColorID = pc.BuyerRefColorID
	                Inner Join bas_Color c On c.ColorID = brc.ColorID
	                Inner Join bas_Buyer b On b.BuyerID = brm.BuyerID 
                Where 
	                pm.IsActive = 1
	                AND pc.IsDeleted = 0
	                AND brm.IsActive = 1
	                AND pc.POChildID = {id} 
                Group By
	                pm.POMasterID, pm.PONo, cpm.ProductName, brm.ChemicalCode, pc.FinishedProductDescription, pc.SizeGram, brm.BuyerRef, 
                    c.ColorName, u.UOM, b.BuyerName, pk.PackagingName,pc.POChildID ; 

                {CommonQuery} ";

            Prod_CMPD_ProductionMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Prod_CMPD_ProductionMaster>();
                if (data != null)
                {
                    data.ProductionByList = records.Read<DropDownList>().ToList();
                    data.ShiftList = records.Read<DropDownList>().ToList();
                    data.ProductList = records.Read<DropDownList>().ToList();
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
        public async Task<Prod_CMPD_ProductionMaster> GetEdit(int id)
        {
            var query = $@"
                -- Master
                Select 
	                prm.*, pm.PONo, cpm.ProductName As FinishedProductName, pc.FinishedProductDescription, pc.POQty,
	                brm.ChemicalCode, brm.BuyerRef, pc.SizeGram, c.ColorName, u.UOM, b.BrandName As BuyerName, pk.PackagingName 
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
	                AND prm.ProductionMasterID = {id}


                -- Childs
                Select 
                    a.*,b.ProductName, c.UOM, pc.ProductCategoryName, pt.ProductTypeName 
                From 
                    prod_CMPD_ProductionChild a
                    join bas_CS_ProductMaster b on a.FinishedProductID=b.ProductID
                    join bas_CS_ProductCategory pc on pc.ProductCategoryID=b.ProductCategoryID
                    join bas_CS_ProductType pt on pt.ProductTypeID=b.ProductTypeID
                    left join bas_UOM c on a.UOMID=c.UOMID
                Where a.ProductionMasterID={id}

                {CommonQuery};  ";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Prod_CMPD_ProductionMaster data = await records.ReadFirstOrDefaultAsync<Prod_CMPD_ProductionMaster>();
                data.Childs = records.Read<Prod_CMPD_ProductionChild>().ToList();

                if (data != null)
                {
                    data.ProductionByList = records.Read<DropDownList>().ToList();
                    data.ShiftList = records.Read<DropDownList>().ToList();
                    data.ProductList = records.Read<DropDownList>().ToList();
                    data.UOMList = records.Read<DropDownList>().ToList();

                    foreach (Prod_CMPD_ProductionChild items in data.Childs)
                    {
                        items.objProductList.id = items.FinishedProductID;
                        items.objProductList.text = items.ProductName;

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
        public async Task<Prod_CMPD_ProductionMaster> SaveAsync(Prod_CMPD_ProductionMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_Prod_CMPD_ProductionChild> TTList = GetChilds(entity.Childs, entity);
                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);

                parameters.Add("@ProductionMasterID", entity.ProductionMasterID);
                parameters.Add("@POMasterID", entity.POMasterID);
                parameters.Add("@POChildID", entity.POChildID);
                parameters.Add("@ProductionDate", entity.ProductionDate);
                parameters.Add("@ProductionBy", entity.ProductionBy);
                parameters.Add("@BatchNo", entity.BatchNo);
                parameters.Add("@LotNo", entity.LotNo);
                // parameters.Add("@StartTime", entity.StartTime);
                // parameters.Add("@EndTime", entity.EndTime);
                parameters.Add("@ShiftID", entity.ShiftID);
                parameters.Add("@ProductionQty", entity.ProductionQty);
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
                entity.NoofRows = await _context.ExecuteAsync("spSet_Prod_CMPD_ProductionMaster", parameters, 30, CommandType.StoredProcedure);
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
        public List<TT_Prod_CMPD_ProductionChild> GetChilds(List<Prod_CMPD_ProductionChild> Childs, Prod_CMPD_ProductionMaster entity)
        {
            List<TT_Prod_CMPD_ProductionChild> TTList = new List<TT_Prod_CMPD_ProductionChild>();
            int Counter = 0;
            foreach (Prod_CMPD_ProductionChild item in Childs)
            {
                ++Counter;
                TT_Prod_CMPD_ProductionChild objItem = new TT_Prod_CMPD_ProductionChild();

                objItem.ProductionChildID = item.ProductionChildID;
                objItem.ProductionMasterID = item.ProductionMasterID;
                objItem.FinishedProductID = item.objProductList.id;
                objItem.UOMID = item.objUOMList.id;
                objItem.Qty = item.Qty;
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
