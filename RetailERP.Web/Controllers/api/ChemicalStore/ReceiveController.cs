
using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.ChemicalStore;
using RetailERP.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPPDL.Web.Controllers.api.Store.ChemicalStore
{
    [Route("api/csreceive")]
    [ApiController]
    public class ReceiveController : ControllerBase
    {
        private readonly IReceiveService _service;
        public ReceiveController(IReceiveService service)
        {
            _service = service;
        }
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
        public async Task<IActionResult> GetEdit(int id, bool isQCPage, int Flag)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            return Ok(await _service.GetEdit(id,isQCPage,Flag));
        }

        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] CS_ReceiveMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];

            CS_ReceiveMaster entity = new CS_ReceiveMaster();
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
        [Route("GetIndentList")]
        public async Task<IActionResult> GetIndentList(string searchKey)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            return Ok(await _service.GetIndentList(searchKey));
        }
        [HttpGet]
        [Route("GetIndentChild")]
        public async Task<IActionResult> GetIndentChild(int indentId)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            return Ok(await _service.GetIndentChild(indentId));
        }
    }
}
