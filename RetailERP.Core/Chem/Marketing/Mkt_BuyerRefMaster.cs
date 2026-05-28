using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    [Table("mkt_BuyerRefMaster")]
    public class Mkt_BuyerRefMaster : BaseEntity
    {
        public Mkt_BuyerRefMaster()
        {
             
        }
        public int BuyerRefMasterID { get; set; }
        public int BuyerRefMasterID_RefID { get; set; }
        public string ChemicalCode { get; set; } = AppGlobalFields.NEW;
        public string BuyerRef { get; set; }
        public DateTime BuyerRefDate { get; set; } = DateTime.Now;
        public int BuyerID { get; set; }
        public int FinishedProductCategoryID { get; set; }
        public int FinishedProductTypeID { get; set; }
        public int FinishedProductID { get; set; }
        public int SeasonID { get; set; }
        public int MarketingPersonID { get; set; }
        public string Remarks { get; set; }
        public string BuyerRefFiles { get; set; } 
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } 
        public bool Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; } 
        public int ReviseNo { get; set; } = 0;
        public int? ReviseBy { get; set; }
        public DateTime? ReviseDate { get; set; }
        public string ReviseReason { get; set; } 
        public bool Reject { get; set; } = false;
        public int? RejectBy { get; set; }
        public DateTime? RejectDate { get; set; }
        public string RejectReason { get; set; }
        public int SendToBulk { get; set; } = 0;
        public int? SendToBulkBy { get; set; }
        public DateTime? SendToBulkDate { get; set; }


        #region Additional Columns  
        [Write(false)]
        public string MktPersonName { get; set; }
        [Write(false)]
        public string ColorIDs { get; set; }
        [Write(false)]
        public string ColorName { get; set; }
        [Write(false)]
        public string BuyerName { get; set; }
        [Write(false)]
        public string ProductName { get; set; }
        [Write(false)]
        public string ProductCategoryName { get; set; }
        [Write(false)]
        public string ProductTypeName { get; set; }
        [Write(false)]
        public string SeasonName { get; set; }

        [Write(false)]
        public bool IsRevise { get; set; }
        [Write(false)]
        public string BuyerRefFiles1 { get; set; }
        [Write(false)]
        public string BuyerRefFiles2 { get; set; }
        [Write(false)]
        public string BuyerRefFiles3 { get; set; }
        [Write(false)]
        public string BuyerRefFiles4 { get; set; }
        [Write(false)]
        public string BuyerRefFiles5 { get; set; }
        [Write(false)]
        public string BuyerRefFiles6 { get; set; }
        [Write(false)]
        public string BuyerRefFiles7 { get; set; }
        [Write(false)]
        public string BuyerRefFiles8 { get; set; }
        [Write(false)]
        public string BuyerRefFiles9 { get; set; }
        [Write(false)]
        public string BuyerRefFiles10 { get; set; } 

        // Master List 
        [Write(false)]
        public List<DropDownList> BuyerList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> ProductList { get; set; } = new List<DropDownList>();

        [Write(false)]
        public List<DropDownList> SeasonList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> MarketingPersonList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> ColorList { get; set; } = new List<DropDownList>();
         
        [Write(false)]
        public List<Mkt_BuyerRefColorInfo> Childs { get; set; } = new List<Mkt_BuyerRefColorInfo>();
        #endregion Additional Columns
    }
}
