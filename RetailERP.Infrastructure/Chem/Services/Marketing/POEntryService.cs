using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager; 
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.Marketing
{   
    public class POEntryService : IPOEntryService
    {
        private readonly DapperContext<Mkt_POMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public POEntryService(DapperContext<Mkt_POMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        private string CommonQuery = $@"
            Select OrderModeID	As id, OrderMode As [text] From bas_OrderMode Where IsActive = 1;
            -- PO
            Select 
	            pm.POMasterID As id, pm.PONo As [text], brm.BuyerRef As valueA, c.ColorName As valueB
            From 
	            mkt_POMaster pm
	            Inner Join mkt_POChild pc On pc.POMasterID = pm.POMasterID
	            Inner Join mkt_BuyerRefMaster brm On brm.BuyerRefMasterID = pc.BuyerRefMasterID
	            Inner Join mkt_BuyerRefColorInfo brci On brci.BuyerRefMasterID = brm.BuyerRefMasterID And brci.BuyerRefColorID = pc.BuyerRefColorID
	            Inner Join bas_Color c On c.ColorID = brci.ColorID
            Where pm.Approved = 1;

            Select CurrencyID As id, CurrencyName As [text] From bas_Currency Where IsActive = 1;


            -- Buyer Ref.
            Select BuyerRefMasterID As id, BuyerRef As [text], ChemicalCode As valueA ,dbo.fn_Conditional_Stuffing_Text(BuyerRefMasterID,13,1) As valueB
            From mkt_BuyerRefMaster Where Approved = 1;
            Select PaymentTermsID As id, PaymentTerms As [text] From bas_PaymentTerms Where IsActive = 1;
            Select IncotermID As id, Incoterm As [text] From bas_Incoterm Where IsActive = 1;

            -- Marketing Person
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
            Select 
	            pc.ProductCategoryID As valueA, pc.ProductCategoryName As valueB, pt.ProductTypeID As valueC, 
	            pt.ProductTypeName As valueD, pm.ProductID As id, pm.ProductName As [text]
            From 
				bas_CS_ProductMaster pm
	            join bas_CS_ProductCategory pc on pc.ProductCategoryID=pm.ProductCategoryID
	            Inner Join bas_CS_ProductType pt On pt.ProductTypeID = pm.ProductTypeID
	            --Inner Join bas_CS_ProductMaster pm On pm.ProductCategoryID = pc.ProductCategoryID AND pm.ProductTypeID = pt.ProductTypeID
            Where 
	            pm.IsActive = 1 AND pm.Approved = 1;

            Select PackagingTypeID As id, PackagingName As [text] From bas_PackagingType Where IsActive = 1;
            Select UOMID As id, UOM As [text] From bas_UOM Where IsActive = 1;
            Select ShippingDestinationID As id, ShippingDestination As [text] From bas_ShippingDestination Where IsActive = 1;
            Select ShipModeID As id, ShipMode As [text] From bas_ShipMode Where IsActive = 1; ";
        public async Task<GridEntity<Mkt_POMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "";


            sql = $@"
                Select 
	                A.POMasterID,A.OrderTypeID,A.OrderModeID,A.PONo,A.PODate,B.OrderMode,C.CurrencyName,D.PaymentTerms,E.Incoterm,
	                F.EmployeeName As POByName,A.TotalPOQty,a.Remarks,dbo.fn_Conditional_Stuffing_Text(A.POMasterID,14,1) As BuyerRef
                From mkt_POMaster A
                Inner Join bas_OrderMode B On A.OrderModeID = B.OrderModeID
                Inner Join bas_Currency C On A.CurrencyID = C.CurrencyID
                Inner Join bas_PaymentTerms D On A.PaymentTermsID = D.PaymentTermsID
                Inner Join bas_Incoterm E On A.IncotermID = E.IncotermID
                left Join ERPDB.dbo.emp_EmploymentInfo F On A.POBy = F.EmployeeID
                Where A.IsActive = 1
                ";
            orderBy = "POMasterID Desc";

            if (status == Status.PENDING)
            {
                sql = $@"
                Select
	                D.BuyerRef,B.SDRFNo,D.ChemicalCode,F.ProductName,E.ColorName,G.BuyerName,H.SeasonName,B.RequiredDate,
	                D.BuyerRefMasterID,B.SDRFMasterID,D.FinishedProductID,E.ColorID,G.BuyerID,H.SeasonID
                From rnd_FeedbackMaster A
                Inner Join mkt_SDRFMaster B On A.SDRFMasterID = B.SDRFMasterID
                Inner Join mkt_BuyerRefColorInfo C On B.BuyerRefColorID = C.BuyerRefColorID
                Inner Join mkt_BuyerRefMaster D On C.BuyerRefMasterID = D.BuyerRefMasterID
                Inner Join bas_Color E On C.ColorID = E.ColorID
                Inner Join bas_CS_ProductMaster F On D.FinishedProductID = F.ProductID
                Inner Join bas_Buyer G On D.BuyerID = G.BuyerID
                Inner Join bas_Season H On D.SeasonID = H.SeasonID
                Where A.FinalApproved = 1";
                orderBy = "SDRFMasterID Desc";
            }
            if (status == Status.PROPOSED_FOR_APPROVAL)
            {
                condition = $@" And A.SendForApproval = 1 And A.Approved = 0 AND A.Reject = 0";
            }

            
            else if (status == Status.EDIT)
            {
                condition = $@" And A.SendForApproval = 0 And A.Approved = 0 AND A.Reject = 0";
            }
            else if (status == Status.APPROVED)
            {
                condition = $@" And A.SendForApproval = 1 And A.Approved = 1 AND A.Reject = 0";
            }
            else if (status == Status.REJECT)
            {
                condition = $@" And A.Reject = 1";
            }
            else if (status == Status.ALL)
            {
                condition = $@" ";
            }

            sql = sql + " " + condition;


            

            return await _context.GetKendoGridDataAsync<Mkt_POMaster>(options, sql, orderBy, "");
        }
        public async Task<Mkt_POMaster> GetNew()
        {
    //        var masterQuery = "Select 0 As POMasterID,GetDate() As PODate;";
    //        if(SDRFMasterID > 0)
    //        {
				//masterQuery = $@"
    //            Select
    //                A.BuyerRefMasterID,A.SDRFMasterID,B.BuyerRefColorID
    //            From rnd_FeedbackMaster A
    //            Inner Join mkt_SDRFMaster B On A.SDRFMasterID = B.SDRFMasterID
    //            Where A.BuyerRefMasterID = {BuyerRefMasterID} And A.SDRFMasterID = {SDRFMasterID} And FinalApproved = 1;";
    //        }

            var query =
                $@" 
                -- Master
                                
                {CommonQuery}

";

            Mkt_POMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                //data = await records.ReadFirstOrDefaultAsync<Mkt_POMaster>(); 
                if(data != null)
                {
                    data.OrderModeList = records.Read<DropDownList>().ToList();
                    data.POMasterList = records.Read<DropDownList>().ToList();
                    data.CurrencyList = records.Read<DropDownList>().ToList();
                    data.BuyerRefMasterList = records.Read<DropDownList>().ToList();
                    
                    data.PaymentTermsList = records.Read<DropDownList>().ToList();
                    data.IncotermList = records.Read<DropDownList>().ToList();
                    data.POByList = records.Read<DropDownList>().ToList();
                    data.ProductList = records.Read<DropDownList>().ToList();
                    data.PackagingTypeList = records.Read<DropDownList>().ToList();
                    data.UOMList = records.Read<DropDownList>().ToList();
                    data.ShippingDestinationList = records.Read<DropDownList>().ToList();
                    data.ShipModeList = records.Read<DropDownList>().ToList();
                    //data.BuyerRefColorList = records.Read<DropDownList>().ToList();


                    //// Dummy Data - Start
                    //data.PONo = "ABC";
                    //data.PaymentTermsID = 4;
                    //data.IncotermID = 5;
                    //data.CurrencyID = 1;
                    //data.OrderModeID = 1;
                    //data.BuyerRefMasterID = 4;
                    //data.BuyerRefColorID = 8;
                    //Mkt_POChild objItem = new Mkt_POChild();
                    //objItem.POChildID = 0;
                    //objItem.POMasterID = 0;
                    //objItem.SDRFMasterID = 0;
                    //objItem.FeedbackMasterID = 0;
                    //objItem.FeedbackChildID = 0;
                    //objItem.BuyerRefMasterID = 4;
                    //objItem.BuyerRefColorID = 8;
                    //objItem.ShippingDestinationID = 2;
                    //objItem.FinishedProductID = 125;
                    //objItem.FinishedProductDescription = "";
                    //objItem.PackagingTypeID = 1;
                    //objItem.SizeGram = 10;
                    //objItem.UOMID = 3;
                    //objItem.ShipmentDate = System.DateTime.Today;
                    //objItem.ExFactoryDate = System.DateTime.Today;
                    //objItem.ShipModeID = 1;
                    //objItem.POQty = 2;
                    //objItem.Remarks = "aa";
                    //objItem.RowSLNo = 1;
                    //objItem.IsDeleted = 0;
                    //objItem.ProductName = "Acid Cellulase ACE- 120";
                    //data.Childs.Add(objItem);
                    //// Dummy Data - End
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
        public async Task<Mkt_POMaster> GetEdit(int id)
        {
            var query =
               $@"
                -- Master
                Select Top 1 A.*,B.BuyerRefMasterID,B.BuyerRefColorID,C.BuyerRef,C.ChemicalCode,E.ColorName,dbo.fn_Conditional_Stuffing_Text(B.BuyerRefMasterID,13,1) as BuyerRefColorIDs
                From mkt_POMaster A
                Inner Join mkt_POChild B On A.POMasterID = B.POMasterID
                Inner Join mkt_BuyerRefMaster C On B.BuyerRefMasterID = C.BuyerRefMasterID
                Inner Join mkt_BuyerRefColorInfo D On B.BuyerRefColorID = D.BuyerRefColorID
                Inner Join bas_Color E On D.ColorID = E.ColorID
                Where A.POMasterID = {id};

                -- Childs
                Select A.*,B.BuyerRef,I.ColorName,D.ShippingDestination,E.ProductName,F.PackagingName,G.UOM,H.ShipMode From mkt_POChild A
                Left Join mkt_BuyerRefMaster B On A.BuyerRefMasterID = B.BuyerRefMasterID
                Left Join mkt_BuyerRefColorInfo C On A.BuyerRefColorID = C.BuyerRefColorID
                Left Join bas_ShippingDestination D On A.ShippingDestinationID = D.ShippingDestinationID
                Left Join bas_CS_ProductMaster E On A.FinishedProductID = E.ProductID
                Left Join bas_PackagingType F On A.PackagingTypeID = F.PackagingTypeID
                Left Join bas_UOM G On A.UOMID = G.UOMID
                Left Join bas_ShipMode H On A.ShipModeID = H.ShipModeID
                Left  Join bas_Color I On C.ColorID = I.ColorID
                Where A.POMasterID = {id} And IsNull(A.IsDeleted,0) = 0;

                {CommonQuery}; 

                Select A.BuyerRefColorID as id,C.ColorName as text From mkt_BuyerRefColorInfo A
                Inner Join
                (
	                Select BuyerRefMasterID From mkt_POChild
	                Where POMasterID = {id} And IsDeleted = 0
	                Group By BuyerRefMasterID
                )B On A.BuyerRefMasterID = B.BuyerRefMasterID
                Inner Join bas_Color C On A.ColorID = C.ColorID
                Where A.IsDeleted = 0;
            ";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Mkt_POMaster data = await records.ReadFirstOrDefaultAsync<Mkt_POMaster>();
                data.Childs = records.Read<Mkt_POChild>().ToList();

                if (data != null)
                {
                    data.OrderModeList = records.Read<DropDownList>().ToList();
                    data.POMasterList = records.Read<DropDownList>().ToList();
                    data.CurrencyList = records.Read<DropDownList>().ToList();
                    data.BuyerRefMasterList = records.Read<DropDownList>().ToList();
                    
                    data.PaymentTermsList = records.Read<DropDownList>().ToList();
                    data.IncotermList = records.Read<DropDownList>().ToList();
                    data.POByList = records.Read<DropDownList>().ToList();
                    data.ProductList = records.Read<DropDownList>().ToList();
                    data.PackagingTypeList = records.Read<DropDownList>().ToList();
                    data.UOMList = records.Read<DropDownList>().ToList();
                    data.ShippingDestinationList = records.Read<DropDownList>().ToList();
                    data.ShipModeList = records.Read<DropDownList>().ToList();
                    data.BuyerRefColorList = records.Read<DropDownList>().ToList();
                    foreach (Mkt_POChild items in data.Childs)
                    {
                        items.objProductList.id = items.FinishedProductID;
                        items.objProductList.text = items.ProductName;

                        items.objPackagingTypeList.id = items.PackagingTypeID;
                        items.objPackagingTypeList.text = items.PackagingName;

                        items.objUOMList.id = items.UOMID;
                        items.objUOMList.text = items.UOM;

                        items.objShippingDestinationList.id = items.ShippingDestinationID;
                        items.objShippingDestinationList.text = items.ShippingDestination;

                        items.objShipModeList.id = items.ShipModeID;
                        items.objShipModeList.text = items.ShipMode;
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
        public async Task<Mkt_POMaster> SaveAsync(Mkt_POMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_Mkt_POChild> TTList = GetChilds(entity.Childs,entity);
                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);

                parameters.Add("@POMasterID", entity.POMasterID);
                parameters.Add("@OrderTypeID", entity.OrderTypeID);
                parameters.Add("@OrderModeID", entity.OrderModeID);
                parameters.Add("@PONo", entity.PONo);
                parameters.Add("@PODate", entity.PODate);
                parameters.Add("@PaymentTermsID", entity.PaymentTermsID);
                parameters.Add("@IncotermID", entity.IncotermID);
                parameters.Add("@CurrencyID", entity.CurrencyID);
                parameters.Add("@POBy", entity.POBy);
                parameters.Add("@BuyerRefMasterID_RPT", entity.BuyerRefMasterID_RPT);
                parameters.Add("@BuyerRefColorID_RPT", entity.BuyerRefColorID_RPT);
                parameters.Add("@POMasterID_RPT", entity.POMasterID_RPT);
                parameters.Add("@TotalPOQty", entity.TotalPOQty);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@POFIle", entity.POFIle);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@SendForApproval", entity.SendForApproval);
                parameters.Add("@ReviseNo", entity.ReviseNo);
                parameters.Add("@Approved", entity.Approved);                
                parameters.Add("@Reject", entity.Reject);
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@Revise", entity.Revise);
                parameters.Add("@ReviseReason", entity.ReviseReason);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@RowToken", entity.RowToken, DbType.Binary, size: 8);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@Message", dbType: DbType.String, size: -1, direction: ParameterDirection.Output);
                entity.NoofRows = await _context.ExecuteAsync("spSet_Mkt_POMaster", parameters, 30, CommandType.StoredProcedure);
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
        public List<TT_Mkt_POChild> GetChilds(List<Mkt_POChild> Childs,Mkt_POMaster entity)
        {
            List<TT_Mkt_POChild> TTList = new List<TT_Mkt_POChild>();
            int Counter = 0;
            foreach (Mkt_POChild item in Childs)
            {
                ++Counter;
                TT_Mkt_POChild objItem = new TT_Mkt_POChild();

                objItem.POChildID = item.POChildID;
                objItem.POMasterID = item.POMasterID;
                objItem.SDRFMasterID = item.SDRFMasterID;
                objItem.FeedbackMasterID = item.FeedbackMasterID;
                objItem.FeedbackChildID = item.FeedbackChildID;
                objItem.BuyerRefMasterID = entity.BuyerRefMasterID;
                objItem.BuyerRefColorID = entity.BuyerRefColorID;
                objItem.ShippingDestinationID = item.objShippingDestinationList.id;
                objItem.FinishedProductID = item.objProductList.id;
                objItem.FinishedProductDescription = item.FinishedProductDescription;
                objItem.PackagingTypeID = item.objPackagingTypeList.id;
                objItem.SizeGram = item.SizeGram;
                objItem.UOMID = item.objUOMList.id;
                objItem.ShipmentDate = item.ShipmentDate;
                objItem.ExFactoryDate = item.ExFactoryDate;
                objItem.ShipModeID = item.objShipModeList.id;
                objItem.POQty = item.POQty;
                objItem.Remarks = item.Remarks;
                objItem.RowSLNo = Counter;
                objItem.IsDeleted = item.IsDeleted;

                TTList.Add(objItem);
            }
            return TTList;
        }
        public async Task<List<DropDownList>> GetColors(int BuyerRefMasterID)
        {
            var query =
            $@"

            Select 
	            brci.BuyerRefColorID As id, c.ColorName As [text]
            From 
	            mkt_BuyerRefMaster brm  
	            Inner Join mkt_BuyerRefColorInfo brci On brci.BuyerRefMasterID = brm.BuyerRefMasterID  
	            Inner Join bas_Color c On c.ColorID = brci.ColorID
            Where 
				brm.BuyerRefMasterID = {BuyerRefMasterID}
	            And brm.Approved = 1
	            And brm.IsActive = 1;
            ";
            try
            {
                var comList = await _context.GetDataAsync<DropDownList>(query);
                if (comList == null) return new List<DropDownList>();
                return comList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open) _connection.Close();
            }
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
