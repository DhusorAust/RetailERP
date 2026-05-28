using Microsoft.AspNetCore.Mvc;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using System.Threading.Tasks;
using RetailERP.Web.Models;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Web.Controllers.api.Security
{
    [Route("api/userrole")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _service;
        public UserRoleController(IUserRoleService service)
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
        //[HttpPost]
        //[Route("PivotGetList")]
        //public async Task<IActionResult> PivotTestGetList()
        //{
        //    var records = await _service.PivotTestGetList();
        //    return Ok(records);
        //}

        [HttpGet]
        [Route("edit")]
        public async Task<IActionResult> GetEdit(int id)
        {
            return Ok(await _service.GetEdit(id));
        } 
        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] UserRole model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            UserRole entity = new UserRole();

            if (allMenu != null)
            { 
                entity = model; 
                if (entity.UserRoleID > 0)
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



