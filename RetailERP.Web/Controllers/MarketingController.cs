using RetailERP.Core.Chem.Security;
using RetailERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace RetailERP.Web.Controllers
{
    public class MarketingController : Controller
    {
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
        public IActionResult BuyerRef(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Marketing/BuyerRef.cshtml", allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult BriefTemplate(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                string menuParam = allMenu.MenuList.Where(x => x.MenuID == menuId).FirstOrDefault().ParameterValue;
                if (menuParam != "" && menuParam == "A") ViewBag.ApprovePage = 1;
                else
                {
                    ViewBag.ApprovePage = 0;
                }
                return View("~/Views/Marketing/BriefTemplate.cshtml", allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        //public IActionResult BriefTemplate2(int menuId, string pageName)
        //{
        //    ALLMENU allMenu = new ALLMENU();
        //    allMenu = MenuAuthenticationCheck(menuId, pageName);
        //    if (allMenu != null)
        //    {
        //        return View("~/Views/Marketing/BriefTemplate2.cshtml", allMenu);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Security");
        //    }
        //}
        public IActionResult SDRFEntry(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                string menuParam = allMenu.MenuList.Where(x => x.MenuID == menuId).FirstOrDefault().ParameterValue;
                if (menuParam != "" && menuParam == "A") ViewBag.ApprovePage = 1;
                else
                {
                    ViewBag.ApprovePage = 0;
                }
                return View("~/Views/Marketing/SDRFEntry.cshtml", allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            } 
        }
        public IActionResult BuyerSampleApproval(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {  
                return View("~/Views/Marketing/BuyerSampleApproval.cshtml", allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult POEntry(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                string menuParam = allMenu.MenuList.Where(x => x.MenuID == menuId).FirstOrDefault().ParameterValue;
                if (menuParam != "" && menuParam == "A") ViewBag.ApprovePage = 1;
                else
                {
                    ViewBag.ApprovePage = 0;
                }
                return View("~/Views/Marketing/POEntry.cshtml", allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
    }
}
