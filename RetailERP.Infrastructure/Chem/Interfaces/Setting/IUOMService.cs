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
    public interface IUOMService
    { 
        Task<GridEntity<Bas_UOM>> GetList(GridOptions options, Status status);
        Task<Bas_UOM> GetNew();
        Task<Bas_UOM> GetEdit(int Id);
        Task<Bas_UOM> SaveAsync(Bas_UOM entity); 
    }
}
