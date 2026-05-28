using RetailERP.Core.Chem.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Security
{
    public interface IChangePasswordService
    {
        Task<UserList> SaveAsync(UserList entity);
		Task<LoginOTP> SaveOTP(LoginOTP entity);
		Task<LoginOTP> GetOTP(string userId);
	}
}
