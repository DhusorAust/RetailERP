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
    [Table("mkt_POMaster")]
    public class Mkt_POMaster : BaseEntity
    {
        public Mkt_POMaster()
        {

        }
        public int POMasterID { get; set; }
        public int OrderTypeID { get; set; }
        public int OrderModeID { get; set; }
        public string PONo { get; set; }
        public DateTime PODate { get; set; } = DateTime.Now;
        public int PaymentTermsID { get; set; }
        public int IncotermID { get; set; }
        public int CurrencyID { get; set; }
        public int POBy { get; set; }
        public int? BuyerRefMasterID_RPT { get; set; }
        public int? BuyerRefColorID_RPT { get; set; }
        public int? POMasterID_RPT { get; set; }
        public int TotalPOQty { get; set; } = 0;
        public string Remarks { get; set; }
        public string POFIle { get; set; } 
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool SendForApproval { get; set; } = false;
        public int? SendForApprovalBy { get; set; }
        public DateTime? SendForApprovalDate { get; set; } 
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
        public byte[] RowToken { get; set; } = new byte[8];

        #region Additional Columns 
        public string ChemicalCode { get; set; }
        public string BuyerRef { get; set; }
        public string POByName { get; set; }
        public string CurrencyName { get; set; }
        public string PaymentTerms { get; set; }
        public string Incoterm { get; set; }
        public string ColorName { get; set; }
        public string OrderMode { get; set; }


        public string SDRFNo { get; set; }
        public string ProductName { get; set; }
        public string BuyerName { get; set; }
        public string SeasonName { get; set; }
        public DateTime RequiredDate { get; set; } = DateTime.Now;

        public int? BuyerRefMasterID { get; set; }
		public int? BuyerRefColorID { get; set; }		
		public int? SDRFMasterID { get; set; }
        public int? FinishedProductID { get; set; }
        public int? ColorID { get; set; }
        public int? BuyerID { get; set; }
        public int? SeasonID { get; set; }
        public string BuyerRefColorIDs { get; set; }        
        public List<DropDownList> OrderModeList { get; set; } = new List<DropDownList>();
        public List<DropDownList> POMasterList { get; set; } = new List<DropDownList>();
        public List<DropDownList> CurrencyList { get; set; } = new List<DropDownList>();
        public List<DropDownList> BuyerRefMasterList { get; set; } = new List<DropDownList>();
        public List<DropDownList> BuyerRefColorList { get; set; } = new List<DropDownList>();
        public List<DropDownList> PaymentTermsList { get; set; } = new List<DropDownList>(); 
        public List<DropDownList> IncotermList { get; set; } = new List<DropDownList>();
        public List<DropDownList> POByList { get; set; } = new List<DropDownList>();
        //Child
        public List<DropDownList> ProductList { get; set; } = new List<DropDownList>();
        public List<DropDownList> PackagingTypeList { get; set; } = new List<DropDownList>();
        public List<DropDownList> UOMList { get; set; } = new List<DropDownList>();
        public List<DropDownList> ShippingDestinationList { get; set; } = new List<DropDownList>();
        public List<DropDownList> ShipModeList { get; set; } = new List<DropDownList>();
        public List<Mkt_POChild> Childs { get; set; } = new List<Mkt_POChild>();

        #endregion Additional Columns
    }
}
