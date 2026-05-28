using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using RetailERP.Core.Chem.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Composition;
using System.IO;
using RetailERP.Web.Models;


namespace RetailERP.Web.Controllers.rpt
{
    public class RptChemicalStoreController : Controller
    {

        IWebHostEnvironment _env;
        int unitID = 0;
        public RptChemicalStoreController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult Chem_PurchaseRequisition()
        {
            int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var rptPath = "RetailERP.Web.Reports.ChemicalStore.rpt_cs_PurchaseRequisition";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            report.Parameters["pPRMasterID"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }

        public IActionResult Chem_MRR()
        {
            int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var rptPath = "RetailERP.Web.Report.ChemicalStore.rpt_cs_MRR";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            report.Parameters["id"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult Chem_Requirement()
        {
            string id = HttpContext.Request.Query["id"];
            var rptPath = "RetailERP.Web.Reports.ChemicalStore.rpt_cs_Requirement";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            report.Parameters["pRequirementMasterID"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Chemical";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }

    }
}
