
using System;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.AuditTrails
{
    public class AuditTrailEntity : BaseEntity
    {
        public int AuditTrailID { get; set; }
        public DateTime AuditTrailDate { get; set; }
        public string AuditTrailType { get; set; }
        public string AuditTrailDescription { get; set; }
        public int ProjectID { get; set; }
        public int? MenuID { get; set; }
        public string ViewPage { get; set; }
        public int? IsActive { get; set; }
    }
}
