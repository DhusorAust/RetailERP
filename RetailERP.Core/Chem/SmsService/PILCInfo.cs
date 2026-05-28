using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.SmsService
{
    public class PILCInfo
    {
        public string MonthYear { get; set; }
        public decimal PIQty { get; set; }
        public decimal PIAmount { get; set; }
        public decimal LCQty { get; set; }
        public decimal LCAmount { get; set; }
        public int SL { get; set; }
    }
}
