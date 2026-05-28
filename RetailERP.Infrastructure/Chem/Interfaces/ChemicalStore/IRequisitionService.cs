using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using Dapper;
using System; 
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.ChemicalStore
{
    public interface IRequisitionService
    {
        UserList User { get; set; }
        Task<GridEntity<CS_PRMaster>> GetList(GridOptions options, Status status);
        Task<CS_PRMaster> GetNew();
        Task<CS_PRMaster> GetEdit(int id); 
        Task<CS_PRMaster> SaveAsync(CS_PRMaster entity); 
    } 
}
