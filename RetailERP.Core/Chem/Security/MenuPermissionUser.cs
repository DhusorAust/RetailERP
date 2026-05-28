using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.Security
{
    [Table("MenuPermissionUser")]
    public class MenuPermissionUser : BaseEntity
    {
        public MenuPermissionUser()
        {
            IsPermission = 1;
            CreatedDate = DateTime.Now;
        }
        public int PermissionID { get; set; }
        public int MenuID { get; set; }
        public int UserID { get; set; }
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
