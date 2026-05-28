using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Chem.SmsService;

namespace RetailERP.Infrastructure.Chem.Interfaces.SmsService
{
    public interface IProductionDeliverySMSService
    {
        Task<List<ProductionDeliverySMS>> GetYarnProduction();
        Task<List<ProductionDeliverySMS>> GetYarnDelivery();
        Task<List<ProductionDeliverySMS>> GetStockData();
    }
}
