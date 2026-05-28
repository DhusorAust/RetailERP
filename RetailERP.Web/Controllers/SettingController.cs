using RetailERP.Core.Chem.Security;
using RetailERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace RetailERP.Web.Controllers
{
    public class SettingController : Controller
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
        public IActionResult INCIMaster(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/INCIMaster.cshtml",
                    allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult IngredientMaster(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/IngredientMaster.cshtml",
                    allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }




        public IActionResult Bank(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/Bank.cshtml",
                    allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult BankBranch(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/BankBranch.cshtml",
                    allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult BuyerBankAccount(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/BuyerBankAccount.cshtml",
                    allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult CS_ProductType(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/Store/ChemicalStore/ProductType.cshtml", allMenu); 
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult CS_ProductCategory(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/Store/ChemicalStore/ProductCategory.cshtml",
                    allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult CS_ProductMaster(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/Store/ChemicalStore/ProductMaster.cshtml",
                    allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult Brand(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/Brand.cshtml", allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            } 
        }
        public IActionResult Supplier(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/Supplier.cshtml", allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }

        public IActionResult UOM(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/UOM.cshtml", allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult Season(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View("~/Views/Setting/Season.cshtml", allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }

    } 
}
