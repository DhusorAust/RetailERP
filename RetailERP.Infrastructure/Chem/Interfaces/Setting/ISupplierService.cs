using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Chem.Setting;

namespace RetailERP.Infrastructure.Chem.Interfaces.Setting
{
    public interface ISupplierService
    { 
        Task<GridEntity<Bas_Supplier>> GetList(GridOptions options, Status status);
        Task<Bas_Supplier> GetNew();
        Task<Bas_Supplier> GetEdit(int Id);
        Task<Bas_Supplier> SaveAsync(Bas_Supplier entity); 
    }
}
