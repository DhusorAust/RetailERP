using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Production.Compounding;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;

namespace RetailERP.Infrastructure.Chem.Interfaces.Marketing
{
    public interface ICMPD_ProductionEntry
    {
        UserList User { get; set; }
        Task<GridEntity<Prod_CMPD_ProductionMaster>> GetList(GridOptions options, Status status);
        Task<Prod_CMPD_ProductionMaster> GetNew(int id);
        Task<Prod_CMPD_ProductionMaster> GetEdit(int id);
        Task<Prod_CMPD_ProductionMaster> SaveAsync(Prod_CMPD_ProductionMaster entity);
    }
}
