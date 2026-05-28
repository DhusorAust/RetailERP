using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using System; 
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks; 

namespace RetailERP.Infrastructure.Chem.Interfaces.ChemicalStore
{
    public interface IIndentService
    {
        Task<GridEntity<CS_IndentMaster>> GetList(GridOptions options, Status status);
        Task<CS_IndentMaster> GetNew(int id);
        Task<CS_IndentMaster> GetEdit(int Id);
        Task<CS_IndentMaster> SaveAsync(CS_IndentMaster entity); 
    }
}
