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
    public interface IBankBranchService
    {

        Task<GridEntity<Bas_BankBranch>> GetList(GridOptions options, Status status);
        Task<Bas_BankBranch> GetNew();
        Task<Bas_BankBranch> GetEdit(int Id);
        Task<Bas_BankBranch> SaveAsync(Bas_BankBranch entity);
    }
}
