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
    public interface ISeasonService
    { 
        Task<GridEntity<Bas_Season>> GetList(GridOptions options, Status status);
        Task<Bas_Season> GetNew();
        Task<Bas_Season> GetEdit(int Id);
        Task<Bas_Season> SaveAsync(Bas_Season entity); 
    }
}
