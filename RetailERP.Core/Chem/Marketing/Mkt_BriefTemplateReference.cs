using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    public class Mkt_BriefTemplateReference
    {
        public int BTReferenceID { get; set; }
        public int BTMasterID { get; set; }
        public string ReferenceProductName { get; set; }
        public string ReferenceBrand { get; set; }
        public string ReferenceLink { get; set; }
        public string Remarks { get; set; }
        public int IsActive { get; set; }
        public int RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }
    public class TT_Mkt_BriefTemplateReference
    {
        public int BTReferenceID { get; set; }
        public int BTMasterID { get; set; }
        public string ReferenceProductName { get; set; }
        public string ReferenceBrand { get; set; }
        public string ReferenceLink { get; set; }
        public string Remarks { get; set; }
        public int IsActive { get; set; }
        public int RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }


}
