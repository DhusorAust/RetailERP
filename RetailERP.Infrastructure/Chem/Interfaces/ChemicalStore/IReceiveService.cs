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
    public interface IReceiveService
    {
        UserList User { get; set; }
        Task<GridEntity<CS_ReceiveMaster>> GetList(GridOptions options, Status status);
        Task<CS_ReceiveMaster> GetNew();
        Task<CS_ReceiveMaster> GetEdit(int Id,bool isQCPage,int Flag);
        Task<CS_ReceiveMaster> SaveAsync(CS_ReceiveMaster entity);
        Task<List<CS_ReceiveMaster>> GetIndentList(string searchKey);
        Task<List<CS_ReceiveChild>> GetIndentChild(int IndentMasterID);
    }
}
