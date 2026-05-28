using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;  
using RetailERP.Infrastructure.Chem.Interfaces.RnD;
using Dapper;
using Microsoft.Data.SqlClient;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.RnD
{   
    public class FeedbackService : IFeedbackService
    {
        private readonly DapperContext<RnD_FeedbackMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public FeedbackService(DapperContext<RnD_FeedbackMaster> context)
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

            Select AbsorptionScoreID As id, AbsorptionScore As [text] From bas_AbsorptionScore Where IsActive = 1;
            Select ResidueScoreID As id, ResidueScore As [text] From bas_ResidueScore Where IsActive = 1;
            Select FragranceScoreID As id, FragranceScore As [text] From bas_FragranceScore Where IsActive = 1;
            Select OverallScoreID As id, OverallScore As [text] From bas_OverallScore  Where IsActive = 1;

            -- Child
            Select VariantID As id, VariantCode As [text] From bas_Variant  Where IsActive = 1;
            --Select FormulationMasterID As id, FormulationCode As [text] From rnd_FormulationMaster  Where IsActive = 1 AND Approved = 1;
            Select PackagingTypeID As id, PackagingName As [text] From bas_PackagingType  Where IsActive = 1;
            Select UOMID As id, UOM As [text] From bas_UOM  Where IsActive = 1 AND Approved = 1; 
            Select ClaimID As id, ClaimName As [text] From bas_Claim  Where IsActive = 1 AND Approved = 1;
            Select FeelID As id, FeelName As [text] From bas_Feel  Where IsActive = 1 AND Approved = 1;
            Select FragranceID As id, FragranceName As [text] From bas_Fragrance  Where IsActive = 1 AND Approved = 1;
            Select RestrictionID As id, RestrictionName As [text] From bas_Restriction  Where IsActive = 1 AND Approved = 1; ";
        public async Task<GridEntity<RnD_FeedbackMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "";

            if (status == Status.PENDING)
            {
                sql = $@"
                    Select
	                    Distinct sm.*, brm.ChemicalCode, brm.BuyerRef, clr.ColorName, (ei.EmployeeNo + '-' + ei.EmployeeName)SDRFByName, 
                        b.BrandName As BuyerName, dt.DevelopmentType, mti.TeamName As MktTeamName, rti.TeamName As RnDTeamName, p.PriorityName,                        
                        dbo.fn_Conditional_Stuffing_Text(sm.SDRFMasterID,9,1) AS ClaimName,
						dbo.fn_Conditional_Stuffing_Text(sm.SDRFMasterID,10,1) As FeelName,
						dbo.fn_Conditional_Stuffing_Text(sm.SDRFMasterID,11,1) As FragranceName,
						dbo.fn_Conditional_Stuffing_Text(sm.SDRFMasterID,12,1) As RestrictionName 
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
                        Left Join rnd_FeedbackMaster q On SM.SDRFMasterID = q.SDRFMasterID
                    Where  
	                     sm.Approved = 1 And sm.IsActive = 1 And q.SDRFMasterID Is Null";

                orderBy = "SDRFMasterID Desc";
            }
            else if (status == Status.EDIT)
            {
                condition = $@" And fm.Approved = 0 And fm.Reject = 0 ";
            } 
            else if (status == Status.APPROVED)
            {
                condition = $@" And fm.Approved = 1 And fm.Reject = 0  And fm.FinalApproved = 0";
            }
            else if (status == Status.CHECK)
            {
                condition = $@" And fm.Approved = 1 And fm.Reject = 0 And fm.FinalApproved = 0";
            }
            else if (status == Status.COMPLETED)
            {
                condition = $@" And fm.FinalApproved = 1";
            }

            else if (status == Status.REJECT)
            {
                condition = $@" And fm.Reject = 1 ";
            }
            else if (status == Status.ALL)
            {
                condition = $@" ";
            }
            if (status != Status.PENDING)
            {
                sql = $@" 
                    Select 
	                    Distinct fm.*, bfm.ChemicalCode, bfm.BuyerRef, b.BrandName As BuyerName, (ei.EmployeeNo + '-' + ei.EmployeeName)RnDByName,
                        dt.DevelopmentType, pm.ProductName
                    From 
	                    rnd_FeedbackMaster fm
	                    Left Join rnd_FeedbackChild fc On fc.FeedbackMasterID = fm.FeedbackMasterID
	                    Left Join mkt_BuyerRefMaster bfm On bfm.BuyerRefMasterID = fm.BuyerRefMasterID
	                    Left Join {DBSource.PDLHRM}..emp_EmploymentInfo ei On ei.EmployeeID = fm.RnDBy
	                    Left Join bas_DevelopmentType dt On dt.DevelopmentTypeID = fm.DevelopmentTypeID 
	                    Left Join bas_Brand b On b.BrandID = bfm.BuyerID
                        Left Join bas_CS_ProductMaster pm On pm.ProductID = bfm.FinishedProductID
                    Where fm.IsActive = 1 {condition} ";

                orderBy = "FeedbackMasterID Desc";
            }
            return await _context.GetKendoGridDataAsync<RnD_FeedbackMaster>(options, sql, orderBy, "");
        }
        public async Task<RnD_FeedbackMaster> GetNew(int id)
        {
            var query =
                $@"  
                -- Master 
                Select 
	                Distinct sm.SDRFMasterID,sm.BuyerRefMasterID, sm.BuyerRefColorID, brm.ChemicalCode, brm.BuyerRef, b.BrandName As BuyerName, pm.ProductName, clr.ColorName,
	                s.SeasonName, sm.SDRFNo, sm.SDRFDate, sm.RequiredDate, (ei.EmployeeNo + '-' + ei.EmployeeName)SDRFByName, p.PriorityName,
                    sm.MktTeamID,sm.PriorityID,sm.DevelopmentTypeID,sm.RnDTeamID
                From 
	                mkt_SDRFMaster sm
	                -- Buyer
	                Inner Join mkt_BuyerRefColorInfo brc On brc.BuyerRefColorID = sm.BuyerRefColorID
	                Inner Join mkt_BuyerRefMaster brm On brm.BuyerRefMasterID = sm.BuyerRefMasterID
	                Left Join bas_Brand b On b.BrandID = sm.BuyerID 
	                Left Join bas_CS_ProductMaster pm ON pm.ProductID = brm.FinishedProductID 
	                Left Join bas_Color clr ON clr.ColorID = brc.ColorID 
	                Left Join bas_Season s On s.SeasonID = brm.SeasonID
	                -- SDRF 
	                Left Join bas_DevelopmentType dt On dt.DevelopmentTypeID = sm.DevelopmentTypeID 
	                Left Join bas_Priority p On p.PriorityID = sm.PriorityID
	                Left Join {DBSource.PDLHRM}..emp_EmploymentInfo ei On ei.EmployeeID = sm.SDRFBy
                Where 
                    sm.SDRFMasterID = {id} ;  
                {CommonQuery}

                -- FormulationMasterList
                Select FormulationMasterID As id, FormulationCode As [text] 
                From rnd_FormulationMaster
                Where --SDRFMasterID = {id} And 
                    IsActive = 1 --AND Approved = 1;
                ";

            RnD_FeedbackMaster data = new(); 
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<RnD_FeedbackMaster>(); 

                if (data != null)
                { 
                    // Master
                    data.RnDByList = records.Read<DropDownList>().ToList();
                    data.AbsorptionScoreList = records.Read<DropDownList>().ToList();
                    data.ResidueScoreList = records.Read<DropDownList>().ToList();
                    data.FragranceScoreList = records.Read<DropDownList>().ToList();
                    data.OverallScoreList = records.Read<DropDownList>().ToList();
                    // Child
                    data.VariantList = records.Read<DropDownList>().ToList();
                    //data.FormulationMasterList = records.Read<DropDownList>().ToList();
                    data.PackagingTypeList = records.Read<DropDownList>().ToList();
                    data.UOMList = records.Read<DropDownList>().ToList();
                    data.ClaimList = records.Read<DropDownList>().ToList();
                    data.FeelList = records.Read<DropDownList>().ToList();
                    data.FragranceList = records.Read<DropDownList>().ToList();
                    data.RestrictionList = records.Read<DropDownList>().ToList();
                    data.FormulationMasterList = records.Read<DropDownList>().ToList();

                    ////// Dummy Data 
                    //data.RnDBy = 484;
                    //data.AbsorptionScoreID = 4;
                    //data.ResidueScoreID = 3;
                    //data.FragranceScoreID = 2;
                    //data.OverallScoreID = 5;
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
        public async Task<RnD_FeedbackMaster> GetEdit(int id)
        {
            var query =
                $@" 
                -- Master
				Select 
					C.ChemicalCode, E.BrandName As BuyerName, C.BuyerRef, F.ProductName, A.FeedbackMasterID, A.SDRFMasterID, G.ColorName
					,H.SeasonName, B.SDRFNo, B.SDRFDate, B.RequiredDate, B.MktTeamID, B.PriorityID, I.PriorityName,
					B.SDRFBy,J.EmployeeName As SDRFByName, A.* 
				From
                    rnd_FeedbackMaster A
				    Inner Join mkt_SDRFMaster B On A.SDRFMasterID = B.SDRFMasterID
				    Inner Join mkt_BuyerRefMaster C On A.BuyerRefMasterID = C.BuyerRefMasterID
				    Inner Join mkt_BuyerRefColorInfo D On B.BuyerRefColorID = D.BuyerRefColorID
				    Inner Join bas_Brand E On C.BuyerID = E.BrandID
				    Inner Join bas_CS_ProductMaster F On C.FinishedProductID = F.ProductID
				    Inner Join bas_Color G On D.ColorID = G.ColorID
				    Inner Join bas_Season H On C.SeasonID = H.SeasonID
				    Inner Join bas_Priority I On B.PriorityID = I.PriorityID
				    Inner Join ERPDB..emp_EmploymentInfo J On B.SDRFBy = J.EmployeeID
				Where A.FeedbackMasterID = {id}; 

                -- Childs
                Select
                    A.*, B.VariantCode, C.FormulationCode,D.PackagingName,E.UOM
                From 
                    rnd_FeedbackChild A
                    Inner Join bas_Variant B On A.VariantID = B.VariantID
                    Inner Join rnd_FormulationMaster C On A.FormulationMasterID = C.FormulationMasterID
                    Inner Join bas_PackagingType D On A.PackagingTypeID = D.PackagingTypeID
                    Inner Join bas_UOM E On A.UOMID = E.UOMID
                Where 
                    A.FeedbackMasterID = {id} And IsNull(A.IsDeleted,0) = 0; 

                -- Claim
                Select 
	                fc.*, c.ClaimName
                From 
	                rnd_FeedbackClaim fc
	                Inner Join bas_Claim c On c.ClaimID = fc.ClaimID 
                Where fc.IsDeleted = 0 AND fc.FeedbackMasterID = {id}; 

                -- Feel 
                Select 
	                fc.*, c.FeelName
                From 
	                rnd_FeedbackFeel fc
	                Inner Join bas_Feel c On c.FeelID = fc.FeelID 
                Where fc.FeedbackMasterID = {id} AND fc.IsDeleted = 0;

                -- Fragrance
                Select 
	                fc.*, c.FragranceName
                From 
	                rnd_FeedbackFragrance fc
	                Inner Join bas_Fragrance c On c.FragranceID = fc.FragranceID 
                Where fc.FeedbackMasterID = {id} AND fc.IsDeleted = 0;

                -- Restriction
                Select 
	                fc.*, c.RestrictionName
                From 
	                rnd_FeedbackRestriction fc
	                Inner Join bas_Restriction c On c.RestrictionID = fc.RestrictionID 
                Where fc.FeedbackMasterID = {id} AND fc.IsDeleted = 0; 

                {CommonQuery}

                -- FormulationMasterList
                Select 
                    B.FormulationMasterID As id, B.FormulationCode As [text] 
                From 
                    rnd_FeedbackMaster A
                    Inner Join rnd_FormulationMaster B On A.SDRFMasterID = A.SDRFMasterID --B.SDRFMasterID
                Where A.FeedbackMasterID = {id};  ";

            RnD_FeedbackMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<RnD_FeedbackMaster>();
                data.Childs = records.Read<RnD_FeedbackChild>().ToList();
                data.ChildsClaim = records.Read<RnD_FeedbackClaim>().ToList();
                data.ChildsFeel = records.Read<RnD_FeedbackFeel>().ToList();
                data.ChildsFragrance = records.Read<RnD_FeedbackFragrance>().ToList();
                data.ChildsRestriction = records.Read<RnD_FeedbackRestriction>().ToList();
                data.RowToken = data.RowToken;

                if (data != null)
                {
                    data.RnDByList = records.Read<DropDownList>().ToList();
                    data.AbsorptionScoreList = records.Read<DropDownList>().ToList();
                    data.ResidueScoreList = records.Read<DropDownList>().ToList();
                    data.FragranceScoreList = records.Read<DropDownList>().ToList();
                    data.OverallScoreList = records.Read<DropDownList>().ToList();
                    // Child
                    data.VariantList = records.Read<DropDownList>().ToList();                    
                    data.PackagingTypeList = records.Read<DropDownList>().ToList();
                    data.UOMList = records.Read<DropDownList>().ToList();
                    data.ClaimList = records.Read<DropDownList>().ToList();
                    data.FeelList = records.Read<DropDownList>().ToList();
                    data.FragranceList = records.Read<DropDownList>().ToList();
                    data.RestrictionList = records.Read<DropDownList>().ToList();
                    data.FormulationMasterList = records.Read<DropDownList>().ToList();

                    // Dummy Child
                    foreach (RnD_FeedbackChild items in data.Childs)
                    {
                        items.objVariantList.id = items.VariantID;
                        items.objVariantList.text = items.VariantCode;
                        items.objFormulationMasterList.id = items.FormulationMasterID;
                        items.objFormulationMasterList.text = items.FormulationCode;
                        items.objPackagingTypeList.id = items.PackagingTypeID;
                        items.objPackagingTypeList.text = items.PackagingName;
                        items.objUOMList.id = items.UOMID;
                        items.objUOMList.text = items.UOM;

                        List<DropDownList> ClaimIDs = data.ChildsClaim.Where(f => f.FeedbackChildID == items.FeedbackChildID)
                                                            .Select(s => new DropDownList
                                                            {
                                                                id = s.ClaimID,
                                                                text = s.ClaimName,
                                                                additionalID = s.RowSLNo

                                                            }).OrderBy(s => s.additionalID).ToList();
                        items.MultipleClaimList.AddRange(ClaimIDs);

                        List<DropDownList> FeelIDs = data.ChildsFeel.Where(f => f.FeedbackChildID == items.FeedbackChildID)
                                                            .Select(s => new DropDownList
                                                            {
                                                                id = s.FeelID,
                                                                text = s.FeelName,
                                                                additionalID = s.RowSLNo
                                                            }).OrderBy(s => s.additionalID).ToList();
                        items.MultipleFeelList.AddRange(FeelIDs);
                        
                        List<DropDownList> FragranceIDs = data.ChildsFragrance.Where(f => f.FeedbackChildID == items.FeedbackChildID)
                                                            .Select(s => new DropDownList
                                                            {
                                                                id = s.FragranceID,
                                                                text = s.FragranceName,
                                                                additionalID = s.RowSLNo
                                                            }).OrderBy(s => s.additionalID).ToList();
                        items.MultipleFragranceList.AddRange(FragranceIDs);

                        List<DropDownList> RestrictionIDs = data.ChildsRestriction.Where(f => f.FeedbackChildID == items.FeedbackChildID)
                                                            .Select(s => new DropDownList
                                                            {
                                                                id = s.RestrictionID,
                                                                text = s.RestrictionName,
                                                                additionalID = s.RowSLNo
                                                            }).OrderBy(s => s.additionalID).ToList();
                        items.MultipleRestrictionList.AddRange(RestrictionIDs);
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
        public async Task<RnD_FeedbackMaster> SaveAsync(RnD_FeedbackMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_RnD_FeedbackChild> TTList = GetChild(entity.Childs);
                //List<TT_RnD_FormulationMethod> TTListB = GetChildB(entity.ChildsB);

                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();

                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@FeedbackMasterID", entity.FeedbackMasterID);
                parameters.Add("@BuyerRefMasterID", entity.BuyerRefMasterID);
                parameters.Add("@SDRFMasterID", entity.SDRFMasterID);               
                parameters.Add("@FeedbackNo", entity.FeedbackNo);
                parameters.Add("@FeedbackDate", entity.FeedbackDate);
                parameters.Add("@RnDBy", entity.RnDBy);
                parameters.Add("@RnDTeamID", entity.RnDTeamID);
                parameters.Add("@DevelopmentTypeID", entity.DevelopmentTypeID);
                parameters.Add("@AbsorptionScoreID", entity.AbsorptionScoreID);
                parameters.Add("@ResidueScoreID", entity.ResidueScoreID);
                parameters.Add("@FragranceScoreID", entity.FragranceScoreID);
                parameters.Add("@OverallScoreID", entity.OverallScoreID);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@ReviseNo", entity.ReviseNo);
                parameters.Add("@ReviseReason", entity.ReviseReason);
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@Revise", entity.Revise);
                parameters.Add("@Reject", entity.Reject);
                parameters.Add("@FinalApproved", entity.FinalApproved);                                
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@RowToken", entity.RowToken, DbType.Binary, size: 8);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@Message", dbType: DbType.String, size: -1, direction: ParameterDirection.Output);

                entity.NoofRows = await _context.ExecuteAsync("spSet_rnd_FeedbackMaster", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");
                entity.Message = parameters.Get<string>("@Message");

                if (entity.NoofRows > 0 && entity.ResultId > 0)
                {
                    entity.Message = "Save operation successful.";
                }
                else
                {
                    entity.Message = entity.Message + " Save operation failed.";
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
        public List<TT_RnD_FeedbackChild> GetChild(List<RnD_FeedbackChild> Childs)
        {
            List<TT_RnD_FeedbackChild> TTList = new List<TT_RnD_FeedbackChild>();
            int Counter = 0;
            foreach (RnD_FeedbackChild item in Childs)
            {
                ++Counter;

                TT_RnD_FeedbackChild objItem = new TT_RnD_FeedbackChild(); 
                objItem.FeedbackChildID = item.FeedbackChildID;
                objItem.FeedbackMasterID = item.FeedbackMasterID;
                objItem.SDRFChildID = item.SDRFChildID;
                objItem.SDRFMasterID = item.SDRFMasterID;
                objItem.VariantID = item.objVariantList.id;
                objItem.FormulationMasterID = item.objFormulationMasterList.id;
                objItem.PackagingTypeID = item.objPackagingTypeList.id;
                objItem.SizeGram = item.SizeGram;
                objItem.UOMID = item.objUOMList.id;
                objItem.BatchNo = item.BatchNo;
                objItem.ClaimIDs = item.ClaimIDs;
                objItem.FeelIDs = item.FeelIDs;
                objItem.FragranceIDs = item.FragranceIDs;
                objItem.RestrictionIDs = item.RestrictionIDs; 
                objItem.BuyerRejected = item.BuyerRejected;
                objItem.BuyerRejectedBy = 0;
                objItem.BuyerRejectedDate = System.DateTime.Now;
                objItem.BuyerRejectReason = item.BuyerRejectReason; 
                objItem.BuyerApproved = item.BuyerApproved;
                objItem.BuyerApprovedBy = 0;
                objItem.BuyerApprovedDate = System.DateTime.Now;
                objItem.ChildRemarks = item.ChildRemarks;                
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
