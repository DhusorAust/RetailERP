using RetailERP.Core.Common;
using Dapper.Contrib.Extensions; 
using System;
using System.Collections.Generic;
using System.Text;

namespace RetailERP.Core.Chem.Marketing
{
    [Table("mkt_BuyerRefColorInfo")]
    public class Mkt_BuyerRefColorInfo
    {
        public Mkt_BuyerRefColorInfo()
        {
             
        }
        public int BuyerRefColorID { get; set; }
        public int BuyerRefMasterID { get; set; }
        public int? ColorID { get; set; } 
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0; 

        #region Additional Columns 
        [Write(false)]
        public string ColorName { get; set; }  

        #endregion Additional Columns
    }
    public class TT_Mkt_BuyerRefColorInfo
    {
        public int BuyerRefColorID { get; set; }
        public int BuyerRefMasterID { get; set; }
        public int? ColorID { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;
    }
}
