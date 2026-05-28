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
    public interface IRequirementService
    {
        UserList User { get; set; }
        Task<GridEntity<CS_RequirementMaster>> GetList(GridOptions options, Status status);
        Task<CS_RequirementMaster> GetNew();
        Task<CS_RequirementMaster> GetEdit(int id,int? UnitID,int? MenuID,int? UserID); 
        Task<CS_RequirementMaster> SaveAsync(CS_RequirementMaster entity); 
    } 
}
