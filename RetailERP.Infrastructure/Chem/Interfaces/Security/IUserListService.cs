using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Security
{
    public interface IUserListService
    {
        Task<GridEntity<UserList>> GetList(GridOptions options, Status status);
        Task<UserList> GetNew();
        Task<UserList> GetEdit(int Id); 
        Task<UserList> Save(UserList entity);
    }
}
