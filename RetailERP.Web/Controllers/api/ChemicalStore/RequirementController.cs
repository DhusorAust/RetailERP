using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.ChemicalStore;
using RetailERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Web.Controllers.api.ChemicalStore
{
    [Route("api/csreq")]
    [ApiController]
    public class RequirementController : ControllerBase
    {
        private readonly IRequirementService _service;
        public RequirementController(IRequirementService service)
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
        public async Task<IActionResult> GetEdit(int id, int? UnitID, int? MenuID, int? UserID)
        {

            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            UserID = allMenu.UserList[0].UserID;
            return Ok(await _service.GetEdit(id,UnitID,MenuID,UserID));
        }
        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] CS_RequirementMaster model)
        { 
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];

            CS_RequirementMaster entity = new CS_RequirementMaster();
            if (allMenu != null)
            {
                entity = model;
                //if (entity.Reject == true)
                //{
                //    entity.SaveOption = 3;
                //}
                //else if (entity.IsRevise == true)
                //{
                //    entity.SaveOption = 4;
                //} 
                //else if(entity.RequirementMasterID > 0)
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
