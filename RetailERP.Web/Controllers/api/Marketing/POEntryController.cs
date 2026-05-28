using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using RetailERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailERP.Web.Controllers.api.Marketing
{
    [Route("api/poentry")]
    [ApiController]
    public class POEntryController : Controller
    {
        private readonly IPOEntryService _service;
        public POEntryController(IPOEntryService service)
        {
            _service = service;
        }
        [HttpPost]
        [Route("list/{status}")]
        public async Task<IActionResult> GetList([FromBody] GridOptions options, Status status)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            var records = await _service.GetList(options, status);
            return Ok(records);
        }
        [HttpGet]
        [Route("new")]
        public async Task<IActionResult> GetNew()
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            return Ok(await _service.GetNew());
        }
        [HttpGet]
        [Route("edit")]
        public async Task<IActionResult> GetEdit(int id)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            return Ok(await _service.GetEdit(id));
        }

        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] Mkt_POMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            Mkt_POMaster entity = new Mkt_POMaster();
            if (allMenu != null)
            {
                entity = model;
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

        [HttpGet]
        [Route("GetColors")]
        public async Task<List<DropDownList>> GetColors(int BuyerRefMasterID)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            return await _service.GetColors(BuyerRefMasterID);
        }
    }
}
