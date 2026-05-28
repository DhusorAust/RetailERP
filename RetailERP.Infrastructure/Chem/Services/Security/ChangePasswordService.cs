using Dapper;
using RetailERP.Core.Common;
using RetailERP.Core.Chem.Security;
using System;
using System.Data;

using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Infrastructure.Chem.Services.Security
{
    public class ChangePasswordService : IChangePasswordService
    {
        private readonly DapperContext<UserList> _context;
        private readonly SqlConnection _connection;
        public ChangePasswordService(DapperContext<UserList> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        public async Task<UserList> SaveAsync(UserList entity)
        {
            CommonFunction _commonFunction = new CommonFunction();
            var parameters = (dynamic)null;
            try
            {  
                parameters = new DynamicParameters();
                parameters.Add("@UserID", entity.UserID);
                parameters.Add("@OldPassword", _commonFunction.PasswordEncrypt(entity.OldPassword));
                parameters.Add("@NewPassword", _commonFunction.PasswordEncrypt(entity.NewPassword));
                parameters.Add("@ConfirmPassword", _commonFunction.PasswordEncrypt(entity.ConfirmPassword));
                parameters.Add("@EmployeeID", entity.EmployeeID);
                parameters.Add("@IPAddress", entity.IPAddress); 
                parameters.Add("@UserBy", entity.UserBy);
                
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrMessage", "", dbType: DbType.String, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync(DBSource.ERPUSERDB + "..spSet_ChangePassword", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");
                entity.Message = parameters.Get<string>("@ErrMessage");
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
		public async Task<LoginOTP> SaveOTP(LoginOTP entity)
		{
			CommonFunction _commonFunction = new CommonFunction();

			var otp = GenerateOTP();
			var pass = _commonFunction.PasswordEncrypt(entity.Password.ToString());

			var parameters = (dynamic)null;
			try
			{
				parameters = new DynamicParameters();
				parameters.Add("@UserID", entity.UserID);
				parameters.Add("@ProjectID", Convert.ToInt32(AppGlobalFields.ProjectID));
				//	parameters.Add("@MobileNo", entity.MobileNo);
				parameters.Add("@OTP", otp);
				parameters.Add("@Password", pass);
				parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
				parameters.Add("@ReturnMobileNo", "", dbType: DbType.String, direction: ParameterDirection.InputOutput);
				parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
				parameters.Add("@ErrMessage", "", dbType: DbType.String, direction: ParameterDirection.InputOutput);
				entity.NoofRows = await _context.ExecuteAsync("ERPDB..spSet_OTP", parameters, 30, CommandType.StoredProcedure);
				entity.ResultId = parameters.Get<int>("@IdentityValue");
				entity.MobileNo = parameters.Get<string>("@ReturnMobileNo");
				entity.ErrorNo = parameters.Get<int>("@ErrNo");
				entity.Message = parameters.Get<string>("@ErrMessage");

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
		public int GenerateOTP()
		{
			Random random = new Random();
			int randomNumber = random.Next(1000, 10000);

			return randomNumber;
		}
		public async Task<LoginOTP> GetOTP(string userId)
		{
			string sql = "";
			sql = $@"Select top 1 * ,IsOTPExpired=case when GETDATE() > OTPExpireDate then 1 else 0 end
                from ERPDB.dbo.UserLoginOTP where UserID=(Select UserID from ERPDB.dbo.UserList where UserName='{userId}') 
                and IsActive=1  and ProjectID = {AppGlobalFields.ProjectID}";
			var otpInfo = await _context.GetSingleDataAsync<LoginOTP>(sql);
			return otpInfo ?? new LoginOTP();
		}
		public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
