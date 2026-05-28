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
var IsSendForApproval = false, IsApproved = false, IsRevise = false, IsReject = false;
var isApprovePage = false;
var BuyerRefColorList = [];

$(document).ready(function () {
    Helper.Initialize();
});

var Manager = {
    GetNew: function () {
        $.get(`/api/poentry/new`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs);
            Helper.GePONo(masterData.POMasterList);
            Helper.GetPOBy(masterData.POByList);
            //BuyerRefColorList = masterData.BuyerRefColorList;
            Helper.GetColors(masterData.BuyerRefColorList);
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id) {
        $.get(`/api/poentry/edit?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();

            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);
            //GridManager.InitChildTable([]);
            GridManager.InitChildTable(masterData.Childs);
            Helper.GePONo(masterData.POMasterList);
            Helper.GetPOBy(masterData.POByList);
            //$("#divReviseNote,#divRejectNote").hide()
            //$("#btnPrintPR").show(); 
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    Save: function (IsSendForApproval, IsApproved, IsRevise, IsReject, SaveOption) {
        if (Helper.IsFrmValid()) {
            var model = FormHelper.FormElToJson(FormDom);
            model.Childs = JSON.parse(JSON.stringify($(TblChildId).data("kendoGrid").dataSource.data()));
            model.SendForApproval = IsSendForApproval;
            model.Approved = IsApproved;
            model.Revise = IsRevise;
            model.Reject = IsReject;
            model.SaveOption = SaveOption;
                    
            var a = $("#BuyerRefMasterID").val() || 0;
            var b = $("#BuyerRefColorID").val() || 0;

            model.BuyerRefMasterID_RPT = a;
            model.BuyerRefColorID_RPT = b;
            //Child Validation
            if (!Helper.IsFrmChildsValid(model.Childs)) {
                return;
            }
            $.post("/api/poentry/save", model, function (data) {
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
        window.open(`/RptCommercialExport/poentry?id=${id}`, "_blank");
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
        var Elements = ["OrderModeID", "PONo", "PODate", "CurrencyID", "PaymentTermsID", "IncotermID", "POBy"];
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
        isApprovePage = FormHelper.ConvertToBoolean($(`#${pageId}`).find("#ApprovePage").val());
        $("#divReviseNote,#divRejectNote").hide();
        if (isApprovePage) {
            ToolbarDom.find("#btnPendingList,#btnApprovedList,#btnRejectList,#btnAllList").show();
            //$("#divReviseNote").hide();
            //$("#divRejectNote").hide();
            ToolbarDom.find("#btnEditList").hide();
            statusCons = statusConstants.PROPOSED_FOR_APPROVAL;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnPendingList"), ToolbarDom);
            GridManager.InitMasterTable();
        } else {
            ToolbarDom.find("#btnNewList,#btnEditList,#btnPendingList,#btnApprovedList,#btnRejectList,#btnAllList").show();
            $("#btnSaveForApprove").show();

            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnEditList"), ToolbarDom);
            GridManager.InitMasterTable();
        }
        // ToolbarDom means Toolbar Buttons  
        ToolbarDom.find("#btnNewList").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnEditList"), ToolbarDom);
            Manager.GetNew();
            $("#divReviseNote,#divRejectNote").hide()

        });
        ToolbarDom.find("#btnPendingList").on("click", function (e) {
            statusCons = statusConstants.PROPOSED_FOR_APPROVAL;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
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
            var SaveOption = Helper.GetSaveOption("btnSave", isApprovePage);
            IsSendForApproval = false, IsApproved = false, IsRevise = false, IsReject = false;
            Manager.Save(IsSendForApproval, IsApproved, IsRevise, IsReject, SaveOption);
        });

        $("#btnSaveForApprove").click(function (e) {
            var SaveOption = Helper.GetSaveOption("btnSaveForApprove", isApprovePage);
            IsSendForApproval = true, IsApproved = false, IsRevise = false, IsReject = false;
            Manager.Save(IsSendForApproval, IsApproved, IsRevise, IsReject, SaveOption);
        });

        $("#btnApproved").click(function (e) {
            var SaveOption = Helper.GetSaveOption("btnApproved", isApprovePage);
            IsSendForApproval = false, IsApproved = true, IsRevise = false, IsReject = false;
            Manager.Save(IsSendForApproval, IsApproved, IsRevise, IsReject, SaveOption);
        });
        $("#btnRevise").click(function (e) {
            var revReason = $("#ReviseReason").val();
            if (revReason == "") {
                ToastrMessageHelper.ValidationToastr("Please Input Revise Reason.");
                $("#ReviseReason").focus();
                return;
            }
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Revise this PO?')
            ).then((result) => {
                if (result.isConfirmed) {
                    var SaveOption = Helper.GetSaveOption("btnRevise", isApprovePage);
                    IsSendForApproval = false, IsApproved = false, IsRevise = true, IsReject = false;
                    Manager.Save(IsSendForApproval, IsApproved, IsRevise, IsReject, SaveOption);
                }
            })

        });

        $("#btnReject").click(function (e) {

            var rejReason = $("#RejectReason").val();
            if (rejReason == "") {
                ToastrMessageHelper.ValidationToastr("Please Input Reject Reason.");
                $("#RejectReason").focus();
                return;
            }
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Reject this PO?')
            ).then((result) => {
                if (result.isConfirmed) {
                    var SaveOption = Helper.GetSaveOption("btnReject", isApprovePage);
                    IsSendForApproval = false, IsApproved = false, IsRevise = false, IsReject = true;
                    Manager.Save(IsSendForApproval, IsApproved, IsRevise, IsReject, SaveOption);
                }
            })





        });

        $("#btnBackToList").on("click", function () {
            Helper.BackToList();
        });

        //Change Event
        $("#OrderModeID").on("change", function () {
            var cboOrderModeID = $(this).data("kendoComboBox");
            var OrderModeID = cboOrderModeID ? cboOrderModeID.dataItem() : null;

            $("#POMasterID_RPT").data("kendoMultiColumnComboBox").text("");
            $("#BuyerRef").val("");
            $("#ColorName").val("");

            if (OrderModeID.id == 2) { //Repeat
                $("#POMasterID_RPT").data("kendoMultiColumnComboBox").enable(true);
            } else {
                $("#POMasterID_RPT").data("kendoMultiColumnComboBox").enable(false);
            }
        });

        $("#POMasterID_RPT").on("change", function () {
            var cboPOMasterID = $(this).data("kendoMultiColumnComboBox");
            var POMasterID = cboPOMasterID ? cboPOMasterID.dataItem() : null;

            if (!POMasterID) {
                $("#BuyerRef").val('');
                $("#ColorName").val('');
                return;
            }
            $("#BuyerRef").val(POMasterID.valueA);
            $("#ColorName").val(POMasterID.valueB);
        });
        $("#BuyerRefMasterID").on("change", function () {
            var cboBuyerRefMasterID = $(this).data("kendoComboBox");
            var BuyerRefMasterID = cboBuyerRefMasterID ? cboBuyerRefMasterID.dataItem() : null;
            if (!BuyerRefMasterID) {
                $("#ChemicalCode").val('');
                return;
            }
            $("#ChemicalCode").val(BuyerRefMasterID.valueA);
            $.get(`/api/poentry/GetColors?BuyerRefMasterID=${BuyerRefMasterID.id}`, function (data) {


                Helper.GetColors(data);


                //$.each(data, function (index, option) {
                //    alert(option.text);

                //    $("#BuyerRefColorID").append($("<option>",
                //        {
                //            value: option.id,
                //            text: option.text
                //        }));

                //});


            }).catch(function (err) {
                MessageHelper.Warning("Error = ", err.response.data.Message);
            });

        });

        //Selected Row in the kendo Grid
        KendoGridHelper.GridRowSelected(TblChildId);
    },
    GePONo: function (data) {
        var columns = [
            { field: "text", title: "PO No", width: "200" },
            { field: "valueA", title: "Buyer Ref.", width: "200" },
            { field: "valueB", title: "Color Name", width: "200" },
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#POMasterID_RPT", "text", "id", columns);
    },
    GetPOBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" },
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#POBy", "text", "id", columns);
    },
    GetColors: function (data) {
        var combo = $("#BuyerRefColorID");
        var columns = [
            { field: "text", title: "Color", width: combo.width() }
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#BuyerRefColorID", "text", "id", columns);
        combo.data("kendoMultiColumnComboBox").bind("open", function () {
            this.popup.element.css({
                width: this.wrapper.outerWidth()
            });
        });
    },
    GetSaveOption: function (flag, isApprovePage) {

        var SaveOption = 0;
        var pk_id = $("#POMasterID").val();
        pk_id = parseInt(pk_id);

        if ((flag == "btnSave") && (pk_id == 0) && (isApprovePage == false)) { SaveOption = 1; } // btnSave,Entry page
        if ((flag == "btnSave") && (pk_id > 0) && (isApprovePage == false)) { SaveOption = 2; } // btnSave,Entry page

        if ((flag == "btnSaveForApprove") && (pk_id == 0) && (isApprovePage == false)) { SaveOption = 1; } // Entry page
        if ((flag == "btnSaveForApprove") && (pk_id > 0) && (isApprovePage == false)) { SaveOption = 2; } // Entry page

        if ((flag == "btnApproved") && (pk_id > 0) && (isApprovePage == true)) { SaveOption = 2; } // Approve page
        if ((flag == "btnReject") && (pk_id > 0) && (isApprovePage == true)) { SaveOption = 3; } // Approve page


        if ((flag == "btnRevise") && (pk_id > 0)) { SaveOption = 5; }




        // btnSave,btnSaveForApprove,btnApproved,btnRevise,btnReject





        //if (pk_id == 0) { SaveOption = 1; }
        //if (pk_id > 0) { SaveOption = 2; }








        return SaveOption;
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
                    url: `/api/poentry/list/${statusCons}`,
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

        if (statusCons == statusConstants.EDIT || statusCons == statusConstants.REJECT) {
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
                    field: "BuyerRefMasterID", title: "BuyerRefMasterID", hidden: true,
                },
                {
                    field: "SDRFMasterID", title: "SDRFMasterID", hidden: true,
                },
                {
                    field: "FinishedProductID", title: "FinishedProductID", hidden: true,
                },

                {
                    field: "ColorID", title: "ColorID", hidden: true,
                },

                {
                    field: "BuyerRef", title: "Buyer Ref,", width: 160, hidden: false, attributes: { "style": "font-weight:700" },
                },
                {
                    field: "RequiredDate", title: "Req. Date", width: 100, hidden: false,
                    template: "#= (RequiredDate == null)? '' : kendo.toString(kendo.parseDate(RequiredDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "SDRFNo", title: "SDRF NO.", width: 100, hidden: false,
                },
                {
                    field: "ChemicalCode", title: "Chemical Code", width: 120, hidden: false,
                },
                {
                    field: "ColorName", title: "ColorName", width: 120, hidden: false,
                },
                {
                    field: "BuyerName", title: "BuyerName", width: 110, hidden: false,
                },
                {
                    field: "SeasonName", title: "Season", width: 150, hidden: false,
                },
            ];
        } else {
            columns = [
                {
                    title: "Action", command: command, width: 60
                },
                {
                    field: "POMasterID", title: "POMasterID", hidden: true,
                },
                {
                    field: "OrderTypeID", title: "OrderTypeID", hidden: true,
                },
                {
                    field: "OrderModeID", title: "OrderModeID", hidden: true,
                },
                {
                    field: "PONo", title: "PO No", width: 150, hidden: false, attributes: { "style": "font-weight:700" },
                },
                {
                    field: "PODate", title: "PO Date", width: 100, hidden: false,
                    template: "#= (PODate == null)? '' : kendo.toString(kendo.parseDate(PODate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "BuyerRef", title: "BuyerRef", width: 180, hidden: false,
                },
                {
                    field: "OrderMode", title: "Order</br>Mode", width: 100, hidden: false,
                },
                {
                    field: "PaymentTerms", title: "Payment</br>Terms", width: 120, hidden: false,
                },
                {
                    field: "Incoterm", title: "Incoterm", width: 120, hidden: false,
                },
                {
                    field: "CurrencyName", title: "Currency", width: 110, hidden: false,
                },
                {
                    field: "POByName", title: "Marketing</br>Person", width: 150, hidden: false,
                },

                {
                    field: "TotalPOQty", title: "Total</br>PO Qty", width: 90, hidden: false,
                },
                {
                    field: "Remarks", title: "Remarks", width: 180, hidden: false,
                },
            ];
        }


        if (status == statusCons.PENDING) {
            search =
            {
                fields: ["BuyerRef", "RequiredDate", "SDRFNo", "ProductName", "ColorName", "BuyerName", "SeasonName"]
            };

        } else {
            search =
            {
                fields: ["PONo", "PODate", "OrderMode", "PaymentTerms", "Incoterm", "CurrencyName", "POByName", "TotalPOQty"]
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
                    }], title: "Action &nbsp;", width: 60,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "POChildID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "POMasterID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "SDRFMasterID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "SDRFChildID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "POMasterID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "FeedbackChildID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "BuyerRefMasterID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "BuyerRefColorID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "objProductList", title: "Product</br>Name", width: 120,
                    editor: GridHelper.cboProduct,
                    template: "#=objProductList != null ? objProductList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "FinishedProductDescription", title: "Product</br>Descriptions", width: 160,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "objPackagingTypeList", title: "Packaging</br>Type", width: 90,
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
                    field: "objUOMList", title: "UOM", width: 80,
                    editor: GridHelper.cboUOM,
                    template: "#=objUOMList != null ? objUOMList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "POQty", title: "PO Qty", width: 80,
                    footerTemplate: "<span id='sumPOQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "objShippingDestinationList", title: "Shipping</br>Destination", width: 120,
                    editor: GridHelper.cboShippingDestination,
                    template: "#=objShippingDestinationList != null ? objShippingDestinationList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "ShipmentDate", title: "Shipment</br>Date", width: 100,
                    template: "#= kendo.toString(ShipmentDate,'dd-MMM-yyyy')#"
                },
                {
                    field: "ExFactoryDate", title: "Ex-Factory</br>Date", width: 100,
                    template: "#= kendo.toString(ExFactoryDate,'dd-MMM-yyyy')#"
                },
                {
                    field: "objShipModeList", title: "Ship Mode", width: 90,
                    editor: GridHelper.cboShipMode,
                    template: "#=objShipModeList != null ? objShipModeList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "Remarks", title: "Remarks", width: 220,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
            ];

        var schema = {
            model: {
                fields: {
                    POChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    POMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    SDRFChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    SDRFMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    POMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    FeedbackChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    BuyerRefMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    BuyerRefColorID: { type: "number", validation: { min: 0 }, defaultValue: 0 },

                    objProductList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    FinishedProductDescription: { type: "string" },
                    objPackagingTypeList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    SizeGram: { type: "number" },
                    objUOMList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    POQty: { type: "number" },
                    objShippingDestinationList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    ShipmentDate: {
                        type: "date",
                        template: '#=kendo.toString("dd-MMM-yyyy")#',
                        editable: true
                    },
                    ExFactoryDate: {
                        type: "date",
                        template: '#=kendo.toString("dd-MMM-yyyy")#',
                        editable: true
                    },
                    objShipModeList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    Remarks: { type: "string" },
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
            $("#btnSave,btnSaveForApprove,#btnApproved,#btnRevise,#btnReject").hide();
            $("#divReviseNote,#divRejectNote").hide();

            childsActionBtn = true;
            childsToolsBar = false;

            if (args.data.commandName == "rptPR") {
                Manager.RptPR(selectedItem.PRMasterID);
                return;
            }

            if (statusCons == statusConstants.PENDING) {
                childsActionBtn = false;
                childsToolsBar = true;
                Manager.GetNew(selectedItem.BuyerRefMasterID, selectedItem.SDRFMasterID);
                return;
            } else {
                if (statusCons == statusConstants.EDIT) {
                    $("#btnSave,#btnSaveForApprove").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                }
                if (statusCons == statusConstants.PROPOSED_FOR_APPROVAL) {

                    if (isApprovePage) {
                        $("#btnApproved,#btnReject").show();
                        $("#divReviseNote").show();
                        //$("#divReviseNote,#divRejectNote").show();
                        childsActionBtn = false;
                        childsToolsBar = true;
                    } else {
                        $("#btnSave,#btnSaveForApprove").hide();
                        childsActionBtn = false;
                        childsToolsBar = true;
                        //$(".k-toolbar", TblChildId).hide();
                    }

                }
                else if (statusCons == statusConstants.APPROVED) {
                    $("#btnRevise").show();
                    $("#divReviseNote").show();

                    if (isApprovePage) {
                        //$("#btnRevise").show();
                        //$("#divReviseNote,#divRejectNote").show();

                    } else {
                        $("#btnSaveForApprove").hide();
                        //$("#divReviseNote").show();

                        //$("#divReviseNote").show();
                        //    $("#divReviseNote,#divRejectNote").show();
                        //    $("#btnRevise,#btnReject").show();
                        //    $("#divReviseNote,#divRejectNote").show();
                    }
                }
                else if (statusCons == statusConstants.REJECT) {
                    if (isApprovePage) {
                        //$("#btnSave,#btnApproved").hide();
                        childsActionBtn = false;
                        childsToolsBar = true;
                        $("#divRejectNote").show();
                    } else {
                        $("#btnSave,#btnSaveForApprove").show();
                        childsActionBtn = false;
                        childsToolsBar = true;
                        //$("#divRejectNote").hide();
                    }
                }

                Manager.GetDetails(selectedItem.POMasterID);
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

                //if (e.field == "objPackagingTypeList") {
                //    selectedItem.set('UOM', obj.objPackagingTypeList.valueB);
                //    selectedItem.set('UOMID', obj.objPackagingTypeList.valueA);
                //}
                if (e.field === "PRQty") {
                    GridHelper.GetSumPOQty();
                }
            }
            GridHelper.GetSumPOQty();
        });
        GridHelper.GetSumPOQty();
    },
    GetSumPOQty: function () {
        TblChilds = $(TblChildId).data("kendoGrid");
        TblCDataSource = TblChilds.dataSource.data();
        var gcTotalQty = 0;

        for (var i = 0; i < TblCDataSource.length; i++) {
            gcTotalQty += kendo.parseFloat(TblCDataSource[i].POQty);
        }
        $("#sumPOQty").text(gcTotalQty.toFixed(2));
    },
    cboProduct: function (container, options) {
        var itemName = "";
        if (options.model.objProductList) {
            itemName = options.model.objProductList.text;
        }

        $('<input id="cmbProduct" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoMultiColumnComboBox({
                filter: "contains",
                minLength: 3,
                autoWidth: true,
                dataTextField: "text",
                dataValueField: "id",
                height: 500,
                columns: [
                    { field: "text", title: "Product Name", width: "300" },
                    { field: "valueB", title: "Product Category", width: "200" },
                    { field: "valueD", title: "Product Type", width: "150px" },
                ],
                dataSource: {
                    type: "json",
                    serverFiltering: true,
                    transport: {
                        read: {
                            type: "GET",
                            url: `/api/helperC/GetAllProductBySearch?itemName=${encodeURIComponent(itemName)}&pageIndex=1&pageSize=400&CountType='HO'`,
                            dataType: "json",
                            contentType: 'application/json',
                            data: {
                                searchKey: GridHelper.filterValue
                            }
                        },
                    }
                },
                index: -1,
                change: function (e) {
                    KendoGridHelper.IsValidItemMultiCombo("cmbProduct");
                    KendoGridHelper.RemoveUndefined(e);
                    //GridHelper.CountChangeEvent(e);

                    // Focus next 3 no column
                    var grid = $(TblChildId).data("kendoGrid");
                    var cell = $(e.sender.element).closest("td");
                    var row = cell.closest("tr");
                    var rowIndex = row.index();
                    var nextColumnIndex = cell.index() + 1;
                    grid.editCell(grid.tbody.find("tr").eq(rowIndex).find("td").eq(nextColumnIndex));
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
    cboShippingDestination: function (container, options) {
        $('<input id="cmbShippingDestination" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.ShippingDestinationList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboShipMode: function (container, options) {
        $('<input id="cmbShipMode" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.ShipModeList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboShippingDestination: function (container, options) {
        $('<input id="cmbShippingDestination" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.ShippingDestinationList,
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