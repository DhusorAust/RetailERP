using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.Security
{
    public class UserList : BaseEntity
    {
        public UserList()
        {
            IsActive = 1;
            CreatedDate = DateTime.Now; 
        }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string PasswordH { get; set; }
        public DateTime PasswordExpireDate { get; set; }
        public int EmployeeID { get; set; }
        public int UserRoleID { get; set; }
        public int UserTypeID { get; set; }
        public byte IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsAuthorizedLogin { get; set; }

        #region Additional Columns  
        [Write(false)]
        public string EmployeeName { get; set; }
        [Write(false)]
        public string EmployeeNo { get; set; }
        [Write(false)]
        public int UnitID { get; set; }
        [Write(false)]
		public int DepartmentID { get; set; }
		[Write(false)]
		public string DepartmentName { get; set; } 
		[Write(false)]
		public int DesignationID { get; set; }
		[Write(false)]
		public string DesignationName { get; set; } 
		[Write(false)]
		public int SectionID { get; set; }
		[Write(false)]
		public string SectionName { get; set; } 

		[Write(false)]
		public int SubSectionID { get; set; }
		[Write(false)]
		public string SubSectionName { get; set; } 
		[Write(false)]
        public string UserRoleName { get; set; }
        [Write(false)]
        public string OldPassword { get; set; }
        [Write(false)]
        public string NewPassword { get; set; }
        [Write(false)]
        public string ConfirmPassword { get; set; }
        [Write(false)]
        public string DabaseName { get; set; }
        [Write(false)]
        public string IPAddress { get; set; }
        [Write(false)]
        public string UserTypeName { get; set; }
        [Write(false)]
        public string UserStatus { get; set; }
        [Write(false)]
        public List<DropDownList> UserRoleList { get; set; }
        [Write(false)]
        public List<DropDownList> EmployeeList { get; set; }
        [Write(false)]
        public List<DropDownList> IsActiveList { get; set; }

        [Write(false)]
        public byte[] EmpPhoto { get; set; }
        [Write(false)]
        public string Image64 { get { return EmpPhoto != null ? Convert.ToBase64String(EmpPhoto) : null; } }
        #endregion Additional Columns   
    }
}
