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
    public interface IINCIMasterService
    { 
        Task<GridEntity<Bas_INCIMaster>> GetList(GridOptions options, Status status);
        Task<Bas_INCIMaster> GetNew();
        Task<Bas_INCIMaster> GetEdit(int Id);
        Task<Bas_INCIMaster> SaveAsync(Bas_INCIMaster entity); 
    }
}
