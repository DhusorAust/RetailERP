
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
    GetNew: function () {
        $.get(`/api/frmlation/new`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
          
            masterData = { ...data };
            masterData.FormulationStatusList = [...data.FormulationStatusList.filter(f => [6, 9].includes(f.id))];
            masterData.LeadTimeUOMList = [...data.FormulationStatusList];
            Helper.GetAllEmployeeListByUnitID("", "");

            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable([]);
            GridManager.InitChildTable_Method([]);

        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id) {
        $.get(`/api/frmlation/edit?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();

            masterData = data;
            masterData.FormulationStatusList = data.FormulationStatusList?.filter(f => [6, 9].includes(f.id));
            masterData.LeadTimeUOMList = data.FormulationStatusList;
            Helper.GetAllEmployeeListByUnitID(masterData.FormulationBy, "");

            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs);
            GridManager.InitChildTable_Method(masterData.ChildsMethod);

        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    Save: function (isApproved, isRevise, isReject) {
        const $btn = $("#btnSave");
        if (Helper.IsFrmValid()) {
            $btn.addClass("disabled").css("pointer-events", "none").css("opacity", "0.6").data("busy", true);
            var model = FormHelper.FormElToJson(FormDom);
            model.IsLocked = $("#IsLocked").data("kendoSwitch").value() == true ? 1 : 0;
            model.Approved = isApproved;
            model.Revise = isRevise;
            model.Reject = isReject;

            var model = FormHelper.FormElToJson(FormDom);
            model.Childs = JSON.parse(JSON.stringify($(TblChildId).data("kendoGrid").dataSource.data()));
            model.ChildsMethod = JSON.parse(JSON.stringify($(TblChildIdMethod).data("kendoGrid").dataSource.data()));


            //Child Validation
            if (!Helper.IsFrmChildsValid(model.Childs)) {
                $btn.removeClass("disabled").css("pointer-events", "").css("opacity", "").data("busy", false);
                return;
            }


            $.post("/api/frmlation/save", model).done(function (data) {
                if (parseInt(data.ResultId) > 0) {
                    MessageHelper.Success("Save Operations", data.Message);
                    Helper.BackToList();
                    //Helper.ResetForm();
                    //Manager.GetDetails(data.ResultId);
                } else if (data.IsLogin == 1) {
                    window.open(data.redirectUrl, "_blank");
                } else {
                    MessageHelper.Warning("Save Operations", data.Message);
                }
            }).fail(function (xhr, status, error) {
                MessageHelper.Warning("Save Operations", error);
            }).always(function () {
                $btn.removeClass("disabled").css("pointer-events", "").css("opacity", "").data("busy", false);
            });
        }

        //if (Helper.IsFrmValid()) {
        //    if (Helper.IsFrmValidCustom()) {
                
        //        $.post("/api/frmlation/save", model, function (data) {
        //            if (parseInt(data.ResultId) > 0) {
        //                MessageHelper.Success("Save Operations", data.Message);
        //                Helper.BackToList();
        //            } else {
        //                if (data.IsLogin == 1) {
        //                    window.open(data.redirectUrl, "_blank");
        //                } else {
        //                    MessageHelper.Warning("Save Operations", data.Message);
        //                }
        //            }
        //        });
        //    }
        //}
    },
    RptPR: function (id) {
        window.open(`/RptCommercialExport/frmlation?id=${id}`, "_blank");
    },
    GetReport: function (id,parameterName) {

        window.open(`/RptFormulation/${parameterName}?id=${id}`, "_blank");
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
            if (obj.objIngredientList.id == 0 || obj.objIngredientList.id == null) {
                MessageHelper.Warning("Warning", "Ingredient is required.");
                isValidItem = false;
            }
        });

        //Childs Length Check
        if (ChildsList.length === 0) {
            MessageHelper.Warning("Warning", "At least 1 Chemical Items is required.");
            return isValidItem = false;
        }
        return isValidItem;
    },
    IsFrmValid: function () {
        var isValidItem = true;
        var Elements = ["BrandID", "UnitID", "ProductID", "BatchQty", "BatchUOMID", "FormulationStatus"];
        if (!FormHelper.IsFormValidation(FormDom, Elements)) {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
        }
        return isValidItem;
    },
    IsFrmValidCustom: function () {

        // For multiselect items and drop down with 0 value
        var isValidItem = true;
        var input = null;

        var MultiSelectElements = ["ColorID"];
        for (var i = 0; i < MultiSelectElements.length; i++) {
            var input = $("#" + MultiSelectElements[i]);
            var multiSelect = ($("#" + MultiSelectElements[i])).data("kendoMultiSelect");
            if (multiSelect && multiSelect.value().length > 0) {
                input.closest('.form-group').find('.' + input[0].id).hide();
            } else {
                input.closest('.form-group').find('.' + input[0].id).show();
                isValidItem = false;
                return;
            }
        }

        if (isValidItem == true) {
            var ValueElements = ["SDRFMasterID", "RnDBy", "SizeGram"];
            for (var i = 0; i < ValueElements.length; i++) {
                input = $("#" + ValueElements[i]);
                if (input.val() == "" || input.val() == null || input.val() == "0") {
                    isValidItem = false;
                    input.closest('.form-group').find('.' + input[0].id).show();
                }
                else {
                    input.closest('.form-group').find('.' + input[0].id).hide();
                }
            }
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
        TblChildIdMethod = pageConstants.CHILD_TABLE_ID2 + pageId;
        FormDom = $(pageConstants.FORM_ID + pageId);
        DivDetailsDom = $(pageConstants.DIV_DETAILS_ID + pageId);

        //Development Code 
        $("#kt_aside_toggle").addClass('active');
        $("#kt_body").addClass('aside-minimize');

        $("#IsLocked").kendoSwitch({ messages: { checked: "YES", unchecked: "NO" } });

        $("#btnNewAdd,#btnEditList,#btnApprovedList,#btnRejectList,#btnAllList").show();
        $("#btnPendingList").hide();

        //Temp 
        statusCons = statusConstants.EDIT;
        FormHelper.TogToolbarBtn($("#btnEditList"), ToolbarDom);
        GridManager.InitMasterTable();

        // ToolbarDom means Toolbar Buttons  
        $("#btnPendingList").on("click", function (e) {
            statusCons = statusConstants.PENDING;
            FormHelper.TogToolbarBtn($("#btnPendingList"), ToolbarDom);
            GridManager.InitMasterTable();
        });
        $("#btnEditList").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
        });
        $("#btnApprovedList").on("click", function (e) {
            statusCons = statusConstants.APPROVED;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
        });
        $("#btnRejectList").on("click", function (e) {
            statusCons = statusConstants.REJECT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
        });
        $("#btnAllList").on("click", function (e) {
            statusCons = statusConstants.ALL;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
        });

        //Button

        $("#btnNewAdd").on("click", function (e) {
            $("#btnRevise,#btnReject,#divRejectNote,#divReviseNote").hide()
            $("#btnSave,#btnApproved").show();
            Manager.GetNew();
        });
        $("#btnSave").click(function (e) {
            var SaveOption = Helper.GetSaveOption();
            IsApproved = false, IsRevise = false, IsReject = false;
            Manager.Save(IsApproved, IsRevise, IsReject, SaveOption);
        });
        $("#btnApproved").click(function (e) {
            var SaveOption = Helper.GetSaveOption();
            IsApproved = true, IsRevise = false, IsReject = false;
            Manager.Save(IsApproved, IsRevise, IsReject, SaveOption);
        });
        $("#btnRevise").click(function (e) {
            var revReason = $("#ReviseReason").val();
            if (revReason == "") {
                ToastrMessageHelper.FxToastr("Please Input Revise Reason.");
                $("#ReviseReason").focus();
                return;
            }
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to revise this formula ?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsApproved = false; IsRevise = true; IsReject = false;
                    Manager.Save(IsApproved, IsRevise, IsReject, 5);
                }
            })
        });
        $("#btnReject").click(function (e) {
            var rejReason = $("#RejectReason").val();
            if (rejReason == "") {
                ToastrMessageHelper.FxToastr("Please Input Reject Reason.");
                $("#RejectReason").focus();
                return;
            }
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to reject this formula ?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsApproved = false; IsRevise = false; IsReject = true;
                    Manager.Save(IsApproved, IsRevise, IsReject, 3);
                }
            })
        });

        $("#btnBackToList").on("click", function () {
            Helper.BackToList();
        });

        $("#BatchQty").on("change", function () {
            //GridHelper.SetChildGridValue();
            GridHelper.CalTotalBatchQtyWise();
        });



        //$("#btnPrintPI").click(function () {
        //    var piid = $("#PINo").val();
        //    if (piid) {
        //        Manager.GetPIReport(piid);
        //    }
        //});

        ////Change Event
        //$("#PRBy").on("change", function () {
        //    var cboPRBy = $(this).data("kendoMultiColumnComboBox");
        //    var PRBy = cboPRBy ? cboPRBy.dataItem() : null;

        //    if (!PRBy) {
        //        $("#DepartmentID").val('');
        //        $("#SectionID").val('');
        //        $("#SubSectionID").val('');
        //        return;
        //    }

        //    $("#DepartmentID").val(PRBy.valueA);
        //    $("#SectionID").val(PRBy.valueB);
        //    $("#SubSectionID").val(PRBy.valueC);
        //});  
        //$("#SDRFMasterID").on("change", function () {
        //    var cbo = $(this).data("kendoMultiColumnComboBox");
        //    var vSDRFMasterID = cbo ? cbo.dataItem() : null;

        //    if (!vSDRFMasterID) {
        //        $("#ChemicalCode").val('');
        //        $("#BuyerRef").val('');
        //        $("#BuyerName").val('');
        //        $("#FinishedProductCategoryID").val('');
        //        $("#FinishedProductTypeID").val('');
        //        $("#FinishedProductID").val('');

        //        return;
        //    }

        //    $("#ChemicalCode").val(vSDRFMasterID.valueD);
        //    $("#BuyerRef").val(vSDRFMasterID.valueH);
        //    $("#BuyerName").val(vSDRFMasterID.valueC);
        //    $("#FinishedProductCategoryID").val(vSDRFMasterID.valueK);
        //    $("#FinishedProductTypeID").val(vSDRFMasterID.valueL);
        //    $("#FinishedProductID").val(vSDRFMasterID.valueJ);


        //    var ColorList = $("#ColorID").data("kendoMultiSelect").dataSource.data();
        //    var values = "";
        //    KendoDropDownHelper.MultiSelectCombo(ColorList, "ColorID", "text", "id", "", "", false);
        //    if (vSDRFMasterID.valueI) {
        //        values = vSDRFMasterID.valueI.toString().split(",");
        //        $("#ColorID").data("kendoMultiSelect").value(values);
        //    }

        //});
        ////Selected Row in the kendo Grid
        KendoGridHelper.GridRowSelected(TblChildId);
    },
    GetAllEmployeeListByUnitID: function (name, unitID) {
        var apUrl = `/api/helperC/GetEmployeeByUnitID?employeeName=${name}&unitID=${unitID}`;
        var columns = [
            { field: "EmployeeNo", title: "Employee No", width: "110px" },
            { field: "EmployeeName", title: "Employee", width: "220px" },
            { field: "DepartmentName", title: "Department", width: "220" },
            { field: "DesignationName", title: "Designation", width: "200px" },
            { field: "SectionName", title: "Section", width: "160px" },
            { field: "UnitFullName", title: "Project", width: "180px" }
        ]

        KendoDropDownHelper.KendoMultiColumn_BySearch("#PreparedBy", apUrl, columns, "EmployeeName", "EmployeeID");
    },
    GetSaveOption: function () {
        var SaveOption = 0;
        var pk_id = $("#FormulationMasterID").val();
        pk_id = parseInt(pk_id);

        if (pk_id == 0) { SaveOption = 1; }
        if (pk_id > 0) { SaveOption = 2; }

        //if ((value == 1) && (pk_id == 0)) { SaveOption = 1; } // Save New
        //if ((value == 1) && (pk_id > 0)) { SaveOption = 2; } // Update
        //if ((value == 3) && (pk_id > 0)) { SaveOption = 3; } // Reject
        //if ((value == 4) && (pk_id > 0)) { SaveOption = 4; } // Approve

        //if ((value == 6) && (pk_id == 0)) { SaveOption = 6; } // Save & Send To Approve
        //if ((value == 6) && (pk_id > 0)) { SaveOption = 7; } // Update & Send To Approve
        //if ((value == 8) && (pk_id == 0)) { SaveOption = 8; } // Save & Send To QC
        //if ((value == 8) && (pk_id > 0)) { SaveOption = 9; } // Update & Send To QC
        //if ((value == 10) && (pk_id > 0)) { SaveOption = 10; } // Update QC
        //if ((value == 11) && (pk_id > 0)) { SaveOption = 11; } // Update QC & Send To MRR
        //if ((value == 12) && (pk_id > 0)) { SaveOption = 12; } // Generate MRR
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
                    url: `/api/frmlation/list/${statusCons}`,
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
        var search;

        if (statusCons == statusConstants.PENDING) {
            command =
                [
                { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                { name: "FormulationDetails", text: "FormulationDetails", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },
                { name: "IngrediantINCIBreakDownDetails", text: "INCIBreakDown", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },
                { name: "IngrediantINCIBreakDownSummary", text: "INCIBreakDownSummary", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },
                ];
        }
        else if (statusCons == statusConstants.EDIT || statusCons == statusConstants.REJECT) {
            command =
                [
                { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                { name: "FormulationDetails", text: "Formulation", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },
                { name: "IngrediantINCIBreakDownDetails", text: "Ingredients List", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },
                { name: "IngrediantINCIBreakDownSummary", text: "INCI Breakdown", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },

                ];
        } else {
            command =
                [
                { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                { name: "FormulationDetails", text: "Formulation", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },
                { name: "IngrediantINCIBreakDownDetails", text: "Ingredients List", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },
                { name: "IngrediantINCIBreakDownSummary", text: "INCI Breakdown", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },

                ];
        }

        columns = [
            {
                title: "Action", command: command, width: 180,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "FormulationMasterID", title: "FormulationMasterID", hidden: true,
            },
            {
                field: "FormulationCode", title: "Formulation</br>Code", width: 130,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "FormulationVersion", title: "Formulation</br>Version", width: 130,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "FormulationDate", title: "Formulation</br>Date", width: 130,
                template: "#= (FormulationDate == null)? '' : kendo.toString(kendo.parseDate(FormulationDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #",
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "BrandName", title: "Brand</br>Name", width: 130,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "UnitName", title: "Project</br>Name", width: 130,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "ProductName", title: "Product</br>Name", width: 180,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "BatchQty", title: "Batch</br>Qty", width: 100,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "BatchUOMID", title: "Batch</br>UOM", width: 100,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "TotalNaturalIndexPct", title: "Total Natural</br>Index %", width: 140,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "TotalNaturalOriginIndexPct", title: "Total Natural</br>Origin Index %", width: 140,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "TotalOrganicIndexPct", title: "Total</br>Organic Index %", width: 150,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "TotalOrganicOriginIndexPct", title: "Total Organic</br>Origin Index %", width: 140,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
        ];
        search =
        {
            fields:
                [
                    "FormulationCode", "FormulationVersion", "BrandName", "UnitName", "ProductName"
                ]
        };

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
                { field: "FormulationChildID", hidden: true, attributes: { class: "non-editable-column" }, },
                { field: "FormulationMasterID", hidden: true, attributes: { class: "non-editable-column" }, },
                { field: "NaturalIndexIngr", hidden: true, attributes: { class: "non-editable-column" }, },
                { field: "NaturalOriginIngr", hidden: true, attributes: { class: "non-editable-column" }, },
                { field: "OrganicIngre", hidden: true, attributes: { class: "non-editable-column" }, },
                { field: "OrganicOriginIngre", hidden: true, attributes: { class: "non-editable-column" }, },
                {
                    field: "RowSLNo", title: "SL#", width: 50, editable: function () { return false },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "objStepList", title: "Step No", width: 100,
                    editor: GridHelper.cboStep,
                    template: "#=(objStepList && objStepList.text) ? objStepList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objIngredientList", title: "Ingredient</br>Name", width: 120,
                    editor: GridHelper.cboIngredient,
                    template: "#=(objIngredientList && objIngredientList.text) ? objIngredientList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "IngredientCode", title: "Ingredient</br>Code", width: 100, editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "INCIName", title: "INCI Name", width: 200, editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "IngredientPercentage", title: "Ingredient</br>(%)", width: 90,
                    editor: KendoGridHelper.NumberTextBox,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "IngredientQtyKg", title: "Ingredient</br>Qty (Kg)", width: 90, editable: function () { return true; },
                    editor: KendoGridHelper.NumberTextBox,
                    footerTemplate: "<span id='TotalIngredientQtyKg'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "IngredientQtyGM", title: "Ingredient</br>Qty (gm)", width: 90, editable: function () { return true; },
                    editor: KendoGridHelper.NumberTextBox,
                    footerTemplate: "<span id='TotalIngredientQtyGM'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "IngredientCost", title: "Ingredient</br>Cost", width: 90,
                    editor: KendoGridHelper.NumberTextBox,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "IngredientPricePerQtyUsed", title: "Ingredient</br>Price /Pcs", width: 100, editable: function () { return false; },
                    editor: KendoGridHelper.NumberTextBox,
                    footerTemplate: "<span id='TotalIngredientPricePerQtyUsed'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "MOQInKg", title: "MOQ In Kg", width: 85, editable: function () { return false; },
                    editor: KendoGridHelper.NumberTextBox,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "LeadTime", title: "Lead Time", width: 85, editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "objLeadTimeUOMList", title: "Lead Time</br>(UOM)", width: 90, editable: function () { return false; },
                    editor: GridHelper.cboLeadTimeUOM,
                    template: "#=(objLeadTimeUOMList && objLeadTimeUOMList.text) ? objLeadTimeUOMList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "NaturalIndexPct", title: "Natural Index</br>% (ISO 16128)", width: 110, editable: function () { return false; },
                    editor: KendoGridHelper.NumberTextBox,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "NaturalOriginIndexPct", title: "Natural Origin Index</br>% (ISO 16128)", width: 150, editable: function () { return false; },
                    editor: KendoGridHelper.NumberTextBox,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "OrganicIndexPct", title: "Natural Index</br>% (ISO 16128)", width: 120, editable: function () { return false; },
                    editor: KendoGridHelper.NumberTextBox,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "OrganicOriginIndexPct", title: "Organic Index</br>% (ISO 16128)", width: 120, editable: function () { return false; },
                    editor: KendoGridHelper.NumberTextBox,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "RegulatoryWarning", title: "Regulatory</br>Warnings", width: 200, editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
            ];

        var schema = {
            model: {
                fields: {
                    objStepList: {
                        defaultValue: { id: null, text: "" },
                    },
                    objIngredientList: {
                        defaultValue: { id: null, text: "" },
                    },
                    objLeadTimeUOMList: {
                        defaultValue: { id: null, text: "" },
                    },
                    IngredientPercentage: { type: "number", defaultValue: null },
                    IngredientCost: { type: "number", defaultValue: null },
                    //NaturalIndexPct: { type: "text", defaultValue: null },
                    //NaturalOriginIndexPct: { type: "text", defaultValue: null },
                    //OrganicIndexPct: { type: "text", defaultValue: null },
                    //OrganicOriginIndexPct: { type: "text", defaultValue: null },
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId, columns, data, schema, true, true, false);
        KendoGridHelper.EnterKeyUp(TblChildId);

        GridHelper.CalTotal();
        GridHelper.SumTotalQty(TblChildId, ["IngredientQtyKg", "IngredientQtyGM", "IngredientPricePerQtyUsed"], ["#TotalIngredientQtyKg", "#TotalIngredientQtyGM", "#TotalIngredientPricePerQtyUsed"]);
    },

    InitChildTable_Method: function (data) {
        if ($(TblChildIdMethod).data("kendoGrid")) $(TblChildIdMethod).kendoGrid('destroy').empty();
        dataSource = data;
        var columns =
            [
                {
                    command: [{
                        name: "delete2", text: "", iconClass: "k-icon k-i-delete", className: "k-success",
                        click: GridHelper.ChildCommandClick,
                    }], title: "Action &nbsp;", width: 80,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                { field: "FormulationMethodID", hidden: true, attributes: { class: "non-editable-column" }, },
                { field: "FormulationMasterID", hidden: true, attributes: { class: "non-editable-column" }, },
                {
                    field: "RowSLNo", title: "SL#", width: 60, editable: function () { return false },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "Instruction", title: "Instruction", //width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-center", true, false, true)
                },
            ];

        var schema = {
            model: {
                fields: {
                    InstructionRemarks: { type: "text", defaultValue: null },
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildIdMethod, columns, data, schema, true, true, false);
        KendoGridHelper.EnterKeyUp(TblChildIdMethod);
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

            //if (args.data.commandName == "rptPR") {
            //    Manager.RptPR(selectedItem.FormulationMasterID);
            //    return;
            //}


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
            if (selectedItem != null) {
                if (args.data.commandName == "edit") {

                    Manager.GetDetails(selectedItem.FormulationMasterID);
                }
                if (args.data.commandName == "FormulationDetails") {
                    Manager.GetReport(selectedItem.FormulationMasterID,'FormulationDetails');
                    return;
                }
                if (args.data.commandName == "IngrediantINCIBreakDownDetails") {
                    Manager.GetReport(selectedItem.FormulationMasterID,'IngrediantINCIBreakDownDetails');
                    return;
                }
                if (args.data.commandName == "IngrediantINCIBreakDownSummary") {
                    Manager.GetReport(selectedItem.FormulationMasterID,'IngrediantINCIBreakDownSummary');
                    return;
                }
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

        if (args.data.commandName == "delete2") {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to DELETE this record.?')
            ).then((result) => {
                if (result.isConfirmed) {
                    var grid = $(TblChildIdMethod).data("kendoGrid");
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

    //InitializeHelper: function () {
    //    //Grid Function
    //    TblChilds = $(TblChildId).data("kendoGrid");
    //    TblCDataSource = TblChilds.dataSource.data();        
    //},
    

    cboStep: function (container, options) {
        $('<input id="cmbStep" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.StepList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined_Final("objStepList", this.dataItem(), options);
                },
            });
    },

    cboLeadTimeUOM: function (container, options) {
        var dataList = [...masterData.LeadTimeUOMList?.filter(f => ![6, 9].includes(f.id))];
        $('<input id="cmbStep" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: dataList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined_Final("objLeadTimeUOMList", this.dataItem(), options);
                },
            });
    },
    cboIngredient: function (container, options) {
        var itemName = "";
        if (options.model.objIngredientList) {
            itemName = options.model.objIngredientList.text;
        }

        $('<input id="cmbIngredient" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoMultiColumnComboBox({
                filter: "contains",
                minLength: 3,
                autoWidth: true,
                dataTextField: "text",
                dataValueField: "id",
                height: 500,
                columns: [
                    {
                        field: "valueA", title: "Ing Code", width: 90,
                    },
                    { field: "text", title: "Ing Name", width: 150 },
                    { field: "valueB", title: "INCI Name", width: 400 },
                    { field: "valueC", title: "Supplier Name", width: 110 },
                    { field: "valueD", title: "MOQ in Kg", width: 100 },
                    { field: "valueE", title: "Lead Time", width: 100 },
                    { field: "valueF", title: "Lead Time in D/W", width: 120 },
                ],
                dataSource: {
                    type: "json",
                    serverFiltering: true,
                    transport: {
                        read: {
                            type: "GET",
                            url: `/api/helperC/GetIngredientBySearch?itemName=${encodeURIComponent(itemName)}`,
                            dataType: "json",
                            contentType: 'application/json',
                            data: function () {
                                return {
                                    searchKey: `${$("#cmbIngredient").data("kendoMultiColumnComboBox").input.val()}`,
                                    pageIndex: 1,
                                    pageSize: 200
                                };
                            }
                        },
                    }
                },
                index: -1,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined_Final("objIngredientList", this.dataItem(), options);
                    GridHelper.IngredientEvent(e);

                    // Focus next 3 no column
                    //var grid = $(TblChildId).data("kendoGrid");
                    //var cell = $(e.sender.element).closest("td");
                    //var row = cell.closest("tr");
                    //var rowIndex = row.index();
                    //var nextColumnIndex = cell.index() + 3; 
                    //grid.editCell(grid.tbody.find("tr").eq(rowIndex).find("td").eq(nextColumnIndex));
                },
            });
    },
    IngredientEvent: function (e) {debugger
        var combo = e.sender;
        var itemGrid = $(TblChildId).data("kendoGrid");
        var data = $("#cmbIngredient").data("kendoMultiColumnComboBox").dataItem();

        var selectedRow = itemGrid.select();
        var selectedItem = itemGrid.dataItem(itemGrid.select());
        var selectedRowIndex = selectedRow.index();

        var ingredientID = $("#cmbIngredient").data("kendoMultiColumnComboBox").value();
        var gridData = $(TblChildId).data("kendoGrid").dataSource.data();

        for (var i = 0; i < gridData.length; i++) {
            var grd = gridData[i];
            if (selectedRowIndex != i) {
                if (grd.objIngredientList.IngredientID == ingredientID) {
                    $("#cmbIngredient").data("kendoMultiColumnComboBox").value("");
                    selectedItem.set('IngredientCode', "");
                    selectedItem.set('INCIName', "");
                    selectedItem.set('MOQInKg', "");
                    selectedItem.set('LeadTime', "");
                    selectedItem.set('objLeadTimeUOMList', { id: null, text: "" });
                    FxToastr("This Ingredient Name Already Added")
                    return;
                }
            }
        }
        selectedItem.set('IngredientCode', data.valueA);
        selectedItem.set('INCIName', data.valueB);
        selectedItem.set('MOQInKg', data.valueD);
        selectedItem.set('LeadTime', data.valueE);
        selectedItem.set('OrganicOriginIngre', data.valueK);
        selectedItem.set('OrganicIngre', data.valueJ);
        selectedItem.set('NaturalOriginIngr', data.valueI);
        selectedItem.set('NaturalIndexIngr', data.valueH);
        selectedItem.set('objLeadTimeUOMList', { id: data.valueG, text: data.valueF });
    },


    CalTotal: function () {
        debugger
        //Get Total Qty
        var tblChilds = $(TblChildId).data("kendoGrid");

        tblChilds.dataSource.bind("change", function (e) {
            if (e.action === "itemchange") {
                var selectedItem = tblChilds.dataItem(tblChilds.select());
                //var data = $("#cmbIngredient").data("kendoMultiColumnComboBox").dataItem();
                var obj = e.items[0];

                if (e.field === "IngredientPercentage" || e.field === "IngredientCost") {debugger
                    var ingredientPercentage = kendo.parseFloat(UtilityHelper.EmptyThenZero(selectedItem.IngredientPercentage));
                    var batchQty = kendo.parseFloat($("#BatchQty").val());
                    var ingredientCost = kendo.parseFloat(UtilityHelper.EmptyThenZero(selectedItem.IngredientCost));

                    var qtyKg = batchQty * ingredientPercentage / 100;
                    var qtyGm = qtyKg * 1000;
                    var ingredientCost = qtyKg * ingredientCost;

                    selectedItem.set("IngredientQtyKg", parseFloat(qtyKg.toFixed(4)));
                    selectedItem.set("IngredientQtyGM", parseFloat(qtyGm.toFixed(4)));
                    selectedItem.set("IngredientPricePerQtyUsed", parseFloat(ingredientCost.toFixed(4)));

                    if (e.field === "IngredientPercentage") {
                        var NaturalIndexPct = (selectedItem.NaturalIndexIngr * ingredientPercentage)/100.0;
                        var NaturalOriginIndexPct = (selectedItem.NaturalOriginIngr * ingredientPercentage) / 100.0;
                        var OrganicIndexPct = (selectedItem.OrganicIngre * ingredientPercentage)/100.0;
                        var OrganicOriginIndexPct = (selectedItem.OrganicOriginIngre * ingredientPercentage)/100.0;
                        selectedItem.set("NaturalIndexPct", parseFloat(NaturalIndexPct.toFixed(4)));
                        selectedItem.set("NaturalOriginIndexPct", parseFloat(NaturalOriginIndexPct.toFixed(4)));
                        selectedItem.set("OrganicIndexPct", parseFloat(OrganicIndexPct.toFixed(4)));
                        selectedItem.set("OrganicOriginIndexPct", parseFloat(OrganicOriginIndexPct.toFixed(4)));
                    }

                    GridHelper.SumTotalQty(TblChildId, ["IngredientQtyKg", "IngredientQtyGM", "IngredientPricePerQtyUsed"], ["#TotalIngredientQtyKg", "#TotalIngredientQtyGM", "#TotalIngredientPricePerQtyUsed"]);
                }
            }
        });
    },

    SumTotalQty: function (gridId, fields, outputSelectors) {
        let grid = $(gridId).data("kendoGrid");
        if (!grid) return;

        let data = grid.dataSource.data();

        // init totals dynamically
        let totals = {};
        fields.forEach(f => totals[f] = 0);

        data.forEach(item => {
            fields.forEach(f => {
                totals[f] += +UtilityHelper.EmptyThenZero(item[f]);
            });
        });

        // bind to UI
        fields.forEach((f, i) => {
            let val = totals[f];
            $(outputSelectors[i]).text(val < 1 ? 0 : val.toFixed(2));
        });
    },

    CalTotalBatchQtyWise: function () {
        let tblChilds = $(TblChildId).data("kendoGrid");
        let tblCDataSource = tblChilds.dataSource.data();
        totalQtyBag = 0;
        totalQty = 0;
        totalAmount = 0;
        for (var i = 0; i < tblCDataSource.length; i++) {
            var ingredientPercentage = kendo.parseFloat(UtilityHelper.EmptyThenZero(tblCDataSource[i].IngredientPercentage));
            var batchQty = kendo.parseFloat($("#BatchQty").val());
            var ingredientCost = kendo.parseFloat(UtilityHelper.EmptyThenZero(tblCDataSource[i].IngredientCost));

            var qtyKg = batchQty * ingredientPercentage / 100;
            var qtyGm = qtyKg * 1000;
            var ingredientCost = qtyKg * ingredientCost;

            tblCDataSource[i].set("IngredientQtyKg", parseFloat(qtyKg.toFixed(4)));
            tblCDataSource[i].set("IngredientQtyGM", parseFloat(qtyGm.toFixed(4)));
            tblCDataSource[i].set("IngredientPricePerQtyUsed", parseFloat(ingredientCost.toFixed(4)));

        }

        GridHelper.SumTotalQty(TblChildId, ["IngredientQtyKg", "IngredientQtyGM", "IngredientPricePerQtyUsed"], ["#TotalIngredientQtyKg", "#TotalIngredientQtyGM", "#TotalIngredientPricePerQtyUsed"]);
    },


    //cboPackagingType: function (container, options) {
    //    $('<input id="cmbPackagingType" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
    //        .appendTo(container).kendoComboBox({
    //            filter: "contains",
    //            suggest: true,
    //            optionLabel: '',
    //            placeholder: "Select",
    //            // autoWidth: true,
    //            dataSource: masterData.PackagingTypeList,
    //            change: function (e) {
    //                KendoGridHelper.RemoveUndefined(e);
    //            },
    //        });
    //},
    //cboUOM: function (container, options) {
    //    $('<input id="cmbUOM" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
    //        .appendTo(container).kendoComboBox({
    //            filter: "contains",
    //            suggest: true,
    //            optionLabel: '',
    //            placeholder: "Select",
    //            // autoWidth: true,
    //            dataSource: masterData.UOMList,
    //            change: function (e) {
    //                KendoGridHelper.RemoveUndefined(e);
    //            },
    //        });
    //},
    //cboSection: function (container, options) {
    //    $('<input id="cmbSection" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
    //        .appendTo(container).kendoComboBox({
    //            filter: "contains",
    //            suggest: true,
    //            optionLabel: '',
    //            placeholder: "Select",
    //            // autoWidth: true,
    //            dataSource: masterData.SectionList,
    //            change: function (e) {
    //                KendoGridHelper.RemoveUndefined(e);
    //            },
    //        });
    //},
    GridCenterAlignAttributes: function (align_header = "", align_body = "", align_footer = "") {
        return {
            headerAttributes: { "class": align_header + " k_input_cell" },
            attributes: { "class": align_body + " k_input_cell" },
            footerAttributes: { "class": align_footer + " k_input_cell" }
        };
    },


    //SetChildGridValue: function () {
    //    gbInvoiceList.forEach(item => {
    //        GridHelper.SetGridDropDownValue(item);
    //    });

    //    GridManager.InitChildTable2(gbInvoiceList);
    //    GridManager.InitChildTable3(gbInvoiceList);
    //},



}





