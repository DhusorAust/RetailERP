using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.Security
{
    [Table("ReportPermissionUser")]
    public class ReportPermissionUser : BaseEntity
    {
        public ReportPermissionUser()
        {
            IsPermission = 0;
            CreatedDate = DateTime.Now;
        }
        public int ReportPermissionID { get; set; }
        public int MenuReportID { get; set; }
        public int UserID { get; set; }
        public int IsPermission { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        #region Additional Columns   

        [Write(false)]
        public string UserName { get; set; }
         
        #endregion Additional Columns   
    }
}
