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
    public class ReportPermissionUserService : IReportPermissionUserService
    {
        private readonly DapperContext<ReportPermissionUser> _context;
        private readonly SqlConnection _connection;
        public ReportPermissionUserService(DapperContext<ReportPermissionUser> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        public async Task<GridEntity<ReportPermissionUser>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "UserID Desc";
            sql = $@"Select * From {DBSource.ERPUSERDB}..UserList Where IsActive = 1";
            return await _context.GetKendoGridDataAsync<ReportPermissionUser>(options, sql, orderBy, "");
        }
        public List<children_node> GetReportsList(string UserID)
        {
            List<children_node> lParents = new List<children_node>();

            string strApplication = @"SELECT 0 APP_ID,'Retail ERP Report' APP_NAME";
            DataTable datatb = _context.ExecuteQuery(strApplication);
            for (int count = 0; count < datatb.Rows.Count; count++)
            {
                children_node Parent = new children_node();
                List<children_node> lChild = new List<children_node>();

                string strMenu1 = $@"SELECT m.MenuReportID, m.ProjectID, m.ProjectModuleID, m.ViewName, m.Controller, m.MenuHead, 
                m.MenuPriority, m.ParentMenuID, Isnull (mp.IsPermission, 0) IsPermission, Isnull (mp.PermissionID, 0) PermissionID
                FROM {DBSource.ERPUSERDB}..Menu m LEFT OUTER JOIN 
                (
	                SELECT PermissionID, MenuID, IsPermission
                    FROM {DBSource.ERPUSERDB}..ReportPermissionUser
                    WHERE UserID = {UserID}
                ) mp ON m.MenuID = mp.MenuID
                WHERE m.IsActive = 1 And Controller = 'Reports'
                ORDER BY m.MenuPriority";
                DataTable dtMenu1 = _context.ExecuteQuery(strMenu1);

                if (dtMenu1.Rows.Count > 0)
                {
                    for (int i = 0; i < dtMenu1.Rows.Count; i++)
                    {
                        children_node child1 = new children_node();
                        checkedstate chkstate = new checkedstate();

                        child1.MenuID = dtMenu1.Rows[i]["MenuID"].ToString();
                        child1.ProjectModuleID = dtMenu1.Rows[i]["ProjectID"].ToString();
                        child1.ProjectModuleID = dtMenu1.Rows[i]["ProjectModuleID"].ToString();
                        child1.data = dtMenu1.Rows[i]["ViewName"].ToString();
                        child1.Controller = dtMenu1.Rows[i]["Controller"].ToString();
                        child1.text = dtMenu1.Rows[i]["MenuHead"].ToString();
                        chkstate.selected = int.Parse(dtMenu1.Rows[i]["IsPermission"].ToString()) == 0 ? false : true;
                        child1.state = chkstate;
                        child1.permission_id = dtMenu1.Rows[i]["PermissionID"].ToString();
                        child1.priority = dtMenu1.Rows[i]["MenuPriority"].ToString();
                        child1.appid = datatb.Rows[count][0].ToString();
                        child1.level = "1";

                        //2nd Sub Menu
                        List<children_node> lChild2 = new List<children_node>();
                        string strMenu2 = $@"
                        SELECT m.MenuReportID, m.ProjectID, m.ProjectModuleID, m.ViewName, m.Controller, m.MenuHead, m.MenuPriority, 
                        m.MenuID As ParentMenuID, Isnull (mp.IsPermission, 0) IsPermission, Isnull (mp.IsPermission, 0) PermissionID
                        FROM {DBSource.ERPUSERDB}..MenuReport m LEFT OUTER JOIN 
                        (
	                        SELECT ReportPermissionID, MenuReportID, IsPermission
                            FROM {DBSource.ERPUSERDB}..ReportPermissionUser
                            WHERE UserID = {UserID} And IsPermission = 1
                        ) mp ON m.MenuReportID = mp.MenuReportID
                        WHERE Isnull (m.MenuID, 0) = {child1.MenuID} And m.IsActive = 1 
                        Order By m.MenuHead";
                        DataTable dtMenu2 = _context.ExecuteQuery(strMenu2);
                        if (dtMenu2.Rows.Count > 0)
                        {
                            for (int j = 0; j < dtMenu2.Rows.Count; j++)
                            {
                                children_node child2 = new children_node();
                                checkedstate chkstate2 = new checkedstate();
                                child2.MenuID = dtMenu2.Rows[j]["MenuReportID"].ToString();
                                child2.ProjectID = dtMenu2.Rows[j]["ProjectID"].ToString();
                                child2.ProjectModuleID = dtMenu2.Rows[j]["ProjectModuleID"].ToString();
                                child2.data = dtMenu2.Rows[j]["ViewName"].ToString();
                                child2.Controller = dtMenu2.Rows[j]["Controller"].ToString();
                                child2.text = dtMenu2.Rows[j]["MenuHead"].ToString();
                                chkstate2.selected = int.Parse(dtMenu2.Rows[j]["IsPermission"].ToString()) == 0 ? false : true;
                                child2.state = chkstate2;
                                child2.permission_id = dtMenu2.Rows[j]["PermissionID"].ToString();
                                child2.priority = dtMenu2.Rows[j]["MenuPriority"].ToString();
                                child2.appid = datatb.Rows[count][0].ToString();
                                child2.level = "2";
                                lChild2.Add(child2);
                            }
                        }
                        //1st Menu
                        child1.children = lChild2;
                        lChild.Add(child1);
                    }
                    Parent.appid = datatb.Rows[count][0].ToString();
                    Parent.text = datatb.Rows[count][1].ToString();
                    Parent.level = "0";
                    Parent.children = lChild;
                    lParents.Add(Parent);
                }
            }

            return lParents;
        }
        public async Task<ReportPermissionUser> SaveAsync(ReportPermissionUser entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@ReportPermissionID", entity.ReportPermissionID);
                parameters.Add("@MenuReportID", entity.MenuReportID);
                parameters.Add("@UserID", entity.UserID);
                parameters.Add("@IsPermission", entity.IsPermission);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync(DBSource.ERPUSERDB + "..spSet_ReportPermissionUser", parameters, 30, CommandType.StoredProcedure);
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
