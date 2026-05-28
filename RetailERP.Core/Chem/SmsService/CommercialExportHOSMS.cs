using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.SmsService
{
    public class CommercialExportHOSMS
    {
        public DateTime TransDate { get; set; }
        public string UnitName { get; set; }
        public string ShortUnitName { get; set; }
        public decimal PIQty { get; set; }
        public decimal PIAmount { get; set; }
        public decimal LCQty { get; set; }
        public decimal LCAmount { get; set; }
        public decimal PartyQty { get; set; }
        public decimal PartyAmount { get; set; }
        public decimal BankQty { get; set; }
        public decimal BankAmount { get; set; }
        public decimal LCQtyTR { get; set; }
        public decimal LCAmountTR { get; set; }
        public decimal PartyAccPendingQty { get; set; }//DH_QTY
        public decimal PartyAccPendingAmount { get; set; }//DH_AMT
        public decimal BankAccPendingAmount { get; set; }//BAP_AMT

        public List<CommercialExportHOSMS> ComExSMS { get; set; } = new List<CommercialExportHOSMS>();
        public List<PILCInfo> PILCInfo { get; set; } = new List<PILCInfo>();
        public List<BankAcceptanceInfo> BankAcceptanceInfo { get; set; } = new List<BankAcceptanceInfo>();
        public List<LCInHand> LCInHand { get; set; } = new List<LCInHand>();

    }
}
