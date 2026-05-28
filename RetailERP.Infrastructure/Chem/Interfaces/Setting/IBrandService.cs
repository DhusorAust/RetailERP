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
    public interface IBrandService
    { 
        Task<GridEntity<Bas_Brand>> GetList(GridOptions options, Status status);
        Task<Bas_Brand> GetNew();
        Task<Bas_Brand> GetEdit(int Id);
        Task<Bas_Brand> SaveAsync(Bas_Brand entity); 
    }
}
