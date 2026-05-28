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
    public interface IBuyerBankAccountService
    {

        Task<GridEntity<Bas_BuyerBankAccount>> GetList(GridOptions options, Status status);
        Task<Bas_BuyerBankAccount> GetNew();
        Task<Bas_BuyerBankAccount> GetEdit(int Id);
        Task<Bas_BuyerBankAccount> SaveAsync(Bas_BuyerBankAccount entity);
    }
}
