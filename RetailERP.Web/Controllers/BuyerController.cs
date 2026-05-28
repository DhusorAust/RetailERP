using Microsoft.AspNetCore.Mvc;

namespace RetailERP.Web.Controllers
{
    public class BuyerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        } 
        public IActionResult BuyerInfo()
        {

            return View("~/Views/Setting/Buyer.cshtml");

        }
    }
}
