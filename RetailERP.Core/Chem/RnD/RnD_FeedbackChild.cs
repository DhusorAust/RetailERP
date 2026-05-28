using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    [Table("rnd_FeedbackChild")]
    public class RnD_FeedbackChild
    {
        public int FeedbackChildID { get; set; }
        public int FeedbackMasterID { get; set; }
        public int SDRFChildID { get; set; }
        public int SDRFMasterID { get; set; }
        public int? VariantID { get; set; }
        public int? FormulationMasterID { get; set; }
        public int? PackagingTypeID { get; set; }
        public decimal? SizeGram { get; set; } = 0;
        public int? UOMID { get; set; }
        public string BatchNo { get; set; }
        public string ClaimIDs { get; set; }
        public string FeelIDs { get; set; }
        public string FragranceIDs { get; set; }
        public string RestrictionIDs { get; set; }        
        public bool BuyerRejected { get; set; }
        public int BuyerRejectedBy { get; set; }
        public DateTime BuyerRejectedDate { get; set; }
        public string BuyerRejectReason { get; set; }
        public bool BuyerApproved { get; set; }
        public int BuyerApprovedBy { get; set; }
        public DateTime BuyerApprovedDate { get; set; }        
        public string ChildRemarks { get; set; }        
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;


        #region Additional Columns 
        public string VariantCode { get; set; }
        public string FormulationCode { get; set; }
        public string PackagingName { get; set; }
        public string UOM { get; set; }
        public string ClaimName { get; set; }
        public string FeelName { get; set; }
        public string FragranceName { get; set; }
        public string RestrictionName { get; set; } 
        public DropDownList objVariantList { get; set; } = new DropDownList();
        public DropDownList objFormulationMasterList { get; set; } = new DropDownList();
        public DropDownList objPackagingTypeList { get; set; } = new DropDownList();
        public DropDownList objUOMList { get; set; } = new DropDownList(); 
        
        //Multi Select 
        public List<DropDownList> MultipleClaimList { get; set; } = new List<DropDownList>();
        public List<DropDownList> MultipleFeelList { get; set; } = new List<DropDownList>();
        public List<DropDownList> MultipleFragranceList { get; set; } = new List<DropDownList>();
        public List<DropDownList> MultipleRestrictionList { get; set; } = new List<DropDownList>();
        #endregion Additional Columns 
    }
    public class TT_RnD_FeedbackChild
    {
        public int FeedbackChildID { get; set; }
        public int FeedbackMasterID { get; set; }        
        public int SDRFChildID { get; set; }
        public int SDRFMasterID { get; set; }
        public int? VariantID { get; set; }
        public int? FormulationMasterID { get; set; }        
        public int? FormulationID { get; set; }
        public int? PackagingTypeID { get; set; }
        public decimal? SizeGram { get; set; } = 0;
        public int? UOMID { get; set; }
        public string BatchNo { get; set; }

        public string ClaimIDs { get; set; }
        public string FeelIDs { get; set; }
        public string FragranceIDs { get; set; }
        public string RestrictionIDs { get; set; }
        public bool BuyerRejected { get; set; }
        public int BuyerRejectedBy { get; set; }
        public DateTime BuyerRejectedDate { get; set; }
        public string BuyerRejectReason { get; set; }
        public bool BuyerApproved { get; set; }
        public int BuyerApprovedBy { get; set; }
        public DateTime BuyerApprovedDate { get; set; }
        public string ChildRemarks { get; set; }        
        public int RowSLNo { get; set; } = 0;
        public int RowSLNoNew { get; set; } = 0;        
        public int IsDeleted { get; set; } = 0; 
    }
}
