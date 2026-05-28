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
    public class RequirementService : IRequirementService
    {
        private readonly DapperContext<CS_RequirementMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public RequirementService(DapperContext<CS_RequirementMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@"
            Select Cast(StoreID As varchar) id, StoreName as [text] 
            From bas_Store
            Where IsActive = 1 And StoreID In(1);

            Select Cast(UnitID As varchar) id,UnitName as [text] 
            From bas_UnitInfo
            Where IsActive = 1 And Approved = 1 And UnitID In(1);

            -- RequirementByList
            SELECT 
	            CAST(ei.EmployeeID AS VARCHAR) id, CONCAT(ei.EmployeeNo, '-', ei.EmployeeName) [text],
	            ei.DepartmentID As valueA, d.DepartmentName As valueD, ei.SectionID As valueB, s.SectionName As valueE, ei.SubSectionID As valueC, 
	            isnull(ss.SubSectionName,'') As valueF 
            FROM 
	            {DBSource.PDLHRM}..emp_EmploymentInfo ei
	            Left Join {DBSource.PDLHRM}..bas_Department d On d.DepartmentID = ei.DepartmentID
	            Left Join {DBSource.PDLHRM}..bas_Section s On s.SectionID = ei.SectionID
	            Left Join {DBSource.PDLHRM}..bas_SubSection ss On ss.SubSectionID = ei.SubSectionID 
            Where 
	            ei.EmpStatusID = 1  AND ei.UnitID = 2
            ORDER BY CONCAT(ei.EmployeeNo, '-', ei.EmployeeName); 


            -- Section
            Select SectionID As id, SectionName As [text] 
            From {DBSource.PDLHRM}..bas_Section 
            Where IsActive = 1;

            Select UOMID as id,UOM as text from bas_UOM where Approved=1;

            ";  
        public async Task<GridEntity<CS_RequirementMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "RequirementMasterID Desc"; 
            string returnCondition = "";
            string condition = "";

            string strRoles = "1,22,114";
            string[] roleList = strRoles.Split(',');

            IEnumerable<string> thirtyYearOlds = roleList.Where(p => p == User.UserRoleID.ToString());
  
            if (status == Status.EDIT)
            { 
                condition = $@" AND A.Approved = 0 And A.Reject = 0 {returnCondition}"; 
            }
            else if (status == Status.APPROVED)
            {
                condition = $@" AND A.Approved = 1 And A.Reject = 0 {returnCondition}";
            }
            else if (status == Status.REJECT)
            {
                condition = $@" AND A.Reject = 1 {returnCondition}";
            }
            else
            {
                condition = $@" And A.Reject = 0 {returnCondition}";
            } 

            sql = $@"

                Select 
	                IsNull(A.RequirementMasterID,0) As RequirementMasterID,
	                A.RequirementNo, Replace(Convert(varchar, A.RequirementDate, 106), ' ','-')PRDate, A.DepartmentID, C.DepartmentName,
	                A.SectionID, D.SectionName, A.SubSectionID, E.SubSectionName, A.RequirementBy, B.EmployeeName As RequirementByName, 
	                A.Remarks, A.StoreID, F.StoreName, 
                    A.UnitID, G.UnitName
                From 
	                CS_RequirementMaster A 
	                Left Join {DBSource.PDLHRM}..emp_EmploymentInfo B On A.RequirementBy = B.EmployeeID
	                Left Join {DBSource.PDLHRM}..bas_Department C On A.DepartmentID  = C.DepartmentID
	                Left Join {DBSource.PDLHRM}..bas_Section D On A.SectionID = D.SectionID
	                Left Join {DBSource.PDLHRM}..bas_SubSection E On A.SubSectionID = E.SubSectionID
	                Left Join bas_Store F On A.StoreID = F.StoreID
	                Left Join bas_UnitInfo G On A.UnitID = G.UnitID
                Where  
                    1 = 1 {condition} "; 
            return await _context.GetKendoGridDataAsync<CS_RequirementMaster>(options, sql, orderBy, "");
        }
        public async Task<CS_RequirementMaster> GetNew()
        {


            var query = $@" 
                {CommonQuery}
            -- ProductList
            Select
	            id,[text], valueA,valueB,valueC
            From dbo.fn_CS_Stock_Item_List(0,0,0,0); 
            ";

            //From dbo.fn_CS_Stock_Item_List({ p.MenuID},{ User.UnitID},{ User.UserID},0);
            var data = new CS_RequirementMaster();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);

                data.StoreList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.RequirementByList = records.Read<DropDownList>().ToList();
                //Child
                data.SectionList = records.Read<DropDownList>().ToList();
                data.UOMList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();

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
        public async Task<CS_RequirementMaster> GetEdit(int id, int? UnitID, int? MenuID, int? UserID)
        {
            var query =
               $@"
                -- Master
                Select A.*,B.DepartmentName,C.SectionName,D.SubSectionName From CS_RequirementMaster A 
	            Left Join {DBSource.PDLHRM}..bas_Department B On A.DepartmentID = B.DepartmentID
	            Left Join {DBSource.PDLHRM}..bas_Section C On A.SectionID = C.SectionID
	            Left Join {DBSource.PDLHRM}..bas_SubSection D On A.SubSectionID = D.SubSectionID 
				Where RequirementMasterID = {id};

                -- Child
                Select 
					B.RequirementChildID,B.UOMID,B.RowSLNo,B.ReceiveChildID,u.UOM,F.valueC As StockQty,b.RequirementQty,B.SectionID,B.ChildsRemarks
					,B.ReceiveChildID As ProductID,F.text As ProductName, s.SectionName
                From 
	                CS_RequirementMaster A 
	                Inner Join CS_RequirementChild B On A.RequirementMasterID = B.RequirementMasterID
	                left Join bas_CS_ProductMaster C On B.ProductID = C.ProductID
	                left Join bas_UOM u On B.UOMID = u.UOMID
	                Left Join {DBSource.PDLHRM}..bas_Section s On B.SectionID  = s.SectionID
					Left Join dbo.fn_CS_Stock_Item_List({MenuID},1,1,0) F On B.ReceiveChildID = F.id
                Where 
	                IsNull(B.IsDeleted,0) = 0 
	                And A.RequirementMasterID = {id}
                Order By B.RowSLNo Asc;

                {CommonQuery}
                -- ProductList
                Select
	                id,[text], valueA,valueB,valueC
                From dbo.fn_CS_Stock_Item_List({MenuID},{UnitID},{UserID},0);   
                ";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                CS_RequirementMaster data = await records.ReadFirstOrDefaultAsync<CS_RequirementMaster>();
                data.Childs = records.Read<CS_RequirementChild>().ToList();

                data.StoreList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.RequirementByList = records.Read<DropDownList>().ToList();
                data.SectionList = records.Read<DropDownList>().ToList();
                data.UOMList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();

                foreach (CS_RequirementChild items in data.Childs)
                {
                    items.objProductList.id = items.ProductID;
                    items.objProductList.text = items.ProductName; 
                    items.objSectionList.id = items.SectionID;
                    items.objSectionList.text = items.SectionName; 
                    items.objUOMList.id = items.UOMID;
                    items.objUOMList.text = items.UOM;
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
        public async Task<CS_RequirementMaster> SaveAsync(CS_RequirementMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_CS_RequirementChild> TTList = GetChild(entity.Childs);
                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
                parameters = new DynamicParameters(); 
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@RequirementMasterID", entity.RequirementMasterID);
                parameters.Add("@UnitID", entity.UnitID);
                parameters.Add("@StoreID", entity.StoreID);
                //parameters.Add("@RequirementNo", entity.RequirementNo);
                parameters.Add("@RequirementDate", entity.RequirementDate);
                parameters.Add("@RequirementBy", entity.RequirementBy);
                parameters.Add("@DepartmentID", entity.DepartmentID);
                parameters.Add("@SectionID", entity.SectionID);
                parameters.Add("@SubSectionID", entity.SubSectionID);
                parameters.Add("@SubStoreID", entity.SubStoreID);
                parameters.Add("@SRKhataNo", entity.SRKhataNo);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@Approved", entity.Approved);                
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@ReviseReason", entity.ReviseReason);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync("spSet_CS_RequirementMaster", parameters, 30, CommandType.StoredProcedure);
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
                entity.Message = "Error : " + ex.Message;
            }
            finally
            {
                parameters = null;
            }
            return entity;
        } 
        public List<TT_CS_RequirementChild> GetChild(List<CS_RequirementChild> Childs)
        { 
            List<TT_CS_RequirementChild> TTList = new List<TT_CS_RequirementChild>();
            int Counter = 0;
            foreach (CS_RequirementChild item in Childs)
            {
                ++Counter;
                TT_CS_RequirementChild objItem = new TT_CS_RequirementChild();
                objItem.RequirementChildID = item.RequirementChildID;
                objItem.RequirementMasterID = 0;
                objItem.StyleID = 0;
                objItem.POMasterID = 0;
                objItem.POChildID = 0;
                objItem.IndentMasterID = 0;
                objItem.IndentChildID = 0;
                objItem.SectionID = item.objSectionList.id;
                objItem.ProductID = item.objProductList.id;
                objItem.UOMID = item.objUOMList.id;
                objItem.RequirementQty = item.RequirementQty; 
                objItem.ChildsRemarks = item.ChildsRemarks;
                objItem.RowSLNo = item.RowSLNo;
                objItem.RowSLNoNew = Counter;
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