using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RetailERP.Web.Models;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Chem.SmsService;
using RetailERP.Infrastructure.Chem.Interfaces.SmsService;

namespace RetailERP.Web.Controllers.api.SmsService
{
    [Route("api/productiondeliverysms")]
    [ApiController]
    public class ProductionDeliverySMSController : ControllerBase
    {
        private readonly IProductionDeliverySMSService _service;
        public ProductionDeliverySMSController(IProductionDeliverySMSService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("getyarnproduction")]
        public async Task<IActionResult> GetYarnProduction()
        {
            return Ok(await _service.GetYarnProduction());
        }

        [HttpGet]
        [Route("getyarndelivery")]
        public async Task<IActionResult> GetYarnDelivery()
        {
            return Ok(await _service.GetYarnDelivery());
        }
        [HttpGet]
        [Route("getstock")]
        public async Task<IActionResult> GetStockData()
        {
            return Ok(await _service.GetStockData());
        }


    }

}
