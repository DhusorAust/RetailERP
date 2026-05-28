using RetailERP.Core.Chem.ChemicalStore;
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
    public class SDRFService : ISDRFService
    {
        private readonly DapperContext<Mkt_SDRFMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public SDRFService(DapperContext<Mkt_SDRFMaster> context)
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

            Select PriorityID As id, PriorityName As [text] From bas_Priority Where IsActive = 1;

            Select ClaimID As id, ClaimName As [text] From bas_Claim Where IsActive = 1 And Approved = 1;

            Select FeelID As id, FeelName As [text] From bas_Feel Where IsActive = 1 And Approved = 1;

            Select FragranceID As id, FragranceName As [text] From bas_Fragrance Where IsActive = 1 And Approved = 1;

            Select RestrictionID As id, RestrictionName As [text] From bas_Restriction Where IsActive = 1 And Approved = 1;

            Select PackagingTypeID As id, PackagingName As [text] From bas_PackagingType Where IsActive = 1;            
            
            Select UOMID As id, UOM As [text] From bas_UOM Where IsActive = 1;  ";
        public async Task<GridEntity<Mkt_SDRFMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "";

            if (status == Status.PENDING)
            {
                sql = $@"
                    Select 
	                    Distinct brm.*, brc.BuyerRefColorID, c.ColorName, b.BrandName As BuyerName, pm.ProductName, pc.ProductCategoryName,
                        pt.ProductTypeName, s.SeasonName, (ei.EmployeeNo + '-' + ei.EmployeeName)MktPersonName 
                    From 
	                    mkt_BuyerRefMaster brm 
	                    Left Join mkt_BuyerRefColorInfo  brc On brc.BuyerRefMasterID = brm.BuyerRefMasterID
	                    Left Join bas_Color c ON c.ColorID = brc.ColorID
	                    Left Join bas_Brand b On b.BrandID = brm.BuyerID 
	                    Left Join bas_CS_ProductMaster pm On pm.ProductID = brm.FinishedProductID 
	                    Left Join bas_CS_ProductCategory pc On pc.ProductCategoryID = brm.FinishedProductCategoryID 
	                    Left Join bas_CS_ProductType pt On pt.ProductTypeID = brm.FinishedProductTypeID 
	                    Left Join bas_Season s On s.SeasonID = brm.SeasonID 
	                    Left Join ERPDB..emp_EmploymentInfo ei On ei.EmployeeID = brm.MarketingPersonID
                        Left Join mkt_SDRFMaster ms On brm.BuyerRefMasterID = ms.BuyerRefMasterID aND ms.BuyerRefColorID = brc.BuyerRefColorID -- New
                    WHERE 
                        brm.Approved = 1
	                    AND brm.IsActive = 1                        
                        And MS.BuyerRefMasterID Is Null And ms.BuyerRefColorID Is Null -- New
                    ";

                orderBy = "BuyerRefMasterID Desc";
            }
            else if (status == Status.EDIT)
            {
                condition = $@" And sm.SendForApproval = 0 And sm.Approved = 0 And sm.Reject = 0 ";
            }
            else if (status == Status.PROPOSED_FOR_APPROVAL)
            {
                condition = $@" And sm.SendForApproval = 1 And sm.Approved = 0 And sm.Reject = 0 ";
            }
            else if (status == Status.APPROVED)
            {
                condition = $@" And sm.SendForApproval = 1 And sm.Approved = 1 And sm.Reject = 0 ";
            }
            else if (status == Status.REJECT)
            {
                condition = $@" And sm.Reject = 1 ";
            }
            else if (status == Status.ALL)
            {
                condition = $@" ";
            }
            if (status != Status.PENDING)
            {
                sql = $@"
                    Select 
	                    Distinct sm.*, brm.ChemicalCode, brm.BuyerRef, clr.ColorName, (ei.EmployeeNo + '-' + ei.EmployeeName)SDRFByName, b.BrandName As BuyerName, 
	                    dt.DevelopmentType, mti.TeamName As MktTeamName, rti.TeamName As RnDTeamName, p.PriorityName, c.ClaimName,
	                    f.FeelName, frg.FragranceName, res.RestrictionName
                    From 
	                    mkt_SDRFMaster sm
	                    Left Join mkt_BuyerRefColorInfo  brc On brc.BuyerRefColorID = sm.BuyerRefColorID
	                    Left Join bas_Color clr ON clr.ColorID = brc.ColorID 
	                    Left Join {DBSource.PDLHRM}..emp_EmploymentInfo ei On ei.EmployeeID = sm.SDRFBy
	                    Left Join bas_Brand b On b.BrandID = sm.BuyerID 
	                    Left Join mkt_BuyerRefMaster brm On brm.BuyerRefMasterID = sm.BuyerRefMasterID 
	                    Left Join bas_DevelopmentType dt On dt.DevelopmentTypeID = sm.DevelopmentTypeID 
	                    Left Join {DBSource.PDLHRM}..bas_TeamInfo mti On mti.TeamID = sm.MktTeamID 
	                    Left Join {DBSource.PDLHRM}..bas_TeamInfo rti On rti.TeamID = sm.RnDTeamID 
	                    Left Join bas_Priority p On p.PriorityID = sm.PriorityID
	                    OUTER APPLY 
	                    (
		                    SELECT
			                    STRING_AGG(x.ClaimName, ', ') WITHIN GROUP (ORDER BY x.ClaimName) AS ClaimName
		                    FROM 
		                    (
			                    SELECT DISTINCT inn_c.ClaimName
			                    FROM 
				                    mkt_SDRFMaster inn_sm
				                    Left Join mkt_SDRFClaim inn_sc ON inn_sc.SDRFMasterID = inn_sm.SDRFMasterID
				                    Left Join bas_Claim inn_c ON inn_c.ClaimID = inn_sc.SDRFClaimID
			                    WHERE 
				                    inn_sm.SDRFMasterID = sm.SDRFMasterID
                                    AND inn_sc.IsDeleted = 0
		                    ) AS x
	                    ) AS c
	                    OUTER APPLY 
	                    (
		                    SELECT
			                    STRING_AGG(x.FeelName, ', ') WITHIN GROUP (ORDER BY x.FeelName) AS FeelName
		                    FROM 
		                    (
			                    SELECT DISTINCT inn_c.FeelName
			                    FROM 
				                    mkt_SDRFMaster inn_sm
				                    Left Join mkt_SDRFFeel inn_sc ON inn_sc.SDRFMasterID = inn_sm.SDRFMasterID
				                    Left Join bas_Feel inn_c ON inn_c.FeelID = inn_sc.SDRFFeelID
			                    WHERE 
				                    inn_sm.SDRFMasterID = sm.SDRFMasterID
                                    AND inn_sc.IsDeleted = 0
		                    ) AS x
	                    ) AS f
	                    OUTER APPLY 
	                    (
		                    SELECT
			                    STRING_AGG(x.FragranceName, ', ') WITHIN GROUP (ORDER BY x.FragranceName) AS FragranceName
		                    FROM 
		                    (
			                    SELECT DISTINCT inn_c.FragranceName
			                    FROM 
				                    mkt_SDRFMaster inn_sm
				                    Left Join mkt_SDRFFragrance inn_sc ON inn_sc.SDRFMasterID = inn_sm.SDRFMasterID
				                    Left Join bas_Fragrance inn_c ON inn_c.FragranceID = inn_sc.SDRFFragranceID
			                    WHERE 
				                    inn_sm.SDRFMasterID = sm.SDRFMasterID
                                    AND inn_sc.IsDeleted = 0
		                    ) AS x
	                    ) AS frg
	                    OUTER APPLY 
	                    (
		                    SELECT
			                    STRING_AGG(x.RestrictionName, ', ') WITHIN GROUP (ORDER BY x.RestrictionName) AS RestrictionName
		                    FROM 
		                    (
			                    SELECT DISTINCT inn_c.RestrictionName
			                    FROM 
				                    mkt_SDRFMaster inn_sm
				                    Left Join mkt_SDRFRestriction inn_sc ON inn_sc.SDRFMasterID = inn_sm.SDRFMasterID
				                    Left Join bas_Restriction inn_c ON inn_c.RestrictionID = inn_sc.SDRFRestrictionID
			                    WHERE 
				                    inn_sm.SDRFMasterID = sm.SDRFMasterID
                                    AND inn_sc.IsDeleted = 0
		                    ) AS x
	                    ) AS res
                    Where  
	                    sm.IsActive = 1 {condition} ";

                orderBy = "SDRFMasterID Desc";
            }
            return await _context.GetKendoGridDataAsync<Mkt_SDRFMaster>(options, sql, orderBy, "");
        }
        public async Task<Mkt_SDRFMaster> GetNew(int BuyerRefMasterID, int BuyerRefColorID)
        {
            var query =
                $@"  
                -- Master 
                Select 
	                brm.*, brc.BuyerRefColorID, b.BrandName As BuyerName, pm.ProductName, c.ColorName, s.SeasonName
                From 
	                mkt_BuyerRefMaster brm
	                Left Join mkt_BuyerRefColorInfo brc On brc.BuyerRefMasterID = brm.BuyerRefMasterID
	                Left Join bas_Brand b On b.BrandID = brm.BuyerID 
	                Left Join bas_CS_ProductMaster pm On pm.ProductID = brm.FinishedProductID
	                Left Join bas_Color c On c.ColorID = brc.ColorID
	                Left Join bas_Season s On s.SeasonID = brm.SeasonID
                Where 
	                brm.BuyerRefMasterID = {BuyerRefMasterID} AND brc.BuyerRefColorID = {BuyerRefColorID}; 
                {CommonQuery} ";

            Mkt_SDRFMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Mkt_SDRFMaster>();
                if (data != null)
                {
                    data.SDRFByList = records.Read<DropDownList>().ToList();
                    data.PriorityList = records.Read<DropDownList>().ToList();
                    data.ClaimList = records.Read<DropDownList>().ToList();
                    data.FeelList = records.Read<DropDownList>().ToList();
                    data.FragranceList = records.Read<DropDownList>().ToList();
                    data.RestrictionList = records.Read<DropDownList>().ToList();
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
        public async Task<Mkt_SDRFMaster> GetEdit(int id)
        {
            var query = $@"
                -- Master

                Select 
                    A.*, B.ChemicalCode, B.BuyerRef, E.BrandName As BuyerName, G.ProductName, H.ColorName, I.SeasonName
				    ,dbo.fn_Conditional_Stuffing_Text({id},3,1) As SDRFClaimIDs
				    ,dbo.fn_Conditional_Stuffing_Text({id},4,1) As SDRFFeelIDs
				    ,dbo.fn_Conditional_Stuffing_Text({id},5,1) As SDRFFragranceIDs
				    ,dbo.fn_Conditional_Stuffing_Text({id},6,1) As SDRFRestrictionIDs
                From 
                    mkt_SDRFMaster A
                    Inner Join mkt_BuyerRefMaster B On A.BuyerRefMasterID = B.BuyerRefMasterID
                    Inner Join mkt_BuyerRefColorInfo C On A.BuyerRefColorID = C.BuyerRefColorID
                    Left Join ERPDB..emp_EmploymentInfo D On A.SDRFBy = D.EmployeeID
                    Left Join bas_Brand E On E.BrandID = A.BuyerID 
                    Inner Join bas_Priority F On A.PriorityID = F.PriorityID
                    Inner Join bas_CS_ProductMaster G On B.FinishedProductID = G.ProductID
                    Inner Join bas_Color H On C.ColorID = H.ColorID
                    Inner Join bas_Season I On B.SeasonID = I.SeasonID
                Where SDRFMasterID = {id};

                -- Childs
                Select
                    A.SDRFChildID, A.SDRFMasterID, A.PackagingTypeID, A.SizeGram, A.UOMID, A.Qty, A.IsPrimary, A.Remarks
                    ,B.PackagingName As PackagingType, C.UOM 
                From 
                    mkt_SDRFChild A
                    Inner Join bas_PackagingType B On A.PackagingTypeID = B.PackagingTypeID
                    Inner Join bas_UOM C On A.UOMID = C.UOMID
                Where 
                    A.SDRFMasterID = {id}
                    And IsNull(A.IsDeleted,0) = 0;
                {CommonQuery}; ";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Mkt_SDRFMaster data = await records.ReadFirstOrDefaultAsync<Mkt_SDRFMaster>();
                data.Childs = records.Read<Mkt_SDRFChild>().ToList();
                if (data != null)
                {
                    data.SDRFByList = records.Read<DropDownList>().ToList();
                    data.PriorityList = records.Read<DropDownList>().ToList();
                    data.ClaimList = records.Read<DropDownList>().ToList();
                    data.FeelList = records.Read<DropDownList>().ToList();
                    data.FragranceList = records.Read<DropDownList>().ToList();
                    data.RestrictionList = records.Read<DropDownList>().ToList();
                    data.PackagingTypeList = records.Read<DropDownList>().ToList();
                    data.UOMList = records.Read<DropDownList>().ToList();
                    foreach (Mkt_SDRFChild items in data.Childs)
                    {
                        items.objPackagingTypeList.id = items.PackagingTypeID;
                        items.objPackagingTypeList.text = items.PackagingType;
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
        public async Task<Mkt_SDRFMaster> SaveAsync(Mkt_SDRFMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_Mkt_SDRFChild> TTList = GetChilds(entity.Childs);
                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@SDRFMasterID", entity.SDRFMasterID);
                parameters.Add("@BuyerRefMasterID", entity.BuyerRefMasterID);
                parameters.Add("@BuyerRefColorID", entity.BuyerRefColorID);
                parameters.Add("@SDRFNo", entity.SDRFNo);
                parameters.Add("@SDRFDate", entity.SDRFDate);
                parameters.Add("@RequiredDate", entity.RequiredDate);
                parameters.Add("@SDRFBy", entity.SDRFBy);
                parameters.Add("@BuyerID", entity.BuyerID);
                parameters.Add("@DevelopmentTypeID", entity.DevelopmentTypeID);
                parameters.Add("@MktTeamID", entity.MktTeamID);
                parameters.Add("@RnDTeamID", entity.RnDTeamID);
                parameters.Add("@PriorityID", entity.PriorityID);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@SDRFFiles", entity.SDRFFiles);
                parameters.Add("@SDRFClaimIDs", entity.SDRFClaimIDs);
                parameters.Add("@SDRFFeelIDs", entity.SDRFFeelIDs);
                parameters.Add("@SDRFFragranceIDs", entity.SDRFFragranceIDs);
                parameters.Add("@SDRFRestrictionIDs", entity.SDRFRestrictionIDs);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@SendForApproval", entity.SendForApproval);
                //parameters.Add("@SendForApprovalBy", entity.SendForApprovalBy);
                //parameters.Add("@SendForApprovalDate", entity.SendForApprovalDate);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@ReviseReason", entity.ReviseReason);
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@Reject", entity.Reject);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@Message", dbType: DbType.String, size: -1, direction: ParameterDirection.Output);
                entity.NoofRows = await _context.ExecuteAsync("spSet_mkt_SDRFMaster", parameters, 30, CommandType.StoredProcedure);
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

        public List<TT_Mkt_SDRFChild> GetChilds(List<Mkt_SDRFChild> Childs)
        {
            List<TT_Mkt_SDRFChild> TTList = new List<TT_Mkt_SDRFChild>();
            int Counter = 0;
            foreach (Mkt_SDRFChild item in Childs)
            {
                ++Counter;

                TT_Mkt_SDRFChild objItem = new TT_Mkt_SDRFChild();
                objItem.SDRFChildID = item.SDRFChildID;
                objItem.SDRFMasterID = item.SDRFMasterID;
                objItem.PackagingTypeID = item.PackagingTypeID;
                //objItem.QCStatus = item.objQCStatusList.id;
                objItem.SizeGram = item.SizeGram;
                objItem.UOMID = item.UOMID;
                objItem.Qty = item.Qty;
                objItem.IsPrimary = item.IsPrimary;
                objItem.Remarks = item.Remarks;
                objItem.RowSLNo = item.RowSLNo;
                objItem.IsDeleted = item.IsDeleted;
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
