namespace RetailERP.Web.Reports.Formulation
{
    partial class Rpt_IngrediantINCIBreakDownDetails
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery1 = new DevExpress.DataAccess.Sql.StoredProcQuery();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.CustomSqlQuery customSqlQuery1 = new DevExpress.DataAccess.Sql.CustomSqlQuery();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Rpt_IngrediantINCIBreakDownDetails));
            DevExpress.XtraReports.Parameters.DynamicListLookUpSettings dynamicListLookUpSettings1 = new DevExpress.XtraReports.Parameters.DynamicListLookUpSettings();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow3 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell11 = new DevExpress.XtraReports.UI.XRTableCell();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.formulationID = new DevExpress.XtraReports.Parameters.Parameter();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell16 = new DevExpress.XtraReports.UI.XRTableCell();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.label1 = new DevExpress.XtraReports.UI.XRLabel();
            this.label2 = new DevExpress.XtraReports.UI.XRLabel();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.pageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "RetailERPConnection";
            this.sqlDataSource1.Name = "sqlDataSource1";
            storedProcQuery1.Name = "spRpt_IngrediantINCIBreakDownDetails";
            queryParameter1.Name = "@FormulationMasterID";
            queryParameter1.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("?formulationID", typeof(int));
            storedProcQuery1.Parameters.AddRange(new DevExpress.DataAccess.Sql.QueryParameter[] {
            queryParameter1});
            storedProcQuery1.StoredProcName = "spRpt_IngrediantINCIBreakDownDetails";
            customSqlQuery1.Name = "FormulationList";
            customSqlQuery1.Sql = "select FormulationMasterID,FormulationCode from rnd_FormulationMaster where IsAct" +
    "ive=1";
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            storedProcQuery1,
            customSqlQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
            this.Detail.HeightF = 25F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.SortFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("RowSLNo", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending),
            new DevExpress.XtraReports.UI.GroupField("DetailsType", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending),
            new DevExpress.XtraReports.UI.GroupField("imRow", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTable2
            // 
            this.xrTable2.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable2.BorderWidth = 0.5F;
            this.xrTable2.Font = new DevExpress.Drawing.DXFont("Times New Roman", 7F);
            this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable2.Name = "xrTable2";
            this.xrTable2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow3});
            this.xrTable2.SizeF = new System.Drawing.SizeF(651F, 25F);
            this.xrTable2.StylePriority.UseBorders = false;
            this.xrTable2.StylePriority.UseBorderWidth = false;
            this.xrTable2.StylePriority.UseFont = false;
            this.xrTable2.StylePriority.UseTextAlignment = false;
            this.xrTable2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTableRow3
            // 
            this.xrTableRow3.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell2,
            this.xrTableCell3,
            this.xrTableCell7,
            this.xrTableCell8,
            this.xrTableCell9,
            this.xrTableCell11});
            this.xrTableRow3.Name = "xrTableRow3";
            this.xrTableRow3.Weight = 1D;
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[IngredientPercentage]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[IngredientName]")});
            this.xrTableCell2.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F);
            this.xrTableCell2.Multiline = true;
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.ProcessDuplicatesMode = DevExpress.XtraReports.UI.ProcessDuplicatesMode.Merge;
            this.xrTableCell2.ProcessDuplicatesTarget = DevExpress.XtraReports.UI.ProcessDuplicatesTarget.Tag;
            this.xrTableCell2.StylePriority.UseFont = false;
            this.xrTableCell2.TextFormatString = "{0:n4}";
            this.xrTableCell2.Weight = 1D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[IngredientName]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[IngredientName]")});
            this.xrTableCell3.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F);
            this.xrTableCell3.Multiline = true;
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.ProcessDuplicatesMode = DevExpress.XtraReports.UI.ProcessDuplicatesMode.Merge;
            this.xrTableCell3.StylePriority.UseFont = false;
            this.xrTableCell3.Weight = 1D;
            // 
            // xrTableCell7
            // 
            this.xrTableCell7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[INCIName]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "BackColor", "Iif([DetailsType] = 2, \'DarkSalmon\', \'Transparent\')")});
            this.xrTableCell7.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F);
            this.xrTableCell7.Multiline = true;
            this.xrTableCell7.Name = "xrTableCell7";
            this.xrTableCell7.StylePriority.UseFont = false;
            this.xrTableCell7.Weight = 1D;
            // 
            // xrTableCell8
            // 
            this.xrTableCell8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Remarks]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "BackColor", "Iif([DetailsType] = 2, \'DarkSalmon\', \'Transparent\')")});
            this.xrTableCell8.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F);
            this.xrTableCell8.Multiline = true;
            this.xrTableCell8.Name = "xrTableCell8";
            this.xrTableCell8.StylePriority.UseFont = false;
            this.xrTableCell8.Weight = 1D;
            // 
            // xrTableCell9
            // 
            this.xrTableCell9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[INCIPercentage]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "BackColor", "Iif([DetailsType] = 2, \'DarkSalmon\', \'Transparent\')")});
            this.xrTableCell9.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F);
            this.xrTableCell9.Multiline = true;
            this.xrTableCell9.Name = "xrTableCell9";
            this.xrTableCell9.StylePriority.UseFont = false;
            this.xrTableCell9.Weight = 1D;
            // 
            // xrTableCell11
            // 
            this.xrTableCell11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[FinalformulationPercentage]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "BackColor", "Iif([DetailsType] = 2, \'DarkSalmon\', \'Transparent\')")});
            this.xrTableCell11.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F);
            this.xrTableCell11.Multiline = true;
            this.xrTableCell11.Name = "xrTableCell11";
            this.xrTableCell11.StylePriority.UseFont = false;
            this.xrTableCell11.TextFormatString = "{0:n5}";
            this.xrTableCell11.Weight = 1D;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 30F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 20F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // formulationID
            // 
            this.formulationID.Description = "Formulation:";
            this.formulationID.Name = "formulationID";
            this.formulationID.Type = typeof(int);
            this.formulationID.ValueInfo = "0";
            dynamicListLookUpSettings1.DataMember = "FormulationList";
            dynamicListLookUpSettings1.DataSource = this.sqlDataSource1;
            dynamicListLookUpSettings1.DisplayMember = "FormulationCode";
            dynamicListLookUpSettings1.FilterString = null;
            dynamicListLookUpSettings1.SortMember = "FormulationCode";
            dynamicListLookUpSettings1.SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
            dynamicListLookUpSettings1.ValueMember = "FormulationMasterID";
            this.formulationID.ValueSourceSettings = dynamicListLookUpSettings1;
            // 
            // PageHeader
            // 
            this.PageHeader.HeightF = 0F;
            this.PageHeader.Name = "PageHeader";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1,
            this.pageInfo1,
            this.label1,
            this.label2});
            this.ReportHeader.HeightF = 130.1666F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrTable1
            // 
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 70.7222F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
            this.xrTable1.SizeF = new System.Drawing.SizeF(650.0001F, 59.44443F);
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.BackColor = System.Drawing.Color.MistyRose;
            this.xrTableRow2.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableRow2.BorderWidth = 0.5F;
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell4,
            this.xrTableCell5,
            this.xrTableCell6,
            this.xrTableCell10,
            this.xrTableCell13,
            this.xrTableCell16});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.StylePriority.UseBackColor = false;
            this.xrTableRow2.StylePriority.UseBorders = false;
            this.xrTableRow2.StylePriority.UseBorderWidth = false;
            this.xrTableRow2.Weight = 1.7666665649414064D;
            // 
            // xrTableCell4
            // 
            this.xrTableCell4.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrTableCell4.Multiline = true;
            this.xrTableCell4.Name = "xrTableCell4";
            this.xrTableCell4.StylePriority.UseFont = false;
            this.xrTableCell4.StylePriority.UseTextAlignment = false;
            this.xrTableCell4.Text = "% of the ingredient (commercial name)";
            this.xrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell4.Weight = 1.0015383674937681D;
            // 
            // xrTableCell5
            // 
            this.xrTableCell5.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrTableCell5.Multiline = true;
            this.xrTableCell5.Name = "xrTableCell5";
            this.xrTableCell5.StylePriority.UseFont = false;
            this.xrTableCell5.StylePriority.UseTextAlignment = false;
            this.xrTableCell5.Text = "Ingredient";
            this.xrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell5.Weight = 1.0015382970685944D;
            // 
            // xrTableCell6
            // 
            this.xrTableCell6.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrTableCell6.Multiline = true;
            this.xrTableCell6.Name = "xrTableCell6";
            this.xrTableCell6.StylePriority.UseFont = false;
            this.xrTableCell6.StylePriority.UseTextAlignment = false;
            this.xrTableCell6.Text = "Ingredient breakdown";
            this.xrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell6.Weight = 1.0015383674937681D;
            // 
            // xrTableCell10
            // 
            this.xrTableCell10.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrTableCell10.Multiline = true;
            this.xrTableCell10.Name = "xrTableCell10";
            this.xrTableCell10.StylePriority.UseFont = false;
            this.xrTableCell10.StylePriority.UseTextAlignment = false;
            this.xrTableCell10.Text = "CAS Number ";
            this.xrTableCell10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell10.Weight = 1.0015378040923786D;
            // 
            // xrTableCell13
            // 
            this.xrTableCell13.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrTableCell13.Multiline = true;
            this.xrTableCell13.Name = "xrTableCell13";
            this.xrTableCell13.StylePriority.UseFont = false;
            this.xrTableCell13.StylePriority.UseTextAlignment = false;
            this.xrTableCell13.Text = "% individual ingredient INCI in the commercial ingredient";
            this.xrTableCell13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell13.Weight = 1.0015394942965468D;
            // 
            // xrTableCell16
            // 
            this.xrTableCell16.Font = new DevExpress.Drawing.DXFont("Tahoma", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrTableCell16.Multiline = true;
            this.xrTableCell16.Name = "xrTableCell16";
            this.xrTableCell16.StylePriority.UseFont = false;
            this.xrTableCell16.StylePriority.UseTextAlignment = false;
            this.xrTableCell16.Text = "%  of each INCI in the final formulation ";
            this.xrTableCell16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrTableCell16.Weight = 0.99230766955494387D;
            // 
            // pageInfo1
            // 
            this.pageInfo1.Font = new DevExpress.Drawing.DXFont("Calibri Light", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(409.0276F, 30.63889F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.pageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.pageInfo1.SizeF = new System.Drawing.SizeF(240.9724F, 26.19445F);
            this.pageInfo1.StylePriority.UseFont = false;
            this.pageInfo1.StylePriority.UseTextAlignment = false;
            this.pageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.pageInfo1.TextFormatString = "Print Date : {0:dd-MMM-yy hh:mm:ss tt}";
            // 
            // label1
            // 
            this.label1.Font = new DevExpress.Drawing.DXFont("Tahoma", 14F, DevExpress.Drawing.DXFontStyle.Bold);
            this.label1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.label1.Multiline = true;
            this.label1.Name = "label1";
            this.label1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label1.SizeF = new System.Drawing.SizeF(650.0001F, 30.36112F);
            this.label1.StylePriority.UseFont = false;
            this.label1.StylePriority.UseTextAlignment = false;
            this.label1.Text = "Cosmetics Chemicals Limited";
            this.label1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new DevExpress.Drawing.DXFont("Tahoma", 12F, DevExpress.Drawing.DXFontStyle.Bold);
            this.label2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 30.63886F);
            this.label2.Multiline = true;
            this.label2.Name = "label2";
            this.label2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.label2.SizeF = new System.Drawing.SizeF(650.0001F, 26.19447F);
            this.label2.StylePriority.UseFont = false;
            this.label2.StylePriority.UseTextAlignment = false;
            this.label2.Text = "List of Ingredients";
            this.label2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageInfo2});
            this.PageFooter.HeightF = 22.99998F;
            this.PageFooter.Name = "PageFooter";
            // 
            // pageInfo2
            // 
            this.pageInfo2.Font = new DevExpress.Drawing.DXFont("Calibri Light", 7F, DevExpress.Drawing.DXFontStyle.Italic);
            this.pageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(500.2591F, 0F);
            this.pageInfo2.Name = "pageInfo2";
            this.pageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.pageInfo2.SizeF = new System.Drawing.SizeF(150.741F, 22.99998F);
            this.pageInfo2.StylePriority.UseFont = false;
            this.pageInfo2.StylePriority.UseTextAlignment = false;
            this.pageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.pageInfo2.TextFormatString = "Page No: {0} Of {1}";
            // 
            // Rpt_IngrediantINCIBreakDownDetails
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.ReportHeader,
            this.PageFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "spRpt_IngrediantINCIBreakDownDetails";
            this.DataSource = this.sqlDataSource1;
            this.Margins = new DevExpress.Drawing.DXMargins(99F, 100F, 30F, 20F);
            this.ParameterPanelLayoutItems.AddRange(new DevExpress.XtraReports.Parameters.ParameterPanelLayoutItem[] {
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.formulationID, DevExpress.XtraReports.Parameters.Orientation.Horizontal)});
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.formulationID});
            this.Version = "22.2";
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.Parameters.Parameter formulationID;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable xrTable2;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow3;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell7;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell8;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell9;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell11;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel label1;
        private DevExpress.XtraReports.UI.XRLabel label2;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.XRTable xrTable1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell4;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell5;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell6;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell10;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell13;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell16;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
    }
}
