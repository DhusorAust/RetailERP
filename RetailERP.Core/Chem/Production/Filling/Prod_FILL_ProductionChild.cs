using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Production.Filling
{
    [Table("prod_FILL_ProductionChild")]
    public class Prod_FILL_ProductionChild
    {
        public int ProductionChildID { get; set; }
        public int ProductionMasterID { get; set; }
        public int? PackagingTypeID { get; set; }
        public double? SizeGram { get; set; }
        public int? UOMID { get; set; }
        public decimal FillingQty { get; set; } = 0;  
        public string Remarks { get; set; } 
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;


        #region Additional Columns 
        public string PackagingName { get; set; }
        public string UOM { get; set; }
        public DropDownList objPackagingTypeList { get; set; } = new DropDownList();
        public DropDownList objUOMList { get; set; } = new DropDownList();

        //public string VariantCode { get; set; }
        //public string FormulationNo { get; set; }


        //public string ClaimName { get; set; }
        //public string FeelName { get; set; }
        //public string FragranceName { get; set; }
        //public string RestrictionName { get; set; } 
        //public DropDownList objVariantList { get; set; } = new DropDownList();
        //public DropDownList objFormulationMasterList { get; set; } = new DropDownList();


        //public DropDownList objClaimList { get; set; } = new DropDownList();
        //public DropDownList objFeelList { get; set; } = new DropDownList();
        //public DropDownList objFragranceList { get; set; } = new DropDownList();
        //public DropDownList objRestrictionList { get; set; } = new DropDownList();

        #endregion Additional Columns 
    }
    public class TT_Prod_FILL_ProductionChild
    {
        public int ProductionChildID { get; set; }
        public int ProductionMasterID { get; set; }
     
        public int? PackagingTypeID { get; set; }
        public double? SizeGram { get; set; }
        public int? UOMID { get; set; }
        public decimal FillingQty { get; set; } = 0;
        public string Remarks { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;
    }
}
