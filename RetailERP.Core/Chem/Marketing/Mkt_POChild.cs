using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    [Table("mkt_POChild")]
    public class Mkt_POChild
    {
        public int POChildID { get; set; }
        public int POMasterID { get; set; }
        public int? SDRFMasterID { get; set; }
        public int? FeedbackMasterID { get; set; }
        public int? FeedbackChildID { get; set; }
        public int BuyerRefMasterID { get; set; }
        public int BuyerRefColorID { get; set; }
        public int? ShippingDestinationID { get; set; }
        public int? FinishedProductID { get; set; }
        public string FinishedProductDescription { get; set; }
        public int? PackagingTypeID { get; set; }
        public decimal SizeGram { get; set; } = 0;
        public int? UOMID { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public DateTime? ExFactoryDate { get; set; }
        public int? ShipModeID { get; set; }
        public int POQty { get; set; }
        public string Remarks { get; set; } 
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;

        #region Additional Columns 
        public string ProductName { get; set; }
        public string PackagingName { get; set; }
        public string UOM { get; set; }
        public string ShippingDestination { get; set; }
        public string ShipMode { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductCategoryName { get; set; }
        public DropDownList objProductList { get; set; } = new DropDownList();
        public DropDownList objPackagingTypeList { get; set; } = new DropDownList();
        public DropDownList objUOMList { get; set; } = new DropDownList();
        public DropDownList objShippingDestinationList { get; set; } = new DropDownList();
        public DropDownList objShipModeList { get; set; } = new DropDownList();



        #endregion Additional Columns 
    }
    public class TT_Mkt_POChild
    {
        public int POChildID { get; set; }
        public int POMasterID { get; set; }
        public int? SDRFMasterID { get; set; }
        public int? FeedbackMasterID { get; set; }
        public int? FeedbackChildID { get; set; }
        public int? BuyerRefMasterID { get; set; }
        public int? BuyerRefColorID { get; set; }
        public int? ShippingDestinationID { get; set; }
        public int? FinishedProductID { get; set; }
        public string FinishedProductDescription { get; set; }
        public int? PackagingTypeID { get; set; }
        public decimal SizeGram { get; set; } = 0;
        public int? UOMID { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public DateTime? ExFactoryDate { get; set; }
        public int? ShipModeID { get; set; }
        public int POQty { get; set; }
        public string Remarks { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;

    }
}
