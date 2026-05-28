using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.Security
{
    [Table("ReportPermissionRole")]
    public class ReportPermissionRole : BaseEntity
    {
        public ReportPermissionRole()
        { 
            IsPermission = 0;
            CreatedDate = DateTime.Now;
        }
        public int ReportPermissionID { get; set; }
        public int MenuReportID { get; set; }
        public int UserRoleID { get; set; }
        public int IsPermission { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        #region Additional Columns   

        [Write(false)]
        public string UserRoleCode { get; set; }
        [Write(false)]
        public string UserRoleName { get; set; }
        #endregion Additional Columns   
    }
}
