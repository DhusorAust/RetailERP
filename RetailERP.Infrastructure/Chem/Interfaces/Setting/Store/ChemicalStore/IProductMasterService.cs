using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using System.Threading.Tasks; 
using RetailERP.Core.Chem.Setting.Store.ChemicalStore;

namespace RetailERP.Infrastructure.Chem.Interfaces.Setting.Store.ChemicalStore
{
    public interface IProductMasterService
    {
        Task<GridEntity<Bas_CS_ProductMaster>> GetList(GridOptions options, Status status,string lotType);
        Task<Bas_CS_ProductMaster> GetNew();
        Task<Bas_CS_ProductMaster> GetEdit(int Id);
        Task<Bas_CS_ProductMaster> SaveAsync(Bas_CS_ProductMaster entity);
    }
}
