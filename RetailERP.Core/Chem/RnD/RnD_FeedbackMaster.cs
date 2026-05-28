using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    [Table("rnd_FeedbackMaster")]
    public class RnD_FeedbackMaster : BaseEntity
    {
        public RnD_FeedbackMaster()
        {

        }
        public int FeedbackMasterID { get; set; }
        public int BuyerRefMasterID { get; set; }
        public string FeedbackNo { get; set; } = AppGlobalFields.NEW;
        public DateTime FeedbackDate { get; set; } = DateTime.Now;

        public int? SDRFMasterID { get; set; } = 0;
        
        public int? RnDBy { get; set; }
        public int? RnDTeamID { get; set; }
        public int? MktTeamID { get; set; }
        public int? DevelopmentTypeID { get; set; }
        public int? AbsorptionScoreID { get; set; }
        public int? ResidueScoreID { get; set; }
        public int? FragranceScoreID { get; set; }
        public int? OverallScoreID { get; set; }
        public string Remarks { get; set; }
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool Revise { get; set; } = false;
        public int ReviseNo { get; set; } = 0;
        public int? ReviseBy { get; set; }
        public DateTime? ReviseDate { get; set; }
        public string ReviseReason { get; set; }
        public bool Reject { get; set; } = false;
        public int? RejectBy { get; set; }
        public DateTime? RejectDate { get; set; }
        public string RejectReason { get; set; }
        public bool FinalApproved { get; set; } = false;
        

        #region Additional Columns  
        public string ChemicalCode { get; set; }
        public string BuyerRef { get; set; }
        public string BuyerName { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public string SeasonName { get; set; }
        public string SDRFNo { get; set; }
        public DateTime SDRFDate { get; set; }
        public DateTime RequiredDate { get; set; } 
        public string SDRFByName { get; set; }
        public string PriorityName { get; set; }
        public string RnDByName { get; set; }
        public string AbsorptionScore { get; set; }
        public string ResidueScore { get; set; }
        public string FragranceScore { get; set; }
        public string OverallScore { get; set; }
        public string DevelopmentType { get; set; }
        public string MktTeamName { get; set; }
        public string RnDTeamName { get; set; }
        public string ClaimName { get; set; }
        public string FeelName { get; set; }
        public string FragranceName { get; set; }
        public string RestrictionName { get; set; } 
        public string SDRFClaimIDs { get; set; }
        public string SDRFFeelIDs { get; set; }
        public string SDRFFragranceIDs { get; set; }
        public string SDRFRestrictionIDs { get; set; }
        public byte[] RowToken { get; set; } = new byte[8];
        

        // Master DropDownList
        public List<DropDownList> RnDByList { get; set; } = new List<DropDownList>();
        public List<DropDownList> AbsorptionScoreList { get; set; } = new List<DropDownList>();
        public List<DropDownList> ResidueScoreList { get; set; } = new List<DropDownList>();
        public List<DropDownList> FragranceScoreList { get; set; } = new List<DropDownList>();
        public List<DropDownList> OverallScoreList { get; set; } = new List<DropDownList>();
        
        //Child DropDownList
        public List<DropDownList> VariantList { get; set; } = new List<DropDownList>();
        public List<DropDownList> FormulationMasterList { get; set; } = new List<DropDownList>();
        public List<DropDownList> PackagingTypeList { get; set; } = new List<DropDownList>();
        public List<DropDownList> UOMList { get; set; } = new List<DropDownList>();
        public List<DropDownList> ClaimList { get; set; } = new List<DropDownList>(); 
        public List<DropDownList> FeelList { get; set; } = new List<DropDownList>();
        public List<DropDownList> FragranceList { get; set; } = new List<DropDownList>();
        public List<DropDownList> RestrictionList { get; set; } = new List<DropDownList>(); 

        //Child Class
        public List<RnD_FeedbackChild> Childs { get; set; } = new List<RnD_FeedbackChild>();
        public List<RnD_FeedbackClaim> ChildsClaim { get; set; } = new List<RnD_FeedbackClaim>();
        public List<RnD_FeedbackFeel> ChildsFeel { get; set; } = new List<RnD_FeedbackFeel>();
        public List<RnD_FeedbackFragrance> ChildsFragrance { get; set; } = new List<RnD_FeedbackFragrance>();
        public List<RnD_FeedbackRestriction> ChildsRestriction { get; set; } = new List<RnD_FeedbackRestriction>(); 
        #endregion Additional Columns
    }
}
