using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Chem.Setting;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using Dapper;
using System; 
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Marketing
{
    public interface IBuyerRefService
    {
        UserList User { get; set; }
        Task<GridEntity<Mkt_BuyerRefMaster>> GetList(GridOptions options, Status status);
        Task<Mkt_BuyerRefMaster> GetNew(); 
        Task<Mkt_BuyerRefMaster> GetEdit(int id);
        Task<Mkt_BuyerRefMaster> SaveAsync(Mkt_BuyerRefMaster entity);
    } 
}
