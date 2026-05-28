using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.Security
{
    [Table("MenuReport")]
    public class MenuReport : BaseEntity
    {
        public MenuReport()
        {  
            IsActive = 1;
            CreatedDate = DateTime.Now;
        } 
        public int MenuReportID { get; set; }
        public int MenuID { get; set; }
        public int ProjectID { get; set; }
        public int ProjectModuleID { get; set; }
        public string ViewName { get; set; }
        public string Controller { get; set; }
        public string MenuHead { get; set; }
        public int MenuPriority { get; set; } 
        public string DevExpressReportName { get; set; }
        public string ReportProcedureName { get; set; }
        public string Remarks { get; set; } 
        public int IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }  
    }
}
