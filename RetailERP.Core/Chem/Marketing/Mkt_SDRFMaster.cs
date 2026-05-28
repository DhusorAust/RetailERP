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
    [Table("mkt_SDRFMaster")]
    public class Mkt_SDRFMaster : BaseEntity
    {
        public Mkt_SDRFMaster()
        {

        }
        public int SDRFMasterID { get; set; }
        public int BuyerRefMasterID { get; set; }
        public int BuyerRefColorID { get; set; }
        public string SDRFNo { get; set; } = AppGlobalFields.NEW;
        public DateTime SDRFDate { get; set; } = DateTime.Now;
        public DateTime RequiredDate { get; set; } = DateTime.Now;
        public int SDRFBy { get; set; }       
        public int BuyerID { get; set; }
        public int DevelopmentTypeID { get; set; }       
        public int MktTeamID { get; set; }        
        public int RnDTeamID { get; set; }        
        public int PriorityID { get; set; }      
        public string Remarks { get; set; }
        public string SDRFFiles { get; set; }
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public bool SendForApproval { get; set; } = false;
        public int? SendForApprovalBy { get; set; }
        public DateTime? SendForApprovalDate { get; set; }      

        public int ReviseNo { get; set; } = 0;
        public int? ReviseBy { get; set; }
        public DateTime? ReviseDate { get; set; }
        public string ReviseReason { get; set; }
        public bool Reject { get; set; } = false;
        public int? RejectBy { get; set; }
        public DateTime? RejectDate { get; set; }
        


        public string RejectReason { get; set; }

        #region Additional Columns 
        public string ChemicalCode { get; set; }
        public string BuyerRef { get; set; }
        public string SDRFByName { get; set; }
        public string BuyerName { get; set; }
        public string DevelopmentType { get; set; }
        public string MktTeamName { get; set; }
        public string RnDTeamName { get; set; }
        public string PriorityName { get; set; }
        public string SeasonName { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; } 
        public string MktPersonName { get; set; } 
        public string ProductCategoryName { get; set; }
        public DateTime BuyerRefDate { get; set; } 
        public string ClaimName { get; set; }
        public string FeelName { get; set; }
        public string FragranceName { get; set; }
        public string RestrictionName { get; set; }

        public string SDRFClaimIDs { get; set; }
        public string SDRFFeelIDs { get; set; }
        public string SDRFFragranceIDs { get; set; }
        public string SDRFRestrictionIDs { get; set; }

        
        [Write(false)]
        public bool? IsRevise { get; set; } = false;
        

        [Write(false)]
        public string ProductTypeName { get; set; }  
        public List<DropDownList> SDRFByList { get; set; } = new List<DropDownList>();
        public List<DropDownList> PriorityList { get; set; } = new List<DropDownList>();
        public List<DropDownList> ClaimList { get; set; } = new List<DropDownList>();
        public List<DropDownList> FeelList { get; set; } = new List<DropDownList>();
        public List<DropDownList> FragranceList { get; set; } = new List<DropDownList>();
        public List<DropDownList> RestrictionList { get; set; } = new List<DropDownList>();
       
        //public List<DropDownList> SeasonList { get; set; } = new List<DropDownList>();

        //Child
        public List<DropDownList> PackagingTypeList { get; set; } = new List<DropDownList>();
        public List<DropDownList> UOMList { get; set; } = new List<DropDownList>();
        public List<Mkt_SDRFChild> Childs { get; set; } = new List<Mkt_SDRFChild>();

        #endregion Additional Columns
    }
}
