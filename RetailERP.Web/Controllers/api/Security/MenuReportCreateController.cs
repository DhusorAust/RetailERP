using Microsoft.AspNetCore.Mvc;
using RetailERP.Core.Chem.Security;
using System.Threading.Tasks;
using RetailERP.Web.Models;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Web.Controllers.api.Security
{
   
    [Route("api/menuRpt_create")]
    [ApiController]
    public class MenuReportCreateController : ControllerBase
    {
        private readonly IMenuReportCreateService _service;
        public MenuReportCreateController(IMenuReportCreateService service)
        {
            _service = service;
        }
        [HttpGet]
        [Route("project")]
        public async Task<IActionResult> Project()
        {
            return Ok(await _service.Project());
        }
        [HttpGet]
        [Route("projectmodule")]
        public async Task<IActionResult> ProjectModule(int id)
        {
            return Ok(await _service.ProjectModule(id));
        }

        [HttpGet]
        [Route("menulist")]
        public IActionResult MenuList()
        {
            return Ok(_service.MenuList());
        }
        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] Menu model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            Menu entity = new Menu();

            if (allMenu != null)
            {
                entity = model;

                if (entity.SaveOption == 1)
                {
                    entity.SaveOption = 1;
                }
                else
                {
                    entity.SaveOption = 2;
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

        //[Route("menumove")]
        //[HttpPost]
        //public IActionResult MoveMenu([FromForm] Menu model)
        //{
        //    ALLMENU allMenu = new ALLMENU();
        //    allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
        //    Menu entity = new Menu();

        //    if (allMenu != null)
        //    {
        //        entity = model;

        //        if (entity.MenuID > 0)
        //        {
        //            entity.SaveOption = 2;
        //        }
        //        else
        //        {
        //            entity.SaveOption = 1;
        //        }
        //        entity.UserBy = allMenu.UserList[0].UserID.ToString();
        //        return Ok(_service.MoveMenu(entity));
        //    }
        //    else
        //    {
        //        entity.IsLogin = 1;
        //        entity.redirectUrl = Url.Action("Index", "Security");
        //        return Ok(entity);
        //    }
        //}
    }
}
