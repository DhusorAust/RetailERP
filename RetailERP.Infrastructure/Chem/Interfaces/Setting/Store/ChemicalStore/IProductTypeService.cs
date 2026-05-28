using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using System.Threading.Tasks; 
using RetailERP.Core.Chem.Setting.Store.ChemicalStore;

namespace RetailERP.Infrastructure.Chem.Interfaces.Setting.Store.ChemicalStore
{
    public interface IProductTypeService
    {
        Task<GridEntity<bas_CS_ProductType>> GetList(GridOptions options, Status status,string lotType);
        Task<bas_CS_ProductType> GetNew();
        Task<bas_CS_ProductType> GetEdit(int Id);
        Task<bas_CS_ProductType> SaveAsync(bas_CS_ProductType entity);
    }
}
