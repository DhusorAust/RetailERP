using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    public class Mkt_BriefTemplateConsumer
    {
        public int BTConsumerID { get; set; }
        public int BTMasterID { get; set; }
        public int GenderID { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public int RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }
    public class TT_Mkt_BriefTemplateConsumer
    {
        public int BTConsumerID { get; set; }
        public int BTMasterID { get; set; }
        public int GenderID { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public int RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }

}
