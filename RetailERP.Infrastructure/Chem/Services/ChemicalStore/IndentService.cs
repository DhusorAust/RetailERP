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
    public class IndentService : IIndentService
    {
        private readonly DapperContext<CS_IndentMaster> _context;
        private readonly SqlConnection _connection; 
        public IndentService(DapperContext<CS_IndentMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        private string CommonQuery = $@"
            Select Cast(StoreID As varchar) id, StoreName as [text] 
            From bas_Store
            Where IsActive = 1 And StoreID In(1);

            Select Cast(UnitID As varchar) id,UnitName as [text] 
            From dbo.bas_UnitInfo
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

            -- Product
            SELECT
	            ProductID AS id, ProductName AS [text], pm.UOMID As valueA, u.UOM As valueB
            FROM 
	            bas_CS_ProductMaster pm
	            Left Join bas_UOM u On u.UOMID = pm.UOMID
            Where 
	            pm.IsActive = 1
            ORDER BY ProductName; ";
        public async Task<GridEntity<CS_IndentMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "";

            if (status == Status.PENDING)
            {
                sql = $@"
                    Select 
	                    IsNull(PRM.PRMasterID,0) As PRMasterID, IsNull(PRM.SSReturnMasterID,0) As SSReturnMasterID, 0 As ReactorDeliveryMasterID, 
	                    PRM.PRNo, Replace(Convert(varchar, PRM.PRDate, 106), ' ','-')PRDate, PRM.DepartmentID, BD.DepartmentName,
	                    PRM.SectionID, BS.SectionName, PRM.SubSectionID, BSS.SubSectionName, PRM.PRBy, E.EmployeeName As PRByName, 
	                    PRM.ConcernPersonID, CP.EmployeeName AS ConcernPerson, PRM.PRRemarks, PRM.StoreID, J.StoreName, 
                        PRM.UnitID, K.UnitName
                    From 
	                    CS_PRMaster PRM 
	                    Left Join ERPDB..emp_EmploymentInfo E On E.EmployeeID = PRM.PRBy 
	                    Left Join ERPDB..bas_Department BD On BD.DepartmentID = PRM.DepartmentID
	                    Left Join ERPDB..bas_Section BS On BS.SectionID = PRM.SectionID
	                    Left Join ERPDB..bas_SubSection BSS On BSS.SubSectionID = PRM.SubSectionID
	                    Left Join ERPDB..emp_EmploymentInfo CP On CP.EmployeeID = PRM.ConcernPersonID 
	                    Left Join bas_Store J On PRM.StoreID = J.StoreID
	                    Left Join bas_UnitInfo K On PRM.UnitID = K.UnitID 
                    WHERE  
                        PRM.Approved = 1 
                        And PRM.Reject = 0 
                        AND PRM.PRMasterID NOT IN(SELECT PRMasterID FROM CS_IndentMaster) ";

                orderBy = "PRMasterID Desc";
            } 
            else if (status == Status.EDIT)
            {
                condition = $@" And im.SendForApproval = 0 And im.Approved = 0 And im.Reject = 0 "; 
            }
            else if (status == Status.PROPOSED_FOR_APPROVAL)
            {
                condition = $@" And im.SendForApproval = 1 And im.Approved = 0 And im.Reject = 0 "; 
            }
            else if (status == Status.APPROVED)
            {
                condition = $@" And im.SendForApproval = 1 And im.Approved = 1 And im.Reject = 0 ";
            }
            else if (status == Status.REJECT)
            {
                condition = $@" im.Reject = 1 ";
            }
            else if (status == Status.ALL)
            {
                condition = $@" ";
            } 
            if (status != Status.PENDING)
            {
                sql = $@"
                    Select 
	                    im.IndentMasterID, IsNull(pm.SSReturnMasterID,0) As SSReturnMasterID, pm.PRMasterID, pm.PRNo, pm.PRDate, 
	                    Replace(Convert(varchar,im.IndentDate,106), ' ','-')IndentDate, im.IndentNo, im.IndentTypeID, it.IndentTypeName, 
	                    (eipr.EmployeeNo + '-' + eipr.EmployeeName)PRByName, (eiind.EmployeeNo + '-' + eiind.EmployeeName)IndentByName, 
	                    im.Remarks, pm.StoreID, s.StoreName, pm.UnitID, ui.UnitName 
                    From 
	                    CS_IndentMaster im  
	                    Left Join bas_CS_IndentType it On it.IndentTypeID = im.IndentTypeID
	                    Left Join CS_PRMaster pm On pm.PRMasterID = im.PRMasterID  
	                    Left Join ERPDB..emp_EmploymentInfo eipr On eipr.EmployeeID = pm.PRBy 
	                    Left Join ERPDB..emp_EmploymentInfo eiind On eiind.EmployeeID = im.IndentBy 
	                    Left Join bas_Store s On pm.StoreID = s.StoreID
	                    Left Join bas_UnitInfo ui On pm.UnitID = ui.UnitID
                    Where 
	                    im.IsActive = 1 {condition} ";

                orderBy = "IndentMasterID Desc";
            }
            return await _context.GetKendoGridDataAsync<CS_IndentMaster>(options, sql, orderBy, "");
        }
        public async Task<CS_IndentMaster> GetNew(int id)
        {
            var query =
                $@"  
                -- Master 
                Select A.*,PRRemarks As IndentRemarks 
                From CS_PRMaster A 
                Where A.PRMasterID = {id};

                -- Child                 
                Select 
					b.IndentChildID,b.PRMasterID,b.IndentMasterID,b.SSReturnChildID,b.ProductID,b.UOMID,b.ValidityDate,b.SectionID,b.PRQty
					,iif( IsNull(b.IndentMasterID,0) = 0 ,b.PRQty,b.IndentQty) As IndentQty
					,b.PRChildRemarks,b.IndentChildRemarks,b.RowSLNo,b.IsDeleted
					, prcm.ProductName, u.UOM, s.SectionName  
                From
                    CS_PRMaster a
                    Inner Join CS_IndentChild b On b.PRMasterID = a.PRMasterID
                    Inner Join bas_CS_ProductMaster prcm On prcm.ProductID = b.ProductID
                    Inner Join bas_UOM u On u.UOMID = b.UOMID
                    Left Join ERPDB..bas_Section s On s.SectionID = b.SectionID 
                Where 
                    b.PRMasterID = {id} And b.IsDeleted = 0
                Order By b.RowSLNo Asc;

                {CommonQuery} ";

            CS_IndentMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query); 
                data = await records.ReadFirstOrDefaultAsync<CS_IndentMaster>();
                data.Childs = records.Read<CS_IndentChild>().ToList();

                data.StoreList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.IndentByList = records.Read<DropDownList>().ToList();
                data.IndentTypeList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();

                foreach (CS_IndentChild items in data.Childs)
                {
                    items.objProductList.id = items.ProductID;
                    items.objProductList.text = items.ProductName; 
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
        public async Task<CS_IndentMaster> GetEdit(int id)
        {
            var query =
                $@" 
                -- Master
                Select im.*, pm.PRNo, pm.PRDate, it.IndentTypeName 
                From 
	                CS_IndentMaster im
	                Left Join CS_PRMaster pm On pm.PRMasterID = im.PRMasterID
                    Left Join bas_CS_IndentType it On it.IndentTypeID = im.IndentTypeID
                Where im.IndentMasterID = {id};

                -- Details
                Select 
                    ic.*, prcm.ProductName, u.UOM, s.SectionName  
                From
                    CS_IndentMaster im 
                    Inner Join CS_IndentChild ic On ic.IndentMasterID = im.IndentMasterID
                    Inner Join bas_CS_ProductMaster prcm On prcm.ProductID = ic.ProductID
                    Inner Join bas_UOM u On u.UOMID = ic.UOMID
                    Left Join ERPDB..bas_Section s On s.SectionID = ic.SectionID 
                Where 
                    im.IndentMasterID = {id} And ic.IsDeleted = 0
                Order By ic.RowSLNo Asc;


                {CommonQuery} ";

            CS_IndentMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<CS_IndentMaster>();
                data.Childs = records.Read<CS_IndentChild>().ToList();
                data.StoreList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.IndentByList = records.Read<DropDownList>().ToList();
                data.IndentTypeList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();
                foreach (CS_IndentChild items in data.Childs)
                {
                    items.objProductList.id = items.ProductID;
                    items.objProductList.text = items.ProductName;
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
        public async Task<CS_IndentMaster> SaveAsync(CS_IndentMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
				List<TT_CS_IndentChild> TTList = GetIndentChild(entity.Childs);
				DataSet DetailsData = CommonFunction.ToDataSet(TTList);
				var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
				parameters = new DynamicParameters(); 

				parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@IndentMasterID", entity.IndentMasterID);
                parameters.Add("@UnitID", entity.UnitID);
                parameters.Add("@StoreID", entity.StoreID);
                parameters.Add("@PRMasterID", entity.PRMasterID);
                parameters.Add("@IndentDate", entity.IndentDate);
                parameters.Add("@IndentTypeID", entity.IndentTypeID);
                parameters.Add("@IndentBy", entity.IndentBy);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@SendForApproval", entity.SendForApproval);
                parameters.Add("@Approved", entity.Approved); 
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@ReviseReason", entity.ReviseReason); 
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);

				parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@Message", dbType: DbType.String, size: -1, direction: ParameterDirection.Output);
                entity.NoofRows = await _context.ExecuteAsync("spSet_CS_Indent", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");
                entity.Message = parameters.Get<string>("@Message");


                if (entity.NoofRows > 0 && entity.ResultId > 0)
                {
                    entity.Message = entity.Message;
                }
                else
                {
                    entity.Message = entity.Message;
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
        public List<TT_CS_IndentChild> GetIndentChild(List<CS_IndentChild> Childs)
        {
            List<TT_CS_IndentChild> TTList = new List<TT_CS_IndentChild>();

            foreach (CS_IndentChild item in Childs)
            {
                TT_CS_IndentChild objItem = new TT_CS_IndentChild(); 
                objItem.IndentChildID = item.IndentChildID;
                objItem.PRMasterID = item.PRMasterID;
                objItem.IndentMasterID = item.IndentMasterID; 
                objItem.SSReturnChildID = item.SSReturnChildID;
                objItem.ProductID = item.ProductID;
                objItem.UOMID = item.UOMID;
                objItem.ValidityDate = item.ValidityDate;
                objItem.SectionID = item.SectionID;
                objItem.PRQty = item.PRQty;
                objItem.IndentQty = item.IndentQty;
                objItem.PRChildRemarks = item.PRChildRemarks;
                objItem.IndentChildRemarks = item.IndentChildRemarks;
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
