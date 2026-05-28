using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Chem.Production.Compounding;
using RetailERP.Core.Chem.Production.Packaging;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;

namespace RetailERP.Infrastructure.Chem.Interfaces.Marketing
{
    public interface IPACK_ProductionEntryService
    {
        UserList User { get; set; }
        Task<GridEntity<Prod_PACK_ProductionMaster>> GetList(GridOptions options, Status status);
        Task<Prod_PACK_ProductionMaster> GetNew(int id);
        Task<Prod_PACK_ProductionMaster> GetEdit(int id);
        Task<Prod_PACK_ProductionMaster> SaveAsync(Prod_PACK_ProductionMaster entity);
    }
}
