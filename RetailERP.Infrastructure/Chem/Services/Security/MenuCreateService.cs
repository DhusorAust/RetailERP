using Dapper;
using Microsoft.VisualBasic;
using RetailERP.Core.Common;
using RetailERP.Core.Chem.Security;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Infrastructure.Chem.Services.Security
{
    public class MenuCreateService : IMenuCreateService
    {
        private readonly DapperContext<Menu> _context;
        private readonly SqlConnection _connection;
        public MenuCreateService(DapperContext<Menu> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        #region Menu Create
        public async Task<Menu> Project()
        {
            var query = $@"Select ProjectID As id, ProjectName As [text] 
            From {DBSource.ERPUSERDB}..Project 
            Where IsActive = 1 And ProjectID = {AppGlobalFields.ProjectID}";
            Menu data = new Menu();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data.ProjectList = records.Read<DropDownList>().ToList();
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
            return data;
        }
        public async Task<Menu> ProjectModule(int id)
        {
            var query = $@"Select ProjectModuleID As id, ModuleName As [text]
            From {DBSource.ERPUSERDB}..ProjectModule 
            Where IsActive = 1 And ProjectID = {AppGlobalFields.ProjectID}";
            Menu data = new Menu();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query); 
                data.ProjectModuleList = records.Read<DropDownList>().ToList(); 
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
            return data;
        } 
        public List<children_node> MenuList()
        {
            List<children_node> lParents = new List<children_node>();

            string strApplication = @"SELECT 0 APP_ID,'Retail ERP' APP_NAME";
            DataTable datatb = _context.ExecuteQuery(strApplication);
            for (int count = 0; count < datatb.Rows.Count; count++)
            {
                children_node Parent = new children_node();
                List<children_node> lChild = new List<children_node>();

                string strMenu1 = $@"Select MenuID, ProjectID, ProjectModuleID, data = ViewName, Controller, [text] = MenuHead, 
                priority = MenuPriority, ParentMenuID, PageName, MenuTitle, MenuTitle2, MenuTitle3, MenuTitle4, MenuIcon, Remarks  
                From {DBSource.ERPUSERDB}..Menu 
                Where ProjectID = {AppGlobalFields.ProjectID} Order By MenuPriority ";
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
        public List<Menu> MoveMenu(Menu entity)
        {
            int ResultId = 0;
            List<Menu> objMenu = new List<Menu>();
            Dictionary<string, string> Dict = new Dictionary<string, string>();
            if (entity.IsParent)
            {
                string SQL = $@" 
                UPDATE {DBSource.ERPUSERDB}..Menu SET ProjectModuleID = '" + entity.ProjectModuleID + $@"'
                WHERE MenuID IN
                (
	                SELECT MenuID FROM {DBSource.ERPUSERDB}..Menu WHERE ProjectID = {AppGlobalFields.ProjectID} And ParentMenuID = '" + entity.MenuID + $@"'
	                UNION
	                SELECT MenuID FROM {DBSource.ERPUSERDB}..Menu WHERE ProjectID = {AppGlobalFields.ProjectID} And ParentMenuID IN (SELECT MenuID FROM {DBSource.ERPUSERDB}..Menu WHERE ProjectID = {AppGlobalFields.ProjectID} And ParentMenuID = '" + entity.MenuID + @"')
                )";
                SqlCommand cmd = new SqlCommand(SQL, _context.Connection);
                _context.Connection.Open();
                cmd.ExecuteNonQuery();
                _context.Connection.Close();
            }

            for (int i = 0; i < entity.sequence.Length; i++)
            {
                string SQL = $@"UPDATE {DBSource.ERPUSERDB}..Menu SET 
                MenuPriority = " + (i + 1).ToString() + $@", 
                ParentMenuID = " + entity.ParentMenuID + $@", 
                ProjectModuleID = '" + entity.ProjectModuleID + $@"' 
                WHERE ProjectID = {AppGlobalFields.ProjectID} And MenuID = '" + entity.sequence[i].ToString() + $@"' ";
                SqlCommand cmd = new SqlCommand(SQL, _context.Connection);
                _context.Connection.Open();
                ResultId = cmd.ExecuteNonQuery();
                _context.Connection.Close();
            }

            if (ResultId > 0)
            {
                Menu MenuList = new Menu();
                MenuList.ResultId = ResultId;
                objMenu.Add(MenuList);
            } 
            return objMenu;
        } 
        public async Task<Menu> SaveAsync(Menu entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@MenuID", entity.ParentMenuID);
                parameters.Add("@ProjectID", AppGlobalFields.ProjectID);
                parameters.Add("@ProjectModuleID", entity.ProjectModuleID);
                parameters.Add("@ViewName", entity.ViewName);
                parameters.Add("@Controller", entity.Controller);
                parameters.Add("@MenuHead", entity.MenuHead);
                parameters.Add("@PageName", entity.PageName);
                parameters.Add("@MenuTitle", entity.MenuTitle);
                parameters.Add("@MenuTitle2", entity.MenuTitle2);
                parameters.Add("@MenuTitle3", entity.MenuTitle3);
                parameters.Add("@MenuTitle4", entity.MenuTitle4);
                parameters.Add("@MenuIcon", entity.MenuIcon);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync(DBSource.ERPUSERDB + "..spSet_Menu", parameters, 30, CommandType.StoredProcedure);
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

        #endregion 
        public void Dispose()
        {
            //throw new NotImplementedException();
        }  
    }
}
