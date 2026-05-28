using Dapper;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
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
    public class UserRoleService : IUserRoleService
    {
        private readonly DapperContext<UserRole> _context;
        private readonly SqlConnection _connection;
        public UserRoleService(DapperContext<UserRole> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@" 
            -- Is Active
            Select 1 id, 'Active' As [text] Union Select 0 id, 'Inactive' As [text];  "; 
        public async Task<GridEntity<UserRole>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "UserRoleName asc";

            sql = $@"
                Select ur.UserRoleID, ur.UserRoleCode, ur.UserRoleName, 
                STRING_AGG(p.ProjectName, ', ')ProjectName, ur.IsActive, (Case When ur.IsActive = 1 then 'Active' Else 'Inactive' End)RoleStatus
                From {DBSource.ERPUSERDB}..UserRole ur
                Left Join {DBSource.ERPUSERDB}..ProjectRolePermission prp On prp.UserRoleID = ur.UserRoleID
                Left Join {DBSource.ERPUSERDB}..Project p On p.ProjectID = prp.ProjectID 
                Group By ur.UserRoleID, ur.UserRoleCode, ur.UserRoleName, ur.IsActive ";
            return await _context.GetKendoGridDataAsync<UserRole>(options, sql, orderBy, "");
        } 
        //public async Task<UserRole> PivotTestGetList()
        //{
        //    UserRole data = new();
        //    string sql = "";
            
        //    try
        //    {
          
        //        sql = $@"Select * From {DBSource.ERPUSERDB}..UserRole";

        //        DataTable DT = _context.ExecuteQuery(sql);

        //        await _connection.OpenAsync();
        //        var records = await _connection.QueryMultipleAsync(sql);
        //       // await records.ReadFirstOrDefaultAsync<UserRole>();
        //        data.C_PivotTest = records.Read<UserRolePivotTest>().ToList();
        //        //data.Childs.ForEach(item =>
        //        //{
        //        //    var itemCode = item.FabricStyleName != null && item.FabricStyleName != "" ? item.FabricStyleName : item.ItemCodeName;
        //        //    item.ItemCodeName = itemCode;
        //        //});
        //    }
        //    catch (Exception ex)
        //    {
        //        data.Message = ex.Message;
        //        data.ErrNo = 1;
        //    }
        //    finally
        //    {
        //        if (_connection.State == ConnectionState.Open) _connection.Close();
        //    }
        //    return data;
        //} 
        //public List<UserRole> DataTableToObject(DataTable dtInput)
        //{
        //    List<UserRole> objectList = new List<UserRole>();

        //    foreach (DataRow dr in dtInput.Rows)
        //    {
        //        UserRole newObj = new UserRole();
        //        newObj.UserRoleID = Convert.ToInt32(dr["UserRoleID"]);  
        //        newObj.UserRoleCode = dr["UserRoleCode"].ToString();
        //        newObj.UserRoleName = dr["UserRoleName"].ToString();
        //        newObj.CreatedDate = Convert.ToDateTime(dr["CreatedDate"].ToString());
        //        objectList.Add(newObj);
        //    }
        //    return objectList;
        //}
        public async Task<UserRole> GetEdit(int id)
        {
            var query =
                $@"SELECT ur.* 
                FROM {DBSource.ERPUSERDB}..UserRole ur
                Inner Join {DBSource.ERPUSERDB}..UserRolePermission urp On urp.UserRoleID = urp.UserRoleID
                Where urp.ProjectID = {AppGlobalFields.ProjectID} And ur.UserRoleID = {id}
                ORDER BY ur.UserRoleName;
                {CommonQuery}";

            UserRole data = new UserRole();

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query); 
                data = await records.ReadFirstOrDefaultAsync<UserRole>();
                if (data != null)
                {
                    data.IsActiveList = records.Read<DropDownList>().ToList();
                }
                else
                {
                    data = new UserRole();
                    data.Message = AppGlobalFields.AdminMessage;
                    data.ErrNo = 1;
                } 
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
                data.ErrNo = 1;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
            return data;
        }
        public async Task<UserRole> Save(UserRole entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption); 
                parameters.Add("@UserRoleID", entity.UserRoleID);
                parameters.Add("@UserRoleName", entity.UserRoleName);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@UserBy", entity.UserBy);

                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync(DBSource.ERPUSERDB + "..spSet_UserRole", parameters, 30, CommandType.StoredProcedure);

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
            //throw new NotImplementedException();
        }
    }
}
