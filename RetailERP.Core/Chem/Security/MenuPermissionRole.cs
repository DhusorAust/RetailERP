using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.Security
{
    [Table("MenuPermissionRole")]
    public class MenuPermissionRole : BaseEntity
    { 
        public MenuPermissionRole()
        { 
            IsPermission = 1;
            CreatedDate = DateTime.Now;
        }
        public int PermissionID { get; set; }
        public int MenuID { get; set; }
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
