using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RetailERP.Core.Chem.Security
{
    public class ProjectModule
    {
        public ProjectModule()
        { 
            IsActive = 1;
            CreatedDate = DateTime.Now;
        }
        public int ProjectModuleID { get; set; }
        public string ModuleName { get; set; } 
        public int ModulePriority { get; set; } 
        public int IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
