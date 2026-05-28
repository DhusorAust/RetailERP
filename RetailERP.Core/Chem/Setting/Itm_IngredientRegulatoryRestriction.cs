using Dapper.Contrib.Extensions;
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;

namespace RetailERP.Core.Chem.Setting

{
    public class Itm_IngredientRegulatoryRestriction
    {
        public int IngredientRegulatoryRestrictionID { get; set; }
        public int IngredientID { get; set; }
        public int? CountryID { get; set; }
        public string RestrictionNote { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }


        #region Additional Columns 
        [Write(false)]
        public DropDownList objCountryList { get; set; } = new DropDownList();
        public string CountryName { get; set; }
        #endregion Additional Columns  
    }

    public class TT_Itm_IngredientRegulatoryRestriction
    {
        public int IngredientRegulatoryRestrictionID { get; set; }
        public int IngredientID { get; set; }
        public int? CountryID { get; set; }
        public string RestrictionNote { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }
}
