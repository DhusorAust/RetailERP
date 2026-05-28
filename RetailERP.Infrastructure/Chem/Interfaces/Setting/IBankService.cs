using RetailERP.Core.Chem.Setting.Store.ChemicalStore;
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
    public interface IBankService
    {

        Task<GridEntity<Bas_Bank>> GetList(GridOptions options, Status status);
        Task<Bas_Bank> GetNew();
        Task<Bas_Bank> GetEdit(int Id);
        Task<Bas_Bank> SaveAsync(Bas_Bank entity); 
    }
}
