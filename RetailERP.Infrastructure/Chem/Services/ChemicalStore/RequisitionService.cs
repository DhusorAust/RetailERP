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
    public class RequisitionService : IRequisitionService
    {
        private readonly DapperContext<CS_PRMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public RequisitionService(DapperContext<CS_PRMaster> context)
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

            -- PRBy
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

            -- Concern Person 
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
	            ei.EmpStatusID = 1 AND ei.DepartmentID = 33 AND ei.UnitID = 2
            ORDER BY CONCAT(ei.EmployeeNo, '-', ei.EmployeeName); 

            -- Section
            Select SectionID As id, SectionName As [text] 
            From {DBSource.PDLHRM}..bas_Section 
            Where IsActive = 1;

            -- Product
            SELECT
	            ProductID AS id, ProductName AS [text], pm.UOMID As valueA, u.UOM As valueB
            FROM 
	            bas_CS_ProductMaster pm
	            Left Join bas_UOM u On u.UOMID = pm.UOMID
            Where 
	            pm.IsActive = 1
            ORDER BY ProductName;


            -- UOM List
            Select UOMID as id,UOM as [text] From bas_UOM Where Approved = 1;
         ";  
        public async Task<GridEntity<CS_PRMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "PRMasterID Desc"; 
            string returnCondition = "";
            string condition = "";

            string strRoles = "1,22,114";
            string[] roleList = strRoles.Split(',');

            IEnumerable<string> thirtyYearOlds = roleList.Where(p => p == User.UserRoleID.ToString());

            //if (thirtyYearOlds.FirstOrDefault() == null)
            //{ 
            //    if (status == Status.RETURN)
            //    {
            //        returnCondition = " And PRM.SectionID Not In(214)";
            //    }
            //    else if ((status == Status.EDIT) || (status == Status.APPROVED))
            //    {
            //        returnCondition = " And BS.SectionID Not In(214)";
            //    }
            //    else
            //    {
            //        returnCondition = " And D.SectionID Not In(214)";
            //    }
            //}

            if (status == Status.EDIT)
            { 
                condition = $@" AND PRM.Approved = 0 And PRM.Reject = 0 {returnCondition}"; 
            }
            else if (status == Status.APPROVED)
            {
                condition = $@" AND PRM.Approved = 1 And PRM.Reject = 0 {returnCondition}";
            }
            else if (status == Status.REJECT)
            {
                condition = $@" AND PRM.Reject = 1 {returnCondition}";
            }
            else
            {
                condition = $@" And PRM.Reject = 0 {returnCondition}";
            } 

            sql = $@"
                Select 
	                IsNull(PRM.PRMasterID,0) As PRMasterID, IsNull(PRM.SSReturnMasterID,0) As SSReturnMasterID, 0 As ReactorDeliveryMasterID, 
	                PRM.PRNo, Replace(Convert(varchar, PRM.PRDate, 106), ' ','-')PRDate, PRM.DepartmentID, BD.DepartmentName,
	                PRM.SectionID, BS.SectionName, PRM.SubSectionID, BSS.SubSectionName, PRM.PRBy, E.EmployeeName As PRByName, 
	                PRM.ConcernPersonID, CP.EmployeeName AS ConcernPerson, PRM.PRRemarks, PRM.StoreID, J.StoreName, 
                    PRM.UnitID, K.UnitName
                From 
	                CS_PRMaster PRM 
	                Left Join {DBSource.PDLHRM}..emp_EmploymentInfo E On E.EmployeeID = PRM.PRBy 
	                Left Join {DBSource.PDLHRM}..bas_Department BD On BD.DepartmentID = PRM.DepartmentID
	                Left Join {DBSource.PDLHRM}..bas_Section BS On BS.SectionID = PRM.SectionID
	                Left Join {DBSource.PDLHRM}..bas_SubSection BSS On BSS.SubSectionID = PRM.SubSectionID
	                Left Join {DBSource.PDLHRM}..emp_EmploymentInfo CP On CP.EmployeeID = PRM.ConcernPersonID 
	                Left Join bas_Store J On PRM.StoreID = J.StoreID
	                Left Join bas_UnitInfo K On PRM.UnitID = K.UnitID
                WHERE  
                    1 = 1 {condition} ";

            #region
            //        else if (status == Status.RETURN)
            //        {
            //            sql = $@"

            //            Select * From
            //            (
            //             Select
            //             0 As PRMasterID
            //                ,IsNull(A.SSReturnMasterID,0) As SSReturnMasterID
            //                ,0 As ReactorDeliveryMasterID
            //             ,A.StoreTypeID,A.Com_UnitID,A.StoreID,A.StockTypeID,A.ReturnDate As PRDate
            //             ,A.ReturnByID As EmployeeID,E.DEPTID As DepartmentID,D.SECID As SectionID,A.SubSectionID
            //             ,E.DEPTNAME As DepartmentName,D.SECNAME As SectionName,C.SSECNAME As SubSectionName
            //             ,F.FlagName As StoreTypeName,H.UnitFullName As Com_UnitName,G.StoreName
            //             ,I.FIRST_NAME As EmployeeName
            //             ,'' As PRNo,'' As ConcernName,'' As PRRemarks,'' As INDENT_NO_ORACLE
            //             ,{User.UserID} As UserID_SubStorePermission
            //             From SubStoreReturnMaster A
            //             Left Join CS_PRMaster B On A.SSReturnMasterID = B.SSReturnMasterID
            //             Left Join F_BAS_SUBSECTION C On A.SubSectionID = C.SSECID
            //             Left Join F_BAS_SECTION D On C.SECID = D.SECID
            //             Left Join F_BAS_DEPARTMENT E On D.DEPTID = E.DEPTID
            //             Left Join FlagTable F On A.StoreTypeID = F.FlagID
            //             Left Join bas_Store G On A.StoreID = G.StoreID
            //             Left Join bas_UnitInfo H On A.Com_UnitID = H.UnitID
            //             Left Join F_HR_EMPLOYEE I On A.ReturnByID = I.EMPID
            //             Where (A.SSReturnMasterID Is Not Null) And (B.SSReturnMasterID Is Null)
            //                {condition}
            //            )K
            //            Inner Join
            //            (
            //             Select UserID From SubStorePermission Where UserID = {User.UserID} And Tag = 6 And Description = 'Sub Store Return Receive'
            //            )L On K.UserID_SubStorePermission = L.UserID";
            //        }
            //        else if (status == Status.OTHERS)
            //        {
            //            sql = $@"

            //            Select
            //            0 As PRMasterID
            //            ,0 As SSReturnMasterID
            //            ,IsNull(A.DeliveryMasterID,0) As ReactorDeliveryMasterID
            //            ,A.DeliveryDate As PRDate,A.DeliveryNo As PRNo,A.ReceiverUnitID As Com_UnitID,A.ReceiverStoreID As StoreID
            //            ,A.ChallanNo
            //            ,A.IndentMasterID,A.IndentChildsID
            //            ,A.DeliveryProductID
            //            ,B.UnitName As Com_UnitName,C.StoreName
            //            ,F.IndentNo
            //            ,G.ProductName + ' (' + Cast(TotalDeliveryQty As varchar) + ' ' + H.UNAME + ')' As ProductName
            //            From RA_DeliveryMaster A
            //            Inner Join bas_UnitInfo B On A.UnitID = B.UnitID
            //            Inner Join bas_Store C On A.StoreID = C.StoreID
            //            Inner Join bas_UnitInfo D On A.ReceiverUnitID = D.UnitID
            //            Inner Join bas_Store E On A.ReceiverStoreID = E.StoreID
            //            Inner Join CS_IndentMaster F On A.IndentMasterID = F.IndentMasterID
            //            Inner Join CS_ProductMaster G On A.DeliveryProductID = G.ProductID
            //Inner Join
            //(
            //	Select A.DeliveryMasterID,B.UNAME,Sum(A.TotalDeliveryQty) As TotalDeliveryQty
            //	From RA_DeliveryChild A
            //	Inner Join F_BAS_UNITS B On A.ProductUnitID = B.UID

            //	Where IsNull(A.IsDeleted,0) = 0
            //	Group By A.DeliveryMasterID,B.UNAME
            //)H On A.DeliveryMasterID = H.DeliveryMasterID
            //            Where IsNull(A.IsActive,0) = 1 And IsNull(A.IsApproved,0) = 1
            //            ";
            //        }

            #endregion

            return await _context.GetKendoGridDataAsync<CS_PRMaster>(options, sql, orderBy, "");
        }
        public async Task<CS_PRMaster> GetNew()
        {
            var query = $@" {CommonQuery}; ";
            var data = new CS_PRMaster();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);

                data.StoreList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.PRByList = records.Read<DropDownList>().ToList();
                data.ConcernPersonList = records.Read<DropDownList>().ToList();
                //Child
                data.SectionList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();
                data.UOMList = records.Read<DropDownList>().ToList();

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
        public async Task<CS_PRMaster> GetEdit(int id)
        {
            var query =
               $@"
                -- Master
                Select A.* From CS_PRMaster A Where PRMasterID = {id};

                -- Child
                Select 
	                CPID.*, IsNull(CSP.ProductName,'') As ProductName, u.UOM, s.SectionName 
                From 
	                CS_PRMaster CPRM 
	                Inner Join CS_IndentChild CPID On CPID.PRMasterID = CPRM.PRMasterID
	                left Join bas_CS_ProductMaster CSP On CSP.ProductID = CPID.ProductID
	                left Join bas_UOM u On u.UOMID = CPID.UOMID
	                Left Join ERPDB..bas_Section s On s.SectionID = CPID.SectionID 
                Where 
	                IsNull(CPID.IsDeleted,0) = 0 
	                And CPRM.PRMasterID = {id}
                Order By CPID.RowSLNo Asc;

                {CommonQuery}; ";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                CS_PRMaster data = await records.ReadFirstOrDefaultAsync<CS_PRMaster>();
                data.Childs = records.Read<CS_IndentChild>().ToList();

                data.StoreList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.PRByList = records.Read<DropDownList>().ToList();
                data.ConcernPersonList = records.Read<DropDownList>().ToList();
                //Child
                data.SectionList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();
                data.UOMList = records.Read<DropDownList>().ToList();

                foreach (CS_IndentChild items in data.Childs)
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
        public async Task<CS_PRMaster> SaveAsync(CS_PRMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_CS_IndentChild> TTList = GetIndentChild(entity.Childs);
                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
                parameters = new DynamicParameters(); 

                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@PRMasterID", entity.PRMasterID);
                parameters.Add("@UnitID", entity.UnitID);
                parameters.Add("@StoreID", entity.StoreID);
                parameters.Add("@PRDate", entity.PRDate);
                parameters.Add("@PRBy", entity.PRBy);
                parameters.Add("@ConcernPersonID", entity.ConcernPersonID);
                parameters.Add("@DepartmentID", entity.DepartmentID);
                parameters.Add("@SectionID", entity.SectionID);
                parameters.Add("@SubSectionID", entity.SubSectionID); 
                parameters.Add("@SSReturnMasterID", entity.SSReturnMasterID); 
                parameters.Add("@PRRemarks", entity.PRRemarks);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@ReviseReason", entity.ReviseReason);

                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("spSet_CS_PR", parameters, 30, CommandType.StoredProcedure);
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
        public List<TT_CS_IndentChild> GetIndentChild(List<CS_IndentChild> Childs)
        { 
            List<TT_CS_IndentChild> TTList = new List<TT_CS_IndentChild>();

            foreach (CS_IndentChild item in Childs)
            { 
                TT_CS_IndentChild objItem = new TT_CS_IndentChild();
                objItem.IndentChildID = item.IndentChildID;
                objItem.PRMasterID = item.PRMasterID;
                objItem.SSReturnChildID = item.SSReturnChildID;
                objItem.ProductID = item.objProductList.id;
                //objItem.UOMID = item.UOMID;
                objItem.UOMID = item.objUOMList.id;
                objItem.SectionID = item.objSectionList.id; 
                objItem.PRQty = item.PRQty; 
                objItem.PRChildRemarks = item.PRChildRemarks; 
                objItem.RowSLNo = item.RowSLNo;
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