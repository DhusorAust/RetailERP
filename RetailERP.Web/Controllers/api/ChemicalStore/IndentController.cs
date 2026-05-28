
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
    [Route("api/csindent")]
    [ApiController]
    public class IndentController : ControllerBase
    {
        private readonly IIndentService _service;
        public IndentController(IIndentService service)
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
        public async Task<IActionResult> GetNew(int id)
        {
            return Ok(await _service.GetNew(id));
        }
        [HttpGet]
        [Route("edit")]
        public async Task<IActionResult> GetEdit(int id)
        {
            return Ok(await _service.GetEdit(id));
        }

        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] CS_IndentMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");

            CS_IndentMaster entity = new CS_IndentMaster();
            if (allMenu != null)
            {
                entity = model; 
                if (entity.Approved == true)
                {
                    entity.SaveOption = 3;
                }
                else if (entity.Reject == true)
                {
                    entity.SaveOption = 4;
                }
                else if (entity.IsRevise == true)
                {
                    entity.SaveOption = 5;
                } 
                else if (entity.IndentMasterID > 0)
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
