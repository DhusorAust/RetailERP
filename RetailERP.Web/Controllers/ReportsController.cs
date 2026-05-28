using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Data.SqlClient;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Core.Chem.Security;
using RetailERP.Web.Models;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMenuReportCreateService _service;
        public SqlConnection Connection { get; set; }
        public ReportsController(IConfiguration configuration, IMenuReportCreateService service)
        {
            _configuration = configuration;
            Connection = new SqlConnection(_configuration.GetConnectionString("ERPDBConnection"));
            _service = service;
        }

        #region Authentication
        public ALLMENU MenuAuthenticationCheck(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            if (allMenu != null && allMenu.MenuList.Where(x => x.MenuID == menuId).FirstOrDefault() != null)
            {
                pageName = string.Join("", pageName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                ViewBag.MenuId = menuId;
                ViewBag.PageName = pageName;
                ViewBag.MenuTitle = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.MenuTitle).FirstOrDefault();
                ViewBag.MenuTitle2 = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.MenuTitle2).FirstOrDefault();
                ViewBag.MenuTitle3 = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.MenuTitle3).FirstOrDefault();
                ViewBag.MenuTitle4 = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.MenuTitle4).FirstOrDefault();
                ViewBag.PageTitle = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.PageName).FirstOrDefault();
                return allMenu;
            }
            else
            {
                ViewBag.PageTitle = "Login";
                return null;
            }
        }
        #endregion Authentication  

        #region Start :: Report List Information
        //----------------- Start :: Action -----------------
        public IActionResult ReportList(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                ViewBag.ReportTitle = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.MenuHead).FirstOrDefault();
                ViewBag.MenuReportID = menuId;
                ViewBag.UserRoleID = allMenu.UserList[0].UserRoleID.ToString();
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult ReportViewer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetReportList([FromBody] GridOptions options, int menuRepportID, int userRoleID)
        {
            var records = await _service.GetReportList(options, menuRepportID, userRoleID);
            return Ok(records);
        }


        //------------------ End :: Action ------------------
        #endregion End :: Default Information

        public IActionResult ExcelExport(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                ViewBag.ReportTitle = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.MenuHead).FirstOrDefault();
                ViewBag.MenuReportID = menuId;
                ViewBag.UserRoleID = allMenu.UserList[0].UserRoleID.ToString();
                return View("../ExcelExport/ExcelExport", allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
    }
}
