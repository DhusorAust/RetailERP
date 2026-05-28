using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.RnD
{
    public interface IFormulationService
    {
        UserList User { get; set; }
        Task<GridEntity<RnD_FormulationMaster>> GetList(GridOptions options, Status status);
        Task<RnD_FormulationMaster> GetNew();
        Task<RnD_FormulationMaster> GetEdit(int Id);
        Task<RnD_FormulationMaster> SaveAsync(RnD_FormulationMaster entity);

    }
}
