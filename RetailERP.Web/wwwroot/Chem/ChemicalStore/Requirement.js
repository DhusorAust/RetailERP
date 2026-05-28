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
        $.get(`/api/csreq/new`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut(); 
            masterData = data; 
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable([]); 
            $("#StoreID").data("kendoComboBox").value("1");
            $("#UnitID").data("kendoComboBox").value("1");
            Helper.GetRequirementBy(masterData.RequirementByList);
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id,UnitID,MenuID,UserID) {
        $.get(`/api/csreq/edit?id=${id}&UnitID=${UnitID}&MenuID=${MenuID}&UserID=${UserID}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();          
            masterData = data; 
            FormHelper.SetFormData(FormDom, masterData);            
            GridManager.InitChildTable(masterData.Childs); 
            Helper.GetRequirementBy(masterData.RequirementByList);
            $("#btnPrintPR").show();   
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    }, 
    Parameter: function (SaveOption) {
        var id = 0;
        id = $("#RequirementMasterID").val();            
        var obj =
        {
            RequirementMasterID: id,
            id: id,
            MenuID: menuId,
            PageName: pageName,
            SaveOption: SaveOption
        };
        return obj;
    },
    GetSaveOption: function (value) {
        var SaveOption = 0;
        var vID = $("#RequirementMasterID").val();
        vID = parseInt(vID);
        if ((value == 1) && (vID == 0)) { SaveOption = 1; }
        if ((value == 1) && (vID > 0)) { SaveOption = 2; }
        if ((value == 8) && (vID == 0)) { SaveOption = 8; }        
        if ((value == 8) && (vID > 0)) { SaveOption = 9; }
        if ((value == 10) && (vID > 0)) { SaveOption = 10; }
        return SaveOption;
    },

    Save: function (SaveOption) {
        debugger;
        if (Helper.IsFrmValid()) {
            var model = FormHelper.FormElToJson(FormDom);
            model.Childs = JSON.parse(JSON.stringify($(TblChildId).data("kendoGrid").dataSource.data()));
            model.Parameter = Helper.Parameter(SaveOption);
            model.DepartmentID = $("#DepartmentID").val();
            model.SectionID = $("#SectionID").val();
            model.SubSectionID = $("#SubSectionID").val(); 
            model.SaveOption = SaveOption; 

            var revReason = $("#ReviseReason").val();
            var rejReason = $("#RejectReason").val();
           
            if ((SaveOption == 5) ||(SaveOption == 10) && revReason == "") {
                //MessageHelper.Warning("Please Input Revise Reason.");
                ToastrMessageHelper.ValidationToastr("Please Input Revise Reason.");
                $("#ReviseReason").focus();
                return;
            }
            if (SaveOption == 4 && rejReason == "") {
                //MessageHelper.Warning("Please Input Reject Reason.");
                ToastrMessageHelper.ValidationToastr("Please Input Reject Reason.");
                $("#RejectReason").focus();
                return;
            } 

            //Child Validation
            if (!Helper.IsFrmChildsValid(model.Childs)) {
                return;
            }
            $.post("/api/csreq/save", model, function (data) {
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
        window.open(`/RptChemicalStore/Chem_Requirement?id=${id}`, "_blank");
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
                MessageHelper.Warning("Warning", "Chemical Items is required.");
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
        var Elements = ["UnitID", "StoreID", "RequirementDate", "RequirementBy"];
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
        ToolbarDom.find("#btnNewAdd,#btnEditList,#btnApprovedList,#btnRejectList,#btnAllList").show();
        $("#divReviseNote").hide();
        $("#divRejectNote").hide();

        statusCons = statusConstants.EDIT;
        FormHelper.TogToolbarBtn(ToolbarDom.find("#btnEditList"), ToolbarDom);
        GridManager.InitMasterTable();

        // ToolbarDom means Toolbar Buttons  
        ToolbarDom.find("#btnNewAdd").on("click", function (e) {
            //return;
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnEditList"), ToolbarDom);
            Helper.ResetControls();
            Manager.GetNew();
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

        $("#btnSave").click(function (e) {
            var SaveOption = Manager.GetSaveOption(1);
            Manager.Save(SaveOption);            
        });
        $("#btnApproved").click(function (e) {
            var SaveOption = Manager.GetSaveOption(8);
            Manager.Save(SaveOption);
        });
        $("#btnRevise").click(function (e) {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Revise this Requirement?')
            ).then((result) => {
                if (result.isConfirmed) {
                    //Manager.Save(5);
                    Manager.Save(10);
                }
            })
        });
        $("#btnReject").click(function (e) {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Reject this Requirement?')
            ).then((result) => {
                if (result.isConfirmed) {
                    Manager.Save(4);
                }
            })
        });

        $("#btnBackToList").on("click", function () {
            Helper.BackToList();
        });
        $("#btnPrintPI").click(function () {
            var piid = $("#PINo").val();
            if (piid) {
                Manager.GetPIReport(piid);
            }
        });

        //Change Event
        $("#RequirementBy").on("change", function () {

            var cboPRBy = $(this).data("kendoMultiColumnComboBox");
            var PRBy = cboPRBy ? cboPRBy.dataItem() : null;

            if (!PRBy) {
                $("#DepartmentID").val('');
                $("#SectionID").val('');
                $("#SubSectionID").val('');
                return;
            }
            $("#DepartmentID").val(PRBy.valueA);
            $("#SectionID").val(PRBy.valueB);
            $("#SubSectionID").val(PRBy.valueC);
            $("#DepartmentName").val(PRBy.valueD);
            $("#SectionName").val(PRBy.valueE);
            $("#SubSectionName").val(PRBy.valueF);
        });

        //Selected Row in the kendo Grid
        KendoGridHelper.GridRowSelected(TblChildId);
    },
    GetRequirementBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "300" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "100" },
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#RequirementBy", "text", "id", columns);
    },
    ResetControls: function () {
        $("#btnBackToList,#btnSave,#btnApproved").show();
        $("#btnRevise,#btnReject").hide();

    },
    Parameter: function (SaveOption) {
        var id = 0;
        id = $("#RequirementMasterID").val();
        var obj =
        {
            RequirementMasterID: id,
            id: id,
            MenuID: menuId,
            PageName: pageName,
            SaveOption: SaveOption
        };
        return obj;
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
                    url: `/api/csreq/list/${statusCons}`,
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
       
        if (statusCons == statusConstants.EDIT || statusCons == statusConstants.REJECT) {
            command =
                [
                    { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                    { name: "rptPR", text: "Requirement", iconClass: "k-icon k-i-print k-grid-button-rpt", className: "k-success", click: GridHelper.HandleCommands },
                ];
        } else {
            command =
                [
                    { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                { name: "rptPR", text: "Requirement", iconClass: "k-icon k-i-print k-grid-button-rpt", className: "k-success", click: GridHelper.HandleCommands },
                ];
        } 
        var columns = [
            {
                title: "Action", command: command, width: 100
            }, 
            {
                field: "RequirementMasterID", title: "RequirementMasterID", hidden: true,
            },
            {
                field: "StoreName", title: "Store", width: 120,
            }, 
            {
                field: "RequirementNo", title: "Requirement No", width: 80, filterable: true, attributes: { "style": "font-weight:700" },
            },
            {
                field: "RequirementDate", title: "Requirement Date", width: 70,
                template: "#= (RequirementDate == null)? '' : kendo.toString(kendo.parseDate(RequirementDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
            }, 
            {
                field: "RequirementByName", title: "Requirement By", width: 100,  
            }, 
            {
                field: "Remarks", title: "Remarks", width: 120,
            },
        ];
        var search =
        {
            fields:
                [
                    "RequirementNo", "RequirementDate", "RequirementByName"
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
                    }], title: "Action &nbsp;", width: 40,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                }, 
                {
                    field: "RequirementChildID", hidden: true, width:100
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "UOMID", hidden: true, width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "RowSLNo", title: "SL#", template: "#= ++gbGridSLNo #", width: 40,
                    editable: function () { return false; },
                },
                {
                    field: "objProductList", title: "Chemical Items", width: 120,
                    editor: GridHelper.cboProduct,
                    template: "#=objProductList != null ? objProductList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                
                {
                    field: "objUOMList", title: "UOM", width: 120,
                    editor: GridHelper.cboUOM,
                    template: "#=objUOMList != null ? objUOMList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "StockQty", title: "Stock Qty", width: 80, editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "RequirementQty", title: "Qty", width: 80,
                    footerTemplate: "<span id='sumRequirementQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "objSectionList", title: "Section", width: 120,
                    editor: GridHelper.cboSection,
                    template: "#=objSectionList != null ? objSectionList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "ChildsRemarks", title: "Remarks", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                }, 
            ];

        var schema = {
            model: {
                fields: {
                    IndentChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    UOMID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    objProductList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true } 
                    },
                    objUOMList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    UOM: { type: "string", editable: function () { return false; } },
                    StockQty: { type: "number", editable: function () { return false; } },
                    RequirementQty: { type: "number", validation: { min: 0 }, defaultValue: null }, 
                    objSectionList: {
                        defaultValue: { id: null, text: "" },
                    }, 
                    ChildsRemarks: { type: "string"}, 
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
                Manager.RptPR(selectedItem.RequirementMasterID);
                return;
            }
            else if (statusCons == statusConstants.EDIT) {
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
            Manager.GetDetails(selectedItem.RequirementMasterID, 0, menuId,0);
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
                if (e.field == "objProductList") { 
                    selectedItem.set('UOM', obj.objProductList.valueB); 
                    selectedItem.set('UOMID', obj.objProductList.valueA); 
                    selectedItem.set('StockQty', obj.objProductList.valueC); 
                } 
                if (e.field === "RequirementQty") {
                    GridHelper.GetSumRequirementQty();
                }
            }
            GridHelper.GetSumRequirementQty();
        });
        GridHelper.GetSumRequirementQty();
    },
    GetSumRequirementQty: function () {
        TblChilds = $(TblChildId).data("kendoGrid");
        TblCDataSource = TblChilds.dataSource.data();
        var gcPRQty = 0;
        debugger;
        for (var i = 0; i < TblCDataSource.length; i++) { 
            gcPRQty += kendo.parseFloat(TblCDataSource[i].RequirementQty);  
        }
        $("#sumRequirementQty").text(gcPRQty.toFixed(2)); 
    },   
    cboProduct: function (container, options) {
        $('<input id="cmbProduct" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.ProductList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                    //this.value()
                },
            });
    },
    cboSection: function (container, options) {
        $('<input id="cmbSection" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.SectionList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboUOM: function (container, options) {
        $('<input id="cmbSection" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
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
    GridCenterAlignAttributes: function (align_header = "", align_body = "", align_footer = "") {
        return {
            headerAttributes: { "class": align_header + " k_input_cell" },
            attributes: { "class": align_body + " k_input_cell" },
            footerAttributes: { "class": align_footer + " k_input_cell" }
        };
    },
}


