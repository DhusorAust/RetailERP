using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.Security
{
    [Table("UserViewPermission")]
    public class UserViewPermission : BaseEntity
    {
        public UserViewPermission()
        {
            IsPermission = 0;
            CreatedDate = DateTime.Now;
        }
        public int ViewPermissionID { get; set; } 
        public int UserID { get; set; }
        public int MenuID { get; set; }
        public int IsPermission { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        #region Additional Columns   

        [Write(false)]
        public string UserName { get; set; }
        [Write(false)]
        public string UserRoleCode { get; set; }
        [Write(false)]
        public string UserRoleName { get; set; }
        [Write(false)]
        public string EmployeeName { get; set; }

        #endregion Additional Columns  
    }
}
