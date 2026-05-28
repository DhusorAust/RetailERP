using RetailERP.Core.Chem.AuditTrails;
using RetailERP.Core.Chem.Security;
using RetailERP.Infrastructure.Chem.Interfaces.AuditTrails;
using RetailERP.Web.Models;
using DevExpress.CodeParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RetailERP.Web.Controllers.api.AuditTrails
{
    [Route("api/audittrail")]
    [ApiController]
    public class AuditTrailController : ControllerBase
    {
        private readonly IAuditTrailService _service;
        public AuditTrailController(IAuditTrailService service)
        {
            _service = service;
        }

        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] AuditTrailEntity model)
        {
            ALLMENU allMenu = new();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            AuditTrailEntity entity = new();
            if (allMenu != null)
            {
                entity = model;
                if (entity.AuditTrailID > 0)
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


        [Route("GetTransactionUserInfo")]
        [HttpGet]
        public async Task<IActionResult> GetTransactionUserInfo(string pageName, int id)
        {
            var records = await _service.GetTransactionUserInfo(pageName, id);
            return Ok(records);
        }

    }
}
