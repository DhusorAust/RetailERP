using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Hosting;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace RetailERP.Web.Reports.Formulation
{
    public partial class Rpt_StabilityTest_File
    {
        private readonly string _webRootPath;
        private readonly IWebHostEnvironment _environment;

        public string StabilityTestTemperature { get; set; }

        public Rpt_StabilityTest_File()
        {
            InitializeComponent();
            _webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            ConfigureImage();
        }

        public Rpt_StabilityTest_File(IWebHostEnvironment environment)
        {
            InitializeComponent();
            _environment = environment;
            _webRootPath = string.IsNullOrWhiteSpace(_environment.WebRootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : _environment.WebRootPath;

            ConfigureImage();
        }

        private void ConfigureImage()
        {
            Detail.MultiColumn.Mode = MultiColumnMode.UseColumnCount;
            Detail.MultiColumn.ColumnCount = 3;
            Detail.MultiColumn.ColumnSpacing = 5F;
            Detail.MultiColumn.Layout = ColumnLayout.AcrossThenDown;
            Detail.HeightF = 230F;

            pictureBox1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            pictureBox1.SizeF = new System.Drawing.SizeF(225F, 225F);
            pictureBox1.Sizing = ImageSizeMode.StretchImage;
            pictureBox1.ImageAlignment = ImageAlignment.MiddleCenter;
            Detail.BeforePrint += Detail_BeforePrint;
            pictureBox1.BeforePrint += pictureBox1_BeforePrint;
        }

        private void Detail_BeforePrint(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StabilityTestTemperature))
            {
                return;
            }

            var reportTemperature = NormalizeTemperature(StabilityTestTemperature);
            var imageTemperature = NormalizeTemperature(Convert.ToString(GetCurrentColumnValue("StabilityTestTemperature")));

            e.Cancel = !string.Equals(reportTemperature, imageTemperature, StringComparison.OrdinalIgnoreCase);
        }

        private void pictureBox1_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XRPictureBox pictureBox = sender as XRPictureBox;

            string dbImagePath = Convert.ToString(GetCurrentColumnValue("FilePath"));

            if (string.IsNullOrWhiteSpace(dbImagePath))
            {
                pictureBox.ImageSource = null;
                return;
            }

            dbImagePath = dbImagePath
                .Replace("~", "")
                .TrimStart('/', '\\');

            string physicalPath = Path.Combine(_webRootPath, dbImagePath);

            if (File.Exists(physicalPath))
            {
                pictureBox.ImageSource = ImageSource.FromFile(physicalPath);
            }
            else
            {
                pictureBox.ImageSource = null;
            }
        }

        private static string NormalizeTemperature(string temperature)
        {
            if (string.IsNullOrWhiteSpace(temperature))
            {
                return string.Empty;
            }

            var match = Regex.Match(temperature, @"-?\d+");
            return match.Success ? match.Value : temperature.Trim();
        }
    }
}
