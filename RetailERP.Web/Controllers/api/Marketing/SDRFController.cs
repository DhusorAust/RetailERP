using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using RetailERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RetailERP.Web.Controllers.api.Marketing
{
    [Route("api/sdrfentry")]
    [ApiController]
    public class SDRFController : Controller
    {
        private readonly ISDRFService _service;
        public SDRFController(ISDRFService service)
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
        public async Task<IActionResult> GetNew(int BuyerRefMasterID, int BuyerRefColorID)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            return Ok(await _service.GetNew(BuyerRefMasterID, BuyerRefColorID));
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
        public async Task<IActionResult> Save([FromForm] Mkt_SDRFMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");

            Mkt_SDRFMaster entity = new Mkt_SDRFMaster();
            if (allMenu != null)
            {
                entity = model;

                //if (entity.SDRFMasterID > 0)
                //{
                //    entity.SaveOption = 2;
                //}
                //else
                //{
                //    entity.SaveOption = 1;
                //}
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
