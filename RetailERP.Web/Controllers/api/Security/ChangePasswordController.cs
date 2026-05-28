using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailERP.Core.Chem.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RetailERP.Web.Models;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Web.Controllers.api.Security
{
    [Route("api/chnPassword")]
    [ApiController]
    public class ChangePasswordController : ControllerBase
    {
        private readonly IChangePasswordService _service;
        public ChangePasswordController(IChangePasswordService service)
        {
            _service = service;
        }
        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] UserList model)
        { 
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            UserList entity = new UserList(); 

            if (allMenu != null)
            {
                entity = model;
                entity.UserID = Convert.ToInt32( allMenu.UserList[0].UserID.ToString());
                entity.EmployeeID = allMenu.UserList[0].EmployeeID; 
                return Ok(await _service.SaveAsync(entity));
            }
            else
            {
                entity.IsLogin = 1;
                entity.redirectUrl = Url.Action("Index", "Security");
                return Ok(entity);
            }
        }

		[Route("SaveOTP")]
		[HttpPost]
		public async Task<IActionResult> SaveOTP([FromForm] LoginOTP model)
		{
			LoginOTP entity = new LoginOTP();

			if (model != null)
			{
				entity = model;
				return Ok(await _service.SaveOTP(entity));
			}
			else
			{
				entity.IsLogin = 1;
				entity.redirectUrl = Url.Action("Index", "Security");
				return Ok(entity);
			}
		}
		[Route("GetOTP/{userID}")]
		[HttpGet]
		public async Task<IActionResult> GetOTP(string userID)
		{
			return Ok(await _service.GetOTP(userID));
		}
	}
}



