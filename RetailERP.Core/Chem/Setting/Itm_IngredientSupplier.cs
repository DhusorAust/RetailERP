using Dapper.Contrib.Extensions;
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;

namespace RetailERP.Core.Chem.Setting
{
    public class Itm_IngredientSupplier
    {
        public int IngredientSupplierID { get; set; }
        public int IngredientID { get; set; }
        public int? SupplierID { get; set; }
        public decimal? PricePerKg { get; set; }
        public decimal? MOQInKg { get; set; }
        public int? LeadTime { get; set; }
        public int? LeadTimeUnitID { get; set; }

        public string Remarks { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }

        #region Additional Columns 
        [Write(false)]
        public DropDownList objSupplierList { get; set; } = new DropDownList();
        public DropDownList objLeadTimeUnitList { get; set; } = new DropDownList();
        public string SupplierName { get; set; }
        public string LeadTimeUnit { get; set; }

        #endregion Additional Columns  


    }

    public class TT_Itm_IngredientSupplier
    {
        public int IngredientSupplierID { get; set; }
        public int IngredientID { get; set; }
        public int? SupplierID { get; set; }
        public decimal? PricePerKg { get; set; }
        public decimal? MOQInKg { get; set; }
        public int? LeadTime { get; set; }
        public int? LeadTimeUnitID { get; set; }
        public string Remarks { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }
}
