using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Security
{
    public interface IMenuPermissionRoleService
    {
        Task<GridEntity<MenuPermissionRole>> GetList(GridOptions options, Status status);
        List<children_node> GetMenuList(string UserRoleID); 
        Task<MenuPermissionRole> SaveAsync(MenuPermissionRole entity); 
    }
}
