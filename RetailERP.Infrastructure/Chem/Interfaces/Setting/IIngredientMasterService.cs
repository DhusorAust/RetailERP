using RetailERP.Core.Chem.Setting;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Setting
{
    public interface IIngredientMasterService
    {
        Task<GridEntity<Itm_IngredientMaster>> GetList(GridOptions options, Status status);
        Task<Itm_IngredientMaster> GetNew();
        Task<Itm_IngredientMaster> GetEdit(int id);
        Task<Itm_IngredientMaster> GetDocumentList(int id, string idname);
        Task<Itm_IngredientMaster> SaveAsync(Itm_IngredientMaster entity);
    }
}
