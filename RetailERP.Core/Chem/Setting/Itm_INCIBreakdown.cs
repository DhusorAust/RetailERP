using Dapper.Contrib.Extensions;
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;
namespace RetailERP.Core.Chem.Setting
{
    public class Itm_INCIBreakdown
    {
        public int INCIBreakdownID { get; set; }
        public int IngredientID { get; set; }
        public int? INCIID { get; set; }
        public decimal? INCIPercentage { get; set; }
        public string Remarks { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }

        #region Additional Columns 
        [Write(false)]
        public string INCIName { get; set; }
        public DropDownList objINCIBreakDownList { get; set; } = new DropDownList();

        #endregion Additional Columns  
    }

    public class TT_Itm_INCIBreakdown
    {
        public int INCIBreakdownID { get; set; }
        public int IngredientID { get; set; }
        public int? INCIID { get; set; }
        public decimal? INCIPercentage { get; set; }
        public string Remarks { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }
}
