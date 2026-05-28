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


$(document).ready(function () {
    Helper.Initialize();
});

var Manager = { 
    GetNew: function (id, MenuID, UnitID) {
        $.get(`/api/csissue/new?ids=${id}&MenuID=${MenuID}&UnitID=${UnitID}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs);
            Helper.GetIssueBy(masterData.IssuedByList);
            
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id, MenuID, UnitID) {
        $.get(`/api/csissue/edit?id=${id}&MenuID=${MenuID}&UnitID=${UnitID}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();          
            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs); 
            Helper.GetIssueBy(masterData.IssuedByList);
            $("#btnPrintIndent").show();   
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    }, 
    Save: function (SaveOption) { 
        if (Helper.IsFrmValid()) {
            //var revReason = $("#ReviseReason").val();
            //var rejReason = $("#RejectReason").val();           
            //if (IsRevise == true && revReason == "") {
            //    MessageHelper.Warning("Please Input Revise Reason.");
            //    return;
            //}
            //if (IsReject == true && rejReason == "") {
            //    MessageHelper.Warning("Please Input Reject Reason.");
            //    return;
            //} 

            var model = FormHelper.FormElToJson(FormDom);
            model.Childs = JSON.parse(JSON.stringify($(TblChildId).data("kendoGrid").dataSource.data()));
            model.SaveOption = SaveOption;

            //Child Validation
            if (!Helper.IsFrmChildsValid(model.Childs)) {
                return;
            }
            $.post("/api/csissue/save", model, function (data) {
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
    RptIssue: function (id) {
        alert('Under construction.');
        //window.open(`/RptCommercialExport/csreceive?id=${id}`, "_blank");
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
        var Elements = ["UnitID", "StoreID", "IssueDate", "IssueTypeID", "IssuedBy"];
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

        if (isApprovePage) {
            ToolbarDom.find("#btnPendingApproveList,#btnApprovedList,#btnRejectList,#btnAllList").show();
            ToolbarDom.find("#btnPendingList,#btnEditList").hide();
            statusCons = statusConstants.PROPOSED_FOR_APPROVAL;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnPendingApproveList"), ToolbarDom);
            GridManager.InitMasterTable(); 
        } else {
            ToolbarDom.find("#btnPendingList,#btnEditList,#btnPendingApproveList,#btnApprovedList,#btnRejectList,#btnAllList").show();
            statusCons = statusConstants.PENDING;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnPendingList"), ToolbarDom);
            GridManager.InitMasterTable();  
        } 

        // ToolbarDom means Toolbar Buttons  
        ToolbarDom.find("#btnPendingList").on("click", function (e) {
            statusCons = statusConstants.PENDING;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable(); 
        });
        ToolbarDom.find("#btnEditList").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable(); 
        }); 
        ToolbarDom.find("#btnPendingApproveList").on("click", function (e) {
            statusCons = statusConstants.PROPOSED_FOR_APPROVAL;
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
        $("#btnSave").on("click", function (e) {
            var SaveOption = Helper.GetSaveOption(1);
            Manager.Save(SaveOption);            
        });
        $("#btnSaveForApprove").on("click", function (e) {
            var SaveOption = Helper.GetSaveOption(6);
            Manager.Save(SaveOption);
        });
        $("#btnApproved").on("click", function (e) { 
            var SaveOption = Helper.GetSaveOption(3);
            Manager.Save(SaveOption);
        });  
        $("#btnRevise").on("click", function (e) {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Revise this Receive?')
            ).then((result) => {
                if (result.isConfirmed) {
                    Manager.Save(5);
                }
            })
        }); 
        $("#btnReject").on("click", function (e) {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Reject this Receive?')
            ).then((result) => {
                if (result.isConfirmed) {
                    Manager.Save(4);
                }
            })
        });  

        $("#btnBackToList").on("click", function () {
            Helper.BackToList();
        });
        $("#btnPrintPI").on("click", function () {
            var piid = $("#IssueMasterID").val();
            if (piid) {
                Manager.RptIssue(piid);
            }
        }); 

        //Selected Row in the kendo Grid
        KendoGridHelper.GridRowSelected(TblChildId);
    },
    GetIssueBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" },
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#IssuedBy", "text", "id", columns);
    }, 
    GetSaveOption: function (value) {
        var SaveOption = 0;
        var id = $("#IssueMasterID").val();
        id = parseInt(id);
        if ((value == 1) && (id == 0)) {SaveOption = 1;} // New save
        if ((value == 1) && (id > 0)) {SaveOption = 2;} // Update
        if ((value == 6) && (id == 0)) {SaveOption = 6;} // New save and sent for approve
        if ((value == 6) && (id > 0)) { SaveOption = 7; }// Update and sent for approve
        //if ((value = 3) && (id > 0)) { SaveOption = 3; }
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
                    url: `/api/csissue/list/${statusCons}`,
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
        var columns;
        var search;
        var commandWidth = 80;

        if (isApprovePage) {
            if (statusCons == statusConstants.PROPOSED_FOR_APPROVAL) {
                command =
                    [
                        { name: "edit", text: "", iconClass: "k-icon k-i-plus", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            } else {
                command =
                    [
                        { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            }
        } else {
            if (statusCons == statusConstants.PENDING) {
                command =
                    [
                        { name: "edit", text: "", iconClass: "k-icon k-i-plus", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            }
            else if (statusCons == statusConstants.EDIT || statusCons == statusConstants.REJECT) {
                command =
                    [
                        { name: "edit", text: "", iconClass: "k-icon k-i-plus", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            }
            else {
                command =
                    [
                        { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            }
        } 
        if (statusCons == statusConstants.PENDING) {
            columns = [
                {
                    title: "Action", command: command, width: 60
                },
                {
                    field: "RequirementMasterID", title: "RequirementMasterID", hidden: true,
                },
                {
                    field: "RequirementNo", title: "Requirement No", width: 120, filterable: true, attributes: { "style": "font-weight:700" },
                },
                {
                    field: "RequirementDate", title: "RequirementDate", width: 100,
                    template: "#= (RequirementDate == null)? '' : kendo.toString(kendo.parseDate(RequirementDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "RequirementByName", title: "Require By", width: 100,
                },
                {
                    field: "UnitName", title: "Unit", width: 120,
                },
                {
                    field: "DepartmentName", title: "Department", width: 120,
                },
                {
                    field: "SectionName", title: "Section", width: 120,
                },
                {
                    field: "SubSectionName", title: "Sub Section", width: 120,
                },
                {
                    field: "StoreName", title: "Store", width: 120,
                },
            ];
        } else {
            columns = [
                {
                    title: "Action", command: command, width: 60
                },
                {
                    field: "IssueMasterID", title: "IssueMasterID", hidden: true,
                },
                {
                    field: "IssueNo", title: "Issue No", width: 140,
                    filterable: true, attributes: { "style": "font-weight:700" },
                },
                {
                    field: "IssueDate", title: "Issue Date", width: 75, 
                    template: "#= (IssueDate == null)? '' : kendo.toString(kendo.parseDate(IssueDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "RequirementNo", title: "Requirement No", width: 100,
                },
                {
                    field: "IssueTypeName", title: "Issue Type", width: 100,
                },
                {
                    field: "UnitName", title: "Unit", width: 120,
                },
                {
                    field: "StoreName", title: "Store", width: 120,
                },
                {
                    field: "DepartmentName", title: "Department", width: 120,
                },
                {
                    field: "SectionName", title: "Section", width: 120,
                },
            ];
        }
        
        if (statusCons == statusConstants.PENDING) {
            search =
            {
                fields:
                    [
                        "RequirementNo", "RequirementDate", "RequirementBy"
                    ]
            };
        } else {
            search =
            {
                fields:
                    [
                        "IssueNo", "IssueDate", "IssueTypeName", "RequirementNo"
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
                    title: "Action&nbsp;",
                    width: 70,
                    command: [
                        {
                            name: "delete1",
                            text: "",
                            iconClass: "k-icon k-i-delete",
                            className: "k-danger",
                            click: GridHelper.ChildCommandClick
                        },
                        {
                            name: "rawClone",
                            text: "",
                            iconClass: "k-icon k-i-copy k-grid-button-copy",
                            className: "k-success",
                            click: function (e) {
                                e.preventDefault();
                                const grid = $(TblChildId).data("kendoGrid");
                                if (!grid) return;
                                const $tr = $(e.currentTarget).closest("tr");
                                const sourceItem = grid.dataItem($tr);
                                if (!sourceItem) return;
                                const clone = sourceItem.toJSON ? sourceItem.toJSON() : $.extend({}, sourceItem);
                                clone.IssueDetailID = 0;
                                clone.IssueQty = 0;
                                clone.ChildRemarks = '';
                                clone.__justCloned = true;
                                const ds = grid.dataSource;
                                const insertIndex = ds.indexOf(sourceItem) + 1;
                                ds.insert(insertIndex, clone);   // creates a new DataSource item
                                // grid.refresh();  // not necessary; insert triggers rebind
                            }
                        }
                    ]
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-center", true, true)
                }, 
                {
                    field: "IssueDetailID", text: "CID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                { field: "IssueMasterID", hidden: true },
                { field: "IndentMasterID", hidden: true },
                { field: "IndentChildID", hidden: true },
                { field: "RequirementChildID", hidden: true },
                { field: "ReceiveChildID", hidden: true },
                {
                    field: "RowSLNo", title: "SL#", template: "#= ++gbGridSLNo #", width: 40,
                    editable: function () { return false; },
                },  
                {
                    field: "objProductList", title: "Chemicals (Require)", width: 140,
                    editor: GridHelper.cboProduct,
                    editable: function () { return true; },
                    template: "#=objProductList != null ? objProductList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objUOMList", title: "UOM", width: 50,
                    editor: GridHelper.cboUOM,
                    editable: function () { return true; },
                    template: "#=objUOMList != null ? objUOMList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objReceiveChildList", title: "Chemicals (Issue)", width: 180,
                    editor: GridHelper.cboReceiveChildList,
                    template: "#=(objReceiveChildList && objReceiveChildList.text) ? objReceiveChildList.text : ''#"
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-center", true, false)
                },
                {
                    field: "BatchNo", title: "Batch No", width: 100,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                }, 
                {
                    field: "IndentNo", title: "Indent No", width: 100,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                }, 
                {
                    field: "StockQty", title: "Stock Qty", width: 70, editable: function () { return false; },
                    footerTemplate: "<span id='sumStockQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "RequirementQty", title: "Require. Qty", width: 60, editable: function () { return false; },
                    footerTemplate: "<span id='sumRequirementQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "IssueQty", title: "Issue Qty</br>New/Edit", width: 70,
                    editable: function () { return true; },
                    footerTemplate: "<span id='sumIssueQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },                
                {
                    field: "SectionName", title: "Section", width: 80,
                    editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ChildRemarks", title: "Remarks", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                }, 
            ];

        var schema = {
            model: {
                fields: {
                    IssueDetailID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    IndentMasterID: { type: "number", editable: function () { return false; } },
                    IndentChildID: { type: "number", editable: function () { return false; } },
                    RequirementChildID: { type: "number", editable: function () { return false; } },
                    ReceiveChildID: { type: "number", editable: function () { return false; } },
                    IndentNo: { type: "string", editable: function () { return false; } },
                    objProductList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    }, 
                    objUOMList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    }, 
                    objReceiveChildList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    }, 
                    BatchNo: { type: "string" },
                    StockQty: { type: "number", editable: function () { return false; } },
                    RequirementQty: { type: "number", validation: { min: 0 }, defaultValue: null }, 
                    IssueQty: { type: "number", validation: { min: 0 }, defaultValue: null }, 
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

        $(".k-toolbar", TblChildId).hide();
    },
}

var GridHelper = {
    // Actions
    HandleCommands: function (args) {
        args.preventDefault();
        var tr = $(args.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);

        if (selectedItem != null) {
            $("#btnSave,#btnSaveForApprove,#btnApproved,#btnRevise,#btnReject").hide();
            $("#divReviseNote,#divRejectNote").hide();

            childsActionBtn = true;
            childsToolsBar = false;  

            if (isApprovePage) {
                if (statusCons == statusConstants.PROPOSED_FOR_APPROVAL) {
                    $("#btnApproved").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                }
                Manager.GetDetails(selectedItem.IssueMasterID);
            } else { 
                if (statusCons == statusConstants.PENDING) {
                    $("#btnSave,#btnSaveForApprove").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                    Manager.GetNew(selectedItem.RequirementMasterID, menuId,selectedItem.UnitID);
                } else {
                    if (statusCons == statusConstants.EDIT) {
                        $("#btnSave,#btnSaveForApprove").show();
                        childsActionBtn = false;
                        childsToolsBar = true;
                    }
                    else if (statusCons == statusConstants.PROPOSED_FOR_APPROVAL) {
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
                    Manager.GetDetails(selectedItem.IssueMasterID, menuId, selectedItem.UnitID);
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
                if (e.field === "IssueQty") {
                    GridHelper.GetSumTotalQty();
                }
            }
            GridHelper.GetSumTotalQty();
        });
        GridHelper.GetSumTotalQty();
    },
    GetSumTotalQty: function () {
        TblChilds = $(TblChildId).data("kendoGrid");
        TblCDataSource = TblChilds.dataSource.data();
        var totalStockQty = 0;
        var totalRequirementQty = 0;
        var totalIssueQty = 0;

        for (var i = 0; i < TblCDataSource.length; i++) {
            //totalStockQty += kendo.parseFloat(TblCDataSource[i].StockQty);
            totalRequirementQty += kendo.parseFloat(TblCDataSource[i].RequirementQty);  
            totalIssueQty += kendo.parseFloat(TblCDataSource[i].IssueQty);  
        }
        //$("#sumStockQty").text(totalStockQty.toFixed(2));
        $("#sumRequirementQty").text(totalRequirementQty.toFixed(2));
        $("#sumIssueQty").text(totalIssueQty.toFixed(2));

    },  
    //GetSumIndentQty: function () {
    //    TblChilds = $(TblChildId).data("kendoGrid");
    //    TblCDataSource = TblChilds.dataSource.data();
    //    var gcIndentQty = 0;

    //    for (var i = 0; i < TblCDataSource.length; i++) {
    //        gcIndentQty += kendo.parseFloat(TblCDataSource[i].IssueQty);
    //    }
    //    $("#sumIndentQty").text(gcIndentQty.toFixed(2));
    //},  
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
    cboReceiveChildList: function (container, options) {

        var itemName = "";
        var unitID = $("#UnitID").val();
        var primaryKeyID = $("#IssueMasterID").val();
        var whereClause = "";
        if (options.model.objReceiveChildList) {
            itemName = options.model.objReceiveChildList.text;
        }
        var grid = $(TblChildId).data("kendoGrid");
        var selectedItem = grid.dataItem(grid.select());
        if (selectedItem) {
            //selectedItem.set('BatchNo', productCmb.dataItem().text);
            whereClause = selectedItem.objProductList.id;
        }
        $('<input id="cmbReceiveChildList" name="cmbReceiveChildList" required data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoMultiColumnComboBox({
                filter: "contains",
                minLength: 3,
                autoWidth: true,
                dataTextField: "text",
                dataValueField: "id",
                height: 500,
                columns: [
                    { field: "text", title: "Chemical", width: "300" },                    
                    { field: "valueI", title: "Batch", width: "100" },
                    { field: "valueC", title: "Stock Qty", width: "100" },
                    { field: "valueB", title: "UOM", width: "100" },
                    { field: "valueH", title: "Indent", width: "150" },
                    { field: "valueJ", title: "ReceiveChildID", width: "150" },
                ],
                dataSource: {
                    type: "json",
                    serverFiltering: true,
                    transport: {
                        read: {
                            type: "GET",
                            url: `/api/csissue/GetProductBySearch?itemName=${encodeURIComponent(itemName)}&menuID=${menuId}&unitID=${unitID}&primaryKeyID=${primaryKeyID}&whereClause=${whereClause}`,
                            dataType: "json",
                            contentType: 'application/json',
                            data: function () {
                                return {
                                    searchKey: `${$("#cmbReceiveChildList").data("kendoMultiColumnComboBox").input.val()}`,  // Get filter text from input
                                    pageIndex: 1,
                                    pageSize: 400
                                };
                            }
                        },
                    }
                },
                index: -1,
                change: function (e) {
                    var productCmb = $("#cmbReceiveChildList").data("kendoMultiColumnComboBox");
                    if (parseInt(productCmb.value()) > 0) {
                        //var grid = $(TblChildId).data("kendoGrid");
                        //var selectedItem = grid.dataItem(grid.select());
                        if (selectedItem) {
                            selectedItem.set('BatchNo', productCmb.dataItem().valueI);
                            selectedItem.set('StockQty', productCmb.dataItem().valueC);
                            selectedItem.set('IndentNo', productCmb.dataItem().valueH);
                        }

                    }
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


