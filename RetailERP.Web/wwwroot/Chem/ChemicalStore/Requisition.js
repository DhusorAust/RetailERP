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
        $.get(`/api/cspr/new`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut(); 
            
            masterData = data; 
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable([]); 
            $("#StoreID").data("kendoComboBox").value("1");
            $("#UnitID").data("kendoComboBox").value("1");
            Helper.GetPRBy(masterData.PRByList);

        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id) {
        $.get(`/api/cspr/edit?id=${id}`, function (data) {
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
        debugger;
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
            $.post("/api/cspr/save", model, function (data) {
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
        window.open(`/RptChemicalStore/Chem_PurchaseRequisition?id=${id}`, "_blank");
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
        var Elements = ["UnitID", "StoreID", "PRDate", "PRBy", "ConcernPersonID"];
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
            statusCons = statusConstants.EDIT; 
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnEditList"), ToolbarDom);
            Manager.GetNew();

            //Button 
            //$("#btnSave").show();
            //$("#btnApproved").show();
            //$("#btnRevise").hide();  
            //$("#btnReject").hide();  
            //$("#btnPrintPR").hide();  
        });
        ToolbarDom.find("#btnEditList").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            //Button 
            //$("#btnSave").show();
            //$("#btnApproved").show();
            //$("#btnRevise").hide();
            //$("#btnReject").hide();
            //$("#btnPrintPR").show();  
        });
        ToolbarDom.find("#btnApprovedList").on("click", function (e) {
            statusCons = statusConstants.APPROVED;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            //Button 
            //$("#btnSave").hide();
            //$("#btnApproved").hide();
            //$("#btnRevise").show();
            //$("#btnReject").show();
            //$("#btnPrintPR").show();  
        });
        ToolbarDom.find("#btnRejectList").on("click", function (e) {
            statusCons = statusConstants.REJECT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            //Button 
            //$("#btnSave").show();
            //$("#btnApproved").show();
            //$("#btnRevise").hide();
            //$("#btnReject").hide();
            //$("#btnPrintPR").show();  
        });
        ToolbarDom.find("#btnAllList").on("click", function (e) {
            statusCons = statusConstants.ALL;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            //Button 
            //$("#btnSave").hide();
            //$("#btnApproved").hide();
            //$("#btnRevise").hide();
            //$("#btnReject").hide();
            //$("#btnPrintPR").show();
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
        $("#btnPrintPR").click(function () {
            var PRMasterID = $("#PRMasterID").val();
            if (PRMasterID) {
                Manager.RptPR(PRMasterID);
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
    GetPRBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" }, 
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#PRBy", "text", "id", columns);
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
                    url: `/api/cspr/list/${statusCons}`,
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
                    { name: "rptPR", text: "PR", iconClass: "k-icon k-i-print k-grid-button-rpt", className: "k-success", click: GridHelper.HandleCommands },
                ];
        } else {
            command =
                [
                    { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                    { name: "rptPR", text: "PR", iconClass: "k-icon k-i-print k-grid-button-rpt", className: "k-success", click: GridHelper.HandleCommands },
                ];
        } 
        var columns = [
            {
                title: "Action", command: command, width: 100
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
                field: "PRNo", title: "Requisition No", width: 80, filterable: true, attributes: { "style": "font-weight:700" },
            },
            {
                field: "PRDate", title: "Requisition Date", width: 70,
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
        var search =
        {
            fields:
                [
                    "PRNo", "PRDate", "PRByName", "ConcernPerson"
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
                    field: "IndentChildID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "UOMID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
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

                //{
                //    field: "UOM", title: "UOM", width: 60,
                //    editable: function () { return false; },
                //    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                //},
                {
                    field: "StockQty", title: "Stock Qty", width: 80, editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "PRQty", title: "Qty", width: 80,
                    footerTemplate: "<span id='sumPRQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "objSectionList", title: "Section", width: 120,
                    editor: GridHelper.cboSection,
                    template: "#=objSectionList != null ? objSectionList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "PRChildRemarks", title: "Remarks", width: 120,
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
                    PRQty: { type: "number", validation: { min: 0 }, defaultValue: null }, 
                    objSectionList: {
                        defaultValue: { id: null, text: "" },
                    }, 
                    PRChildRemarks: { type: "string"}, 
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId, columns, data, schema, true, true, false);
        GridHelper.InitializeHelper();
        KendoGridHelper.EnterKeyUp(TblChildId);
    },

    ////Manager.GetPIReport(piid);
    GetPIReport: function (piid) {
        debugger;

        var id = $("#ReceiveMasterID").val();

        window.open(`/RptChemicalStoreController/Chem_PurchaseRequisition?id=${id}`, "_blank");
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
             
            Manager.GetDetails(selectedItem.PRMasterID);
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
    GridCenterAlignAttributes: function (align_header = "", align_body = "", align_footer = "") {
        return {
            headerAttributes: { "class": align_header + " k_input_cell" },
            attributes: { "class": align_body + " k_input_cell" },
            footerAttributes: { "class": align_footer + " k_input_cell" }
        };
    },



    //CountChangeEvent: function (e) {
    //    var combo = e.sender;
    //    var itemGrid = $(TblChildId).data("kendoGrid");
    //    var data = $("#cmbCount").data("kendoMultiColumnComboBox").dataItem();

    //    var selectedRow = itemGrid.select();
    //    var selectedItem = itemGrid.dataItem(itemGrid.select());
    //    var selectedRowIndex = selectedRow.index();
    //    var countId = $("#cmbCount").data("kendoMultiColumnComboBox").value();
    //    var gridData = $(TblChildId).data("kendoGrid").dataSource.data();
    //    for (var i = 0; i < gridData.length; i++) {
    //        var grd = gridData[i];
    //        if (selectedRowIndex != i) {
    //            if (grd.objCountList.CountID == countId) {
    //                $("#cmbCount").data("kendoMultiColumnComboBox").value("");
    //                selectedItem.set('HSCode', "");

    //                FxToastr("This Style Already Added")
    //                return;
    //            }
    //        }
    //    }
    //    selectedItem.set('HSCode', data.HSCode);
    //},
    //filterValue() {
    //    var itemCombo = $("#cmbCount").data("kendoMultiColumnComboBox");
    //    filter = itemCombo.text();
    //    return filter;
    //},
    //SaveGridFilter: function () {
    //    savedFilter = $(TblMasterId).data("kendoGrid").dataSource.filter();
    //},
    
}


