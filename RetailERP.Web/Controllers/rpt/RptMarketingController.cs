using RetailERP.Web.Report.Marketing;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;

namespace RetailERP.Web.Controllers.rpt
{
    public class RptMarketingController : Controller
    {
        public IActionResult BriefTemplate(int id = 0)
        {
            XtraReport report = new rpt_BriefTemplate();
            report.Parameters["id"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
    }
}
