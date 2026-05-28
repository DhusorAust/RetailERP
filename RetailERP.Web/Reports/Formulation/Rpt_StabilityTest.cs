using System;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Hosting;

namespace RetailERP.Web.Reports.Formulation
{
    public partial class Rpt_StabilityTest
    {
        private readonly IWebHostEnvironment _environment;

        public Rpt_StabilityTest()
        {
            InitializeComponent();
            ConfigureTemperatureSubreport();
        }

        public Rpt_StabilityTest(IWebHostEnvironment environment)
        {
            _environment = environment;
            InitializeComponent();
            ConfigureTemperatureSubreport();
        }

        private void ConfigureTemperatureSubreport()
        {
            if (GroupHeader1.GroupFields.Count == 0)
            {
                GroupHeader1.GroupFields.Add(new GroupField("StabilityTestTemperature"));
            }

            subreport1.BeforePrint += subreport1_BeforePrint;
        }

        private void subreport1_BeforePrint(object sender, CancelEventArgs e)
        {
            var subreport = sender as XRSubreport;
            if (subreport == null)
            {
                return;
            }

            var report = _environment == null
                ? new Rpt_StabilityTest_File()
                : new Rpt_StabilityTest_File(_environment);

            report.RequestParameters = false;
            report.StabilityTestTemperature = Convert.ToString(GetCurrentColumnValue("StabilityTestTemperature"));
            report.Parameters["stabilityID"].Value = Parameters["stabilityID"].Value;

            subreport.ReportSource = report;
        }
    }
}
