using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.ChemicalStore;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.ChemicalStore
{
    public class IssueService : IIssueService
    {
        private readonly DapperContext<CS_IssueMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public IssueService(DapperContext<CS_IssueMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@"
            -- StoreList
            Select Cast(StoreID As varchar) id, StoreName as [text] 
            From bas_Store
            Where IsActive = 1 And StoreID In(1);
            -- UnitList
            Select Cast(UnitID As varchar) id,UnitName as [text] 
            From bas_UnitInfo
            Where IsActive = 1 And Approved = 1 And UnitID In(1);
            -- IssueTypeList
            Select ISSUID as id,ISSUTYPE as [text] From PDLERP..F_BAS_ISSUE_TYPE;
            -- SectionList
            Select SectionID as id,SectionName as [text] From ERPDB..bas_Section;

            -- IssuedByList
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

            -- Product
            SELECT
	            ProductID AS id, ProductName AS [text], pm.UOMID As valueA, u.UOM As valueB
            FROM 
	            bas_CS_ProductMaster pm
	            Left Join bas_UOM u On u.UOMID = pm.UOMID
            Where 
	            pm.IsActive = 1
            ORDER BY ProductName;

            -- Child_UOMList
            Select UOMID as id,UOM as [text] From   bas_UOM;
            ";
        public async Task<GridEntity<CS_IssueMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "Where 1 = 1";

            sql = $@"
                    Select 
                    --A.*
                    A.IssueMasterID,A.UnitID,A.StoreID,A.IssueDate,A.IssueNo,A.IssueTypeID,A.RequirementMasterID,A.DepartmentID,A.SectionID,A.SubSectionID,
                    D.UnitName,F.DepartmentName,G.SectionName,H.SubSectionName,E.StoreName,C.ISSUTYPE As IssueTypeName,I.EmployeeName As IssuedByName
					,B.RequirementNo
                    From CS_IssueMaster A
                    Inner Join CS_RequirementMaster B On A.RequirementMasterID = B.RequirementMasterID
                    Inner Join PDLERP..F_BAS_ISSUE_TYPE C On A.IssueTypeID = C.ISSUID
                    Inner Join bas_UnitInfo D On A.UnitID = D.UnitID
                    Inner Join bas_Store E On A.StoreID = E.StoreID
                    Inner Join {DBSource.PDLHRM}..bas_Department F On A.DepartmentID = F.DepartmentID
                    Left Join {DBSource.PDLHRM}..bas_Section G On A.SectionID = G.SectionID
                    Left Join {DBSource.PDLHRM}..bas_SubSection H On A.SubSectionID = H.SubSectionID
                    Left Join {DBSource.PDLHRM}..emp_EmploymentInfo I On A.IssuedBy = I.EmployeeID
                    And A.IsActive = 1
                ";
            orderBy = "IssueMasterID Desc";
            if (status == Status.PENDING)
            {
                sql = $@"
                    Select
	                    A.RequirementMasterID,A.UnitID,A.StoreID,A.RequirementNo,A.RequirementDate,
	                    B.UnitName,C.StoreName,D.EmployeeName As RequirementByName,E.DepartmentName,F.SectionName,G.SubSectionName
                    From CS_RequirementMaster A
                    Inner Join bas_UnitInfo B On A.UnitID = B.UnitID
                    Inner Join bas_Store C On A.StoreID = C.StoreID
                    Inner Join {DBSource.PDLHRM}..emp_EmploymentInfo D On A.RequirementBy = D.EmployeeID
                    Inner Join {DBSource.PDLHRM}..bas_Department E On A.DepartmentID = E.DepartmentID
                    Inner Join {DBSource.PDLHRM}..bas_Section F On A.SectionID = F.SectionID
					Left Join {DBSource.PDLHRM}..bas_SubSection G On A.SubSectionID = G.SubSectionID
                    Where A.IsActive = 1
                    -- And A.Approved = 1 And A.Reject = 0
                    ";
                condition = "";
                orderBy = "RequirementMasterID Desc";

            }
            else if (status == Status.EDIT)
            {
                condition = condition + $@" And A.SendForApproval = 0 And A.Approved = 0 And A.Reject = 0 ";
            }
            else if (status == Status.PROPOSED_FOR_APPROVAL)
            {
                condition = condition + $@" And A.SendForApproval = 1 And A.Approved = 0 And A.Reject = 0 ";
            }
            else if (status == Status.APPROVED)
            {
                condition = condition + $@" And A.SendForApproval = 1 And A.Approved = 1 And A.Reject = 0 ";
            }
            else if (status == Status.REJECT)
            {
                condition = condition + $@" And A.Reject = 1 ";
            }
            else if (status == Status.ALL)
            {
                condition = $@" ";
            }

            return await _context.GetKendoGridDataAsync<CS_IssueMaster>(options, sql + " " + condition, orderBy, "");
        }
        public async Task<CS_IssueMaster> GetNew(string Ids, int? MenuID, int? UnitID, int? UserID)
        {
            var query =
                $@"  
                -- Master
                Select
                    A.*,
                    B.DepartmentName,C.SectionName,D.SubSectionName,E.EmployeeName As RequirementByName
                From CS_RequirementMaster A
                Left Join ERPDB..bas_Department B On A.DepartmentID = B.DepartmentID
                Left Join ERPDB..bas_Section C On A.SectionID = C.SectionID
                Left Join ERPDB..bas_SubSection D On A.SubSectionID = D.SubSectionID
                Left Join ERPDB..emp_EmploymentInfo E On A.RequirementBy = E.EmployeeID
                Where A.RequirementMasterID In({Ids});

                -- Childs
                Select
                0 As IssueDetailID,0 As IssueMasterID,Null As IssueDate,D.IndentMasterID,D.IndentChildID,A.RequirementChildID,A.ReceiveChildID,A.SectionID,A.ProductID,A.UOMID,A.RequirementQty,A.RequirementQty As IssueQty,Null As ChildRemarks,0 As RowSLNo
                ,F.UOM,E.ProductName,G.SectionName
                From CS_RequirementChild A
                Inner Join CS_RequirementMaster B On A.RequirementMasterID = B.RequirementMasterID
                Inner Join CS_ReceiveChild C On A.ReceiveChildID = C.ReceiveChildID
                Inner Join CS_IndentChild D On C.IndentChildID = D.IndentChildID
                Inner Join bas_CS_ProductMaster E On A.ProductID = E.ProductID
				Inner Join bas_UOM F On A.UOMID = F.UOMID				
				Inner Join {DBSource.PDLHRM}..bas_Section G On A.SectionID = G.SectionID
                where B.RequirementMasterID In({Ids})
                Order By A.RowSLNo;  
                {CommonQuery} ";
            CS_IssueMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<CS_IssueMaster>();
                data.Childs = records.Read<CS_IssueChild>().ToList();
                //data.ReceiveChildList = records.Read<DropDownList>().ToList();

                // CommonQuery
                data.StoreList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.IssueTypeList = records.Read<DropDownList>().ToList();
                data.SectionList = records.Read<DropDownList>().ToList();
                data.IssuedByList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();
                data.UOMList = records.Read<DropDownList>().ToList();

                foreach (CS_IssueChild items in data.Childs)
                {
                    items.objProductList.id = items.ProductID;
                    items.objProductList.text = items.ProductName;
                    items.objUOMList.id = items.UOMID;
                    items.objUOMList.text = items.UOM;
                }


                //// DELETE START
                data.IssuedBy = 429;
                data.IssueTypeID = 300000;
                //// DELETE End

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
        public async Task<CS_IssueMaster> GetEdit(int id, int? MenuID, int? UnitID, int? UserID)
        {

            var query =
                $@" 
                -- Master
                Select
                A.*
                ,B.RequirementNo,B.RequirementDate,B.RequirementBy,G.EmployeeName As RequirementByName
                ,C.DepartmentName,D.SectionName,E.SubSectionName,F.EmployeeName As IssuedByName
                From CS_IssueMaster A
                Inner Join CS_RequirementMaster B On A.RequirementMasterID = B.RequirementMasterID
                Left Join ERPDB.dbo.bas_Department C On A.DepartmentID  = C.DepartmentID
                Left Join ERPDB.dbo.bas_Section D On A.SectionID = D.SectionID
                Left Join ERPDB.dbo.bas_SubSection E On A.SubSectionID = E.SubSectionID
                Left Join ERPDB.dbo.emp_EmploymentInfo F On A.IssuedBy = F.EmployeeID
				Left Join ERPDB.dbo.emp_EmploymentInfo G On B.RequirementBy = G.EmployeeID
                Where IssueMasterID = {id};

                -- Details
                Select 
                A.*
                ,F.IndentNo,G.SectionName
                ,B.ProductName
                ,C.BatchNo,D.UOMID,D.UOM
                ,E.RequirementQty
                ,H.valueC As StockQty
				,H.valueE As ReceiveChildName
                From CS_IssueChild A
				Inner Join bas_CS_ProductMaster B On A.ProductID = B.ProductID
				Inner Join CS_ReceiveChild C On A.ReceiveChildID = C.ReceiveChildID
				Inner Join bas_UOM D On A.UOMID = D.UOMID
				Inner Join CS_RequirementChild E On A.RequirementChildID = E.RequirementChildID
				Inner Join CS_IndentMaster F On A.IndentMasterID = F.IndentMasterID
                Inner Join dbo.fn_CS_Stock_Item_List({MenuID},{UnitID},{UserID},{id}) H On A.ReceiveChildID = H.id
				Left Join ERPDB..bas_Section G On A.SectionID = G.SectionID				
				Left Join CS_IndentMaster I On A.IndentMasterID = I.IndentMasterID
				Where A.IssueMasterID = {id}
                Order By A.RowSLNo Asc;

                {CommonQuery} ";

            CS_IssueMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<CS_IssueMaster>();
                data.Childs = records.Read<CS_IssueChild>().ToList();

                // CommonQuery
                data.StoreList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.IssueTypeList = records.Read<DropDownList>().ToList();
                data.SectionList = records.Read<DropDownList>().ToList();
                data.IssuedByList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();
                data.UOMList = records.Read<DropDownList>().ToList();

                foreach (CS_IssueChild items in data.Childs)
                {
                    items.objProductList.id = items.ProductID;
                    items.objProductList.text = items.ProductName;
                    items.objUOMList.id = items.UOMID;
                    items.objUOMList.text = items.UOM;
                    items.objReceiveChildList.id = items.ReceiveChildID;
                    items.objReceiveChildList.text = items.ReceiveChildName;
                    items.IndentNo = items.IndentNo;
                    items.BatchNo = items.BatchNo;
                    items.StockQty = items.StockQty;
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
        public async Task<CS_IssueMaster> SaveAsync(CS_IssueMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_CS_IssueChild> TTList = GetChild(entity.Childs);
                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@IssueMasterID", entity.IssueMasterID);
                parameters.Add("@UnitID", entity.UnitID);
                parameters.Add("@StoreID", entity.StoreID);
                parameters.Add("@IssueDate", entity.IssueDate);
                parameters.Add("@IssueNo", entity.IssueNo);
                parameters.Add("@IssueTypeID", entity.IssueTypeID);
                parameters.Add("@RequirementMasterID", entity.RequirementMasterID);
                parameters.Add("@DepartmentID", entity.DepartmentID);
                parameters.Add("@SectionID", entity.SectionID);
                parameters.Add("@SubSectionID", entity.SubSectionID);
                parameters.Add("@IssuedBy", entity.IssuedBy);
                parameters.Add("@IssueRemarks", entity.IssueRemarks);
                parameters.Add("@IssueToLocation", entity.IssueToLocation);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@IsReturnable", entity.IsReturnable);
                parameters.Add("@SendForApproval", entity.SendForApproval);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@Acknowledge", entity.Acknowledge);
                parameters.Add("@Reject", entity.Reject);
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync("spSet_CS_IssueMaster", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");

                if (entity.NoofRows > 0 && entity.ResultId > 0)
                {
                    entity.Message = "Save operation successful.";
                }
                else
                {
                    entity.Message = "Save operation failed.";
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
        public List<TT_CS_IssueChild> GetChild(List<CS_IssueChild> Childs)
        {
            List<TT_CS_IssueChild> TTList = new List<TT_CS_IssueChild>();

            int Counter = 0;

            foreach (CS_IssueChild item in Childs)
            {
                ++Counter;
                TT_CS_IssueChild objItem = new TT_CS_IssueChild();
                objItem.IssueDetailID = item.IssueDetailID;
                objItem.IssueMasterID = item.IssueMasterID;
                objItem.IssueDate = item.IssueDate;
                objItem.IndentMasterID = item.IndentMasterID;
                objItem.IndentChildID = item.IndentChildID;
                objItem.RequirementChildID = item.RequirementChildID;
                objItem.ReceiveChildID = item.objReceiveChildList.id;
                objItem.SectionID = item.SectionID;
                objItem.ProductID = item.ProductID;
                objItem.UOMID = item.UOMID;
                objItem.IssueQty = item.IssueQty;
                objItem.ChildRemarks = item.ChildRemarks;
                objItem.RowSLNo = item.RowSLNo;
                objItem.RowSLNoNew = Counter;
                TTList.Add(objItem);
            }
            return TTList;
        }

        public async Task<List<DropDownList>> GetProductBySearch(string searchKey, string itemName, int? menuID, int? unitID, int? userID, int? primaryKeyID, string whereClause, int pageIndex, int pageSize)
        {
            var condition = "";
            var query = "";
            var search = "";
            var orderBy = " order by a.text\r\n";

            if (!string.IsNullOrEmpty(whereClause))
            {
                condition = " and a.ValueD = " + whereClause;
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                itemName = "";
                search = searchKey;
                condition = condition + "  and (a.text LIKE '%" + search + "%')";
            }
            else if (!string.IsNullOrEmpty(itemName))
            {
                search = itemName;
            }

            if (!string.IsNullOrEmpty(search)) { }

            query = $@"
            select a.* from
            (
                Select id,valueE As [text],valueB,valueC,valueI,valueH,ValueD,ValueJ From dbo.fn_CS_Stock_Item_List({menuID},{unitID},{userID},{primaryKeyID}) A
				Inner Join CS_RequirementChild B On Convert(Int,A.valueD) = B.ProductID
				-- Where B.RequirementMasterID In(1) And IsNull(B.IsDeleted,0) = 0
				Where IsNull(B.IsDeleted,0) = 0
				Group By id,valueE,valueB,valueC,valueI,valueH,ValueD,ValueJ
            )a
            Where 1=1 {condition}{orderBy}
            OFFSET
            (
                {pageSize} * ({pageIndex} - 1)
            ) ROWS FETCH NEXT {pageSize} ROWS ONLY
            ;";

            var dataList = await _context.GetDataAsync<DropDownList>(query);
            return dataList;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
