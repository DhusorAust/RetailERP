using Microsoft.AspNetCore.Mvc;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using System.Threading.Tasks;
using RetailERP.Web.Models;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Web.Controllers.api.Security
{
    [Route("api/menuper_userView")]
    [ApiController]
    public class UserViewPermissionController : ControllerBase
    {
        private readonly IUserViewPermissionService _service;
        public UserViewPermissionController(IUserViewPermissionService service)
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
        [Route("GetMenuList")]
        public IActionResult GetMenuList(string UserID)
        {
            return Ok(_service.GetMenuList(UserID));
        }

        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] UserViewPermission model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");

            UserViewPermission entity = new UserViewPermission();

            if (allMenu != null)
            {
                entity = model;

                if (entity.ViewPermissionID > 0)
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
