using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using RetailERP.Infrastructure.Chem.Interfaces.RnD;
using RetailERP.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RetailERP.Web.Controllers.api.RnD
{
    [Route("api/fback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _service;
        public FeedbackController(IFeedbackService service)
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
        public async Task<IActionResult> Save([FromForm] RnD_FeedbackMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            RnD_FeedbackMaster entity = new RnD_FeedbackMaster();
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

    }
}
