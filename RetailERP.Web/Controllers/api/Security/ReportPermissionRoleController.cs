using Microsoft.AspNetCore.Mvc;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using System.Threading.Tasks;
using RetailERP.Web.Models;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Web.Controllers.api.Security
{ 
    [Route("api/reportper_role")]
    [ApiController]
    public class ReportPermissionRoleController : ControllerBase
    {
        private readonly IReportPermissionRoleService _service;
        public ReportPermissionRoleController(IReportPermissionRoleService service)
        {
            _service = service;
        }
        [HttpPost]
        [Route("list/{status}")]
        public async Task<IActionResult> GetList([FromBody] GridOptions options, Status status)
        {
            var records = await _service.GetList(options, status);
            return Ok(records);
        }
        [HttpGet]
        [Route("GetReportsList")]
        public IActionResult GetReportsList(string UserRoleID)
        {
            return Ok(_service.GetReportsList(UserRoleID));
        }

        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] ReportPermissionRole model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");

            ReportPermissionRole entity = new ReportPermissionRole();

            if (allMenu != null)
            {
                entity = model;

                if (entity.ReportPermissionID > 0)
                {
                    entity.SaveOption = 2;
                }
                else
                {
                    entity.SaveOption = 1;
                }
                entity.UserBy = allMenu.UserList[0].UserID.ToString();
                return Ok(await _service.SaveAsync(entity));
            }
            else
            {
                entity.IsLogin = 1;
                entity.redirectUrl = Url.Action("Index", "Security");
                return Ok(entity);
            }
        }
    }
}
