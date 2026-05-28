using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;

namespace RetailERP.Core.Chem.Production.Packaging
{
    [Table("prod_PACK_ProductionChild")]
    public class Prod_PACK_ProductionChild
    {
        public int ProductionChildID { get; set; }
        public int ProductionMasterID { get; set; }
        public int? PackagingTypeID { get; set; }
        public int? ProductID { get; set; }
        public int FillProductionMasterID { get; set; } = 0;
        public int FillProductionChildID { get; set; } = 0;
        public int? UOMID { get; set; }
        public decimal FillingQty { get; set; } = 0;
        public decimal PackQty { get; set; } = 0;
        public decimal RejectQty { get; set; } = 0;
        public string SizeGram { get; set; } 
        public string Remarks { get; set; } 
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;


        #region Additional Columns 
        public string ProductName { get; set; }
        public string ProductCategoryName { get; set; }
        public string PackagingName { get; set; }
        public string ProductTypeName { get; set; }
        public string UOM { get; set; }

        public DropDownList objProductList { get; set; } = new DropDownList();
        public DropDownList objUOMList { get; set; } = new DropDownList();
        public DropDownList objPackagingTypeList { get; set; } = new DropDownList();

        #endregion Additional Columns 
    }
    public class TT_Prod_PACK_ProductionChild
    {
        public int ProductionChildID { get; set; }
        public int ProductionMasterID { get; set; }
        public int? ProductID { get; set; }
        public int FillProductionMasterID { get; set; } = 0;
        public int FillProductionChildID { get; set; } = 0;
        public int? PackagingTypeID { get; set; }
        public int? UOMID { get; set; }
        public decimal FillingQty { get; set; } = 0;
        public decimal PackQty { get; set; } = 0;
        public decimal RejectQty { get; set; } = 0;
        public string SizeGram { get; set; }
        public string Remarks { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;
    }
}
