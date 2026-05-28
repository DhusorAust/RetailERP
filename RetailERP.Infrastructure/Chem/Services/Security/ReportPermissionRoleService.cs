using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using Microsoft.Data.SqlClient;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Infrastructure.Chem.Services.Security
{
    public class ReportPermissionRoleService : IReportPermissionRoleService
    {
        private readonly DapperContext<ReportPermissionRole> _context;
        private readonly SqlConnection _connection;
        public ReportPermissionRoleService(DapperContext<ReportPermissionRole> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        public async Task<GridEntity<ReportPermissionRole>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "UserRoleName";
            sql = $@"
            Select Distinct ur.* 
            From {DBSource.ERPUSERDB}..UserRole ur
            Inner Join {DBSource.ERPUSERDB}..UserRolePermission urp On urp.UserRoleID = ur.UserRoleID And urp.ProjectID = {AppGlobalFields.ProjectID}
            Where urp.ProjectID = {AppGlobalFields.ProjectID}  And ur.IsActive = 1 ";
            return await _context.GetKendoGridDataAsync<ReportPermissionRole>(options, sql, orderBy, "");
        }
        //public List<children_node> GetReportsList(string UserRoleID)
        //{
        //    List<children_node> lParents = new List<children_node>();

        //    string strApplication = @"SELECT 0 APP_ID,'Retail ERP' APP_NAME";
        //    DataTable datatb = _context.ExecuteQuery(strApplication);
        //    for (int count = 0; count < datatb.Rows.Count; count++)
        //    {
        //        children_node Parent = new children_node();
        //        List<children_node> lChild = new List<children_node>();

        //        string strMenu1 = $@"





        //        SELECT m.MenuID, m.ProjectID, m.ProjectModuleID, m.ViewName, m.Controller, m.MenuHead, 
        //        m.MenuPriority As priority, m.ParentMenuID, Isnull (mp.IsPermission, 0) IsPermission,
        //        Isnull (mp.PermissionID, 0) PermissionID
        //        FROM {DBSource.ERPUSERDB}..Menu m 
        //        LEFT OUTER JOIN 
        //        (
        //         SELECT PermissionID, MenuID, IsPermission
        //            FROM {DBSource.ERPUSERDB}..MenuPermissionRole
        //            WHERE UserRoleID = {UserRoleID}
        //        ) mp ON m.MenuID = mp.MenuID
        //        WHERE m.IsActive = 1 And Controller = 'Reports' And m.ProjectID = {AppGlobalFields.ProjectID}
        //        ORDER BY m.MenuPriority";
        //        DataTable dtMenu1 = _context.ExecuteQuery(strMenu1);

        //        if (dtMenu1.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dtMenu1.Rows.Count; i++)
        //            {
        //                children_node child1 = new children_node();
        //                checkedstate chkstate = new checkedstate();

        //                child1.MenuID = dtMenu1.Rows[i]["MenuID"].ToString();
        //                child1.ProjectID = dtMenu1.Rows[i]["ProjectID"].ToString();
        //                child1.ProjectModuleID = dtMenu1.Rows[i]["ProjectModuleID"].ToString();
        //                child1.data = dtMenu1.Rows[i]["ViewName"].ToString();
        //                child1.Controller = dtMenu1.Rows[i]["Controller"].ToString();
        //                child1.text = dtMenu1.Rows[i]["MenuHead"].ToString();
        //                chkstate.selected = Int32.Parse(dtMenu1.Rows[i]["IsPermission"].ToString()) == 0 ? false : true;
        //                child1.state = chkstate;
        //                child1.permission_id = dtMenu1.Rows[i]["PermissionID"].ToString();
        //                child1.priority = dtMenu1.Rows[i]["priority"].ToString();
        //                child1.appid = datatb.Rows[count][0].ToString();
        //                child1.level = "1";


        //                //2nd Sub Menu
        //                List<children_node> lChild2 = new List<children_node>();
        //                string strMenu2 = $@"
        //                SELECT m.MenuID, m.ProjectID, m.ProjectModuleID, m.ViewName, m.Controller, m.MenuHead, 
        //                m.MenuPriority As priority, m.ParentMenuID, Isnull (mp.IsPermission, 0) IsPermission,
        //                Isnull (mp.PermissionID, 0) PermissionID
        //                FROM {DBSource.ERPUSERDB}..Menu m 
        //                LEFT OUTER JOIN 
        //                (
        //                 SELECT PermissionID, MenuID, IsPermission
        //                    FROM {DBSource.ERPUSERDB}..MenuPermissionRole
        //                    WHERE UserRoleID = {UserRoleID}
        //                ) mp ON m.MenuID = mp.MenuID
        //                WHERE m.IsActive = 1 And Controller = 'Reports' And m.ProjectID = {AppGlobalFields.ProjectID}
        //                And Isnull (m.ParentMenuID, 0) = {child1.MenuID}
        //                ORDER BY m.MenuPriority ";
        //                DataTable dtMenu2 = _context.ExecuteQuery(strMenu2);
        //                if (dtMenu2.Rows.Count > 0)
        //                {
        //                    for (int j = 0; j < dtMenu2.Rows.Count; j++)
        //                    {
        //                        children_node child2 = new children_node();
        //                        checkedstate chkstate2 = new checkedstate();
        //                        child2.MenuID = dtMenu2.Rows[j]["MenuID"].ToString();
        //                        child2.ProjectID = dtMenu2.Rows[j]["ProjectID"].ToString();
        //                        child2.ProjectModuleID = dtMenu2.Rows[j]["ProjectModuleID"].ToString();
        //                        child2.data = dtMenu2.Rows[j]["ViewName"].ToString();
        //                        child2.Controller = dtMenu2.Rows[j]["Controller"].ToString();
        //                        child2.text = dtMenu2.Rows[j]["MenuHead"].ToString();
        //                        chkstate.selected = Int32.Parse(dtMenu2.Rows[j]["IsPermission"].ToString()) == 0 ? false : true;
        //                        child2.state = chkstate;
        //                        child2.permission_id = dtMenu2.Rows[j]["PermissionID"].ToString();
        //                        child2.priority = dtMenu2.Rows[j]["priority"].ToString();
        //                        child2.appid = datatb.Rows[count][0].ToString();
        //                        child2.level = "2";

        //                        //Report Menu
        //                        List<children_node> lChild3 = new List<children_node>();
        //                        string strMenu3 = $@"
        //                        SELECT m.MenuReportID, m.ProjectID, m.ProjectModuleID, m.ViewName, m.Controller, m.MenuHead,
        //                        m.MenuID As ParentMenuID, Isnull (mp.IsPermission, 0) IsPermission,
        //                        Isnull (mp.ReportPermissionID, 0) ReportPermissionID
        //                        FROM {DBSource.ERPUSERDB}..MenuReport m LEFT OUTER JOIN 
        //                        (
        //                         SELECT ReportPermissionID, MenuReportID, IsPermission
        //                            FROM {DBSource.ERPUSERDB}..ReportPermissionRole
        //                            WHERE UserRoleID = {UserRoleID} And IsPermission = 1
        //                        ) mp ON m.MenuReportID = mp.MenuReportID
        //                        WHERE Isnull (m.MenuID, 0) = {child2.MenuID} 
        //                        And m.IsActive = 1 
        //                        Order By m.MenuHead ";
        //                        DataTable dtMenu3 = _context.ExecuteQuery(strMenu3);
        //                        if (dtMenu3.Rows.Count > 0)
        //                        {
        //                            for (int k = 0; k < dtMenu3.Rows.Count; k++)
        //                            {
        //                                children_node child3 = new children_node();
        //                                checkedstate chkstate3 = new checkedstate();
        //                                child3.MenuID = dtMenu3.Rows[k]["MenuReportID"].ToString();
        //                                child3.ProjectID = dtMenu3.Rows[k]["ProjectID"].ToString();
        //                                child3.ProjectModuleID = dtMenu3.Rows[k]["ProjectModuleID"].ToString();
        //                                child3.data = dtMenu3.Rows[k]["ViewName"].ToString();
        //                                child3.Controller = dtMenu3.Rows[k]["Controller"].ToString();
        //                                child3.text = dtMenu3.Rows[k]["MenuHead"].ToString();
        //                                chkstate3.selected = Int32.Parse(dtMenu3.Rows[k]["IsPermission"].ToString()) == 0 ? false : true;
        //                                child3.state = chkstate3;
        //                                child3.permission_id = dtMenu3.Rows[k]["ReportPermissionID"].ToString();
        //                                child3.priority = "0";
        //                                child3.appid = datatb.Rows[count][0].ToString();
        //                                child3.level = "3";
        //                                lChild3.Add(child3);
        //                            }
        //                        }
        //                        //2nd Menu
        //                        child2.children = lChild3;
        //                        lChild2.Add(child2);
        //                    }
        //                }
        //                else
        //                { 
        //                    children_node child2 = new children_node();
        //                    checkedstate chkstate2 = new checkedstate();

        //                    //Report Menu
        //                    List<children_node> lChild3 = new List<children_node>();
        //                    string strMenu3 = $@"
        //                        SELECT m.MenuReportID, m.ProjectID, m.ProjectModuleID, m.ViewName, m.Controller, m.MenuHead,
        //                        m.MenuID As ParentMenuID, Isnull (mp.IsPermission, 0) IsPermission,
        //                        Isnull (mp.ReportPermissionID, 0) ReportPermissionID
        //                        FROM {DBSource.ERPUSERDB}..MenuReport m LEFT OUTER JOIN 
        //                        (
        //                         SELECT ReportPermissionID, MenuReportID, IsPermission
        //                            FROM {DBSource.ERPUSERDB}..ReportPermissionRole
        //                            WHERE UserRoleID = {UserRoleID} And IsPermission = 1
        //                        ) mp ON m.MenuReportID = mp.MenuReportID
        //                        WHERE Isnull (m.MenuID, 0) = {child1.MenuID} 
        //                        And m.IsActive = 1 
        //                        Order By m.MenuHead ";
        //                    DataTable dtMenu3 = _context.ExecuteQuery(strMenu3);
        //                    if (dtMenu3.Rows.Count > 0)
        //                    {
        //                        for (int k = 0; k < dtMenu3.Rows.Count; k++)
        //                        {
        //                            children_node child3 = new children_node();
        //                            checkedstate chkstate3 = new checkedstate();
        //                            child3.MenuID = dtMenu3.Rows[k]["MenuReportID"].ToString();
        //                            child3.ProjectID = dtMenu3.Rows[k]["ProjectID"].ToString();
        //                            child3.ProjectModuleID = dtMenu3.Rows[k]["ProjectModuleID"].ToString();
        //                            child3.data = dtMenu3.Rows[k]["ViewName"].ToString();
        //                            child3.Controller = dtMenu3.Rows[k]["Controller"].ToString();
        //                            child3.text = dtMenu3.Rows[k]["MenuHead"].ToString();
        //                            chkstate3.selected = Int32.Parse(dtMenu3.Rows[k]["IsPermission"].ToString()) == 0 ? false : true;
        //                            child3.state = chkstate3;
        //                            child3.permission_id = dtMenu3.Rows[k]["ReportPermissionID"].ToString();
        //                            child3.priority = "0";
        //                            child3.appid = datatb.Rows[count][0].ToString();
        //                            child3.level = "3";
        //                            lChild3.Add(child3);
        //                        }
        //                    }
        //                    //2nd Menu
        //                    child2.children = lChild3;
        //                    lChild2.Add(child2); 
        //                }

        //                //1st Menu
        //                child1.children = lChild2;
        //                lChild.Add(child1);
        //            }
        //            Parent.appid = datatb.Rows[count][0].ToString();
        //            Parent.text = datatb.Rows[count][1].ToString();
        //            Parent.level = "0";
        //            Parent.children = lChild;
        //            lParents.Add(Parent);
        //        }
        //    }

        //    return lParents;
        //}


        public List<children_node> GetReportsList(string UserRoleID)
        {
            List<children_node> lParents = new List<children_node>();

            string strApplication = @"SELECT 0 APP_ID,'Retail ERP' APP_NAME";
            DataTable datatb = _context.ExecuteQuery(strApplication);
            for (int count = 0; count < datatb.Rows.Count; count++)
            {
                children_node Parent = new children_node();
                List<children_node> lChild = new List<children_node>();

                string strMenu1 = $@"
                    Select * From 
                    (
	                    Select m.MenuID, m.ProjectID, m.ProjectModuleID, data=m.ViewName, m.Controller, text=m.MenuHead, 
	                    priority = m.MenuPriority, m.ParentMenuID, Isnull (mp.IsPermission, 0) IsPermission, Isnull (mp.PermissionID, 0) PermissionID
	                    From {DBSource.ERPUSERDB}..Menu m left outer join 
	                    (
		                    Select PermissionID, MenuID, IsPermission
		                    From {DBSource.ERPUSERDB}..MenuPermissionRole
		                    Where UserRoleID = {UserRoleID}
	                    ) mp on m.MenuID = mp.MenuID
	                    Where m.IsActive = 1 And m.ProjectID = {AppGlobalFields.ProjectID} And Controller = 'Reports'

	                    Union

	                    Select m.MenuReportID As MenuID, m.ProjectID, m.ProjectModuleID, data=m.ViewName, m.Controller, text=m.MenuHead, 
                        m.MenuPriority As priority, m.ParentMenuID, isnull (mp.IsPermission, 0) IsPermission, isnull (mp.ReportPermissionID, 0) PermissionID 
	                    From {DBSource.ERPUSERDB}..MenuReport m left outer join 
	                    (
		                    Select ReportPermissionID, MenuReportID, IsPermission
		                    From {DBSource.ERPUSERDB}..ReportPermissionRole
		                    Where UserRoleID = {UserRoleID} 
	                    ) mp on m.MenuReportID = mp.MenuReportID
	                    Where m.IsActive = 1 And m.ProjectID = {AppGlobalFields.ProjectID}
                    )A Order By A.priority ";

                var dtMenu1 = _context.GetDataAsync<children_node>(strMenu1);
                var menuList = dtMenu1.Result.ToList();
                if (menuList.Count > 0)
                {
                    var loopCount = 1;
                    foreach (var parentMenu in menuList)
                    {
                        if (parentMenu.ParentMenuID == 0)
                        {
                            children_node child1 = new children_node();
                            parentMenu.state = new checkedstate() { selected = Convert.ToBoolean(parentMenu.IsPermission) };
                            parentMenu.appid = "0";
                            parentMenu.level = loopCount.ToString();
                            child1 = parentMenu;

                            var chloopCount = loopCount + 1;
                            ChildMenu(menuList, parentMenu, child1, chloopCount);

                            lChild.Add(child1);
                        }
                    }
                    Parent.appid = "0";
                    Parent.text = "Retail ERP";
                    Parent.level = "0";
                    Parent.children = lChild;
                    lParents.Add(Parent);
                }
            }
            return lParents;
        }
        private static void ChildMenu(List<children_node> menuList, children_node parentMenu, children_node child1, int loopCount)
        {
            var childMenuList = menuList;
            if (childMenuList != null && childMenuList.Count > 0)
            {
                foreach (var chMenu in childMenuList)
                {
                    if (chMenu.ParentMenuID == Convert.ToInt32(parentMenu.MenuID))
                    {
                        chMenu.state = new checkedstate() { selected = Convert.ToBoolean(chMenu.IsPermission) };
                        chMenu.appid = "0";
                        chMenu.level = loopCount.ToString();
                        child1 = parentMenu;

                        child1.children.Add(chMenu);

                        //Recursive Call Child Node
                        loopCount++;
                        ChildMenu(childMenuList, chMenu, child1, loopCount);
                    }
                }
            }
        } 
        public async Task<ReportPermissionRole> SaveAsync(ReportPermissionRole entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                //parameters.Add("@ReportPermissionID", entity.ReportPermissionID);
                parameters.Add("@MenuReportID", entity.MenuReportID); 
                parameters.Add("@UserRoleID", entity.UserRoleID);
                parameters.Add("@IsPermission", entity.IsPermission);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync(DBSource.ERPUSERDB + "..spSet_ReportPermissionRole", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");

                if (entity.NoofRows > 0 && entity.ResultId > 0)
                {
                    entity.Message = "Save operation successful.";
                }
                else
                {
                    entity.Message = "Save operation failed.";
                }
            }
            catch (Exception ex)
            {
                entity.Message = ex.Message;
            }
            finally
            {
                parameters = null;
            }
            return entity;
        } 
    }
}
