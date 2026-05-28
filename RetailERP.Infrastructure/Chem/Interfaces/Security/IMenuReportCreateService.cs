using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Security
{ 
    public interface IMenuReportCreateService
    { 
        Task<GridEntity<Menu>> GetReportList(GridOptions options, int menuID, int userRoleID);
        #region Menu Report Create
        Task<Menu> Project();
        Task<Menu> ProjectModule(int id);
        List<children_node> MenuList();
        //List<Menu> MoveMenu(Menu entity);
        Task<Menu> SaveAsync(Menu entity);
        #endregion
    }
}
