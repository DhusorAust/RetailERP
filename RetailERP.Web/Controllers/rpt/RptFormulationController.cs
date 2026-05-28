using RetailERP.Web.Reports.Formulation;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace RetailERP.Web.Controllers.rpt
{
    public class RptFormulationController : Controller
    {
        IWebHostEnvironment _env;
        int unitID = 0;
        public RptFormulationController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult FormulationDetails(int id=0)
        {
            //int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var rptPath = "RetailERP.Web.Reports.Formulation.Rpt_FormulationDetails";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            report.Parameters["formulationID"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult IngrediantINCIBreakDownDetails(int id = 0)
        {
            //int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var rptPath = "RetailERP.Web.Reports.Formulation.Rpt_IngrediantINCIBreakDownDetails";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            report.Parameters["formulationID"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult IngrediantINCIBreakDownSummary(int id = 0)
        {
            //int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var rptPath = "RetailERP.Web.Reports.Formulation.Rpt_IngrediantINCIBreakDownSummary";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            report.Parameters["formulationID"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult StabilityTest(int id = 0)
        {
            //int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            XtraReport report = new Rpt_StabilityTest(_env);
            report.Parameters["stabilityID"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }

    }
}
