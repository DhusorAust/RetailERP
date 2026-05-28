using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.ChemicalStore;
using Dapper;
using Microsoft.Data.SqlClient;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.ChemicalStore
{
    public class ReceiveService : IReceiveService
    {
        private readonly DapperContext<CS_ReceiveMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public ReceiveService(DapperContext<CS_ReceiveMaster> context)
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

            -- Indented by
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

            -- Indent Type
            SELECT CAST(IndentTypeID AS VARCHAR) id, IndentTypeName [text] 
            FROM bas_CS_IndentType 
            ORDER BY IndentTypeID; 

            -- Supplier
            Select CAST(SupplierID AS VARCHAR) id, SupplierName [text] 
            From bas_Supplier 
            Order By SupplierID; 

            -- Product
            SELECT
	            ProductID AS id, ProductName AS [text], pm.UOMID As valueA, u.UOM As valueB
            FROM 
	            bas_CS_ProductMaster pm
	            Left Join bas_UOM u On u.UOMID = pm.UOMID
            Where 
	            pm.IsActive = 1
            ORDER BY ProductName;

            -- QCStatustList
            Select id,[text] From
            (
	            Select 1 as id,'Pass' As [text]
	            Union All
	            Select 2 as id,'Fail' As [text]
	            Union All
	            Select 3 as id,'Hold' As [text]
            )A;


            Select LocationID as id,LocationName as [text] From bas_CS_Location
            Order By SerialNo;

            Select RackID As id, RackName as [text] From bas_CS_Rack
            Order By SerialNo;

            -- End Common
            ";
        public async Task<GridEntity<CS_ReceiveMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "Where 1 = 1";
            string conditionInner = "";

            sql = $@"
                    Select
                    A.ReceiveMasterID,A.ReceiveNo,A.ReceiveDate,B.IndentTypeName,
                    C.EmployeeName As ReceiveByName,D.EmployeeName As CheckedByName
                    ,dbo.fn_Conditional_Stuffing_Text(A.ReceiveMasterID,1,1)As IndentNo
                    ,dbo.fn_Conditional_Stuffing_Text(A.ReceiveMasterID,2,1)As MRRNos
                    From CS_ReceiveMaster A
                    Inner Join bas_CS_IndentType B On A.ReceiveTypeID = B.IndentTypeID
                    Inner Join {DBSource.PDLHRM}..emp_EmploymentInfo C On A.ReceiveBy = C.EmployeeID
                    Inner Join {DBSource.PDLHRM}..emp_EmploymentInfo D On A.CheckedBy = D.EmployeeID
                ";
            orderBy = "ReceiveMasterID Desc";



            if (status == Status.LIST_2)
            {
                // QC Pass List
                conditionInner = $@"Where A.IsQCDone = 1 And E.QCStatus = 1 And E.SendForMRR = 0 And IsNull(E.IsDeleted,0) = 0";
            }
            if (status == Status.LIST_3)
            {
                // QC Fail List
                conditionInner = $@"Where A.IsQCDone = 1 And E.QCStatus In(2,3) And E.SendForMRR = 0 And IsNull(E.IsDeleted,0) = 0";
            }
            if (status == Status.LIST_4)
            {
                // Mrr Pending List

                conditionInner = $@"Where A.IsQCDone = 1 And E.QCStatus =1 And E.SendForMRR = 1  And E.MRR_Number Is Null And IsNull(E.IsDeleted,0) = 0";
            }

            if (status == Status.LIST_5)
            {
                // Mrr Pending List

                conditionInner = $@"Where A.IsQCDone = 1 And E.QCStatus =1 And E.SendForMRR = 1  And E.MRR_Number Is Not Null And IsNull(E.IsDeleted,0) = 0";
            }




            if (status == Status.PENDING)
            {
                sql = $@"
                    Select 
                        B.IndentMasterID As CHKID,B.IndentMasterID As check_row,
	                    0 As ReceiveMasterID,B.IndentMasterID,B.UnitID,B.StoreID,B.PRMasterID,B.IndentNo,B.IndentDate
	                    ,IsNull(C.SSReturnMasterID,0) As SSReturnMasterID
	                    ,F.EmployeeName As IndentByName,B.Remarks,D.UnitName,E.StoreName
	                    ,G.IndentTypeName
                        ,'false' As RowSelected
                    From
                    (
	                    Select IndentMasterID,Sum(IndentQty) As IndentQty From
	                    (
		                    Select IndentMasterID,IndentQty From CS_IndentChild
		                    Where IsDeleted = 0
		                    Union All
		                    Select IndentMasterID,-ReceiveQty IndentQty From CS_ReceiveChild
		                    Where IsDeleted = 0
	                    )B
	                    Group By IndentMasterID
                    )A
                    Inner Join CS_IndentMaster B On A.IndentMasterID = B.IndentMasterID
                    Inner Join CS_PRMaster C On B.PRMasterID = C.PRMasterID
                    Inner Join bas_UnitInfo D On B.UnitID = D.UnitID
                    Inner Join bas_Store E On B.StoreID = E.StoreID
                    Inner Join {DBSource.PDLHRM}..emp_EmploymentInfo F On B.IndentBy = F.EmployeeID
                    Inner Join bas_CS_IndentType G On B.IndentTypeID = G.IndentTypeID
                    Where B.IsActive = 1 And IndentQty > 0
                ";
                condition = "";
                orderBy = "IndentMasterID Desc";

            }

            else if (status == Status.LIST_1)
            {
                // QC Pending List

                condition = condition + $@" And A.SendForQC = 1 And A.Approved = 0 And A.Reject = 0 And A.IsQCDone = 0 ";
            }

            else if ((status == Status.LIST_2) || (status == Status.LIST_3) || (status == Status.LIST_4) || (status == Status.LIST_5))
            {
                // QC Pass List


                sql = $@"
                Select
                A.ReceiveMasterID,A.ReceiveNo,A.ReceiveDate,A.IndentTypeName,
                A.ReceiveByName,A.CheckedByName
                ,A.IndentNo
                ,dbo.fn_Conditional_Stuffing_Text(A.ReceiveMasterID,2,1)As MRRNos
                From
                (
	                Select
	                A.ReceiveMasterID,A.ReceiveNo,A.ReceiveDate,B.IndentTypeName,
	                C.EmployeeName As ReceiveByName,D.EmployeeName As CheckedByName
	                ,dbo.fn_Conditional_Stuffing_Text(A.ReceiveMasterID,1,1)As IndentNo
	                From CS_ReceiveMaster A
	                Inner Join bas_CS_IndentType B On A.ReceiveTypeID = B.IndentTypeID
	                Inner Join {DBSource.PDLHRM}..emp_EmploymentInfo C On A.ReceiveBy = C.EmployeeID
	                Inner Join {DBSource.PDLHRM}..emp_EmploymentInfo D On A.CheckedBy = D.EmployeeID
	                Inner Join CS_ReceiveChild E On A.ReceiveMasterID = E.ReceiveMasterID
	                {conditionInner}
                )A
                Group By
                A.ReceiveMasterID,A.ReceiveNo,A.ReceiveDate,A.IndentTypeName,
                A.ReceiveByName,A.CheckedByName
                ,A.IndentNo

                ";
                condition = "";
                orderBy = "ReceiveMasterID Desc";
                //condition = condition + $@" And A.SendForQC = 1 And A.Approved = 0 And A.Reject = 0 And A.IsQCDone = 0 ";
            }

            else if (status == Status.EDIT)
            {
                condition = condition + $@" And A.SendForQC = 0 And A.Approved = 0 And A.Reject = 0 And A.IsQCDone = 0 ";
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

            return await _context.GetKendoGridDataAsync<CS_ReceiveMaster>(options, sql + " " + condition, orderBy,"" );
        }
        public async Task<CS_ReceiveMaster> GetNew()
        {
            var query =
                $@"  
                {CommonQuery} ";
            CS_ReceiveMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data.StoreList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.ReceiveByList = records.Read<DropDownList>().ToList();
                data.ReceiveTypeList = records.Read<DropDownList>().ToList();
                data.SupplierList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();                
                data.QCStatustList = records.Read<DropDownList>().ToList();
                data.LocationList = records.Read<DropDownList>().ToList();
                data.RackList = records.Read<DropDownList>().ToList();
                data.CheckedByList = data.ReceiveByList;
                data.QCBytList = data.ReceiveByList;


                // DELETE START
                data.StoreID = 1;
                data.UnitID = 1;
                data.ReceiveBy = 485;
                data.CheckedBy = 483;
                data.SupplierID = 2;
                data.ReceiveTypeID = 2;

                // DELETE End

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
        public async Task<CS_ReceiveMaster> GetEdit(int id, bool isQCPage, int Flag)
        {
            string condition = "";

            /*

                LIST_1: 40, btnQCPendingList
                LIST_2: 41, btnQCPassedList
                LIST_3: 42, btnQCFailedList
                LIST_4: 43, btnMrrPendingList
                LIST_5: 44, btnMrrList
                LIST_6: 45, btnAllList
             */

            condition = $@"Where A.ReceiveMasterID = {id}  And  IsNull(A.IsDeleted,0) = 0 And IsNull(B.IsDeleted,0) = 0 ";

            if ((isQCPage == true) && (Flag == 40))
            {

                condition = condition + "And IsNull(A.QCStatus,0) = 0";
            }

            if ((isQCPage == true) && (Flag == 41))
            {

                condition = condition + "And IsNull(A.QCStatus,0) = 1  And IsNull(A.SendForMRR,0) = 0 ";
            }

            if ((isQCPage == true) && ((Flag == 42)))
            {

                condition = condition + "And A.QCStatus In(2,3) And IsNull(A.SendForMRR,0) = 0";
            }
            if ((isQCPage == true) && (Flag == 43))
            {

                condition = condition + "And IsNull(A.QCStatus,0) = 1 And IsNull(A.SendForMRR,0) = 1 And Len(Trim(IsNull(A.MRR_Number,''))) = 0";
            }
            if ((isQCPage == true) && (Flag == 44))
            {
                condition = condition + "And IsNull(A.QCStatus,0) = 1 And IsNull(A.SendForMRR,0) = 1 And Len(Trim(IsNull(A.MRR_Number,''))) > 0";
            }

            if ((isQCPage == true) && (Flag == 44))
            {
                condition = condition + "And IsNull(A.QCStatus,0) = 1 And IsNull(A.SendForMRR,0) = 1 And Len(Trim(IsNull(A.MRR_Number,''))) > 0";
            }


            if ((isQCPage == false) && (Flag == 40))
            {
                condition = condition + "";
            }

            var query =
                $@" 
                -- Master
                Select * From CS_ReceiveMaster Where ReceiveMasterID = {id};
                -- Details
	            Select
	            *
	            ,IIF(GroupSL > 1, 0,IndentQtyB) As IndentQty
	            ,IIF(GroupSL > 1, 0,ReceiveQtyPrevB) As ReceivedQty
                ,IIF(GroupSL > 1, 0,StockQtyB) As StockQty
	            From
	            (
		            Select 
			            ROW_NUMBER()Over(Partition By A.IndentChildID Order By A.ReceiveChildID) As GroupSL,
			            A.ReceiveChildID,A.IndentChildID,0 As PRMasterID,A.IndentMasterID,A.UnitID,A.StoreID,A.ProductID,A.UOMID,A.SectionID,A.LocationID,A.RackID,
			            A.BatchNo,A.ManufacturingDate,A.ExpireDate,
                        A.QCStatus,A.QCBy,A.QCDate,A.QCRemarks,A.SendForMRR,
                        C.IndentTypeID,U.IndentTypeName,C.IndentNo,B.IndentChildRemarks,A.ChildsRemarks,
                        B.IndentQty As IndentQtyB ,IsNull(J.ReceiveQtyPrev,0) As ReceiveQtyPrevB,A.ReceiveQty,IsNull(J.StockQty,0) As StockQtyB,			            
			            D.ProductName,E.UOM,F.SectionName,G.UnitName,H.StoreName,
                        CONCAT(K.EmployeeNo, '-', K.EmployeeName) As QCByName,
						Case
							When A.QCStatus = 1 Then 'Pass'
							When A.QCStatus = 2 Then 'Fail'
							When A.QCStatus = 3 Then 'Hold'
						End As QCStatusName,L.LocationName,M.RackName,
                        A.MRR_Number,
                        A.RowSLNo      
		            From CS_ReceiveChild A
		            Inner Join CS_IndentChild B On A.IndentChildID = B.IndentChildID
		            Inner Join CS_IndentMaster C On B.IndentMasterID = C.IndentMasterID
		            Inner Join bas_CS_ProductMaster D On A.ProductID = D.ProductID
		            Inner Join bas_UOM E On A.UOMID = E.UOMID
		            Inner Join {DBSource.PDLHRM}..bas_Section F On A.SectionID = F.SectionID
		            Inner Join bas_UnitInfo G On A.UnitID= G.UnitID
		            Inner Join bas_Store H On A.StoreID = H.StoreID
		            Inner Join bas_CS_IndentType U On C.IndentTypeID = U.IndentTypeID
		            Left Join dbo.fn_CS_Receive(0,{User.UnitID},{User.UserID},'',{id}) J On B.IndentChildID = J.IndentChildID
                    Left Join ERPDB..emp_EmploymentInfo K On A.QCBy = K.EmployeeID
					Left Join bas_CS_Location L On A.LocationID = L.LocationID
					Left Join bas_CS_Rack M On A.RackID = M.RackID

		            {condition}
	            )A
	            Order By A.RowSLNo;
                {CommonQuery} ";

            CS_ReceiveMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<CS_ReceiveMaster>();
                data.Childs = records.Read<CS_ReceiveChild>().ToList();
                data.StoreList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.ReceiveByList = records.Read<DropDownList>().ToList();
                data.ReceiveTypeList = records.Read<DropDownList>().ToList();
                data.SupplierList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();
                data.QCStatustList = records.Read<DropDownList>().ToList();
                data.LocationList = records.Read<DropDownList>().ToList();
                data.RackList = records.Read<DropDownList>().ToList();

                data.CheckedByList = data.ReceiveByList;
                data.QCBytList = data.ReceiveByList;


                var vText = "";
                foreach (CS_ReceiveChild items in data.Childs)
                {
                    items.objProductList.id = items.ProductID;
                    items.objProductList.text = items.ProductName;
                    data.UnitID = items.UnitID;
                    data.StoreID = items.StoreID;

                    //vText = "";
                    //vText = data.ReceiveByList.FirstOrDefault(x => x.id == items.QCBy)?.text;
                    items.objQCByList.id = items.QCBy;
                    items.objQCByList.text = items.QCByName;
                    //items.objQCByList.text = vText;


                    //vText = "";
                    //vText = data.QCStatustList.FirstOrDefault(x => x.id == items.QCStatus)?.text;
                    items.objQCStatusList.id = items.QCStatus;
                    items.objQCStatusList.text = items.QCStatusName;


                    items.objLocationList.id = items.LocationID;
                    items.objLocationList.text = items.LocationName;

                    items.objRackList.id = items.RackID;
                    items.objRackList.text = items.RackName;


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
        public async Task<CS_ReceiveMaster> SaveAsync(CS_ReceiveMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
				List<TT_CS_ReceiveChild> TTList = GetReceiveChild(entity.Childs);
				DataSet DetailsData = CommonFunction.ToDataSet(TTList);
				var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
				parameters = new DynamicParameters(); 
				parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@ReceiveMasterID", entity.ReceiveMasterID);
                parameters.Add("@ReceiveNo", entity.ReceiveNo);
                parameters.Add("@ReceiveDate", entity.ReceiveDate);
                parameters.Add("@ReceiveTypeID", entity.ReceiveTypeID);
                parameters.Add("@ReceiveBy", entity.ReceiveBy);
                parameters.Add("@CheckedBy", entity.CheckedBy);
                parameters.Add("@PIMasterID", entity.PIMasterID);
                parameters.Add("@SupplierID", entity.SupplierID);
                parameters.Add("@Country", entity.Country);
                parameters.Add("@ChallanNo", entity.ChallanNo);
                parameters.Add("@ChallanDate", entity.ChallanDate);
                parameters.Add("@GatePassNo", entity.GatePassNo);
                parameters.Add("@GatePassDate", entity.GatePassDate);
                parameters.Add("@TransportID", entity.TransportID);
                parameters.Add("@VehicleNo", entity.VehicleNo);
                parameters.Add("@QCNo", entity.QCNo);
                parameters.Add("@MRRNo", entity.MRRNo);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@Reject", entity.Reject);
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
				parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@Message", dbType: DbType.String, size: -1, direction: ParameterDirection.Output);
                entity.NoofRows = await _context.ExecuteAsync("spSet_CS_ReceiveMaster", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");
                entity.Message = parameters.Get<string>("@Message");
                if (entity.NoofRows > 0 && entity.ResultId > 0)
                {
                    entity.Message = "Save operation successful.";
                }
                else
                {
                    entity.Message = entity.Message + ". Save operation failed.";
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

        public async Task<List<CS_ReceiveMaster>> GetIndentList(string searchKey)
        {
            string condition = " Where 1 = 1";
            string query = "";
            var orderBy = " order by B.IndentNo ";

            if (!string.IsNullOrEmpty(searchKey))
            {
                condition += "  and (B.IndentNo like '%" + searchKey + "%' )";

                //orderBy = $@" order by (
                //                CASE 
                //                    WHEN vii.IndentNo = N'{searchKey}' THEN 1 
                //                    ELSE 2
                //                END
                //            )";
            }


            query = $@" 
					Select A.IndentMasterID,B.IndentNo,B.IndentDate,Sum(IndentQty) As IndentQty,Sum(ReceiveQty) As ReceivedQty,Sum(BalanceQty) As BalanceQty From
					(
						Select A.IndentMasterID,B.IndentQty,0 As ReceiveQty,B.IndentQty As BalanceQty From CS_IndentMaster A
						Inner Join CS_IndentChild B On A.IndentMasterID = B.IndentMasterID
						Where A.Approved = 1 And  IsNull(B.IsDeleted,0) = 0

						Union All

						Select A.IndentMasterID,0 As IndentQty,A.ReceiveQty,-A.ReceiveQty  As BalanceQty From CS_ReceiveChild A
						Inner Join CS_ReceiveMaster B On A.ReceiveMasterID = B.ReceiveMasterID
						Where IsNull(IsDeleted,0) = 0
						--And B.Approved = 1					
					)A
					Inner Join CS_IndentMaster B On A.IndentMasterID = B.IndentMasterID					
                    {condition}
					Group By A.IndentMasterID,B.IndentNo,B.IndentDate
					Having Sum(BalanceQty) > 0
                    ";

            var indProdList = await _context.GetDataAsync<CS_ReceiveMaster>(query);
            if (indProdList == null) return new List<CS_ReceiveMaster>();
            return indProdList;
        }
        public async Task<List<CS_ReceiveChild>> GetIndentChild(int IndentMasterID)
        {
            string condition = "";

            var query = 
            $@" 
                -- Childs
                Select
                    0 As ReceiveChildID,A.IndentChildID,A.PRMasterID,A.IndentMasterID,B.UnitID,B.StoreID,A.ProductID,A.UOMID,A.SectionID,
	                '' As BatchNo,Null As ManufacturingDate,Null As ExpireDate,
					A.IndentQty,
                    --IsNull(I.ReceiveQty,0) As ReceiveQty,
                    A.IndentQty - IsNull(I.ReceiveQtyPrev,0) As ReceiveQty,
                    IsNull(I.ReceiveQtyPrev,0) As ReceivedQty,
                    IsNull(I.ReceiveQtyPrev,0) As ReceiveQtyPrev,
					IsNull(I.StockQty,0) As StockQty,
	                A.IndentChildRemarks,A.IndentChildRemarks As ChildsRemarks,
	                C.ProductName,D.UOM,E.SectionName,F.UnitName,G.StoreName,B.IndentTypeID,H.IndentTypeName,B.IndentNo
                From CS_IndentChild A
                Inner Join CS_IndentMaster B On A.IndentMasterID = B.IndentMasterID
                Inner Join bas_CS_ProductMaster C On A.ProductID = C.ProductID
                Inner Join bas_UOM D On A.UOMID = D.UOMID
                Left Join {DBSource.PDLHRM}..bas_Section E On A.SectionID = E.SectionID 
                Left Join bas_UnitInfo F On B.UnitID = F.UnitID
                Left Join bas_Store G On B.StoreID = G.StoreID
                Left Join bas_CS_IndentType H On B.IndentTypeID = H.IndentTypeID
                Left Join dbo.fn_CS_Receive(0,{User.UnitID},{User.UserID},'{IndentMasterID}',0) I On A.IndentChildID = I.IndentChildID
                Where A.IndentMasterID In({IndentMasterID})
                And A.IndentQty > IsNull(I.ReceiveQtyPrev,0)
                Order By A.RowSLNo;
            ";

            var soList = await _context.GetDataAsync<CS_ReceiveChild>(query);

            foreach (CS_ReceiveChild items in soList)
            {
                items.objProductList.id = items.ProductID;
                items.objProductList.text = items.ProductName;

                // DELETE START
                items.BatchNo = "ABC";
                items.ManufacturingDate = DateTime.Now;
                items.ExpireDate = DateTime.Now;
            }

            if (soList == null) return new List<CS_ReceiveChild>();

            return soList;
        }
        public List<TT_CS_ReceiveChild> GetReceiveChild(List<CS_ReceiveChild> Childs)
        {
            List<TT_CS_ReceiveChild> TTList = new List<TT_CS_ReceiveChild>();

            int Counter = 0;
            
            foreach (CS_ReceiveChild item in Childs)
            {
                ++Counter;

                TT_CS_ReceiveChild objItem = new TT_CS_ReceiveChild();
                objItem.ReceiveChildID = item.ReceiveChildID;
                objItem.StoreID = item.StoreID;
                objItem.UnitID = item.UnitID;
                objItem.ReceiveMasterID = item.ReceiveMasterID;
                objItem.IndentMasterID = item.IndentMasterID;
                objItem.IndentChildID = item.IndentChildID;
                objItem.ProductID = item.ProductID;
                objItem.UOMID = item.UOMID;
                objItem.BatchNo = item.BatchNo;
                objItem.ManufacturingDate = item.ManufacturingDate;
                objItem.ExpireDate = item.ExpireDate;
                objItem.SectionID = item.SectionID;
                objItem.LocationID = item.objLocationList.id;
                objItem.RackID = item.objRackList.id;
                objItem.ReceiveQty = item.ReceiveQty;
                objItem.RejectionQty = item.RejectionQty;
                objItem.DamageQty = item.DamageQty;
                objItem.ChildsRemarks = item.ChildsRemarks;
                objItem.QCStatus = item.objQCStatusList.id;
                objItem.QCBy = item.objQCByList.id;
                objItem.QCDate = item.QCDate;
                objItem.QCRemarks = item.QCRemarks;
                objItem.RowSLNo = item.RowSLNo;
                objItem.RowSLNoNew = Counter;
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
