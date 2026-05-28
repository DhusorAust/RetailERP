/// <reference path="../../common/js/utilitycommon.js" />

var statusCons;
var masterData;
var menuId, pageName;
var ToolbarId;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom;
var isApprovePage = false;
var currentDate;
var dt = new Date();
var fristDayOfMonth = `${dt.getFullYear()}-${dt.getMonth() + 1}-01`;
var lastDayOfMonth = new Date(dt.getFullYear(), dt.getMonth() + 1, 0);


$(document).ready(function () {
    Helper.Initialize();
});

var Manager = {
    GetNew: function () {
        $.get(`/api/export/new`, function (data) {
            if (data.ErrNo > 0) {
                MessageHelper.Warning("Error = ", data.Message);
            } else {
                DivToolbarDom.fadeOut();
                DivDetailsDom.fadeIn();

                masterData = data;

                $("#ReportMenuID").val("0");
                $("#ReportMenuName").val("");
                $("#IsExcelExportAllowed").val("0");

                KendoDropDownHelper.ComboBox(masterData.ReportMenuList, "cmbReportType", "text", "id", false);
                KendoDropDownHelper.ComboBox(masterData.UnitList, "cmbUnit", "text", "id", true);
                KendoDropDownHelper.ComboBox(masterData.BuyerList, "cmbBuyer", "text", "id", true);
                KendoDropDownHelper.ComboBox(masterData.NegoBankList, "cmbNegoBank", "text", "id", true);
                KendoDropDownHelper.ComboBox(masterData.CertTypeList, "cmbCertificateType", "text", "id", true);
                KendoDropDownHelper.ComboBox(masterData.ZoneList, "cmbZone", "text", "id", true);
                KendoDropDownHelper.ComboBox(masterData.YarnTypeList, "cmbYarnType", "text", "id", true);


                var reportType = $("#cmbReportType").data("kendoComboBox").value();
                var userRoleId = CurrentUser.UserRoleID;
                GridManager.InitReportsTable(reportType, userRoleId);

            }
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.Message);
        });
    },
    GetExportExcelData: function (reportTypeId) {
        MessageHelper.FxSpinners("1");
        var model = Helper.GetReportParameter(reportTypeId);
        $.ajax({
            type: "POST",
            url: "/api/export/GetExportExcelData",
            data: model,
            success: function (data) {
                MessageHelper.FxSpinners("2");
                if (data) {
                    var rptFileName = model.ReportMenuName;
                    ExportFileHelper.GetExcelFile(data, rptFileName);
                } else {
                    MessageHelper.Warning("No data returned.");
                }
            },
            error: function (xhr) {
                MessageHelper.FxSpinners("2");
                if (xhr.status === 404) {
                    MessageHelper.Warning(xhr.responseText || "No data found to export.");
                } else {
                    MessageHelper.Error("Error occurred: " + xhr.status + " - " + xhr.statusText);
                }
            }
        });
    },

    GetExportExcelData_View: function (reportTypeId) {
        MessageHelper.FxSpinners("1");
        var model = Helper.GetReportParameter(reportTypeId);
        $.ajax({
            type: "POST",
            url: "/api/export/GetExportExcelData",
            data: model,
            success: function (data) {
                MessageHelper.FxSpinners("2");
                if (data) {
                    var rptFileName = model.ReportMenuName;
                    ExportFileHelper.GetExcelFile(data, rptFileName);
                } else {
                    MessageHelper.Warning("No data returned.");
                }
            },
            error: function (xhr) {
                MessageHelper.FxSpinners("2");
                if (xhr.status === 404) {
                    MessageHelper.Warning(xhr.responseText || "No data found to export.");
                } else {
                    MessageHelper.Error("Error occurred: " + xhr.status + " - " + xhr.statusText);
                }
            }
        });
    }
}
var Helper = {
    //Pages Function
    Initialize: function () {
        //Library Code
        if (!menuId)
            menuId = localStorage.getItem("menuId");
        if (!pageName)
            pageName = localStorage.getItem("pageName");

        var pageId = pageName + "-" + menuId;
        DivToolbarDom = $(pageConstants.DIV_TOOLBAR_ID + pageId);
        ToolbarId = pageConstants.TOOLBAR_ID + pageId;
        ToolbarDom = $(ToolbarId);
        TblMasterId = pageConstants.MASTER_TABLE_ID + pageId;
        TblChildId = pageConstants.CHILD_TABLE_ID + pageId;
        FormDom = $(pageConstants.FORM_ID + pageId);
        DivDetailsDom = $(pageConstants.DIV_DETAILS_ID + pageId);

        Manager.GetNew();

        $("#cmbUnit").on("change", function () {
            var unitId = $("#cmbUnit").data("kendoComboBox").value();
        })

        FormHelper.LoadDatePicker("txtFromDate");
        FormHelper.LoadDatePicker("txtToDate");

        Helper.LoadInvoiceComboBox();
        Helper.LoadLCComboBox(0);


        $("#cmbReportType").change(function () {
            var reportType = $("#cmbReportType").data("kendoComboBox").value();
            var userRoleId = CurrentUser.UserRoleID;
            GridManager.InitReportsTable(reportType, userRoleId);
        });

        $("#btnGenerateReport").on("click", function (e) {
            var rptTypeId = $("#ReportMenuID").val();
            var isAllowed = $("#IsExcelExportAllowed").val();

            if (kendo.parseInt(rptTypeId) > 0) {
                if (kendo.parseInt(isAllowed) > 0) {
                    Manager.GetExportExcelData(1);
                } else {
                    MessageHelper.Warning("Excel export option not available!");
                }
            }
            else {
                MessageHelper.Warning("Please Select a Report");
            }
        });

        $("#btnViewReport").on("click", function (e) {
            var rptTypeId = $("#ReportMenuID").val();
            var isAllowed = $("#IsExcelExportAllowed").val();

            if (kendo.parseInt(rptTypeId) > 0) {
                Manager.GetExportExcelData(3);
            }
            else {
                MessageHelper.Warning("Please Select a Report");
            }
        });

        $("#btnToday").click(function (e) {
            e.preventDefault();
            var currentDate = new Date();
            $("#txtFromDate").data("kendoDatePicker").value(currentDate);
            $("#txtToDate").data("kendoDatePicker").value(currentDate);
        });
        $("#btnYesterday").click(function (e) {
            e.preventDefault();
            var yesterday = new Date();
            yesterday.setDate(yesterday.getDate() - 1);
            $("#txtFromDate").data("kendoDatePicker").value(yesterday);
            $("#txtToDate").data("kendoDatePicker").value(yesterday);
        });
        $("#btnThisMonth").click(function (e) {
            e.preventDefault();
            var currentDate = new Date();
            var date = new Date();
            var fristDayOfMonth = `${date.getFullYear()}-${date.getMonth() + 1}-01`;
            $("#txtFromDate").data("kendoDatePicker").value(new Date(fristDayOfMonth));
            $("#txtToDate").data("kendoDatePicker").value(currentDate);
        });
        $("#btnLastMonth").click(function (e) {
            e.preventDefault();
            var now = new Date();
            var prevMonthLastDate = new Date(now.getFullYear(), now.getMonth(), 0);
            var prevMonthFirstDate = new Date(now.getFullYear() - (now.getMonth() > 0 ? 0 : 1), (now.getMonth() - 1 + 12) % 12, 1);
            $("#txtFromDate").data("kendoDatePicker").value(prevMonthFirstDate);
            $("#txtToDate").data("kendoDatePicker").value(prevMonthLastDate);
        });
        $("#btnThisYear").click(function (e) {
            e.preventDefault();
            var currentYear = new Date().getFullYear();
            var firstDay = new Date(currentYear, 0, 1);
            var lastDay = new Date(currentYear, 11, 31);
            var firstDayString = firstDay.toISOString().split('T')[0];
            var lastDayString = lastDay.toISOString().split('T')[0];

            $("#txtFromDate").data("kendoDatePicker").value(firstDay);
            $("#txtToDate").data("kendoDatePicker").value(lastDay);
        });
        $("#btnLastYear").click(function (e) {
            e.preventDefault();
            var currentYear = new Date().getFullYear();
            var previousYear = currentYear - 1;
            var firstDayPrevYear = new Date(previousYear, 0, 1);
            var lastDayPrevYear = new Date(previousYear, 11, 31);

            $("#txtFromDate").data("kendoDatePicker").value(firstDayPrevYear);
            $("#txtToDate").data("kendoDatePicker").value(lastDayPrevYear);
        });

        $("#tblReportId").change(function (e) {
            GridHelper.GridonRowSelect(e);
        });

    },

    GetReportParameter: function (reportTypeID) {
        var obj = new Object();
        obj.ReportTypeID = reportTypeID;//View/Excel/PDF
        obj.ReportMenuID = $("#ReportMenuID").val();
        obj.ReportMenuName = $("#ReportMenuName").val();
        obj.UnitID = $("#cmbUnit").data("kendoComboBox").value() || 0;
        obj.LCID = $("#cmbLC").data("kendoMultiColumnComboBox").value() || 0;
        obj.InvoiceMasterID = $("#cmbInvoice").data("kendoMultiColumnComboBox").value() || 0;
        obj.CertificateTypeID = $("#cmbCertificateType").data("kendoComboBox").value() || 0;
        obj.NegBankID = $("#cmbNegoBank").data("kendoComboBox").value() || 0;
        obj.BuyerID = $("#cmbBuyer").data("kendoComboBox").value() || 0;
        obj.ZoneID = $("#cmbZone").data("kendoComboBox").value() || "";
        obj.YarnTypeID = $("#cmbYarnType").data("kendoComboBox").value() || 0;

        obj.DateFrom = kendo.toString(kendo.parseDate($("#txtFromDate").data("kendoDatePicker").value()), "dd-MMM-yyyy");
        obj.DateTo = kendo.toString(kendo.parseDate($("#txtToDate").data("kendoDatePicker").value()), "dd-MMM-yyyy");

        return obj;
    },
    GenerateDatePicker: function () {
        $("#txtFromDate").kendoDatePicker({
            format: "dd-MMM-yyyy"
        });

        $("#txtToDate").kendoDatePicker({
            format: "dd-MMM-yyyy"
        });


    },

    LoadDatePicker: function (identity) {
        $("#" + identity).kendoDatePicker({
            format: "dd-MMM-yyyy",
            change: function () {

                var inputDate = $("#" + identity).val();
                var date;

                // Determine format based on input length
                if (inputDate.length === 6) {
                    date = kendo.parseDate(inputDate, "ddMMyy");
                } else if (inputDate.length === 8) {
                    date = kendo.parseDate(inputDate, "ddMMyyyy");
                } else {
                    date = kendo.parseDate(inputDate, "dd-MMM-yyyy");
                }

                // Set the parsed date in the correct format
                if (date) {
                    this.value(kendo.toString(date, "dd-MMM-yyyy"));
                } else {
                    this.value(kendo.toString(inputDate, "dd-MMM-yyyy"));
                }
            }
        });
        $("#" + identity).on("change", function () {
            var inputValue = $(this).val();

            $("#" + identity).data("kendoDatePicker").trigger("change");
        });
    },

    LoadInvoiceComboBox: function () {
        var apUrl = `/api/helperC/GetAllInvoiceBySearch`;
        var columns = [
            {
                field: "InvoiceNo", title: "Invoice No", width: 150,
                template: function (dataItem) {
                    return KendoDropDownHelper.HighlightText(dataItem.FileNo, comboBoxSearchText);
                }
            },
            {
                field: "InvoiceDate", title: "Inv. Date", hidden: statusCons != statusConstants.PENDING, width: 150,
                template: "#= (InvoiceDate == null)? '' : kendo.toString(kendo.parseDate(InvoiceDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
            },
            //{
            //    field: "FileNo", title: "File No", width: 150,
            //    template: function (dataItem) {
            //        return KendoDropDownHelper.HighlightText(dataItem.FileNo, comboBoxSearchText);
            //    }
            //},
            //{
            //    field: "LCNo", title: "LC /No", width: 250,
            //    template: function (dataItem) {
            //        return KendoDropDownHelper.HighlightText(dataItem.LCNo, comboBoxSearchText);
            //    }
            //},
            {
                field: "BuyerName", title: "Buyer", width: 280,
                template: function (dataItem) {
                    return KendoDropDownHelper.HighlightText(dataItem.BuyerName, comboBoxSearchText);
                }
            },

        ];

        KendoDropDownHelper.KendoMultiColumn_BySearch("#cmbInvoice", apUrl, columns, "InvoiceNo", "InvoiceMasterID");
    },

    LoadLCComboBox: function (unitID) {
        var apUrl = `/api/invoiceinfo/GetAllLCListByUnitID?unitId=` + unitID;
        var columns = [
            {
                field: "LCNo", title: "LC No", width: 150,
                template: function (dataItem) {
                    return KendoDropDownHelper.HighlightText(dataItem.FileNo, comboBoxSearchText);
                }
            },
            {
                field: "LCDate", title: "LC Date", hidden: statusCons != statusConstants.PENDING, width: 150,
                template: "#= (LCDate == null)? '' : kendo.toString(kendo.parseDate(LCDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
            },
            {
                field: "FileNo", title: "File No", width: 150,
                template: function (dataItem) {
                    return KendoDropDownHelper.HighlightText(dataItem.FileNo, comboBoxSearchText);
                }
            },
            {
                field: "BuyerName", title: "Buyer", width: 280,
                template: function (dataItem) {
                    return KendoDropDownHelper.HighlightText(dataItem.BuyerName, comboBoxSearchText);
                }
            },

        ];

        KendoDropDownHelper.KendoMultiColumn_BySearch("#cmbLC", apUrl, columns, "LCNo", "LCID");
    },
}

var GridManager = {
    InitReportsTable: function (MenuReportID, UserRoleID) {
        if ($("#tblReportId").data("kendoGrid")) $("#tblReportId").kendoGrid('destroy').empty();
        dataSource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 100,
            transport: {
                read: {
                    type: "POST",
                    url: `/Reports/GetReportList?menuRepportID=${MenuReportID}&&UserRoleID=${UserRoleID}`,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                },
                parameterMap: function (options) {
                    return JSON.stringify(options);
                }
            },
            schema: {
                data: "Items", total: "TotalCount",
                model: {

                }
            }
        });

        var command = [{
            name: "view", text: "", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt",
            click: GridHelper.handleCommands,
        },];

        var command2 = [{
            name: "export", text: "", iconClass: "k-icon k-i-checkbox", className: "k-i-checkbox k-grid-checkbox"
            , click: GridHelper.handleCommands

            , visible: function (dataItem) {
                return dataItem.IsExcelExportAllowed === 1;
            }
        },];

        var columns = [


            {
                field: "MenuReportID", title: "MenuReportID", hidden: true,
            },
            {
                field: "MenuID	", title: "MenuID", hidden: true,
            },
            {
                field: "ViewName", title: "ViewName", hidden: true,
            },
            {
                field: "Controller", title: "Controller", hidden: true,
            },
            { field: "RowSLNo", title: "Sl. No.", template: "#= ++gbGridSLNo #", width: 80 },
            {
                field: "MenuHead", title: "Report Name",
            },
            {
                title: "View", command: command, width: 90,
            },
            {
                title: "Export", command: command2, width: 90,
            },
        ];
        var search =
        {
            fields:
                [
                    "MenuHead"
                ]
        }
        var height = $(window).height() - 200;
        KendoGridHelper.MasterGridLoad("#tblReportId", height, search, columns);

        $("#tblReportId").data("kendoGrid").bind("change", function (e) {
            var selected = this.select();
            var dataItem = this.dataItem(selected);
            if (dataItem) {
                $("#divReportParam").hide();
                if (dataItem.IsExcelExportAllowed) {
                    $("#divReportParam").show();
                }
                $("#lblReportName").text(dataItem.MenuHead);
                $("#ReportMenuID").val(dataItem.MenuReportID);
                $("#ReportMenuName").val(dataItem.ViewName);
                $("#IsExcelExportAllowed").val(dataItem.IsExcelExportAllowed);
                GridHelper.ShowHideReportParameter(dataItem.ViewName);
            }

            var grid = this;
            grid.tbody.find("tr").each(function () {
                var tr = $(this);
                var iconSpan = tr.find(".k-grid-checkbox .k-icon");
                var exportBtnIcon = tr.find(".k-grid-checkbox .k-icon");
                if (exportBtnIcon.length) {
                    if (iconSpan.length) {
                        if (tr.hasClass("k-state-selected")) {
                            iconSpan.removeClass("k-i-checkbox").addClass("fa fa-check-square");
                        } else {
                            iconSpan.removeClass("fa fa-check-square").addClass("k-i-checkbox");
                        }
                    }
                }
            });
        });
    }
}

var GridHelper = {
    handleCommands: function (args) {
        args.preventDefault();
        var tr = $(args.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);
        this.select(tr)
        if (selectedItem != null) {
            
            $("#divReportParam").hide();
            $("#ReportMenuID").val(selectedItem.MenuReportID);
            $("#ReportMenuName").val(selectedItem.MenuReportID);
            $("#IsExcelExportAllowed").val(selectedItem.IsExcelExportAllowed);

            if (args.data.commandName == "view") {
                var controller = selectedItem.Controller;
                var viewPage = selectedItem.ViewName;
                window.open("/" + controller + "/" + viewPage + "", "_blank");
            }

            if (args.data.commandName == "export") {
                GridHelper.ShowHideReportParameter(selectedItem.ViewName);
                $("#lblReportName").text(selectedItem.MenuHead);
                $("#divReportParam").show();
                $("#ReportMenuID").val(selectedItem.MenuReportID);
                $("#ReportMenuName").val(selectedItem.ViewName);
                $("#IsExcelExportAllowed").val(selectedItem.IsExcelExportAllowed);

              
                //var iconSpan = $(args.currentTarget).find(".k-icon");
                //if (iconSpan.length) {
                //    if (iconSpan.hasClass("k-i-checkbox")) {
                //        iconSpan.removeClass("k-i-checkbox").addClass("fa fa-check-square");
                //    } else {
                //        iconSpan.removeClass("fa fa-check-square").addClass("k-i-checkbox");
                //    }
                //}

            }
        }
    },

    ShowHideReportParameter: function (reportName) {
        $("#divNegoBank").show();
        $("#divLCNo").show();
        $("#divInvoice").show();
        $("#divZone").show();
        $("#divCertType").show();
        $("#divYarnType").show();

        if (reportName == "LCRegisterBook") {
            $("#divLCNo").hide();
            $("#divInvoice").hide();
            $("#divYarnType").hide();
        }
        if (reportName == "LCDOMonitoring") {
            $("#divNegoBank").hide();
            $("#divLCNo").hide();
            $("#divInvoice").hide();
            $("#divZone").hide();
            $("#divCertType").hide();
            $("#divYarnType").hide();

        }
    }

}


