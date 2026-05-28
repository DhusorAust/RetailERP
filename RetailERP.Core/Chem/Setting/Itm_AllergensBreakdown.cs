using Dapper.Contrib.Extensions;
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;
namespace RetailERP.Core.Chem.Setting

{
    public class Itm_AllergensBreakdown
    {
        public int AllergenBreakdownID { get; set; }
        public int IngredientID { get; set; }
        public int? INCIID { get; set; }
        public decimal? AllergenPercentage { get; set; }
        public string Remarks { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }

        #region Additional Columns 
        [Write(false)]
        public DropDownList objAllergensBreakDownList { get; set; } = new DropDownList();
        public string INCIName { get; set; }

        #endregion Additional Columns 
    }

    public class TT_Itm_AllergensBreakdown
    {
        public int AllergenBreakdownID { get; set; }
        public int IngredientID { get; set; }
        public int? INCIID { get; set; }
        public decimal? AllergenPercentage { get; set; }
        public string Remarks { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }
}
