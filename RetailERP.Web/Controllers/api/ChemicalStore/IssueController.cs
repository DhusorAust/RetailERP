
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
    [Route("api/csissue")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly IIssueService _service;
        public IssueController(IIssueService service)
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
        public async Task<IActionResult> GetNew(string Ids, int? MenuID, int? UnitID)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            int? UserID = _service.User.UserID;
            return Ok(await _service.GetNew(Ids,MenuID,UnitID,UserID));
        }
        [HttpGet]
        [Route("edit")]
        public async Task<IActionResult> GetEdit(int id, int? MenuID, int? UnitID)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            int? UserID = _service.User.UserID;
            return Ok(await _service.GetEdit(id,MenuID,UnitID,UserID));
        }
        [HttpGet("GetProductBySearch")]
        public async Task<IActionResult> GetProductBySearch(string searchKey, string itemName, int? menuID, int? unitID, int? userID, int? primaryKeyID, string whereClause, int pageIndex, int pageSize)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            userID = _service.User.UserID;
            return Ok(await _service.GetProductBySearch(searchKey,itemName,menuID, unitID,userID,primaryKeyID,whereClause, pageIndex,pageSize));
        }
        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] CS_IssueMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];

            CS_IssueMaster entity = new CS_IssueMaster();
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
