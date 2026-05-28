/// <reference path="../../../common/js/utilitycommon.js" />

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
var isApprovePage = false;

$(document).ready(function () {
    Helper.Initialize();
});

var Manager = { 
    GetNew: function (id) {   
        $.get(`/api/fillprode/new?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut(); 
            
            masterData = data;  
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable([]);  
            Helper.GetProductionBy(masterData.ProductionByList); 

        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id) {
   
        $.get(`/api/fillprode/edit?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut(); 
         
            masterData = data; 
             
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs); 
            Helper.GetProductionBy(masterData.ProductionByList);  
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
            $.post("/api/fillprode/save", model, function (data) {
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
        window.open(`/RptCommercialExport/fillprode?id=${id}`, "_blank");
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
            if (obj.objPackagingTypeList.id == 0 || obj.objPackagingTypeList.text == null) {
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
        var Elements = ["UnitID", "StoreID", "PRDate", "ProductionBy"];
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
        statusCons = statusConstants.PENDING;
        FormHelper.TogToolbarBtn(ToolbarDom.find("#btnPendingList"), ToolbarDom);
        GridManager.InitMasterTable();  
 
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
        $("#btnPrintPI").click(function () {
            var piid = $("#PINo").val();
            if (piid) {
                Manager.GetPIReport(piid);
            }
        });

        //Change Event
        $("#PRBy").on("change", function () {
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
        });  

        //Selected Row in the kendo Grid
        KendoGridHelper.GridRowSelected(TblChildId);
    },
    GetProductionBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" }, 
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#ProductionBy", "text", "id", columns);
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
                    url: `/api/fillprode/list/${statusCons}`,
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
        var search = []; 
         
        if (statusCons == statusConstants.PENDING) {
            command =
                [
                    { name: "edit", text: "", iconClass: "k-icon k-i-plus", className: "k-success", click: GridHelper.HandleCommands },
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

        if (statusCons == statusConstants.PENDING) {
            columns = [
                {
                    title: "Action", command: command, width: 60
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ProductionMasterID", title: "ProductionMasterID", hidden: true,
                },
                {
                    field: "PONo", title: "PO No", width: 160
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "PODate", title: "PO Date", width: 120,
                    template: "#= (PODate == null)? '' : kendo.toString(kendo.parseDate(PODate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ChemicalCode", title: "Chemical</br>Code", width: 160
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "BuyerRef", title: "Brand Ref.", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "LotNo", title: "Lot No", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "BatchNo", title: "Batch No", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "FinishedProductName", title: "Product</br>Name", width: 110
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "ProductionDate", title: "Production</br>Date", width: 120,
                    template: "#= (ProductionDate == null)? '' : kendo.toString(kendo.parseDate(ProductionDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ProductionQty", title: "Batch</br>Qty", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "ProductionByName", title: "Production</br>By", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "OrderMode", title: "Order Mode", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
            ];

            search =
            {
                fields:
                    [
                        "PONo", "PODate", "ChemicalCode", "BuyerRef", "LotNo", "BatchNo", "FinishedProductName",
                        "ProductionDate", "ProductionByName", "OrderMode"
                    ]
            };
        } else {
            columns = [
                {
                    title: "Action", command: command, width: 60
                },
                {
                    field: "ProductionMasterID", title: "ProductionMasterID", hidden: true,
                },
                {
                    field: "PONo", title: "PO No", width: 160
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "PODate", title: "PO Date", width: 120,
                    template: "#= (PODate == null)? '' : kendo.toString(kendo.parseDate(PODate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ChemicalCode", title: "Chemical</br>Code", width: 160
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "BuyerRef", title: "Brand Ref.", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "LotNo", title: "Lot No", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "BatchNo", title: "Batch No", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "FinishedProductName", title: "Product</br>Name", width: 110
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "ProductionDate", title: "Production</br>Date", width: 120,
                    template: "#= (ProductionDate == null)? '' : kendo.toString(kendo.parseDate(ProductionDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ProductionQty", title: "Batch</br>Qty", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "ProductionByName", title: "Production</br>By", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "OrderMode", title: "Order Mode", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
            ];

            search =
            {
                fields:
                    [
                        "PONo",  
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
                    }], title: "Action &nbsp;", width: 60,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                }, 
                {
                    field: "ProductionChildID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "ProductionMasterID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                
                {
                    field: "objPackagingTypeList", title: "Packaging Type", width: 120,
                    editor: GridHelper.cboPackagingType,
                    template: "#=objPackagingTypeList != null ? objPackagingTypeList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "SizeGram", title: "Size Gram", width: 80,
                    footerTemplate: "<span id='sumSizeGram'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "objUOMList", title: "UOM", width: 120,
                    editor: GridHelper.cboUOM,
                    template: "#=objUOMList != null ? objUOMList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "FillingQty", title: "Filling Qty", width: 80,
                    footerTemplate: "<span id='sumQtyPerSample'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "Remarks", title: "Remarks", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                }, 
            ];

        var schema = {
            model: {
                fields: {
                    ProductionChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    ProductionMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    objPackagingTypeList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objUOMList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    Qty: { type: "number", editable: function () { return false; } },  
                    Remarks: { type: "string"}, 
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

            if (statusCons == statusConstants.PENDING) {
                $("#btnSave,#btnApproved").show();
                childsActionBtn = false;
                childsToolsBar = true;
                Manager.GetNew(selectedItem.ProductionMasterID);
            }
            else {
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
                Manager.GetDetails(selectedItem.ProductionMasterID);
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
    GridCenterAlignAttributes: function (align_header = "", align_body = "", align_footer = "") {
        return {
            headerAttributes: { "class": align_header + " k_input_cell" },
            attributes: { "class": align_body + " k_input_cell" },
            footerAttributes: { "class": align_footer + " k_input_cell" }
        };
    }, 
}


