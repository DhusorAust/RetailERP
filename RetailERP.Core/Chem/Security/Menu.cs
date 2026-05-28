using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.Security
{
    [Table("Menu")]
    public class Menu : BaseEntity
    {
        public Menu()
        { 
            ParentMenuID = 0;
            IsActive = 1;
            CreatedDate = DateTime.Now;
            IsExcelExportAllowed = 0;
        }
        [Key]
        public int MenuID { get; set; }
        public int ProjectID { get; set; }
        public int ProjectModuleID { get; set; }
        public string ViewName { get; set; }
        public string Controller { get; set; }
        public string MenuHead { get; set; }
        public int MenuPriority { get; set; }
        public int ParentMenuID { get; set; }
        public string PageName { get; set; }
        public string MenuTitle { get; set; }
        public string MenuTitle2 { get; set; }
        public string MenuTitle3 { get; set; }
        public string MenuTitle4 { get; set; }
        public string MenuIcon { get; set; }
        public string Remarks { get; set; }
        public int IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? MenuReportID { get; set; }
        public int? IsExcelExportAllowed { get; set; }= 0; 

        #region Additional Columns   
        [Write(false)]
        public bool IsParent { get; set; }
        [Write(false)]
        public string[] sequence { get; set; }
        public string SequenceNo { get; set; }

        [Write(false)]
        public string ParameterValue { get; set; }

        [Write(false)]
        public int IsPermission { get; set; }
        [Write(false)]
        public string ProjectName { get; set; }
        [Write(false)]
        public string ModuleName { get; set; }

        [Write(false)]
        public List<DropDownList> ProjectList { get; set; }
        [Write(false)]
        public List<DropDownList> ProjectModuleList { get; set; }
        [Write(false)]
        public MenuPermissionRole objMenuPermissionRole { get; set; }=new MenuPermissionRole();
        [Write(false)]
        public List<MenuPermissionRole> MenuPermissionRoleList { get; set; } = new List<MenuPermissionRole>();

        #endregion Additional Columns   
    }
}
