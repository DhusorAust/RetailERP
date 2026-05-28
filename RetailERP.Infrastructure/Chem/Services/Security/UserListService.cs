using Dapper;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RetailERP.Infrastructure.Chem.Services.Common;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Infrastructure.Chem.Services.Security
{
    public class UserListService : IUserListService
    {
        private readonly DapperContext<UserList> _context;
        private readonly SqlConnection _connection;
        CommonFunction _commonFunction = new CommonFunction();
        public UserListService(DapperContext<UserList> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@"
            --User Role
            SELECT Distinct CAST(ur.UserRoleID AS VARCHAR(100)) id, ur.UserRoleName [text] 
            FROM {DBSource.ERPUSERDB}..UserRole ur
            Inner Join {DBSource.ERPUSERDB}..UserRolePermission urp On urp.UserRoleID = ur.UserRoleID And ProjectID = {AppGlobalFields.ProjectID}
            Where ProjectID = {AppGlobalFields.ProjectID}
            ORDER BY [text]; 
 
                
            -- Employee
           --Select CAST(EmployeeID AS VARCHAR(100)) id, EmployeeName [text]  
            --From {DBSource.PDLHRM}..emp_EmploymentInfo  
            --Order By EmployeeName

            {QueryService.GetEmployee()}
                
            -- Is Active
            Select 1 id, 'Active' As [text] Union Select 0 id, 'Inactive' As [text]; "; 

        public async Task<GridEntity<UserList>> GetList(GridOptions options, Status status)
        { 
            string sql = "";
            string orderBy = "UserRoleID asc";

            sql = $@"
            Select Distinct UL.UserID, UL.UserName, UL.PasswordH, UL.PasswordExpireDate, UL.EmployeeID, Isnull(EL.EmployeeName,'')EmployeeName,
            urp.UserRoleID, UR.UserRoleName, UL.UserTypeID, UL.IsActive, (Case When UL.IsActive = 1 then 'Active' Else 'Inactive' End)UserStatus  
            From {DBSource.ERPUSERDB}..UserList UL  
            Inner Join {DBSource.ERPUSERDB}..UserRolePermission urp On urp.UserID = UL.UserID
            Inner Join {DBSource.ERPUSERDB}..UserRole UR On UR.UserRoleID = urp.UserRoleID And UR.IsActive = 1
            Left Join {DBSource.PDLHRM}..emp_EmploymentInfo EL On EL.EmployeeID = UL.EmployeeID 
            left join {DBSource.ERPUSERDB}..ProjectPermission pp on ul.UserID = pp.UserID
            Where pp.ProjectID = {AppGlobalFields.ProjectID} ";

            return await _context.GetKendoGridDataAsync<UserList>(options, sql, orderBy, "");
        }
        public async Task<UserList> GetNew()
        {
            var query = $@" {CommonQuery}";

            UserList data = new UserList();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data.UserRoleList = records.Read<DropDownList>().ToList();
                data.EmployeeList = records.Read<DropDownList>().ToList();
                data.IsActiveList = records.Read<DropDownList>().ToList();
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
        public async Task<UserList> GetEdit(int id)
        {
            var query =
                $@"
                Select ul.*, (Case When ul.IsActive = 1 then 'Active' Else 'Inactive' End)UserStatus 
                From {DBSource.ERPUSERDB}..UserList ul
                Inner Join {DBSource.ERPUSERDB}..UserRolePermission urp On urp.UserID = UL.UserID
                Inner Join {DBSource.ERPUSERDB}..UserRole UR On UR.UserRoleID = urp.UserRoleID
                left join {DBSource.ERPUSERDB}..ProjectPermission pp on ul.UserID = pp.UserID
                Left Join {DBSource.PDLHRM}..emp_EmploymentInfo EL On EL.EmployeeID = UL.EmployeeID 
                Where pp.ProjectID = {AppGlobalFields.ProjectID} And ul.UserID = {id}
                ORDER BY UserID 
                
                {CommonQuery} ";

            UserList data =new UserList();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<UserList>();
                if (data != null)
                {
                    data.UserRoleList = records.Read<DropDownList>().ToList();
                    data.EmployeeList = records.Read<DropDownList>().ToList();
                    data.IsActiveList = records.Read<DropDownList>().ToList();
                }
                else
                {
                    data = new UserList();
                    data.Message = AppGlobalFields.AdminMessage;
                    data.ErrNo = 1;
                } 
            }
            catch (Exception ex)
            {
                data.Message= ex.Message;
                data.ErrNo = 1;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
            return data;
        }
        public async Task<UserList> Save(UserList entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@UserID", entity.UserID);
                parameters.Add("@ProjectID", AppGlobalFields.ProjectID);
                parameters.Add("@UserName", entity.UserName);
                parameters.Add("@PasswordH", _commonFunction.PasswordEncrypt(entity.PasswordH));
                parameters.Add("@EmployeeID", entity.EmployeeID);
                parameters.Add("@UserRoleID", entity.UserRoleID);
                parameters.Add("@UserTypeID", entity.UserTypeID);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync(DBSource.ERPUSERDB + "..spSet_UserList", parameters, 30, CommandType.StoredProcedure);
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
