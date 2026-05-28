using RetailERP.Core.Chem.Marketing;
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
    public interface ISDRFService
    {
        UserList User { get; set; }
        Task<GridEntity<Mkt_SDRFMaster>> GetList(GridOptions options, Status status);
        Task<Mkt_SDRFMaster> GetNew(int BuyerRefMasterID, int BuyerRefColorID);
        Task<Mkt_SDRFMaster> GetEdit(int id);
        Task<Mkt_SDRFMaster> SaveAsync(Mkt_SDRFMaster entity);

    }
}
