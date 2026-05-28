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
    [Route("api/BuyerRef")]
    [ApiController]
    public class BuyerRefController : ControllerBase
    {
        private readonly IBuyerRefService _service;
        public BuyerRefController(IBuyerRefService service)
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
        public async Task<IActionResult> Save([FromForm] Mkt_BuyerRefMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");

            Mkt_BuyerRefMaster entity = new Mkt_BuyerRefMaster();
            if (allMenu != null)
            {
                entity = model;

                if (entity.BuyerRefMasterID > 0)
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

        //[Route("save")]
        //[HttpPost]
        //public async Task<IActionResult> Save([FromForm] CS_PRMaster model)
        //{ 
        //    ALLMENU allMenu = new ALLMENU();
        //    allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
        //    _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];

        //    CS_PRMaster entity = new CS_PRMaster();
        //    if (allMenu != null)
        //    {
        //        entity = model;
        //        if (entity.Reject == true)
        //        {
        //            entity.SaveOption = 3;
        //        }
        //        else if (entity.IsRevise == true)
        //        {
        //            entity.SaveOption = 4;
        //        } 
        //        else if(entity.PRMasterID > 0)
        //        {
        //            entity.SaveOption = 2;
        //        }
        //        else
        //        {
        //            entity.SaveOption = 1;
        //        }
        //        entity.UserBy = allMenu.UserList[0].UserID.ToString();
        //        return Ok(await _service.SaveAsync(entity));
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
