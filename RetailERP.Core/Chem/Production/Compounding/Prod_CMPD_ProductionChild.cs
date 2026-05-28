using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Production.Compounding
{
    [Table("prod_CMPD_ProductionChild")]
    public class Prod_CMPD_ProductionChild
    {
        public int ProductionChildID { get; set; }
        public int ProductionMasterID { get; set; }
        public int? FinishedProductID { get; set; }
        public int? UOMID { get; set; }
        public decimal Qty { get; set; } = 0;
        public string Remarks { get; set; } 
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;

        #region Additional Columns 
        public string ProductName { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductTypeName { get; set; }
        public string UOM { get; set; }

        public DropDownList objProductList { get; set; } = new DropDownList();
        public DropDownList objUOMList { get; set; } = new DropDownList();
        

        #endregion Additional Columns 
    }
    public class TT_Prod_CMPD_ProductionChild
    {
        public int ProductionChildID { get; set; }
        public int ProductionMasterID { get; set; }
        public int? FinishedProductID { get; set; }
        public int? UOMID { get; set; }
        public decimal Qty { get; set; } = 0;
        public string Remarks { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;
    }
}
