using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Chem.Setting;
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
    public class BuyerRefService : IBuyerRefService
    {
        private readonly DapperContext<Mkt_BuyerRefMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public BuyerRefService(DapperContext<Mkt_BuyerRefMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@"
            -- Buyer
            Select BrandID As id, BrandName As [text] From bas_Brand Where IsActive = 1 AND Approved = 1;;

            -- Product
            Select 
	            pm.ProductID As id, pm.ProductName As [text], pc.ProductCategoryID As valueA, pc.ProductCategoryName As valueB, pt.ProductTypeID As valueC, 
	            pt.ProductTypeName As valueD
            From 
	            bas_CS_ProductMaster pm 
	            Inner Join bas_CS_ProductType pt On pt.ProductTypeID = pm.ProductTypeID
	            Inner Join bas_CS_ProductCategory pc On pc.ProductCategoryID = pm.ProductCategoryID 
            Where 
	            pm.IsActive = 1 AND pm.Approved = 1;

            -- Season
            Select SeasonID As id, SeasonName As [text] From bas_Season Where IsActive = 1 AND Approved = 1;

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

            -- Color
            Select ColorID As id, ColorName As [text] From bas_Color Where IsActive = 1 AND Approved = 1  ";  
        public async Task<GridEntity<Mkt_BuyerRefMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "BuyerRefMasterID Desc";  
            string condition = ""; 

            if (status == Status.EDIT)
            { 
                condition = $@" AND brm.Approved = 0 And brm.Reject = 0"; 
            }
            else if (status == Status.APPROVED)
            {
                condition = $@" AND brm.Approved = 1 And brm.Reject = 0 ";
            }
            else if (status == Status.REJECT)
            {
                condition = $@" AND brm.Reject = 1 ";
            }
            else
            {
                condition = $@" And brm.Reject = 0 ";
            } 

            sql = $@"
                Select 
	                brm.*, c.ColorName, b.BrandName As BuyerName, pm.ProductName, pc.ProductCategoryName, pt.ProductTypeName, s.SeasonName,
	                (ei.EmployeeNo + '-' + ei.EmployeeName)MktPersonName, tpf.BuyerRefFiles1, tpf.BuyerRefFiles2, tpf.BuyerRefFiles3,
	                tpf.BuyerRefFiles4, tpf.BuyerRefFiles5, tpf.BuyerRefFiles6, tpf.BuyerRefFiles7, tpf.BuyerRefFiles8, tpf.BuyerRefFiles9,
	                tpf.BuyerRefFiles10
                From 
	                mkt_BuyerRefMaster brm 
	                Left Join bas_Brand b On b.BrandID = brm.BuyerID 
	                Left Join bas_CS_ProductMaster pm On pm.ProductID = brm.FinishedProductID 
	                Left Join bas_CS_ProductCategory pc On pc.ProductCategoryID = brm.FinishedProductCategoryID 
	                Left Join bas_CS_ProductType pt On pt.ProductTypeID = brm.FinishedProductTypeID 
	                Left Join bas_Season s On s.SeasonID = brm.SeasonID 
	                Left Join {DBSource.PDLHRM}..emp_EmploymentInfo ei On ei.EmployeeID = brm.MarketingPersonID
	                Left Join stuff_mkt_BuyerRefFiles tpf On tpf.BuyerRefMasterID = brm.BuyerRefMasterID 
	                OUTER APPLY 
	                (
		                SELECT
			                STRING_AGG(x.ColorName, ', ') WITHIN GROUP (ORDER BY x.ColorName) AS ColorName
		                FROM 
		                (
			                SELECT DISTINCT c.ColorName
			                FROM 
				                mkt_BuyerRefColorInfo bci
				                Left Join bas_Color c ON c.ColorID = bci.ColorID
			                WHERE 
				                bci.BuyerRefMasterID = brm.BuyerRefMasterID
                                AND bci.IsDeleted = 0
		                ) AS x
	                ) AS c
                WHERE  
                    1 = 1 {condition} "; 

            return await _context.GetKendoGridDataAsync<Mkt_BuyerRefMaster>(options, sql, orderBy, "");
        }
        public async Task<Mkt_BuyerRefMaster> GetNew()
        {
            var query = $@" {CommonQuery}; ";
            var data = new Mkt_BuyerRefMaster();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);

                data.BuyerList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();
                data.SeasonList = records.Read<DropDownList>().ToList();
                data.MarketingPersonList = records.Read<DropDownList>().ToList(); 
                data.ColorList = records.Read<DropDownList>().ToList(); 

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
        public async Task<Mkt_BuyerRefMaster> GetEdit(int id)
        {
            var query =
               $@"
                -- Master
                SELECT 
                    brm.*,
                    STUFF(
                        (
                            SELECT ',' + CAST(c2.ColorID AS varchar(20))
                            FROM mkt_BuyerRefColorInfo brc2
                            INNER JOIN bas_Color c2 ON c2.ColorID = brc2.ColorID
                            WHERE brc2.IsDeleted = 0
                              AND brc2.BuyerRefMasterID = brm.BuyerRefMasterID
                            FOR XML PATH(''), TYPE
                        ).value('.', 'nvarchar(max)'),
                        1, 1, ''
                    ) AS ColorIDs, pc.ProductCategoryName, pt.ProductTypeName
                FROM 
                    mkt_BuyerRefMaster brm
                    Inner Join bas_CS_ProductCategory pc On pc.ProductCategoryID = brm.FinishedProductCategoryID
                    Inner Join bas_CS_ProductType pt On pt.ProductTypeID = brm.FinishedProductTypeID
                WHERE brm.BuyerRefMasterID = {id};

                {CommonQuery}; ";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Mkt_BuyerRefMaster data = await records.ReadFirstOrDefaultAsync<Mkt_BuyerRefMaster>(); 
                data.BuyerList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();
                data.SeasonList = records.Read<DropDownList>().ToList();
                data.MarketingPersonList = records.Read<DropDownList>().ToList();
                data.ColorList = records.Read<DropDownList>().ToList();
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

        public async Task<Mkt_BuyerRefMaster> SaveAsync(Mkt_BuyerRefMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@BuyerRefMasterID", entity.BuyerRefMasterID);
                parameters.Add("@BuyerRefMasterID_RefID", entity.BuyerRefMasterID_RefID); 
                parameters.Add("@BuyerRef", entity.BuyerRef);
                parameters.Add("@BuyerRefDate", entity.BuyerRefDate);
                parameters.Add("@BuyerID", entity.BuyerID);
                parameters.Add("@FinishedProductCategoryID", entity.FinishedProductCategoryID);
                parameters.Add("@FinishedProductTypeID", entity.FinishedProductTypeID);
                parameters.Add("@FinishedProductID", entity.FinishedProductID);
                parameters.Add("@SeasonID", entity.SeasonID);
                parameters.Add("@MarketingPersonID", entity.MarketingPersonID);
                parameters.Add("@Remarks", entity.Remarks); 
                parameters.Add("@ColorIDs", entity.ColorIDs);                
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@Approved", entity.Approved); 
                parameters.Add("@Revise", entity.IsRevise); 
                parameters.Add("@ReviseReason", entity.ReviseReason);
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync("spSet_mkt_BuyerRefMaster", parameters, 30, CommandType.StoredProcedure);
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
                entity.Message = ex.Message;
            }
            finally
            {
                parameters = null;
            }
            return entity;
        }
        //public async Task<CS_PRMaster> SaveAsync(CS_PRMaster entity)
        //{
        //    var parameters = (dynamic)null;
        //    try
        //    {
        //        List<TT_CS_IndentChild> TTList = GetIndentChild(entity.Childs);
        //        DataSet DetailsData = CommonFunction.ToDataSet(TTList);
        //        var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
        //        parameters = new DynamicParameters(); 

        //        parameters.Add("@SaveOption", entity.SaveOption);
        //        parameters.Add("@PRMasterID", entity.PRMasterID);
        //        parameters.Add("@UnitID", entity.UnitID);
        //        parameters.Add("@StoreID", entity.StoreID);
        //        parameters.Add("@PRDate", entity.PRDate);
        //        parameters.Add("@PRBy", entity.PRBy);
        //        parameters.Add("@ConcernPersonID", entity.ConcernPersonID);
        //        parameters.Add("@DepartmentID", entity.DepartmentID);
        //        parameters.Add("@SectionID", entity.SectionID);
        //        parameters.Add("@SubSectionID", entity.SubSectionID); 
        //        parameters.Add("@SSReturnMasterID", entity.SSReturnMasterID); 
        //        parameters.Add("@PRRemarks", entity.PRRemarks);
        //        parameters.Add("@Approved", entity.Approved);
        //        parameters.Add("@RejectReason", entity.RejectReason);
        //        parameters.Add("@ReviseReason", entity.ReviseReason);

        //        parameters.Add("@UserBy", entity.UserBy);
        //        parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
        //        parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
        //        parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

        //        entity.NoofRows = await _context.ExecuteAsync("spSet_CS_PR", parameters, 30, CommandType.StoredProcedure);
        //        entity.ResultId = parameters.Get<int>("@IdentityValue");
        //        entity.ErrorNo = parameters.Get<int>("@ErrNo");

        //        if (entity.NoofRows > 0 && entity.ResultId > 0)
        //        {
        //            entity.Message = "Save operation successful.";
        //        }
        //        else
        //        {
        //            entity.Message = "Save operation failed.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        entity.Message = "Error : " + ex.Message;
        //    }
        //    finally
        //    {
        //        parameters = null;
        //    }
        //    return entity;
        //} 
        //public List<TT_CS_IndentChild> GetIndentChild(List<Mkt_BuyerRefColorInfo> Childs)
        //{ 
        //    List<TT_CS_IndentChild> TTList = new List<TT_CS_IndentChild>();

        //    foreach (Mkt_BuyerRefColorInfo item in Childs)
        //    { 
        //        TT_CS_IndentChild objItem = new TT_CS_IndentChild();
        //        objItem.IndentChildID = item.IndentChildID;
        //        objItem.PRMasterID = item.PRMasterID;
        //        objItem.SSReturnChildID = item.SSReturnChildID;
        //        objItem.ProductID = item.objProductList.id;
        //        objItem.UOMID = item.UOMID; 
        //        objItem.SectionID = item.objSectionList.id; 
        //        objItem.PRQty = item.PRQty; 
        //        objItem.PRChildRemarks = item.PRChildRemarks; 
        //        objItem.RowSLNo = item.RowSLNo;
        //        TTList.Add(objItem);
        //    }
        //    return TTList;
        //}
        //public void Dispose()
        //{
        //    //throw new NotImplementedException();
        //} 
    }
}