
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
var sheetData = [];
var filePathData = [];

$(document).ready(function () {
    Helper.Initialize();
});

var Manager = {
    GetNew: function () {
        $.get(`/api/StabilityTest/new`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
          
            masterData = { ...data };
            sheetData = [];
            filePathData = [];
           // masterData.FormulationStatusList = [...data.FormulationStatusList.filter(f => [6, 9].includes(f.id))];
            //Helper.GetAllEmployeeListByUnitID("", "");

            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable([]);
             var container = $("#dynamicGrid");
             container.empty();

        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id) {
        $.get(`/api/StabilityTest/edit?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
             var container = $("#dynamicGrid");
        container.empty();
            masterData = data;
           // masterData.FormulationStatusList = data.FormulationStatusList?.filter(f => [6, 9].includes(f.id));
          //  Helper.GetAllEmployeeListByUnitID(masterData.FormulationBy, "");

            FormHelper.SetFormData(FormDom, masterData);
            sheetData = masterData.Childs || [];
            filePathData = masterData.StabilityTestFilePaths || [];
            GridManager.InitChildTable(masterData.StabilityTestTemperatures);
            GridHelper.CreateInitChildTableDynamicGrid();
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    Save: function (isApproved, isRevise, isReject) {
        const $btn = $("#btnSave");
        if (Helper.IsFrmValid()) {
            $btn.addClass("disabled").css("pointer-events", "none").css("opacity", "0.6").data("busy", true);
            var model = FormHelper.FormElToJson(FormDom);
            //model.Approved = isApproved;
            //model.Revise = isRevise;
            //model.Reject = isReject;
            var modelChilds = [];
            //var modelChilds = JSON.parse(JSON.stringify($(TblChildId).data("kendoGrid").dataSource.data()));

            var TblChilds = $(TblChildId).data("kendoGrid");
            var TblCDataSource = TblChilds.dataSource.data();
       
       
            $.each(TblCDataSource, function (i, temp) {
                var gridId = GridHelper.GetTemperatureGridId(temp.StabilityTestTemperature);
                var grid = $(document.getElementById(gridId)).data("kendoGrid");
                if (grid) {
                    var data = JSON.parse(JSON.stringify(grid.dataSource.data())) ;
                    modelChilds.push(...data);
                }
            });
            model.Childs = modelChilds;
            //Child Validation
            if (!Helper.IsFrmChildsValid(model.Childs)) {
                $btn.removeClass("disabled").css("pointer-events", "").css("opacity", "").data("busy", false);
                return;
            }

            if (!Helper.IsValidImageFiles()) {
                $btn.removeClass("disabled").css("pointer-events", "").css("opacity", "").data("busy", false);
                return;
            }

            var formData = Helper.BuildSaveFormData(model);
            $.ajax({
                url: "/api/StabilityTest/save",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false
            }).done(function (data) {
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
        /*//Quantity Check
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
        }*/
        return isValidItem;
    },
    IsFrmValid: function () {
        var isValidItem = true;
        var Elements = ["FormulationMasterID"];
        if (!FormHelper.IsFormValidation(FormDom, Elements)) {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
        }
        return isValidItem;
    },
    BuildSaveFormData: function (model) {
        var formData = new FormData();
        Helper.AppendFormData(formData, model);
        $(".stability-temperature-files").each(function () {
            var temperature = $(this).data("temperature");
            var files = this.files;
            for (var i = 0; i < files.length; i++) {
                formData.append("StabilityTestFiles", files[i]);
                formData.append("StabilityTestFileTemperatures", temperature);
            }
        });
        return formData;
    },
    IsValidImageFiles: function () {
        var isValid = true;
        var allowedExtensions = ["jpg", "jpeg", "png"];

        $(".stability-temperature-files").each(function () {
            var files = this.files;
            for (var i = 0; i < files.length; i++) {
                var extension = files[i].name.split(".").pop().toLowerCase();
                if (allowedExtensions.indexOf(extension) === -1) {
                    MessageHelper.Warning("Invalid file", "Only .jpg and .png image files are allowed.");
                    $(this).val("");
                    isValid = false;
                    return false;
                }
            }
        });

        return isValid;
    },
    AppendFormData: function (formData, value, keyPrefix) {
        if (value === undefined || value === null) {
            return;
        }

        if (Array.isArray(value)) {
            $.each(value, function (index, item) {
                Helper.AppendFormData(formData, item, keyPrefix + "[" + index + "]");
            });
            return;
        }

        if (typeof value === "object") {
            $.each(value, function (key, item) {
                var formKey = keyPrefix ? keyPrefix + "." + key : key;
                Helper.AppendFormData(formData, item, formKey);
            });
            return;
        }

        formData.append(keyPrefix, value);
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
                    url: `/api/StabilityTest/list/${statusCons}`,
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
                { name: "StabilityTest", text: "StabilityTest", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },
                ];
        }
        else if (statusCons == statusConstants.EDIT || statusCons == statusConstants.REJECT) {
            command =
                [
                { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                { name: "StabilityTest", text: "Stability Test", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },

                ];
        } else {
            command =
                [
                { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                { name: "StabilityTest", text: "Stability Test", iconClass: "k-icon k-i-print", className: "k-success k-grid-button-rpt", click: GridHelper.HandleCommands },

                ];
        }

        columns = [
            {
                title: "Action", command: command, width: 180,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "StabilityTestMasterID", title: "StabilityTestMasterID", hidden: true,
            },
            {
                field: "FormulationMasterID", title: "FormulationMasterID", hidden: true,
            },
            {
                field: "FormulationCode", title: "Formulation Code", //width: 130,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "FormulationVersion", title: "Formulation Version", //width: 130,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
           
            {
                field: "BrandName", title: "Brand Name", //width: 130,
                filterable: { extra: false }
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            
            {
                field: "ProductName", title: "Product Name", //width: 180,
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
                { field: "StabilityTestTemperatureID", hidden: true, attributes: { class: "non-editable-column" }, },
                { field: "StabilityTestMasterID", hidden: true, attributes: { class: "non-editable-column" }, },
               
                {
                    field: "StabilityTestTemperature", title: "Temperature", width: 90,
                    editor: KendoGridHelper.NumberTextBox,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
               
            ];

        var schema = {
            model: {
                fields: {
                   
                    StabilityTestTemperature: { type: "number", defaultValue: null },
                    
                    //NaturalIndexPct: { type: "text", defaultValue: null },
                    //NaturalOriginIndexPct: { type: "text", defaultValue: null },
                    //OrganicIndexPct: { type: "text", defaultValue: null },
                    //OrganicOriginIndexPct: { type: "text", defaultValue: null },
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId, columns, data, schema, true, true, false);
        KendoGridHelper.EnterKeyUp(TblChildId);
        var grid = $(TblChildId).data("kendoGrid");
        grid.dataSource.bind("change", GridHelper.CreateInitChildTableDynamicGrid);
       
       
    },

    InitChildTableDynamicGrid: function (data,TableID) {
        if ($(TableID).data("kendoGrid")) $(TableID).kendoGrid('destroy').empty();
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
        KendoGridHelper.ChildGridLoad(TableID, columns, data, schema, true, true, false);
        KendoGridHelper.EnterKeyUp(TableID);
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

                    Manager.GetDetails(selectedItem.StabilityTestMasterID);
                }
                if (args.data.commandName == "StabilityTest") {
                    Manager.GetReport(selectedItem.StabilityTestMasterID,'StabilityTest');
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
    GetTemperatureGridId: function (temperature) {
        return "StabilityGrid_" + String(temperature).replace(/[^A-Za-z0-9_-]/g, "_");
    },
    GetTemperatureFileInputId: function (temperature) {
        return "StabilityFiles_" + String(temperature).replace(/[^A-Za-z0-9_-]/g, "_");
    },
    RenderTemperatureFiles: function (temperature) {
        var files = (filePathData || []).filter(function (x) {
            return x.StabilityTestTemperature == temperature;
        });

        if (files.length === 0) {
            return "";
        }

        var html = '<div class="mt-2 stability-file-list">';
        $.each(files, function (i, file) {
            html += '<span class="stability-file-item d-inline-block mr-1 mb-1" data-file-path-id="' + file.StabilityTestFilePathID + '">' +
                '<button type="button" class="btn btn-sm btn-light-primary stability-file-download" data-file-path="' + file.FilePath + '">' +
                '<i class="fa fa-download"></i> ' + (file.FileName || ("File " + (i + 1))) +
                '</button>' +
                '<button type="button" class="btn btn-sm btn-light-danger stability-file-delete ml-1" data-file-path-id="' + file.StabilityTestFilePathID + '">' +
                '<i class="fa fa-trash"></i>' +
                '</button>' +
                '</span>';
        });

        html += '</div>';
        return html;
    },
    CreateInitChildTableDynamicGrid: function (data) {
        var TblChilds = $(TblChildId).data("kendoGrid");
        var TblCDataSource = TblChilds.dataSource.data();
        var percentage = 0;
        var container = $("#dynamicGrid");
        container.empty();
        var parameterList = [
            { ParameterName: "Appearance" },
            { ParameterName: "Colour" },
            { ParameterName: "Odour" },
            { ParameterName: "Viscosity" },
            { ParameterName: "pH" },
            { ParameterName: "SG" }
        ];
       
        $.each(TblCDataSource, function (i, temp) {
            if(temp.StabilityTestTemperature !=null){
            var gridId = GridHelper.GetTemperatureGridId(temp.StabilityTestTemperature);
            var fileInputId = GridHelper.GetTemperatureFileInputId(temp.StabilityTestTemperature);
            var html = `
            <div class="stability-section" style="margin-bottom:20px;">
                <h5>Temperature: ${temp.StabilityTestTemperature}</h5>
                <div id="${gridId}"></div>
                <div class="form-group mt-2">
                    <input type="file" id="${fileInputId}" class="form-control stability-temperature-files" data-temperature="${temp.StabilityTestTemperature}" accept=".jpg,.jpeg,.png,image/jpeg,image/png" multiple />
                    ${GridHelper.RenderTemperatureFiles(temp.StabilityTestTemperature)}
                </div>
            </div>
        `;

            container.append(html);
            var gridData = parameterList.map(function (p) {
                 var existingItem = sheetData.find(function (x) {
                return x.StabilityTestTemperature == temp.StabilityTestTemperature
                    && x.ParameterName == p.ParameterName;
            });

            return {
                StabilityTestMasterID: existingItem ? existingItem.StabilityTestMasterID : 0,
                StabilityTestChildID: existingItem ? existingItem.StabilityTestChildID : 0,
                StabilityTestTemperature: temp.StabilityTestTemperature,
                ParameterName: p.ParameterName,

                T0: existingItem ? existingItem.T0 : "",
                T1: existingItem ? existingItem.T1 : "",
                T2: existingItem ? existingItem.T2 : "",
                T3: existingItem ? existingItem.T3 : "",
                T4: existingItem ? existingItem.T4 : "",
                T5: existingItem ? existingItem.T5 : "",
                T6: existingItem ? existingItem.T6 : ""
            };
            });

            


            $("#" + gridId).kendoGrid({
                dataSource: {
                    data: gridData,
                    schema: {
                        model: {
                            fields: {
                                StabilityTestChildID: { editable: false, hidden: true },
                                StabilityTestMasterID: { editable: false, hidden: true },
                                StabilityTestTemperature: { editable: false, hidden: true },
                                ParameterName: { editable: false },
                                T0: { type: "string" },
                                T1: { type: "string" },
                                T2: { type: "string" },
                                T3: { type: "string" },
                                T4: { type: "string" },
                                T5: { type: "string" },
                                T6: { type: "string" }
                            }
                        }
                    }
                },
                editable: true,
                columns: [
                    { field: "StabilityTestChildID", title: "StabilityTestChildID", width: 120 , hidden: true},
                    { field: "StabilityTestMasterID", title: "StabilityTestMasterID", width: 120, hidden: true },
                    { field: "StabilityTestTemperature", title: "StabilityTestTemperature", width: 120, hidden: true },
                    { field: "ParameterName", title: "Parameter", width: 120 },
                    { field: "T0", title: "T0" },
                    { field: "T1", title: "T1 (4 weeks)" },
                    { field: "T2", title: "T2 (8 weeks)" },
                    { field: "T3", title: "T3 (12 weeks)" },
                    { field: "T4", title: "T4 (16 weeks)" },
                    { field: "T5", title: "T5 (20 weeks)" },
                    { field: "T6", title: "T6 (24 weeks)" }
                ]
            });
            }
        });
        $(".stability-file-download").off("click").on("click", function () {
            FileUploadHelper.DownloadFile($(this).data("file-path"));
        });
        $(".stability-file-delete").off("click").on("click", function () {
            var filePathId = parseInt($(this).data("file-path-id"));
            var fileItem = $(this).closest(".stability-file-item");
            var deleteButton = $(this);

            if (!(filePathId > 0)) {
                return;
            }

            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to DELETE this file.?')
            ).then((result) => {
                if (result.isConfirmed) {
                    deleteButton.addClass("disabled").css("pointer-events", "none").css("opacity", "0.6");
                    $.ajax({
                        url: "/api/StabilityTest/file/delete",
                        type: "POST",
                        data: { stabilityTestFilePathId: filePathId }
                    }).done(function (data) {
                        if (data.IsLogin == 1) {
                            window.open(data.redirectUrl, "_blank");
                            return;
                        }

                        if (parseInt(data.ResultId) > 0) {
                            filePathData = (filePathData || []).filter(function (file) {
                                return file.StabilityTestFilePathID != filePathId;
                            });
                            fileItem.remove();
                            MessageHelper.Success("Delete Operations", data.Message);
                        } else {
                            MessageHelper.Warning("Delete Operations", data.Message);
                            deleteButton.removeClass("disabled").css("pointer-events", "").css("opacity", "");
                        }
                    }).fail(function (xhr, status, error) {
                        MessageHelper.Warning("Delete Operations", error);
                        deleteButton.removeClass("disabled").css("pointer-events", "").css("opacity", "");
                    });
                }
            });
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





