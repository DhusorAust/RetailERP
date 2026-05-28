using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.RnD;
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
    public interface IStabilityTestService
    {
        UserList User { get; set; }
        Task<GridEntity<RnD_StabilityTestMaster>> GetList(GridOptions options, Status status);
        Task<RnD_StabilityTestMaster> GetNew();
        Task<RnD_StabilityTestMaster> GetEdit(int Id);
        Task<RnD_StabilityTestMaster> SaveAsync(RnD_StabilityTestMaster entity);
        Task<int> SaveFilePathsAsync(List<Rnd_StabilityTestFilePath> filePaths);
        Task<int> SoftDeleteFilePathsAsync(List<int> filePathIds);

    }
}
