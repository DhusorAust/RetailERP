using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.AuditTrails
{
    public class TransactionUserInfo
    {
        public string EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public string EditBy { get; set; }
        public DateTime EditDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string AuditedBy { get; set; }
        public DateTime AuditedDate { get; set; }
        public string AcknowledgedBy { get; set; }
        public DateTime AcknowledgedDate { get; set; }
        public string Rejected { get; set; }
        public DateTime RejectedDate { get; set; }
    }
}
