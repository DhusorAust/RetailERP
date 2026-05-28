using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Security
{
    public interface IUserRoleService
    {
        Task<GridEntity<UserRole>> GetList(GridOptions options, Status status);

        //Task<UserRole> PivotTestGetList();
        Task<UserRole> GetEdit(int Id);
        Task<UserRole> Save(UserRole entity);
    }
}
