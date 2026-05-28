using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Production.Compounding;
using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Production.Filling
{
    [Table("prod_FILL_ProductionMaster")]
    public class Prod_FILL_ProductionMaster : BaseEntity
    {
        public Prod_FILL_ProductionMaster()
        {

        }
        public int ProductionMasterID { get; set; }
        public int POMasterID { get; set; }
        public int POChildID { get; set; }
        public DateTime ProductionDate { get; set; } = DateTime.Now;
        public DateTime? StartTime { get; set; } = DateTime.Now;
        public DateTime? EndTime { get; set; } = DateTime.Now;
        public string BatchNo { get; set; }
        public int? ProductionBy { get; set; }
        public decimal ProductionQty { get; set; } = 0;
        public int ShiftID { get; set; }
        public string Remarks { get; set; } 
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


        #region Additional Columns   
        public string PONo { get; set; }
        public string ChemicalCode { get; set; }
        public string BuyerRef { get; set; }
        public string BuyerName { get; set; }
        public string ColorName { get; set; }
        public string OrderType { get; set; }
        public string OrderMode { get; set; }
        public string PaymentTerms { get; set; }
        public string Incoterm { get; set; }
        public string POByName { get; set; }
        public string ProductionByName { get; set; }
        public string ShiftName { get; set; }
        public DateTime PODate { get; set; }
        public string POQty { get; set; }
        public string SizeGram { get; set; }
        public string PackagingName { get; set; }
        public string LotNo { get; set; }

        //Child 
        public string FinishedProductName { get; set; }
        public string FinishedProductDescription { get; set; }
        public string UOM { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductTypeName { get; set; }
        public int? CMPD_ProductionMasterID { get; set; }
        public int? CMPD_ProductionChildID { get; set; }
        public List<DropDownList> ProductionByList { get; set; } = new List<DropDownList>();
        public List<DropDownList> ShiftList { get; set; } = new List<DropDownList>();

        //Child
        public List<DropDownList> ProductList { get; set; } = new List<DropDownList>();
        public List<DropDownList> UOMList { get; set; } = new List<DropDownList>();
        public List<DropDownList> PackagingTypeList { get; set; } = new List<DropDownList>();
        public List<Prod_FILL_ProductionChild> Childs { get; set; } = new List<Prod_FILL_ProductionChild>();

        #endregion Additional Columns
    }
}
