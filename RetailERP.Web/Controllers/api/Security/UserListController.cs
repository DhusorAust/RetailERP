using Microsoft.AspNetCore.Mvc;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using System.Threading.Tasks;
using RetailERP.Web.Models;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Web.Controllers.api.Security
{
    [Route("api/userlist")]
    [ApiController]
    public class UserListController : ControllerBase
    {
        private readonly IUserListService _service;
        public UserListController(IUserListService service)
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
        [Route("new")]
        public async Task<IActionResult> GetNew()
        {
            return Ok(await _service.GetNew());
        }
        [HttpGet]
        [Route("edit")]
        public async Task<IActionResult> GetEdit(int id)
        {
            return Ok(await _service.GetEdit(id));
        } 
        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] UserList model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            UserList entity = new UserList();

            if (allMenu != null)
            { 
                entity = model;

                if (entity.UserID > 0)
                { 
                    entity.SaveOption = 2;
                }
                else
                {  
                    entity.SaveOption = 1;
                }
                entity.UserBy = allMenu.UserList[0].UserID.ToString();
                return Ok(await _service.Save(entity));
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
