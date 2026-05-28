using RetailERP.Core.Chem.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Security
{
    public interface IMenuCreateService
    {
        #region Menu Create
        Task<Menu> Project();
        Task<Menu> ProjectModule(int id); 
        List<children_node> MenuList();
        List<Menu> MoveMenu(Menu entity);
        Task<Menu> SaveAsync(Menu entity); 
        #endregion   
    }
}
