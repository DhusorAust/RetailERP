using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    public class Mkt_BriefTemplateProductTest
    {
        public int BTProductTestID { get; set; }
        public int BTMasterID { get; set; }
        public int ProductTestID { get; set; }
        public bool IsTested { get; set; }
        public string Remarks { get; set; }
        public int RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }
    public class TT_Mkt_BriefTemplateProductTest
    {
        public int BTProductTestID { get; set; }
        public int BTMasterID { get; set; }
        public int ProductTestID { get; set; }
        public bool IsTested { get; set; }
        public string Remarks { get; set; }
        public int RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }


}
