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
    public class RptAccountsController : Controller
    {
        IWebHostEnvironment _env;
        int unitID = 0;
        public RptAccountsController(IWebHostEnvironment env) {
            _env = env;  
        }
        public int GetUnitID()
        {
            ALLMENU allMenu = new();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            unitID = allMenu.UserList[0].UnitID == 0 ? 2 : allMenu.UserList[0].UnitID;
            return unitID;
        }

        public IActionResult TDSAndVDSSummary()
        {

            string reportFilePath = Path.Combine(_env.ContentRootPath, "Report\\Accounts", "rptSourceTDSAndVDSSummary.repx");
            XtraReport report = XtraReport.FromFile(reportFilePath, true);
            //report.Parameters["unitID"].Value = GetUnitID();

            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Source TDS And VDS Summary";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }

        public IActionResult TDSAndVDSChallan()
        {
            string reportFilePath = Path.Combine(_env.ContentRootPath, "Report\\Accounts", "rptSourceTDSAndVDSList.repx");
            XtraReport report = XtraReport.FromFile(reportFilePath, true);
            //report.Parameters["unitID"].Value = GetUnitID();
            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Source TDS And VDS Summary";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }

        public IActionResult FabricPhysicalInventory()
        {
            var rptPath = "RetailERP.Web.Report.Accounts.rptFabricPhysicalInventory";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult ImportRegister()
        {
            var rptPath = "RetailERP.Web.Report.Accounts.rptImportRegister";            
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }

        //Export DO Grid
        public IActionResult SMReport()
        {
            int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var rptPath = "RetailERP.Web.Report.Accounts.rptDOInformation_S";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            report.Parameters["trnsid"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult MDReport()
        {
            int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var rptPath = "RetailERP.Web.Report.Accounts.rptDOInformation_Small";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
             report.Parameters["trnsid"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult XLReport()
        {
            int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var rptPath = "RetailERP.Web.Report.Accounts.rptDOInformation";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            report.Parameters["trnsid"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult AfterAudit()
        {
            int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var rptPath = "RetailERP.Web.Report.Accounts.rptDOInformation_A";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            report.Parameters["trnsid"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        //Local DO Grid
        public IActionResult AfterAuditLocal()
        {
            int id = Convert.ToInt32(HttpContext.Request.Query["id"]);
            var rptPath = "RetailERP.Web.Report.Accounts.rptDOInformation_Local";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            report.Parameters["trnsid"].Value = id;
            report.RequestParameters = false;
            ViewBag.ReportName = report;
            return View("~/Views/Reports/ReportViewer.cshtml");
        }

        public IActionResult AccountsPRCRegister()
        {
            var rptPath = "RetailERP.Web.Report.Accounts.rptDateWisePRCRegisterAccounts";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Accounts PRC Register";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult ExportDo()
        {
            var rptPath = "RetailERP.Web.Report.Accounts.rptExportDo";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Accounts PRC Register";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
         public IActionResult AccountsMushak66VDSCertificate()
        {
            var rptPath = "RetailERP.Web.Report.Accounts.rptAccountsMushak66VDSCertificate";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Accounts PRC Register";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
         public IActionResult AccountsChallanInfo()
        {
            var rptPath = "RetailERP.Web.Report.Accounts.rptAccountsChallanInfo";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Accounts PRC Register";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult AccountsBillInfo()
        {            
            var rptPath = "RetailERP.Web.Report.Accounts.rptAccountsBillInfo";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            //report.Parameters["unitID"].Value = GetUnitID();

            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Accounts Bill Info";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }

        public IActionResult AccountsBillInfoDtls()
        {           

            var rptPath = "RetailERP.Web.Report.Accounts.rptAccountsBillInfoDtls";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            //report.Parameters["unitID"].Value = GetUnitID();
            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Accounts Bill Info Details";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }

        public IActionResult AccountsSupplierWiseBillInfo()
        {
            var rptPath = "RetailERP.Web.Report.Accounts.rptAccountsSupplierWiseBillInfo";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            //report.Parameters["unitID"].Value = GetUnitID();
            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Supplier Wise Bill Info";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult SourceTDSAndVDSList()
        {
            var rptPath = "RetailERP.Web.Report.Accounts.rptSourceTDSAndVDSList";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
           
            //report.Parameters["unitID"].Value = GetUnitID();
            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Source TDS And VDS List";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }
        public IActionResult SourceTDSAndVDSSummary()
        {
            var rptPath = "RetailERP.Web.Report.Accounts.rptSourceTDSAndVDSSummary";
            XtraReport report = (XtraReport)Activator.CreateInstance(Type.GetType(rptPath));
            //report.Parameters["unitID"].Value = GetUnitID();
            ViewBag.ReportName = report;
            ViewBag.ReportHeader = "Source TDS And VDS Summary";
            return View("~/Views/Reports/ReportViewer.cshtml");
        }


    }
}
