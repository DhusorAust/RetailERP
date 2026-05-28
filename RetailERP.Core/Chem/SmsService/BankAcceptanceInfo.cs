using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.SmsService
{
    public class BankAcceptanceInfo
    {
        public int SL { get; set; }
        public string UnitShortName { get; set; }
        public string BankName { get; set; }
        public string TotalAmount { get; set; }
    }
}
