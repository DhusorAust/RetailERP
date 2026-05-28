using Microsoft.AspNetCore.Mvc;

namespace RetailERP.Web.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ProductInfo()
        {

            return View("~/Views/Setting/Product.cshtml");

        }
    }
}
