/// <reference path="../../common/js/utilitycommon.js" />

var statusCons;
var masterData = {};
var menuId, pageName;
var ToolbarId;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom;
var TblMasterId, TblConsumerId, TblIngredientId, TblReferenceId, TblProductTestId;
var TblPrimaryPackagingId, TblSecondaryPackagingId, TblShipperPackagingId;

var defaultConsumers = [
    { BTConsumerID: 0, BTMasterID: 0, GenderID: 0, MinAge: 16, MaxAge: 28, IsActive: 1 }
];

var requirementTypes = [
    { id: 1, text: "Must Have" },
    { id: 2, text: "Nice to Have" },
    { id: 3, text: "Restricted" }
];

var defaultProductTests = [
    { BTProductTestID: 0, BTMasterID: 0, ProductTestID: 1, ProductTestName: "Stability (Standard cosmetic - 3months)", IsTested: true, Remarks: "yes", IsActive: 1 },
    { BTProductTestID: 0, BTMasterID: 0, ProductTestID: 2, ProductTestName: "Stability TGA protocol", IsTested: false, Remarks: "No", IsActive: 1 },
    { BTProductTestID: 0, BTMasterID: 0, ProductTestID: 3, ProductTestName: "Packaging Compatibility", IsTested: true, Remarks: "yes", IsActive: 1 },
    { BTProductTestID: 0, BTMasterID: 0, ProductTestID: 4, ProductTestName: "PET", IsTested: true, Remarks: "yes", IsActive: 1 },
    { BTProductTestID: 0, BTMasterID: 0, ProductTestID: 5, ProductTestName: "Microbiology Test", IsTested: true, Remarks: "yes", IsActive: 1 },
    { BTProductTestID: 0, BTMasterID: 0, ProductTestID: 6, ProductTestName: "SPF TEST", IsTested: false, Remarks: "no", IsActive: 1 },
    { BTProductTestID: 0, BTMasterID: 0, ProductTestID: 7, ProductTestName: "HRIPT", IsTested: true, Remarks: "yes", IsActive: 1 },
    { BTProductTestID: 0, BTMasterID: 0, ProductTestID: 8, ProductTestName: "Clinical/Consumer Trial", IsTested: true, Remarks: "yes", IsActive: 1 },
    { BTProductTestID: 0, BTMasterID: 0, ProductTestID: 9, ProductTestName: "Other", IsTested: false, Remarks: "no", IsActive: 1 }
];

var packagingLevels = {
    primary: 1,
    secondary: 2,
    shipper: 3
};

$(document).ready(function () {
    BriefTemplate2Helper.Initialize();
});

var BriefTemplate2Manager = {
    GetNew: function () {
        $.get("/api/BriefTemplate/new", function (data) {
            masterData = data || {};
            BriefTemplate2Helper.ResetForm();
            BriefTemplate2Helper.BindLists(masterData);
            BriefTemplate2Helper.ShowDetails();
            BriefTemplate2Grid.InitChildTables(masterData);
        }).catch(function (err) {
            MessageHelper.Warning("Error", err.responseJSON ? err.responseJSON.Message : err.responseText);
        });
    },

    GetDetails: function (id) {
        $.get("/api/BriefTemplate/edit?id=" + id, function (data) {
            masterData = data || {};
            FormHelper.SetFormData(FormDom, masterData);
            BriefTemplate2Helper.BindLists(masterData);
            BriefTemplate2Helper.SetMultiValue("#CountryIDs", $.map(masterData.ComplainceChilds || [], function (item) {
                return item.CountryID;
            }));
            BriefTemplate2Helper.ShowDetails();
            BriefTemplate2Grid.InitChildTables(masterData);
        }).catch(function (err) {
            MessageHelper.Warning("Error", err.responseJSON ? err.responseJSON.Message : err.responseText);
        });
    },

    Save: function (crudValue) {
        if (!BriefTemplate2Helper.IsFrmValid()) {
            return;
        }

        var model = FormHelper.FormElToJson(FormDom);
        model.SaveOption = BriefTemplate2Helper.GetSaveOption(crudValue);
        model.ConsumerChilds = BriefTemplate2Grid.GridData(TblConsumerId);
        model.IngredientChilds = BriefTemplate2Grid.GridData(TblIngredientId);
        model.ReferenceChilds = BriefTemplate2Grid.GridData(TblReferenceId);
        model.PackagingChilds = BriefTemplate2Grid.PackagingGridData();
        model.ComplainceChilds = BriefTemplate2Helper.GetComplianceChilds();
        model.ProductTestChilds = BriefTemplate2Grid.GridData(TblProductTestId);

        $.ajax({
            url: "/api/BriefTemplate/save",
            type: "POST",
            data: BriefTemplate2Helper.ToFormData(model),
            processData: false,
            contentType: false,
            success: function (data) {
                var resultId = parseInt(data.ResultId) || parseInt(data.resultId) || 0;
                if (resultId > 0) {
                    BriefTemplate2Grid.UploadPackagingFiles(resultId)
                        .done(function () {
                            MessageHelper.Success("Save Operations", data.Message || data.message || "Save operation successful.");
                            BriefTemplate2Helper.BackToList();
                        })
                        .fail(function (err) {
                            MessageHelper.Warning("File Upload", err.responseJSON ? err.responseJSON.Message : "Brief saved, but packaging file upload failed.");
                        });
                } else if (data.IsLogin == 1 || data.isLogin == 1) {
                    window.open(data.redirectUrl, "_blank");
                } else {
                    MessageHelper.Warning("Save Operations", data.Message || data.message || "Save operation failed.");
                }
            },
            error: function (err) {
                MessageHelper.Warning("Save Operations", err.responseJSON ? err.responseJSON.Message : err.responseText);
            }
        });
    },

    GetList: function () {
        BriefTemplate2Grid.InitMasterTable();
    },

    GetReport: function (id) {
        window.open("/RptMarketing/BriefTemplate?id=" + id, "_blank");
    }
};

var BriefTemplate2Helper = {
    Initialize: function () {
        menuId = menuId || localStorage.getItem("menuId");
        pageName = pageName || localStorage.getItem("pageName");

        var pageId = pageName + "-" + menuId;
        DivToolbarDom = $(pageConstants.DIV_TOOLBAR_ID + pageId);
        ToolbarId = pageConstants.TOOLBAR_ID + pageId;
        ToolbarDom = $(ToolbarId);
        TblMasterId = pageConstants.MASTER_TABLE_ID + pageId;
        TblConsumerId = "#tblConsumer" + pageId;
        TblIngredientId = pageConstants.CHILD_TABLE_ID2 + pageId;
        TblReferenceId = pageConstants.CHILD_TABLE_ID3 + pageId;
        TblProductTestId = "#tblProductTest" + pageId;
        TblPrimaryPackagingId = "#tblPrimaryPackaging" + pageId;
        TblSecondaryPackagingId = "#tblSecondaryPackaging" + pageId;
        TblShipperPackagingId = "#tblShipperPackaging" + pageId;
        FormDom = $(pageConstants.FORM_ID + pageId);
        DivDetailsDom = $(pageConstants.DIV_DETAILS_ID + pageId);

        ToolbarDom.find("#btnNewAdd").on("click", function () {
            BriefTemplate2Manager.GetNew();
        });

        ToolbarDom.find("#btnEditList").on("click", function () {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            BriefTemplate2Manager.GetList();
        });

        ToolbarDom.find("#btnApprovedList").on("click", function () {
            statusCons = statusConstants.APPROVED;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            BriefTemplate2Manager.GetList();
        });

        ToolbarDom.find("#btnRejectList").on("click", function () {
            statusCons = statusConstants.REJECT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            BriefTemplate2Manager.GetList();
        });

        ToolbarDom.find("#btnAllList").on("click", function () {
            statusCons = statusConstants.ALL;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            BriefTemplate2Manager.GetList();
        });

        $("#btnBackToList").on("click", function () {
            BriefTemplate2Helper.BackToList();
        });

        $("#btnSave,#btnSaveForApprove").on("click", function () {
            BriefTemplate2Manager.Save(1);
        });

        $("#btnAcknowledgement").on("click", function () {
            BriefTemplate2Manager.Save(4);
        });

        statusCons = statusConstants.EDIT;
        FormHelper.TogToolbarBtn(ToolbarDom.find("#btnEditList"), ToolbarDom);
        BriefTemplate2Manager.GetList();
    },

    ShowDetails: function () {
        DivToolbarDom.stop(true, true).hide();
        DivDetailsDom.stop(true, true).show();
    },

    BackToList: function () {
        BriefTemplate2Helper.ResetForm();
        BriefTemplate2Grid.DestroyChildTables();
        DivDetailsDom.stop(true, true).hide();
        DivToolbarDom.stop(true, true).show();
        BriefTemplate2Grid.InitMasterTable();
    },

    ResetForm: function () {
        FormDom.trigger("reset");
        FormDom.find("input[type='hidden']").val("0");
        $("#BTMasterID").val("0");
    },

    BindLists: function (data) {
        this.Combo("#BrandID", data.BrandList || []);
        this.Combo("#ProductID", data.ProductList || []);
        this.Combo("#ProductTypeID", data.ProductTypeList || []);
        this.Combo("#ServiceOptionID", data.ServiceOptionList || []);
        this.Combo("#TextureID", data.TextureTypeList || []);
        this.Combo("#ColorID", data.ColorIList || []);
        this.Combo("#FragranceID", data.FragranceList || []);
        this.EmployeeCombo("#ProjectLeaderBy", data.EmployeeList || []);
        this.MultiSelect("#CountryIDs", data.CountryList || []);

        this.DestroyKendoWidget("#CompletionDate", "kendoDatePicker");
        var completionDate = kendo.parseDate(data.CompletionDate);
        if (!completionDate || completionDate.getFullYear() <= 1900) {
            completionDate = new Date();
        }
        $("#CompletionDate").kendoDatePicker({
            format: "dd-MMM-yyyy",
            value: completionDate
        });
    },

    Combo: function (selector, data) {
        var input = $(selector);
        this.DestroyKendoWidget(selector, "kendoComboBox");
        input.kendoComboBox({
            dataTextField: "text",
            dataValueField: "id",
            filter: "contains",
            suggest: true,
            dataSource: data,
            value: input.val()
        });
    },

    EmployeeCombo: function (selector, data) {
        var input = $(selector);
        this.DestroyKendoWidget(selector, "kendoMultiColumnComboBox");
        input.kendoMultiColumnComboBox({
            dataTextField: "text",
            dataValueField: "id",
            filter: "contains",
            filterFields: ["text", "valueD", "valueE", "valueF"],
            minLength: 4,
            suggest: true,
            valuePrimitive: true,
            columns: [
                { field: "text", title: "Name", width: 200 },
                { field: "valueD", title: "Department", width: 200 },
                { field: "valueE", title: "Section", width: 200 },
                //{ field: "valueF", title: "Sub Section", width: 300 }
            ],
            dataSource: new kendo.data.DataSource({
                serverFiltering: true,
                transport: {
                    read: function (options) {
                        var searchKey = BriefTemplate2Helper.EmployeeSearchKey(input, options);
                        if (searchKey && searchKey.length < 4) {
                            options.success([]);
                            return;
                        }

                        $.get("/api/BriefTemplate/employees", {
                            searchKey: searchKey,
                            employeeId: searchKey ? 0 : (parseInt(input.val()) || 0)
                        }, function (response) {
                            options.success(response || []);
                        }).catch(function () {
                            options.success([]);
                        });
                    }
                }
            }),
            filtering: function (e) {
                var searchKey = BriefTemplate2Helper.EmployeeFilterValue(e.filter);
                if (searchKey && searchKey.length < 4) {
                    e.preventDefault();
                }
            },
            value: input.val()
        });
    },

    EmployeeSearchKey: function (input, options) {
        var searchKey = this.EmployeeFilterValue(options.data ? options.data.filter : null);
        if (searchKey) return searchKey;

        var widget = input.data("kendoMultiColumnComboBox");
        searchKey = widget ? $.trim(widget.text() || "") : "";
        if (parseInt(searchKey) === parseInt(input.val())) return "";
        return searchKey;
    },

    EmployeeFilterValue: function (filter) {
        if (!filter) return "";
        if (filter.value) return $.trim(filter.value || "");

        var filters = filter.filters || [];
        for (var i = 0; i < filters.length; i++) {
            var value = this.EmployeeFilterValue(filters[i]);
            if (value) return value;
        }

        return "";
    },

    MultiSelect: function (selector, data) {
        var input = $(selector);
        this.DestroyKendoWidget(selector, "kendoMultiSelect");
        input.kendoMultiSelect({
            dataTextField: "text",
            dataValueField: "id",
            autoClose: false,
            dataSource: data
        });
    },

    DestroyKendoWidget: function (selector, widgetName) {
        var input = $(selector);
        var widget = input.data(widgetName);
        if (!widget) {
            input.show();
            return;
        }

        var element = widget.element || input;
        var wrapper = widget.wrapper;
        var popup = widget.popup && widget.popup.element ? widget.popup.element.closest(".k-popup, .k-animation-container") : $();

        widget.destroy();

        if (wrapper && wrapper.length) {
            wrapper.before(element);
            wrapper.remove();
        }

        if (popup && popup.length) {
            popup.remove();
        }

        element
            .removeAttr("data-role aria-disabled aria-readonly")
            .removeClass("k-input k-valid k-invalid")
            .show();
    },

    SetMultiValue: function (selector, values) {
        var widget = $(selector).data("kendoMultiSelect");
        if (widget) {
            widget.value(values || []);
        }
    },

    GetComplianceChilds: function () {
        var widget = $("#CountryIDs").data("kendoMultiSelect");
        var values = widget ? widget.value() : [];
        return $.map(values, function (countryId) {
            return {
                BTComplainceID: 0,
                BTMasterID: parseInt($("#BTMasterID").val()) || 0,
                CountryID: parseInt(countryId) || 0,
                IsActive: 1
            };
        });
    },

    IsFrmValid: function () {
        var elements = ["ProjectName", "BrandID", "CompletionDate", "ProductID", "OrderQuantity", "TargetPerQtyCost", "ServiceOptionID", "MaxBulkPrice", "ProductTypeID"];
        if (!FormHelper.IsFormValidation(FormDom, elements)) {
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
            return false;
        }
        if (BriefTemplate2Grid.GridData(TblConsumerId).length === 0) {
            MessageHelper.Warning("Warning", "At least one consumer row is required.");
            return false;
        }
        return true;
    },

    ConsumerRows: function (rows) {
        if (rows && rows.length > 0) {
            return rows;
        }

        var defaultGender = $.grep(masterData.GenderList || [], function (item) {
            return (item.text || "").toLowerCase().indexOf("female") >= 0;
        })[0] || (masterData.GenderList || [])[0];

        return $.map(defaultConsumers, function (item) {
            return $.extend({}, item, { GenderID: defaultGender ? defaultGender.id : 0 });
        });
    },

    ProductTestRows: function (rows) {
        if (rows && rows.length > 0) {
            return rows;
        }

        return $.map(defaultProductTests, function (item) {
            return $.extend({}, item);
        });
    },

    GetSaveOption: function (value) {
        var pkId = parseInt($("#BTMasterID").val()) || 0;
        if (value == 1 && pkId == 0) return 1;
        if (value == 1 && pkId > 0) return 2;
        if (value == 4 && pkId > 0) return 4;
        return 1;
    },

    ToFormData: function (model) {
        var formData = new FormData();

        function appendValue(prefix, value) {
            if ($.isArray(value)) {
                $.each(value, function (index, item) {
                    appendValue(prefix + "[" + index + "]", item);
                });
                return;
            }

            if (value !== null && typeof value === "object") {
                $.each(value, function (key, item) {
                    if (key.charAt(0) === "_") return;
                    appendValue(prefix ? prefix + "." + key : key, item);
                });
                return;
            }

            if (typeof value !== "undefined") {
                formData.append(prefix, value == null ? "" : value);
            }
        }

        appendValue("", model);
        return formData;
    }
};

var BriefTemplate2Grid = {
    InitChildTables: function (data) {
        this.InitConsumerTable(BriefTemplate2Helper.ConsumerRows(data.ConsumerChilds));
        this.InitIngredientTable(data.IngredientChilds || []);
        this.InitReferenceTable(data.ReferenceChilds || []);
        this.InitPackagingTables(data.PackagingChilds || []);
        this.InitProductTestTable(BriefTemplate2Helper.ProductTestRows(data.ProductTestChilds));
        this.ResizeChildTables();
    },

    DestroyChildTables: function () {
        $.each(this.ChildGridIds(), function (_, gridId) {
            var gridElement = $(gridId);
            if (gridElement.data("kendoGrid")) {
                gridElement.kendoGrid("destroy").empty();
            }
        });
    },

    ResizeChildTables: function () {
        $.each(this.ChildGridIds(), function (_, gridId) {
            var grid = $(gridId).data("kendoGrid");
            if (grid) grid.resize(true);
        });
    },

    ChildGridIds: function () {
        return [
            TblConsumerId,
            TblIngredientId,
            TblReferenceId,
            TblPrimaryPackagingId,
            TblSecondaryPackagingId,
            TblShipperPackagingId,
            TblProductTestId
        ];
    },

    InitMasterTable: function () {
        if ($(TblMasterId).data("kendoGrid")) $(TblMasterId).kendoGrid("destroy").empty();

        dataSource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 50,
            transport: {
                read: {
                    type: "POST",
                    url: "/api/BriefTemplate/list/" + statusCons,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8"
                },
                parameterMap: function (options) {
                    return JSON.stringify(options);
                }
            },
            schema: {
                data: "Items",
                total: "TotalCount"
            }
        });

        var columns = [
            {
                title: "Action",
                command: [
                    { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: BriefTemplate2Grid.HandleCommands },
                    { name: "BriefTemplate", text: "Print", iconClass: "k-icon k-i-print", className: "k-grid-button-rpt brief-print-command", click: BriefTemplate2Grid.HandleCommands }
                ],
                width: 180,
                ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            { field: "BTMasterID", title: "BTMasterID", hidden: true },
            {
                field: "ProjectName",
                title: "Project",
                filterable: true,
                template: "<span class='project-cell'>#= kendo.htmlEncode(ProjectName || '') #</span>",
                ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
            },
            {
                field: "BrandName",
                title: "Brand",
                width: 135,
                filterable: true,
                template: "#= kendo.htmlEncode(BrandName || '') #",
                ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
            },
            {
                field: "ProductName",
                title: "Product",
                filterable: true,
                template: "#= kendo.htmlEncode(ProductName || '') #",
                ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
            },
            {
                field: "ProductTypeName",
                title: "Type",
                width: 125,
                filterable: true,
                template: "<span class='muted-cell'>#= kendo.htmlEncode(ProductTypeName || '') #</span>",
                ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
            },
            {
                field: "CompletionDate",
                title: "Date",
                width: 105,
                template: "#= BriefTemplate2Grid.DateText(CompletionDate) #",
                ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "OrderQuantity",
                title: "Qty",
                width: 85,
                template: "#= BriefTemplate2Grid.NumberText(OrderQuantity, 'n0') #",
                ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-right", "k-text-right", true, true)
            },
            {
                field: "TargetPerQtyCost",
                title: "Cost",
                width: 90,
                template: "#= BriefTemplate2Grid.NumberText(TargetPerQtyCost, 'n2') #",
                ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-right", "k-text-right", true, true)
            },
            {
                field: "MaxBulkPrice",
                title: "Bulk",
                width: 90,
                template: "#= BriefTemplate2Grid.NumberText(MaxBulkPrice, 'n2') #",
                ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-right", "k-text-right", true, true)
            }
        ];

        var search = { fields: ["ProjectName", "BrandName", "ProductName", "ProductTypeName"] };
        var height = $(window).height() - 120;
        KendoGridHelper.MasterGridLoad(TblMasterId, height + "px", search, columns, true);
        $(TblMasterId).addClass("brief-master-grid");
    },

    InitConsumerTable: function (data) {
        this.InitGrid(TblConsumerId, data, [
            { command: [{ name: "delete", text: "", iconClass: "k-icon k-i-delete", className: "k-success", click: BriefTemplate2Grid.ChildCommandClick }], title: "Action", width: 50 },
            { field: "BTConsumerID", hidden: true },
            { field: "BTMasterID", hidden: true },
            { field: "GenderID", title: "Consumer", width: 180, editor: BriefTemplate2Grid.GenderEditor, template: "#= BriefTemplate2Grid.GenderText(GenderID) #" },
            { field: "MinAge", title: "Min Age", width: 100 },
            { field: "MaxAge", title: "Max Age", width: 100 }
        ], {
            BTConsumerID: { type: "number", defaultValue: 0 },
            BTMasterID: { type: "number", defaultValue: 0 },
            GenderID: { type: "number", defaultValue: 0 },
            MinAge: { type: "number", defaultValue: 0 },
            MaxAge: { type: "number", defaultValue: 0 },
            IsActive: { type: "number", defaultValue: 1 }
        });
    },

    InitIngredientTable: function (data) {
        this.InitGrid(TblIngredientId, data, [
            { command: [{ name: "delete", text: "", iconClass: "k-icon k-i-delete", className: "k-success", click: BriefTemplate2Grid.ChildCommandClick }], title: "Action", width: 50 },
            { field: "BTIngredientID", hidden: true },
            { field: "BTMasterID", hidden: true },
            { field: "IngredientRequirementID", title: "Type", width: 150, editor: BriefTemplate2Grid.RequirementTypeEditor, template: "#= BriefTemplate2Grid.RequirementTypeText(IngredientRequirementID) #" },
            { field: "IngredientID", title: "Ingredient", width: 220, editor: BriefTemplate2Grid.IngredientEditor, template: "#= BriefTemplate2Grid.IngredientText(IngredientID) #" },
            { field: "RequirmentLevel", title: "Level / %", width: 150 },
           
            { field: "Remarks", title: "Remarks", width: 260 }
        ], {
            BTIngredientID: { type: "number", defaultValue: 0 },
            BTMasterID: { type: "number", defaultValue: 0 },
            IngredientID: { type: "number", defaultValue: 0 },
            IngredientRequirementID: { type: "number", defaultValue: 0 },
            RequirmentLevel: { type: "string" },
            Remarks: { type: "string" },
            IsActive: { type: "number", defaultValue: 1 }
        });
    },

    InitReferenceTable: function (data) {
        this.InitGrid(TblReferenceId, data, [
            { command: [{ name: "delete", text: "", iconClass: "k-icon k-i-delete", className: "k-success", click: BriefTemplate2Grid.ChildCommandClick }], title: "Action", width: 50 },
            { field: "BTReferenceID", hidden: true },
            { field: "BTMasterID", hidden: true },
            { field: "ReferenceProductName", title: "Reference Product", width: 220 },
            { field: "ReferenceBrand", title: "Brand", width: 140 },
            { field: "ReferenceLink", title: "Link", width: 220, template: "#= BriefTemplate2Grid.ReferenceLinkTemplate(ReferenceLink) #" },
            { field: "Remarks", title: "Remarks", width: 260 }
        ], {
            BTReferenceID: { type: "number", defaultValue: 0 },
            BTMasterID: { type: "number", defaultValue: 0 },
            ReferenceProductName: { type: "string" },
            ReferenceBrand: { type: "string" },
            ReferenceLink: { type: "string" },
            Remarks: { type: "string" },
            IsActive: { type: "number", defaultValue: 1 }
        });
    },

    InitPackagingTables: function (data) {
        this.InitPrimaryPackagingTable(this.PackagingRows(data, packagingLevels.primary));
        this.InitSecondaryPackagingTable(this.PackagingRows(data, packagingLevels.secondary));
        this.InitShipperPackagingTable(this.PackagingRows(data, packagingLevels.shipper));
    },

    PackagingRows: function (rows, packagingLevelId) {
        return $.map(rows || [], function (item) {
            if (parseInt(item.PakagingLevelID) !== parseInt(packagingLevelId)) {
                return null;
            }

            return $.extend({}, item, {
                PakagingLevelID: packagingLevelId
            });
        });
    },

    InitPrimaryPackagingTable: function (data) {
        this.InitGrid(TblPrimaryPackagingId, data, [
            { title: "Action", width: 112, template: "#= BriefTemplate2Grid.PackagingActionTemplate(data) #" },
            { field: "BTPackagingID", hidden: true },
            { field: "BTMasterID", hidden: true },
            { field: "PakagingLevelID", hidden: true },
            { field: "PackagingTypeID", title: "Packaging Type", width: 180, editor: BriefTemplate2Grid.PackagingTypeEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.PackagingTypeList, PackagingTypeID) #" },
            { field: "PackagingStyle", title: "Packaging Style", width: 160 },
            { field: "PackageSize", title: "Size", width: 100 },
            { field: "UOMID", title: "UOM", width: 120, editor: BriefTemplate2Grid.UOMEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.UOMList, UOMID) #" },
            { field: "PackagingColorID", title: "Packaging Color", width: 140, editor: BriefTemplate2Grid.ColorEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.ColorIList, PackagingColorID) #" },
            { field: "PrintColorID", title: "Print Color", width: 140, editor: BriefTemplate2Grid.ColorEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.ColorIList, PrintColorID) #" },
            { field: "MaterialID", title: "Material", width: 160, editor: BriefTemplate2Grid.MaterialTypeEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.MaterialTypeList, MaterialID) #" },
            { field: "FinishID", title: "Finish", width: 140, editor: BriefTemplate2Grid.FinishTypeEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.FinishTypeList, FinishID) #" },
            { field: "LabelDescription", title: "Label", width: 160 },
            { field: "DesignDescription", title: "Design", width: 160, hidden: true },
            { field: "Certification", title: "Certification", width: 160 },
            { field: "Remarks", title: "Remarks", width: 160 },
        ], this.PackagingFields(packagingLevels.primary));
    },

    InitSecondaryPackagingTable: function (data) {
        this.InitGrid(TblSecondaryPackagingId, data, [
            { title: "Action", width: 112, template: "#= BriefTemplate2Grid.PackagingActionTemplate(data) #" },
            { field: "BTPackagingID", hidden: true },
            { field: "BTMasterID", hidden: true },
            { field: "PakagingLevelID", hidden: true },
            { field: "PackagingTypeID", title: "Packaging Type", width: 180, editor: BriefTemplate2Grid.PackagingTypeEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.PackagingTypeList, PackagingTypeID) #" },
            { field: "PackagingStyle", title: "Packaging Style", width: 160, hidden: true },
            { field: "PackageSize", title: "Size", width: 100 },
            { field: "UOMID", title: "UOM", width: 120, editor: BriefTemplate2Grid.UOMEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.UOMList, UOMID) #" },
            { field: "PackagingColorID", title: "Packaging Color", width: 140, editor: BriefTemplate2Grid.ColorEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.ColorIList, PackagingColorID) #", hidden: true },
            { field: "PrintColorID", title: "Print Color", width: 140, editor: BriefTemplate2Grid.ColorEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.ColorIList, PrintColorID) #" },
            { field: "MaterialID", title: "Material", width: 160, editor: BriefTemplate2Grid.MaterialTypeEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.MaterialTypeList, MaterialID) #" },
            { field: "FinishID", title: "Finish", width: 140, editor: BriefTemplate2Grid.FinishTypeEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.FinishTypeList, FinishID) #" },
            { field: "LabelDescription", title: "Label", width: 160, hidden: true },
            { field: "DesignDescription", title: "Design", width: 160 },
            { field: "Certification", title: "Certification", width: 160 },
            { field: "Remarks", title: "Remarks", width: 160 },
        ], this.PackagingFields(packagingLevels.secondary));
    },

    InitShipperPackagingTable: function (data) {
        this.InitGrid(TblShipperPackagingId, data, [
            { title: "Action", width: 112, template: "#= BriefTemplate2Grid.PackagingActionTemplate(data) #" },
            { field: "BTPackagingID", hidden: true },
            { field: "BTMasterID", hidden: true },
            { field: "PakagingLevelID", hidden: true },
            { field: "PackagingTypeID", title: "Packaging Type", width: 180, editor: BriefTemplate2Grid.PackagingTypeEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.PackagingTypeList, PackagingTypeID) #" },
            { field: "PackagingStyle", title: "Packaging Style", width: 160, hidden: true },
            { field: "PackageSize", title: "Size", width: 100 },
            { field: "UOMID", title: "UOM", width: 120, editor: BriefTemplate2Grid.UOMEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.UOMList, UOMID) #" },
            { field: "PackagingColorID", title: "Packaging Color", width: 140, editor: BriefTemplate2Grid.ColorEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.ColorIList, PackagingColorID) #", hidden: true },
            { field: "PrintColorID", title: "Print Color", width: 140, editor: BriefTemplate2Grid.ColorEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.ColorIList, PrintColorID) #" },
            { field: "MaterialID", title: "Material", width: 160, editor: BriefTemplate2Grid.MaterialTypeEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.MaterialTypeList, MaterialID) #", hidden: true },
            { field: "FinishID", title: "Finish", width: 140, editor: BriefTemplate2Grid.FinishTypeEditor, template: "#= BriefTemplate2Grid.LookupText(masterData.FinishTypeList, FinishID) #", hidden: true },
            { field: "LabelDescription", title: "Label", width: 160, hidden: true },
            { field: "DesignDescription", title: "Design", width: 160 },
            { field: "Certification", title: "Certification", width: 160 },
            { field: "Remarks", title: "Remarks", width: 160 },
        ], this.PackagingFields(packagingLevels.shipper));
    },

    PackagingFields: function (packagingLevelId) {
        return {
            BTPackagingID: { type: "number", defaultValue: 0 },
            BTMasterID: { type: "number", defaultValue: 0 },
            PakagingLevelID: { type: "number", defaultValue: packagingLevelId },
            PackagingTypeID: { type: "number", defaultValue: 0 },
            PackagingStyle: { type: "string" },
            PackageSize: { type: "number", defaultValue: 0 },
            UOMID: { type: "number", defaultValue: 0 },
            MaterialID: { type: "number", defaultValue: 0 },
            FinishID: { type: "number", defaultValue: 0 },
            PackagingColorID: { type: "number", defaultValue: 0 },
            PrintColorID: { type: "number", defaultValue: 0 },
            LabelDescription: { type: "string" },
            DesignDescription: { type: "string" },
            Certification: { type: "string" },
            Remarks: { type: "string" },
            FilePath: { type: "string" },
            FileName: { type: "string" },
            RowSlNo: { type: "number", defaultValue: 0 },
            IsDeleted: { type: "number", defaultValue: 0 }
        };
    },

    InitProductTestTable: function (data) {
        this.InitGrid(TblProductTestId, data, [
            { field: "BTProductTestID", hidden: true },
            { field: "BTMasterID", hidden: true },
            { field: "ProductTestID", title: "Test ID", width: 80 },
            { field: "ProductTestID", title: "Product Test", width: 280, template: "#= BriefTemplate2Grid.ProductTestText(ProductTestID, ProductTestName) #" },
            { field: "IsTested", title: "Required?", width: 110 },
            { field: "Remarks", title: "Remarks", width: 280 }
        ], {
            BTProductTestID: { type: "number", defaultValue: 0 },
            BTMasterID: { type: "number", defaultValue: 0 },
            ProductTestID: { type: "number", defaultValue: 0 },
            ProductTestName: { type: "string" },
            IsTested: { type: "boolean", defaultValue: false },
            Remarks: { type: "string" },
            IsActive: { type: "number", defaultValue: 1 }
        });
    },

    InitGrid: function (gridId, data, columns, fields) {
        if ($(gridId).data("kendoGrid")) $(gridId).kendoGrid("destroy").empty();
        KendoGridHelper.ChildGridLoad(gridId, columns, data, {
            model: {
                fields: fields
            }
        }, true, true, false);
        KendoGridHelper.EnterKeyUp(gridId);
    },

    GenderEditor: function (container, options) {
        $('<input data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoComboBox({
                dataTextField: "text",
                dataValueField: "id",
                filter: "contains",
                suggest: true,
                dataSource: masterData.GenderList || [],
                valuePrimitive: true
            });
    },

    GenderText: function (genderId) {
        var item = $.grep(masterData.GenderList || [], function (row) {
            return parseInt(row.id) === parseInt(genderId);
        })[0];
        return item ? item.text : "";
    },

    IngredientEditor: function (container, options) {
        $('<input data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoComboBox({
                dataTextField: "text",
                dataValueField: "id",
                filter: "contains",
                suggest: true,
                dataSource: masterData.IngredientList || [],
                valuePrimitive: true
            });
    },

    IngredientText: function (ingredientId) {
        var item = $.grep(masterData.IngredientList || [], function (row) {
            return parseInt(row.id) === parseInt(ingredientId);
        })[0];
        return item ? item.text : "";
    },

    RequirementTypeEditor: function (container, options) {
        $('<input data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                dataTextField: "text",
                dataValueField: "id",
                optionLabel: "Select",
                dataSource: requirementTypes,
                valuePrimitive: true
            });
    },

    RequirementTypeText: function (typeId) {
        var item = $.grep(requirementTypes, function (row) {
            return parseInt(row.id) === parseInt(typeId);
        })[0];
        return item ? item.text : "";
    },

    PackagingLevelEditor: function (container, options) {
        BriefTemplate2Grid.DropDownEditor(container, options, masterData.PakagingLevelList || []);
    },

    PackagingTypeEditor: function (container, options) {
        BriefTemplate2Grid.DropDownEditor(container, options, masterData.PackagingTypeList || []);
    },

    UOMEditor: function (container, options) {
        BriefTemplate2Grid.DropDownEditor(container, options, masterData.UOMList || []);
    },

    MaterialTypeEditor: function (container, options) {
        BriefTemplate2Grid.DropDownEditor(container, options, masterData.MaterialTypeList || []);
    },

    FinishTypeEditor: function (container, options) {
        BriefTemplate2Grid.DropDownEditor(container, options, masterData.FinishTypeList || []);
    },

    ColorEditor: function (container, options) {
        BriefTemplate2Grid.DropDownEditor(container, options, masterData.ColorIList || []);
    },

    DropDownEditor: function (container, options, data) {
        $('<input data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoComboBox({
                dataTextField: "text",
                dataValueField: "id",
                filter: "contains",
                suggest: true,
                dataSource: data,
                valuePrimitive: true
            });
    },

    LookupText: function (list, id) {
        var item = $.grep(list || [], function (row) {
            return parseInt(row.id) === parseInt(id);
        })[0];
        return item ? item.text : "";
    },

    PackagingActionTemplate: function (dataItem) {
        var savedPath = $.trim((dataItem && dataItem.FilePath) || "");
        var selectedFileName = $.trim((dataItem && dataItem.FileName) || "");
        var downloadDisabled = savedPath ? "" : " disabled";
        var downloadTitle = savedPath ? "Download file" : "No file";
        var fileNameTitle = selectedFileName || (savedPath ? savedPath.split(/[\\/]/).pop() : "");

        return '<div class="btn-group btn-group-sm brief-packaging-actions" title="' + kendo.htmlEncode(fileNameTitle) + '">' +
            '<button type="button" class="btn btn-sm btn-secondary" title="Delete row" onclick="BriefTemplate2Grid.DeletePackagingRow(event, this)">' +
            '<span class="k-icon k-i-delete"></span>' +
            '</button>' +
            '<button type="button" class="btn btn-sm btn-secondary" title="Upload file" onclick="BriefTemplate2Grid.OpenPackagingFile(event, this)">' +
            '<span class="k-icon k-i-upload"></span>' +
            '</button>' +
            '<button type="button" class="btn btn-sm btn-secondary" title="' + downloadTitle + '"' + downloadDisabled + ' onclick="BriefTemplate2Grid.DownloadPackagingFile(event, this)">' +
            '<span class="k-icon k-i-download"></span>' +
            '</button>' +
            '<input type="file" class="d-none brief-packaging-file-input" onclick="event.stopPropagation()" onchange="BriefTemplate2Grid.PackagingFileChanged(event, this)" />' +
            '</div>';
    },

    EncodeJsString: function (value) {
        return String(value || "")
            .replace(/\\/g, "\\\\")
            .replace(/'/g, "\\'")
            .replace(/\r/g, "")
            .replace(/\n/g, "");
    },

    PackagingGridFromElement: function (element) {
        return $(element).closest(".k-grid").data("kendoGrid");
    },

    PackagingDataItemFromElement: function (element) {
        var grid = BriefTemplate2Grid.PackagingGridFromElement(element);
        if (!grid) return null;
        return grid.dataItem($(element).closest("tr"));
    },

    OpenPackagingFile: function (event, button) {
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }
        $(button).siblings(".brief-packaging-file-input").trigger("click");
    },

    PackagingFileChanged: function (event, input) {
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }
        var file = input.files && input.files.length > 0 ? input.files[0] : null;
        var dataItem = BriefTemplate2Grid.PackagingDataItemFromElement(input);
        var grid = BriefTemplate2Grid.PackagingGridFromElement(input);
        if (!file || !dataItem) return;

        dataItem.PackagingFile = file;
        dataItem.set("FileName", file.name);

        if (grid) {
            grid.refresh();
        }
    },

    DownloadPackagingFile: function (event, button) {
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }
        var dataItem = BriefTemplate2Grid.PackagingDataItemFromElement(button);
        FileUploadHelper.DownloadFile(dataItem ? dataItem.FilePath : "");
    },

    DeletePackagingRow: function (event, button) {
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        var grid = BriefTemplate2Grid.PackagingGridFromElement(button);
        var dataItem = BriefTemplate2Grid.PackagingDataItemFromElement(button);
        if (!grid || !dataItem) return;

        Swal.fire(
            SwalHelper.SwalConfirmationConfig('Are you sure want to DELETE this record.?')
        ).then((result) => {
            if (result.isConfirmed) {
                grid.dataSource.remove(dataItem);
            }
        });
    },

    DateText: function (value) {
        var dateValue = kendo.parseDate(value);
        if (!dateValue || dateValue.getFullYear() <= 1900) return "";
        return kendo.toString(dateValue, "dd-MMM-yyyy");
    },

    NumberText: function (value, format) {
        var numberValue = kendo.parseFloat(value);
        if (numberValue == null || isNaN(numberValue)) return "";
        return kendo.toString(numberValue, format || "n2");
    },

    ProductTestText: function (testId, testName) {
        if ($.trim(testName || "")) return testName;

        var item = $.grep(defaultProductTests, function (row) {
            return parseInt(row.ProductTestID) === parseInt(testId);
        })[0];

        return item ? item.ProductTestName : "";
    },

    ReferenceLinkTemplate: function (referenceLink) {
        var link = $.trim(referenceLink || "");
        if (!link) return "";

        var displayText = link;
        if (!/^https?:\/\//i.test(link)) {
            link = "https://" + link;
        }

        return '<a href="' + kendo.htmlEncode(link) + '" target="_blank" rel="noopener noreferrer">' + kendo.htmlEncode(displayText) + '</a>';
    },

    GridData: function (gridId) {
        var grid = $(gridId).data("kendoGrid");
        if (!grid) return [];

        return $.map(grid.dataSource.data(), function (dataItem, index) {
            var item = dataItem.toJSON ? dataItem.toJSON() : $.extend({}, dataItem);
            item.BTMasterID = parseInt($("#BTMasterID").val()) || 0;
            item.RowSLNo = index + 1;
            item.RowSlNo = index + 1;
            item.IsDeleted = item.IsDeleted || 0;
            return item;
        });
    },

    PackagingGridData: function () {
        var rows = []
            .concat(this.GridDataWithPackagingLevel(TblPrimaryPackagingId, packagingLevels.primary))
            .concat(this.GridDataWithPackagingLevel(TblSecondaryPackagingId, packagingLevels.secondary))
            .concat(this.GridDataWithPackagingLevel(TblShipperPackagingId, packagingLevels.shipper));

        return $.map(rows, function (item, index) {
            item.RowSLNo = index + 1;
            item.RowSlNo = index + 1;
            return item;
        });
    },

    PackagingFileRows: function () {
        var rows = []
            .concat(this.GridFileRowsWithPackagingLevel(TblPrimaryPackagingId, packagingLevels.primary))
            .concat(this.GridFileRowsWithPackagingLevel(TblSecondaryPackagingId, packagingLevels.secondary))
            .concat(this.GridFileRowsWithPackagingLevel(TblShipperPackagingId, packagingLevels.shipper));

        return $.map(rows, function (item, index) {
            item.RowSLNo = index + 1;
            item.RowSlNo = index + 1;
            return item.PackagingFile ? item : null;
        });
    },

    GridFileRowsWithPackagingLevel: function (gridId, packagingLevelId) {
        var grid = $(gridId).data("kendoGrid");
        if (!grid) return [];

        return $.map(grid.dataSource.data(), function (dataItem, index) {
            return {
                PackagingFile: dataItem.PackagingFile,
                FileName: dataItem.FileName,
                FilePath: dataItem.FilePath,
                PakagingLevelID: packagingLevelId,
                RowSLNo: index + 1,
                RowSlNo: index + 1
            };
        });
    },

    GridDataWithPackagingLevel: function (gridId, packagingLevelId) {
        return $.map(this.GridData(gridId), function (item) {
            item.PakagingLevelID = packagingLevelId;
            return item;
        });
    },

    UploadPackagingFiles: function (btMasterId) {
        var rows = this.PackagingFileRows();
        if (rows.length === 0) {
            return $.Deferred().resolve().promise();
        }

        var fileDataArray = $.map(rows, function (item) {
            var rowNo = parseInt(item.RowSlNo) || parseInt(item.RowSLNo) || 0;
            var levelId = parseInt(item.PakagingLevelID) || 0;
            return {
                Name: item.FileName,
                File: item.PackagingFile,
                Usql: " Update Mkt_BriefTemplatePackaging Set FilePath = ",
                Condition: " Where BTMasterID = " + parseInt(btMasterId) + " And PakagingLevelID = " + levelId + " And RowSlNo = " + rowNo
            };
        });

        FormData_File = new FormData();
        FileDataArray = [];
        FileUploadHelper.SetUploadedFileArray(
            fileDataArray,
            "\\Files\\Marketing\\BriefTemplate\\Packaging\\",
            btMasterId,
            "BTP_",
            "",
            "",
            0,
            "",
            false,
            false,
            ""
        );

        return $.ajax({
            method: "POST",
            processData: false,
            contentType: false,
            cache: false,
            data: FormData_File,
            enctype: "multipart/form-data",
            url: "/api/helperC/FileUpload"
        });
    },

    HandleCommands: function (args) {
        args.preventDefault();
        var selectedItem = this.dataItem($(args.currentTarget).closest("tr"));
        if (selectedItem) {
            if (args.data.commandName == "BriefTemplate") {
                BriefTemplate2Manager.GetReport(selectedItem.BTMasterID);
                return;
            }

            BriefTemplate2Manager.GetDetails(selectedItem.BTMasterID);
        }
    },

    ChildCommandClick: function (args) {
        args.preventDefault();
        var grid = this;
        var selectedItem = grid.dataItem($(args.currentTarget).closest("tr"));
        if (selectedItem) {
            grid.dataSource.remove(selectedItem);
        }
    }
};
