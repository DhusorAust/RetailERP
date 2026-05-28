using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailERP.Core.Chem.Security
{
    public class SecurityModels
    {
    }
    [Serializable]
    public class ALLMENU
    {
        public List<Menu> MenuList { get; set; }
        public List<UserList> UserList = new List<UserList>();
    }

    [Serializable]
    public class children_node
    {
        public children_node()
        {
            children = new List<children_node>();
            state = new checkedstate();
        }
        public List<children_node> children { get; set; }
        public checkedstate state { get; set; }
        public string data { get; set; }
        public string text { get; set; }
        public string MenuID { get; set; }
        public string MenuReportID { get; set; }
        public string permission_id { get; set; }
        public string ProjectID { get; set; }
        public string ProjectModuleID { get; set; }
        public string PageName { get; set; }
        public string appid { get; set; }
        public string level { get; set; }
        public string Controller { get; set; }
        public string priority { get; set; }
        public string isReloaded { get; set; }
        public string isNewWindow { get; set; }
        public string icon { get; set; }
        public int ParentMenuID { get; set; }
        public int IsPermission { get; set; } 
        public string MenuTitle { get; set; }
        public string MenuTitle2 { get; set; }
        public string MenuTitle3 { get; set; }
        public string MenuTitle4 { get; set; }
        public string MenuIcon { get; set; }
        public string Remarks { get; set; }        
    }

    [Serializable]
    public class checkedstate
    {
        public bool selected { get; set; }
    } 
}


