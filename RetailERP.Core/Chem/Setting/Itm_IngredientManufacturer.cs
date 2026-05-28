using Dapper.Contrib.Extensions;
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;
namespace RetailERP.Core.Chem.Setting

{
    public class Itm_IngredientManufacturer
    {
        public int IngredientManufacturerID { get; set; }
        public int IngredientID { get; set; }
        public int? ManufacturerID { get; set; }
        public string Remarks { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }

        #region Additional Columns 
        [Write(false)]
        public string ManufacturerName { get; set; }
        #endregion Additional Columns  
    }

    public class TT_Itm_IngredientManufacturer
    {
        public int IngredientManufacturerID { get; set; }
        public int IngredientID { get; set; }
        public int? ManufacturerID { get; set; }
        public string Remarks { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }
}
