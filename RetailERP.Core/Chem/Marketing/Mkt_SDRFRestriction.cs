using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    public class Mkt_SDRFRestriction
    {
        public int SDRFRestrictionID { get; set; }
        public int SDRFMasterID { get; set; }
        public int RestrictionID { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;

    }
    public class TT_Mkt_SDRFRestriction
    {
        public int SDRFRestrictionID { get; set; }
        public int SDRFMasterID { get; set; }
        public int RestrictionID { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;

    }
}
