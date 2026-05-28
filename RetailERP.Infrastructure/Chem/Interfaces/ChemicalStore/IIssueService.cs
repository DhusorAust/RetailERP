using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using System; 
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks; 

namespace RetailERP.Infrastructure.Chem.Interfaces.ChemicalStore
{
    public interface IIssueService
    {
        UserList User { get; set; }
        Task<GridEntity<CS_IssueMaster>> GetList(GridOptions options, Status status);
        Task<CS_IssueMaster> GetNew(string Ids,int? MenuID,int? UnitID,int? UserID);
        Task<CS_IssueMaster> GetEdit(int Id, int? MenuID, int? UnitID, int? UserID);
        Task<CS_IssueMaster> SaveAsync(CS_IssueMaster entity);

        Task<List<DropDownList>> GetProductBySearch(string searchKey, string itemName, int? menuID, int? unitID,int? userID,int? primaryKeyID,string whereClause, int pageIndex, int pageSize);

    }
}
