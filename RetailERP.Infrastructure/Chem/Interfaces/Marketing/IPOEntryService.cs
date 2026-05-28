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
    public interface IPOEntryService
    {
        UserList User { get; set; }
        Task<GridEntity<Mkt_POMaster>> GetList(GridOptions options, Status status);
        Task<Mkt_POMaster> GetNew();
        Task<Mkt_POMaster> GetEdit(int id);
        Task<Mkt_POMaster> SaveAsync(Mkt_POMaster entity);
        Task<List<DropDownList>> GetColors(int BuyerRefMasterID);
    }
}
