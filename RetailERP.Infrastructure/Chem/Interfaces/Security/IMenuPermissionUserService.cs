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
    public interface IMenuPermissionUserService
    {
        Task<GridEntity<MenuPermissionUser>> GetList(GridOptions options, Status status);
        List<children_node> GetMenuList(string UserID);
        Task<MenuPermissionUser> SaveAsync(MenuPermissionUser entity);
    }
}
