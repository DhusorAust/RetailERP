using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.Security
{
    [Table("UserRole")]
    public class UserRole : BaseEntity
    {
        public UserRole()
        {
            UserRoleCode = AppGlobalFields.NEW;
            CreatedDate = DateTime.Now;
            IsActive = 1;
        }
        public int UserRoleID { get; set; }
        public string ProjectID { get; set; }
        public string UserRoleCode { get; set; }
        public string UserRoleName { get; set; }
        public byte IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        #region Additional Columns  
        [Write(false)]
        public string RoleStatus { get; set; }
        [Write(false)]
        public string ProjectName { get; set; }
        [Write(false)]
        public List<DropDownList> IsActiveList { get; set; }
        [Write(false)]
        public List<UserRoleChild> Childs { get; set; }=new List<UserRoleChild>();
        [Write(false)]
        public List<DropDownList> ProjectList { get; set; } = new List<DropDownList>();

        [Write(false)]
        public List<UserRolePivotTest> C_PivotTest { get; set; } = new List<UserRolePivotTest>();
        #endregion Additional Columns   
    }

    public class UserRoleChild
    { 
        public int UserRoleID { get; set; }
        public string UserRoleCode { get; set; }
        public string UserRoleName { get; set; }
        public byte IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } 
        public string RoleStatus { get; set; } 
    }
    public class UserRolePivotTest
    {
        public int UserRoleID { get; set; }
        public int ProjectID { get; set; }
        public string UserRoleCode { get; set; }
        public string UserRoleName { get; set; }
        public byte IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

