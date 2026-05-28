using RetailERP.Core.Chem.Security;
using RetailERP.Core.Chem.Setting;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Common;
using RetailERP.Infrastructure.Chem.Interfaces.Setting;
using RetailERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RetailERP.Web.Controllers.api.Setting
{
    [Route("api/INCIM")]
    [ApiController]
    public class INCIMaster : ControllerBase
    {
        private readonly IINCIMasterService _service;
        public INCIMaster(IINCIMasterService service)
        {
            _service = service;
        }
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
        public async Task<IActionResult> Save([FromForm] Bas_INCIMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");

            Bas_INCIMaster entity = new Bas_INCIMaster();
            if (allMenu != null)
            {
                entity = model;

                if (entity.INCIID > 0)
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
