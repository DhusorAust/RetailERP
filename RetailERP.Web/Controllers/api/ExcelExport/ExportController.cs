using RetailERP.Core.Chem.ExcelExport;
using RetailERP.Core.Chem.Security;
using RetailERP.Infrastructure.Chem;
using RetailERP.Infrastructure.Chem.Services.Common;
using RetailERP.Infrastructure.Chem.Interfaces.Common;
using RetailERP.Web.Models;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using RetailERP.Infrastructure.Chem.Interfaces.ExcelExport;

namespace RetailERP.Web.Controllers.api.ExcelExport
{
    [Route("api/export")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly IExcelExportService _service;
        private readonly IHelperService _helperService;
        public ExportController(IExcelExportService service, IHelperService helperService)
        {
            _service = service;
            _helperService = helperService;
        }

        [HttpGet]
        [Route("new")]
        public async Task<IActionResult> GetNew()
        {
            ALLMENU allMenu = new();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            var userId = allMenu.UserList[0].UserID;
            return Ok(await _service.GetNew(userId));
        }

        [HttpPost("GetExportExcelData")]
        public async Task<IActionResult> GetExportExcelData([FromForm] ExcelExportParam reportParemaeters)
        {
            var base64 = "";
            try
            {
                var data =await  _service.GetExportExcelData(reportParemaeters);
                if (data == null || data.Rows.Count == 0)
                {
                    return NotFound("No data found to export.");
                }

                var base64Excel = await _helperService.ExcelFileConvertBase64String(data);
                return Ok(base64Excel);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
