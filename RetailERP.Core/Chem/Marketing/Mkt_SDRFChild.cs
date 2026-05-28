using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    [Table("mkt_SDRFChild")]
    public class Mkt_SDRFChild
    {
        public int SDRFChildID { get; set; }
        public int SDRFMasterID { get; set; }
        public int PackagingTypeID { get; set; }
        public decimal SizeGram { get; set; } = 0;
        public int UOMID { get; set; }
        public decimal Qty { get; set; } = 0;
        public bool IsPrimary { get; set; } = false;
        public string Remarks { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;

        #region Additional Columns 
        public string PackagingType { get; set; }
        public string UOM { get; set; }
        public DropDownList objPackagingTypeList { get; set; } = new DropDownList();
        public DropDownList objUOMList { get; set; } = new DropDownList();

        #endregion Additional Columns



    }
    public class TT_Mkt_SDRFChild
    {
        public int SDRFChildID { get; set; }
        public int SDRFMasterID { get; set; }
        public int PackagingTypeID { get; set; }
        public decimal SizeGram { get; set; }
        public int UOMID { get; set; }
        public decimal Qty { get; set; }
        public bool IsPrimary { get; set; } = false;
        public string Remarks { get; set; }
        public int RowSLNo { get; set; }
        public int RowSLNoNew { get; set; }        
        public int IsDeleted { get; set; }

    }
}
