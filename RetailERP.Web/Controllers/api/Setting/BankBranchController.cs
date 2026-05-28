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
    [Route("api/BankBranch")]
    [ApiController]
    public class BankBranchController : ControllerBase
    {
        private readonly IBankBranchService _service;
        public BankBranchController(IBankBranchService service)
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
        public async Task<IActionResult> Save([FromForm] Bas_BankBranch model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");

            Bas_BankBranch entity = new Bas_BankBranch();
            if (allMenu != null)
            {
                entity = model;

                if (entity.BankBranchID > 0)
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
