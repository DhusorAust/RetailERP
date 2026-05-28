using RetailERP.Core.Chem.Production.Filling;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Marketing
{
    public interface IFILL_ProductionEntryService
    {
        UserList User { get; set; }
        Task<GridEntity<Prod_FILL_ProductionMaster>> GetList(GridOptions options, Status status);
        Task<Prod_FILL_ProductionMaster> GetNew(int id);
        Task<Prod_FILL_ProductionMaster> GetEdit(int id);
        Task<Prod_FILL_ProductionMaster> SaveAsync(Prod_FILL_ProductionMaster entity);
    }
}
