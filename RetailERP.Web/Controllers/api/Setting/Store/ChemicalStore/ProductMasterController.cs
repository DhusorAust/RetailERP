using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RetailERP.Web.Models;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using RetailERP.Infrastructure.Chem.Interfaces.Setting.Store.ChemicalStore;
using RetailERP.Core.Chem.Setting.Store.ChemicalStore;

namespace RetailERP.Web.Controllers.api.Setting.Store.ChemicalStore
{
    [Route("api/ProductMaster")]
    [ApiController]
    public class ProductMasterController : ControllerBase
    {
        private readonly IProductMasterService _service;
        public ProductMasterController(IProductMasterService service)
        {
            _service = service;
        }
        [Route("list/{status}")]
        public async Task<IActionResult> GetList([FromBody] GridOptions options, Status status,string lotType)
        {
            var records = await _service.GetList(options, status,lotType);
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
        public async Task<IActionResult> Save([FromForm] Bas_CS_ProductMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");

            Bas_CS_ProductMaster entity = new Bas_CS_ProductMaster();
            if (allMenu != null)
            {
                entity = model;

                if (entity.ProductID > 0)
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
