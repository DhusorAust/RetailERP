using RetailERP.Core.Chem.Production.Compounding;
using RetailERP.Core.Chem.Production.Filling;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using RetailERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RetailERP.Web.Controllers.api.Marketing
{
    [Route("api/fillprode")]
    [ApiController]
    public class FILL_ProductionEntryController : Controller
    {
        private readonly IFILL_ProductionEntryService _service;
        public FILL_ProductionEntryController(IFILL_ProductionEntryService service)
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
        public async Task<IActionResult> GetNew(int id)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            return Ok(await _service.GetNew(id));
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
        public async Task<IActionResult> Save([FromForm] Prod_FILL_ProductionMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            Prod_FILL_ProductionMaster entity = new Prod_FILL_ProductionMaster();

            if (allMenu != null)
            {
                entity = model;
                entity.UserBy = allMenu.UserList[0].UserID.ToString();

                if (entity.ProductionMasterID > 0)
                {
                    entity.SaveOption = 2;
                }
                else
                {
                    entity.SaveOption = 1;
                }
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
