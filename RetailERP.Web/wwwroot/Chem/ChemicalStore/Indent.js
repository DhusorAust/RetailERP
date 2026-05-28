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
    GetNew: function (id) { 
        $.get(`/api/csindent/new?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut(); 
            
            masterData = data; 
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs);  
            Helper.GetIndentBy(masterData.IndentByList); 
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id) {
        $.get(`/api/csindent/edit?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut(); 
         
            masterData = data; 
            Helper.GetIndentBy(masterData.IndentByList); 
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs); 
            $("#btnPrintIndent").show();   
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    }, 
    Save: function (IsSendForApproval,IsApproved, IsRevise, IsReject) { 
        if (Helper.IsFrmValid()) {
            var model = FormHelper.FormElToJson(FormDom);
            model.Childs = JSON.parse(JSON.stringify($(TblChildId).data("kendoGrid").dataSource.data()));
            model.SendForApproval = IsSendForApproval;
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
            $.post("/api/csindent/save", model, function (data) {
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
    RptIndent: function (id) {
        window.open(`/RptCommercialExport/csindent?id=${id}`, "_blank");
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
        var Elements = ["UnitID", "StoreID", "IndentDate", "IndentBy", "IndentTypeID"];
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
            IsSendForApproval = false, IsApproved = false, IsRevise = false, IsReject = false;
            Manager.Save(IsSendForApproval, IsApproved, IsRevise, IsReject);
        });
        $("#btnSaveForApprove").on("click", function (e) {
            IsSendForApproval = true, IsApproved = false, IsRevise = false, IsReject = false;
            Manager.Save(IsSendForApproval, IsApproved, IsRevise, IsReject);
        });
        $("#btnApproved").on("click", function (e) { 
            IsSendForApproval = false, IsApproved = true, IsRevise = false, IsReject = false;
            Manager.Save(IsSendForApproval, IsApproved, IsRevise, IsReject);
        });  
        $("#btnRevise").on("click", function (e) {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Revise this Requisition?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsSendForApproval = false, IsApproved = false, IsRevise = true, IsReject = false;
                    Manager.Save(IsApproved, IsRevise, IsReject);
                }
            })
        }); 

        $("#btnReject").on("click", function (e) {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Reject this Requisition?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsSendForApproval = false, IsApproved = false, IsRevise = false, IsReject = true;
                    Manager.Save(IsApproved, IsRevise, IsReject);
                }
            })
        });  

        $("#btnBackToList").on("click", function () {
            Helper.BackToList();
        });
        $("#btnPrintPI").on("click", function () {
            var piid = $("#IndentMasterID").val();
            if (piid) {
                Manager.RptIndent(piid);
            }
        }); 

        //Selected Row in the kendo Grid
        KendoGridHelper.GridRowSelected(TblChildId);
    },
    GetIndentBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" }, 
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#IndentBy", "text", "id", columns);
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
                    url: `/api/csindent/list/${statusCons}`,
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

        debugger

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
        }
        else {
            if (statusCons == statusConstants.PENDING) {
                command =
                    [
                        { name: "add", text: "", iconClass: "k-icon k-i-plus", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            }
            else if (statusCons == statusConstants.EDIT || statusCons == statusConstants.REJECT) {
                command =
                    [
                        { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
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
                    field: "PRMasterID", title: "PRMasterID", hidden: true,
                },
                {
                    field: "SSReturnMasterID", title: "SSReturnMasterID", hidden: true,
                },

                {
                    field: "StoreName", title: "Store", width: 120,
                },
                {
                    field: "PRNo", title: "Requisition No", width: 120, filterable: true, attributes: { "style": "font-weight:700" },
                },
                {
                    field: "PRDate", title: "Requisition Date", width: 100,
                    template: "#= (PRDate == null)? '' : kendo.toString(kendo.parseDate(PRDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "PRByName", title: "Requisition By", width: 100,
                },
                {
                    field: "ConcernPerson", title: "Concern Person", width: 100,
                },
                {
                    field: "PRRemarks", title: "Remarks", width: 120,
                },
            ];
        } else {
            columns = [
                {
                    title: "Action", command: command, width: 60
                },
                {
                    field: "IndentMasterID", title: "IndentMasterID", hidden: true,
                },
                {
                    field: "PRMasterID", title: "PRMasterID", hidden: true,
                },
                {
                    field: "SSReturnMasterID", title: "SSReturnMasterID", hidden: true,
                },
                {
                    field: "IndentTypeID", title: "IndentTypeID", hidden: true,
                }, 
                {
                    field: "StoreName", title: "Store", width: 100,
                },
                {
                    field: "IndentNo", title: "Indent No", width: 140,
                    filterable: true, attributes: { "style": "font-weight:700" },
                },
                {
                    field: "IndentDate", title: "Indent</br>Date", width: 75, 
                    template: "#= (IndentDate == null)? '' : kendo.toString(kendo.parseDate(IndentDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "PRNo", title: "Requisition No", width: 140, filterable: true,
                    attributes: { "style": "font-weight:700" },
                },
                {
                    field: "PRDate", title: "Requisition</br>Date", width: 100,
                    template: "#= (PRDate == null)? '' : kendo.toString(kendo.parseDate(PRDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "IndentTypeName", title: "Indent</br>Type", width: 100,
                },
                {
                    field: "IndentByName", title: "Indented</br>By", width: 100,
                },
                {
                    field: "PRByName", title: "Requisition</br>By", width: 120,
                },
                {
                    field: "IndentRemarks", title: "Remarks", width: 120,
                },
            ];
        }
        
        if (statusCons == statusConstants.PENDING) {
            search =
            {
                fields:
                    [
                        "PRNo", "PRDate", "PRByName"
                    ]
            };
        } else {
            search =
            {
                fields:
                    [
                        "IndentNo", "PRNo", " IndentDate", "PRDate", "IndentByName", "PRByName", "IndentTypeName"
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
                    }], title: "Action &nbsp;", width: 40,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                }, 
                {
                    field: "IndentChildID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                { field: "PRMasterID", hidden: true },
                {
                    field: "RowSLNo", title: "SL#", template: "#= ++gbGridSLNo #", width: 40,
                    editable: function () { return false; },                },  
                {
                    field: "objProductList", title: "Chemical Items", width: 120,
                    editor: GridHelper.cboProduct,
                    editable: function () { return false; },
                    template: "#=objProductList != null ? objProductList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "UOM", title: "UOM", width: 60,
                    editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ValidityDate", title: "Validity", width: 100,
                    template: "#= kendo.toString(ValidityDate,'dd-MMM-yyyy')#"
                }, 
                {
                    field: "StockQty", title: "Stock Qty", width: 80, editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "PRQty", title: "Requisition Qty", width: 80,
                    editable: function () { return false; },
                    footerTemplate: "<span id='sumPRQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "IndentQty", title: "Indent Qty", width: 80,
                    footerTemplate: "<span id='sumIndentQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },  
                {
                    field: "SectionName", title: "Section", width: 60,
                    editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "IndentChildRemarks", title: "Remarks", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                }, 
            ];

        var schema = {
            model: {
                fields: {
                    IndentChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    PRMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    objProductList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    }, 
                    UOM: { type: "string", editable: function () { return false; } },
                    ValidityDate: {
                        type: "date",
                        template: '#=kendo.toString("dd-MMM-yyyy")#',
                        editable: true
                    },
                    StockQty: { type: "number", editable: function () { return false; } },
                    PRQty: { type: "number", validation: { min: 0 }, defaultValue: null }, 
                    IndentQty: { type: "number", validation: { min: 0 }, defaultValue: null }, 
                    objSectionList: {
                        defaultValue: { id: null, text: "" },
                    }, 
                    IndentChildRemarks: { type: "string"}, 
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
                Manager.GetDetails(selectedItem.IndentMasterID);
            } else { 
                if (statusCons == statusConstants.PENDING) {
                    $("#btnSave,#btnSaveForApprove").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                    Manager.GetNew(selectedItem.PRMasterID);
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
                    Manager.GetDetails(selectedItem.IndentMasterID);
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
                if (e.field === "PRQty") {
                    GridHelper.GetSumPRQty();
                }
                else if (e.field === "IndentQty") {
                    GridHelper.GetSumIndentQty();
                }
            }
            GridHelper.GetSumPRQty();
            GridHelper.GetSumIndentQty();
        });
        GridHelper.GetSumPRQty();
        GridHelper.GetSumIndentQty();
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
    GetSumIndentQty: function () {
        TblChilds = $(TblChildId).data("kendoGrid");
        TblCDataSource = TblChilds.dataSource.data();
        var gcIndentQty = 0;

        for (var i = 0; i < TblCDataSource.length; i++) {
            gcIndentQty += kendo.parseFloat(TblCDataSource[i].IndentQty);
        }
        $("#sumIndentQty").text(gcIndentQty.toFixed(2));
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


