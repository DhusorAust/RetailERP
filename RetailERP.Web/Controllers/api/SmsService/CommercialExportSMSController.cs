using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RetailERP.Web.Models;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Chem.SmsService;
using RetailERP.Infrastructure.Chem.Interfaces.SmsService;

namespace RetailERP.Web.Controllers.api.SmsService
{
    [Route("api/commercialexportsms")]
    [ApiController]
    public class CommercialExportSMSController : ControllerBase
    {
        private readonly ICommercialExportSMSService _service;
        public CommercialExportSMSController(ICommercialExportSMSService service)
        {
            _service = service;
        }
        [HttpGet]
        [Route("GetCommExportHOSMSData")]
        public async Task<IActionResult> GetCommercialExportHOSMSData(string trnsDate)
        {
            return Ok(await _service.GetCommercialExportHOSMSData(trnsDate));
        }
    }
}
