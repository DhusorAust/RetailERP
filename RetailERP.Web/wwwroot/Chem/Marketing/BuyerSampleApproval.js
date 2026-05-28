/// <reference path="../../common/js/utilitycommon.js" />  

$.ajaxSetup({
    async: false
});

var statusCons;
var masterData;
var menuId, pageName;
var ToolbarId;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom;
var childsActionBtn = true;
var childsToolsBar = false;
var savedFilter = null;
var IsApproved = false, IsRevise = false, IsReject = false;

$(document).ready(function () {
    Helper.Initialize();
});

var Manager = {
    GetNew: function (id) {
        $.get(`/api/fback/new?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();

            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable([]);

            Helper.GetRnDBy(masterData.RnDByList);
            //KendoDropDownHelper.MultiSelectCombo(masterData.ClaimList, "SDRFClaimID", "text", "id", "", "", false);
            //KendoDropDownHelper.MultiSelectCombo(masterData.FeelList, "SDRFFeelID", "text", "id", "", "", false);
            //KendoDropDownHelper.MultiSelectCombo(masterData.FragranceList, "SDRFFragranceID", "text", "id", "", "", false);
            //KendoDropDownHelper.MultiSelectCombo(masterData.RestrictionList, "SDRFRestrictionID", "text", "id", "", "", false);

            ////New Code
            //masterData.SDRFBy = 485;
            //$("#SDRFNo").val("SDRF-2025-11-000001");
            //$("#SDRFBy").data("kendoMultiColumnComboBox").value(487);
            //$("#PriorityID").data("kendoComboBox").value(3);
            //$("#FeedbackNo").val("<<--System Value-->>")


        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id) {
        $.get(`/api/fback/edit?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();

            masterData = data;
            Helper.GetPRBy(masterData.PRByList);
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs);
            $("#btnPrintPR").show();
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    Save: function (IsApproved, IsRevise, IsReject) {
        if (Helper.IsFrmValid()) {
            var model = FormHelper.FormElToJson(FormDom);
            model.Childs = JSON.parse(JSON.stringify($(TblChildId).data("kendoGrid").dataSource.data()));
            model.Approved = IsApproved;
            model.IsRevise = IsRevise;
            model.Reject = IsReject;

            var revReason = $("#ReviseReason").val();
            var rejReason = $("#RejectReason").val();

            if (IsRevise == true && revReason == "") {
                MessageHelper.Warning("Please Input Revise Reason.");
                return;
            }
            if (IsReject == true && rejReason == "") {
                MessageHelper.Warning("Please Input Reject Reason.");
                return;
            }

            //Child Validation
            if (!Helper.IsFrmChildsValid(model.Childs)) {
                return;
            }
            $.post("/api/fback/save", model, function (data) {
                if (parseInt(data.ResultId) > 0) {
                    MessageHelper.Success("Save Operations", data.Message);
                    Helper.BackToList();
                } else {
                    if (data.IsLogin == 1) {
                        window.open(data.redirectUrl, "_blank");
                    } else {
                        MessageHelper.Warning("Save Operations", data.Message);
                    }
                }
            });
        }
    },
    RptPR: function (id) {
        window.open(`/RptCommercialExport/fback?id=${id}`, "_blank");
    },
}

var Helper = {
    BackToList: function () {
        DivDetailsDom.fadeOut();
        Helper.ResetForm();
        DivToolbarDom.fadeIn();
        GridManager.InitMasterTable();
    },
    ResetForm: function () {
        FormDom.trigger("reset");
        $('input[type="hidden"]').val("0");
    },
    IsFrmChildsValid: function (ChildsList) {
        var isValidItem = true;
        //Quantity Check
        $.each(ChildsList, function (i, obj) {
            if (obj.objProductList.id == 0 || obj.objProductList.text == null) {
                MessageHelper.Warning("Warning", "Items is required.");
                isValidItem = false;
            }
        });

        //Childs Length Check
        if (ChildsList.length === 0) {
            MessageHelper.Warning("Warning", "At least 1 Items is required.");
            return isValidItem = false;
        }
        return isValidItem;
    },
    IsFrmValid: function () {
        var isValidItem = true;
        var Elements = ["UnitID", "StoreID", "PRDate", "SDRFBy"];
        if (!FormHelper.IsFormValidation(FormDom, Elements)) {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
        }
        return isValidItem;
    },
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

        //Development Code 
        ToolbarDom.find("#btnPendingList,#btnEditList,#btnApprovedList,#btnRejectList,#btnAllList").show();
        $("#divReviseNote").hide();
        $("#divRejectNote").hide();

        //Temp 
        statusCons = statusConstants.PENDING;
        FormHelper.TogToolbarBtn(ToolbarDom.find("#btnPendingList"), ToolbarDom);
        GridManager.InitMasterTable();

        // ToolbarDom means Toolbar Buttons  
        ToolbarDom.find("#btnPendingList").on("click", function (e) {
            statusCons = statusConstants.PENDING;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnPendingList"), ToolbarDom);
            GridManager.InitMasterTable();
        });
        ToolbarDom.find("#btnEditList").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
        });
        ToolbarDom.find("#btnApprovedList").on("click", function (e) {
            statusCons = statusConstants.APPROVED;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
        });
        ToolbarDom.find("#btnRejectList").on("click", function (e) {
            statusCons = statusConstants.REJECT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
        });
        ToolbarDom.find("#btnAllList").on("click", function (e) {
            statusCons = statusConstants.ALL;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
        });

        //Button
        $("#btnSave").click(function (e) {
            IsApproved = false, IsRevise = false, IsReject = false;
            Manager.Save(IsApproved, IsRevise, IsReject);
        });
        $("#btnApproved").click(function (e) {
            IsApproved = true, IsRevise = false, IsReject = false;
            Manager.Save(IsApproved, IsRevise, IsReject);
        });
        $("#btnRevise").click(function (e) {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Revise this Requisition?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsApproved = false, IsRevise = true, IsReject = false;
                    Manager.Save(IsApproved, IsRevise, IsReject);
                }
            })
        });

        $("#btnReject").click(function (e) {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Reject this Requisition?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsApproved = false, IsRevise = false, IsReject = true;
                    Manager.Save(IsApproved, IsRevise, IsReject);
                }
            })
        });

        $("#btnBackToList").on("click", function () {
            Helper.BackToList();
        });

        //Selected Row in the kendo Grid
        KendoGridHelper.GridRowSelected(TblChildId);
    },
    GetRnDBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" },
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#RnDBy", "text", "id", columns);
    },
}

var GridManager = {
    InitMasterTable: function () {
        if ($(TblMasterId).data("kendoGrid")) $(TblMasterId).kendoGrid('destroy').empty();
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
                    url: `/api/fback/list/${statusCons}`,
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

                },
            }
        });

        var command;
        var commandWidth = 80;
        var columns = [];

        if (statusCons == statusConstants.PENDING) {
            command =
                [
                    { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                ];
        }
        else if (statusCons == statusConstants.EDIT || statusCons == statusConstants.REJECT) {
            command =
                [
                    { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                ];
        } else {
            command =
                [
                    { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                ];
        }

        if (statusCons == statusConstants.PENDING) {
            columns = [
                {
                    title: "Action", command: command, width: 60
                },
                {
                    field: "SDRFMasterID", title: "SDRFMasterID", hidden: true,
                },
                {
                    field: "BuyerRefMasterID", title: "BuyerRefMasterID", hidden: true,
                },
                {
                    field: "BuyerRefColorID", title: "BuyerRefColorID", hidden: true,
                },
                {
                    field: "ChemicalCode", title: "Chemical Code", width: 160, attributes: { "style": "font-weight:700" }
                },
                {
                    field: "SDRFNo", title: "SDRF No", width: 160, attributes: { "style": "font-weight:700" }
                },
                {
                    field: "SDRFDate", title: "SDRF Date", width: 110,
                    template: "#= (SDRFDate == null)? '' : kendo.toString(kendo.parseDate(SDRFDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                },
                {
                    field: "RequiredDate", title: "Required</br>Date", width: 110,
                    template: "#= (RequiredDate == null)? '' : kendo.toString(kendo.parseDate(RequiredDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                },
                {
                    field: "BuyerName", title: "Buyer</brName", width: 100,
                },
                {
                    field: "BuyerRef", title: "Buyer</br>Ref", width: 120,
                },
                {
                    field: "ColorName", title: "Color</brName", width: 100,
                },
                {
                    field: "SDRFByName", title: "SDRF By", width: 100,
                },
                {
                    field: "DevelopmentType", title: "Development</br>Type", width: 130,
                },
                {
                    field: "MktTeamName", title: "Mkt. Team", width: 110,
                },
                {
                    field: "RnDTeamName", title: "R & D Team", width: 120,
                },
                {
                    field: "PriorityName", title: "Priority", width: 90,
                },
                {
                    field: "ClaimName", title: "Claim", width: 120,
                },
                {
                    field: "FeelName", title: "Feel", width: 120,
                },
                {
                    field: "FragranceName", title: "Fragrance", width: 120,
                },
                {
                    field: "RestrictionName", title: "Restriction", width: 120,
                },
            ];

            search =
            {
                fields:
                    [
                        "ChemicalCode", "SDRFNo", "SDRFDate", "RequiredDate", "BuyerName", "BuyerRef", "ColorName", "SDRFByName",
                        "DevelopmentType", "MktTeamName", "RnDTeamName", "PriorityName", "ClaimName", "FeelName", "FragranceName",
                        "RestrictionName"
                    ]
            };
        } else {
            columns = [
                {
                    title: "Action", command: command, width: 100
                },
                {
                    field: "FeedbackMasterID", title: "FeedbackMasterID", hidden: true,
                },
                {
                    field: "BuyerRefMasterID", title: "BuyerRefMasterID", hidden: true,
                },

                {
                    field: "FeedbackNo", title: "FeedbackNo", width: 120,
                },
                {
                    field: "FeedbackDate", title: "Feedback Date", width: 70,
                    template: "#= (FeedbackDate == null)? '' : kendo.toString(kendo.parseDate(FeedbackDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "ChemicalCode", title: "Chemical Code", width: 120,
                },
                {
                    field: "BuyerRef", title: "Buyer Ref", width: 100,
                },
                {
                    field: "ProductName", title: "Product Name", width: 100,
                },
                {
                    field: "BuyerName", title: "Buyer", width: 100,
                },
                {
                    field: "RnDByName", title: "R&D By Name", width: 100,
                },
                {
                    field: "DevelopmentType", title: "Development Type", width: 100,
                },
            ];

            search =
            {
                fields:
                    [
                        "FeedbackNo", "FeedbackDate", "ChemicalCode", "BuyerRef", "ProductName", "BuyerName", "RnDByName", "DevelopmentType"
                    ]
            };
        }

        var height = $(window).height() - 120;
        KendoGridHelper.MasterGridLoad(TblMasterId, height + "px", search, columns, true)
    },
    InitChildTable: function (data) {
        if ($(TblChildId).data("kendoGrid")) $(TblChildId).kendoGrid('destroy').empty();
        dataSource = data;
        var columns =
            [
                {
                    command: [{
                        name: "delete1", text: "", iconClass: "k-icon k-i-delete", className: "k-success",
                        click: GridHelper.ChildCommandClick,
                    }], title: "Action &nbsp;", width: 70,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "FeedbackMasterID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "FeedbackChildID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "SDRFChildID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "SDRFMasterID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "objVariantList", title: "Variant", width: 80,
                    editor: GridHelper.cboVariant,
                    template: "#=objVariantList != null ? objVariantList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objFormulationMasterList", title: "Formulation", width: 150,
                    editor: GridHelper.cboFormulation,
                    template: "#=objFormulationMasterList != null ? objFormulationMasterList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objPackagingTypeList", title: "Packaging</br>Type", width: 100,
                    editor: GridHelper.cboPackagingType,
                    template: "#=objPackagingTypeList != null ? objPackagingTypeList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "SizeGram", title: "Size</br>Gram", width: 80,
                    footerTemplate: "<span id='sumSizeGram'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "objUOMList", title: "UOM", width: 90,
                    editor: GridHelper.cboUOM,
                    template: "#=objUOMList != null ? objUOMList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "BatchNo", title: "Batch No", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "objClaimList", title: "Claim", width: 140,
                    editor: GridHelper.cboClaim,
                    template: "#=objClaimList != null ? objClaimList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objFeelList", title: "Feel", width: 140,
                    editor: GridHelper.cboFeel,
                    template: "#=objFeelList != null ? objFeelList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objFragranceList", title: "Fragrance", width: 140,
                    editor: GridHelper.cboFragrance,
                    template: "#=objFragranceList != null ? objFragranceList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objRestrictionList", title: "Restriction", width: 140,
                    editor: GridHelper.cboRestriction,
                    template: "#=objRestrictionList != null ? objRestrictionList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "Remarks", title: "Remarks", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "BuyerApproved", title: "Is Approve?", width: 100,
                    template: "<input class='customClass' #if (BuyerApproved) { # checked='checked' # } # type='checkbox' />"
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-center", true, true)
                },
                {
                    field: "BuyerRejectReason", title: "Buyer</br>Reject</br>Reason", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
            ];

        var schema = {
            model: {
                fields: {
                    FeedbackMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    FeedbackChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    SDRFChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    SDRFMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },

                    objVariantList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objFormulationMasterList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objPackagingTypeList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    SizeGram: { type: "number", editable: function () { return false; } },
                    objUOMList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    BatchNo: { type: "string" },
                    objClaimList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objFeelList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objFragranceList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objRestrictionList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    Remarks: { type: "string" },
                    BuyerApproved: { editable: false },
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId, columns, data, schema, true, true, false);
        GridHelper.InitializeHelper();
        KendoGridHelper.EnterKeyUp(TblChildId);
    },
}
var GridHelper = {
    // Actions
    HandleCommands: function (args) {
        args.preventDefault();
        var tr = $(args.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);

        if (selectedItem != null) {
            $("#btnSave,#btnApproved,#btnRevise,#btnReject").hide();
            $("#divReviseNote,#divRejectNote").hide();

            childsActionBtn = true;
            childsToolsBar = false;

            if (args.data.commandName == "rptPR") {
                Manager.RptPR(selectedItem.PRMasterID);
                return;
            }

            if (statusCons == statusConstants.PENDING) {
                $("#btnSave,#btnApproved").show();
                childsActionBtn = false;
                childsToolsBar = true;
                Manager.GetNew(selectedItem.SDRFMasterID);
                return;
            } else {
                if (statusCons == statusConstants.EDIT) {
                    $("#btnSave,#btnApproved").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                }
                else if (statusCons == statusConstants.APPROVED) {
                    $("#btnRevise,#btnReject").show();
                    $("#divReviseNote,#divRejectNote").show();
                }
                else if (statusCons == statusConstants.REJECT) {
                    $("#btnSave,#btnApproved").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                    $("#divRejectNote").show();
                }

                Manager.GetDetails(selectedItem.FeedbackMasterID);
            }
        }
    },
    ChildCommandClick(args) {
        if (args.data.commandName == "delete1") {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to DELETE this record.?')
            ).then((result) => {
                if (result.isConfirmed) {
                    var grid = $(TblChildId).data("kendoGrid");
                    var tr = $(args.currentTarget).closest("tr");
                    var selectedItem = this.dataItem(tr);
                    grid.select(tr);
                    if (selectedItem != null) {
                        grid.dataSource.remove(selectedItem);
                    }
                }
            })
        }
    },

    InitializeHelper: function () {
        //Grid Function
        TblChilds = $(TblChildId).data("kendoGrid");
        TblCDataSource = TblChilds.dataSource.data();
        GridHelper.CalTotal();
    },
    CalTotal: function () {
        //Get Total Qty
        TblChilds = $(TblChildId).data("kendoGrid");

        TblChilds.dataSource.bind("change", function (e) {
            if (e.action === "itemchange") {
                var selectedItem = TblChilds.dataItem(TblChilds.select());
                var obj = e.items[0];

                if (e.field == "objPackagingTypeList") {
                    selectedItem.set('UOM', obj.objPackagingTypeList.valueB);
                    selectedItem.set('UOMID', obj.objPackagingTypeList.valueA);
                }
                if (e.field === "PRQty") {
                    GridHelper.GetSumPRQty();
                }
            }
            GridHelper.GetSumPRQty();
        });
        GridHelper.GetSumPRQty();
    },
    GetSumPRQty: function () {
        TblChilds = $(TblChildId).data("kendoGrid");
        TblCDataSource = TblChilds.dataSource.data();
        var gcPRQty = 0;

        for (var i = 0; i < TblCDataSource.length; i++) {
            gcPRQty += kendo.parseFloat(TblCDataSource[i].PRQty);
        }
        $("#sumPRQty").text(gcPRQty.toFixed(2));
    },

    cboVariant: function (container, options) {
        $('<input id="cmbVariant" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.VariantList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboFormulation: function (container, options) {
        $('<input id="cmbFormulation" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.FormulationMasterList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboPackagingType: function (container, options) {
        $('<input id="cmbPackagingType" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.PackagingTypeList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboUOM: function (container, options) {
        $('<input id="cmbUOM" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.UOMList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboClaim: function (container, options) {
        $('<input id="cmbClaim" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.ClaimList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboFeel: function (container, options) {
        $('<input id="cmbFeel" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.FeelList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboFragrance: function (container, options) {
        $('<input id="cmbFragrance" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.FragranceList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboRestriction: function (container, options) {
        $('<input id="cmbRestriction" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.RestrictionList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    GridCenterAlignAttributes: function (align_header = "", align_body = "", align_footer = "") {
        return {
            headerAttributes: { "class": align_header + " k_input_cell" },
            attributes: { "class": align_body + " k_input_cell" },
            footerAttributes: { "class": align_footer + " k_input_cell" }
        };
    },
}


