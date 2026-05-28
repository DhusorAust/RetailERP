using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using System.Threading.Tasks;
using RetailERP.Core.Chem.Setting.Store.ChemicalStore;
using RetailERP.Core.Chem.Setting;

namespace RetailERP.Infrastructure.Chem.Interfaces.Setting.Store.ChemicalStore
{
    public interface IProductCategoryService
    {
        Task<GridEntity<Bas_CS_ProductCategory>> GetList(GridOptions options, Status status,string lotType);
        Task<Bas_CS_ProductCategory> GetNew();
        Task<Bas_CS_ProductCategory> GetEdit(int Id);
        Task<Bas_CS_ProductCategory> SaveAsync(Bas_CS_ProductCategory entity);
    }
}
