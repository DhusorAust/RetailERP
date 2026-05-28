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
    public class UserViewPermissionService : IUserViewPermissionService
    {
        private readonly DapperContext<UserViewPermission> _context;
        private readonly SqlConnection _connection;
        public UserViewPermissionService(DapperContext<UserViewPermission> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        public async Task<GridEntity<UserViewPermission>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "UserID Desc";
            sql = $@"Select UL.*, UR.UserRoleCode, UR.UserRoleName, EL.EmployeeName
            From {DBSource.ERPUSERDB}..UserList UL
            Inner Join {DBSource.ERPUSERDB}..UserRole UR On UR.UserRoleID = UL.UserRoleID
            Left Join {DBSource.PDLHRM}..emp_EmploymentInfo EL On EL.EmployeeID = UL.EmployeeID
            Where UL.ProjectID = {AppGlobalFields.ProjectID} And UL.IsActive = 1 And UR.IsActive = 1";
            return await _context.GetKendoGridDataAsync<UserViewPermission>(options, sql, orderBy, "");
        }
        public List<children_node> GetMenuList(string UserID)
        {
            List<children_node> lParents = new List<children_node>();

            string strApplication = @"SELECT 0 APP_ID,'Retail ERP' APP_NAME";
            DataTable datatb = _context.ExecuteQuery(strApplication);
            for (int count = 0; count < datatb.Rows.Count; count++)
            {
                children_node Parent = new children_node();
                List<children_node> lChild = new List<children_node>();

                string strMenu1 = $@"Select m.MenuID, m.ProjectID, m.ProjectModuleID, data=m.ViewName, m.Controller, text=m.MenuHead, 
                priority = m.MenuPriority, m.ParentMenuID, Isnull (mp.IsPermission, 0) IsPermission, Isnull (mp.ViewPermissionID, 0) ViewPermissionID
                From {DBSource.ERPUSERDB}..Menu m left outer join 
                (
	                Select ViewPermissionID, MenuID, IsPermission
                    From {DBSource.ERPUSERDB}..UserViewPermission
                    Where UserID = {UserID}
                ) mp on m.MenuID = mp.MenuID
                Where m.IsActive = 1 And m.ProjectID = {AppGlobalFields.ProjectID}
                Order By m.MenuPriority";
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
        public async Task<UserViewPermission> SaveAsync(UserViewPermission entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@MenuID", entity.MenuID);
                parameters.Add("@UserID", entity.UserID);
                parameters.Add("@IsPermission", entity.IsPermission);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync(DBSource.ERPUSERDB + "..spSet_UserViewPermission", parameters, 30, CommandType.StoredProcedure);
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
        public void Dispose()
        {

        }
    }
}
