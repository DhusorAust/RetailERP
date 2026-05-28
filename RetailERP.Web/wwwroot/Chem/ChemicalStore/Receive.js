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
var isQCPage = false;
var vQCPage = "";

var selectedItemData = [];
var checkedItemList = [];
var gbIndentList = [];
$(document).ready(function () {
    Helper.Initialize();
});

var Manager = {
    GetNew: function () {
        $.get(`/api/csreceive/new`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable([]);
            FormHelper.SetDateFormat("ReceiveDate", new Date());
            Helper.GetReceiveBy(masterData.ReceiveByList);
            Helper.GetCheckedBy(masterData.CheckedByList);
            //$("#UnitID").val("21").trigger("change");
            //$("#StoreID").val("12").trigger("change");
            Helper.LoadVirtualizationCombo();

        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id, isQCPage, Flag) {
        $.get(`/api/csreceive/edit?id=${id}&isQCPage=${isQCPage}&Flag=${Flag}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs);
            Helper.GetReceiveBy(masterData.ReceiveByList);
            Helper.GetCheckedBy(masterData.CheckedByList);
            $("#btnPrintIndent").show();
            Helper.LoadVirtualizationCombo();
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetIndentChild: function (indentId) {
        var obj = "";
        var url = `/api/csreceive/GetIndentChild?indentId=${indentId}`;
        AjaxManager.GetList(url, onSuccess, onFailed);

        function onSuccess(jsonData) {
            obj = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return obj;
    },
    Save: function (SaveOption) {

        if (Helper.IsFrmValid()) {


            var model = FormHelper.FormElToJson(FormDom);
            model.Childs = JSON.parse(JSON.stringify($(TblChildId).data("kendoGrid").dataSource.data()));
            model.SaveOption = SaveOption;


            //Child Validation
            if (!Helper.IsFrmChildsValid(model.Childs)) {
                return;
            }
            $.post("/api/csreceive/save", model, function (data) {
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
        alert('Under construction.');
        //window.open(`/RptCommercialExport/csreceive?id=${id}`, "_blank");
    },

    RptMRR: function (id) {
        window.open(`/RptChemicalStore/Chem_MRR?id=${id}`, "_blank");
    }, 


    AddToList: function () {
        var indentProd = FormDom.find("#cboIndentNo").data("kendoMultiColumnComboBox").dataItem();
        var indentMasterID = indentProd.IndentMasterID;
        var prodId = indentProd.ItemID;

        if (indentMasterID == "" || indentMasterID == null || indentMasterID == 0) {
            MessageHelper.Warning("Warning", "Please Select Indent No.");
        } else {

            var grid = $(TblChildId).data("kendoGrid");
            var data = grid.dataSource.data();
            gbIndentList = data;
            var duplicateChecking = gbIndentList.filter(x => x.IndentMasterID == indentMasterID);
            if (duplicateChecking.length > 0) {
                MessageHelper.Warning("Duplicate", "Indent alredy exists.");
                return;
            }
            var indentList = Manager.GetIndentChild(indentMasterID);
            var pi = $("#ReceiveMasterID").val();
            if (pi) {

                //if (pi == indentList[0].PIMasterID) {
                if (indentList.length > 0) {

                    for (let indInfo of indentList) {
                        gbIndentList.push(indInfo);
                    }
                    GridManager.InitChildTable(gbIndentList);
                }
                //}
                //else {
                //    MessageHelper.Warning("Warning", "Please Add Indent to previous selected PI No");
                //    return;
                //}
            }
            else {
                if (indentList.length > 0) {
                    //$("#PIMasterID").val(indentList[0].PIMasterID).trigger("change");
                    for (let indInfo of indentList) {
                        gbIndentList.push(indInfo);
                    }
                    GridManager.InitChildTable(gbIndentList);
                }
            }
        }
        KendoGridHelper.getRowSLNo(TblChildId);
    },

    GetIndentChild: function (indentId, prodId) {
        var obj = "";
        var url = `/api/csreceive/GetIndentChild?indentId=${indentId}`;
        AjaxManager.GetList(url, onSuccess, onFailed);

        function onSuccess(jsonData) {
            obj = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return obj;
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
        var Elements = ["UnitID", "StoreID", "ReceiveDate", "ReceiveBy", "CheckedBy", "ReceiveTypeID", "ChallanNo", "ChallanDate", "GatePassNo", "GatePassDate"];
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
        isQCPage = FormHelper.ConvertToBoolean($(`#${pageId}`).find("#QCPage").val());

        if (isQCPage) {
            // QC Page
            ToolbarDom.find("#btnQCPendingList,#btnQCPassedList,#btnQCFailedList,#btnMrrPendingList,#btnMrrList,#btnAllList").show();
            statusCons = statusConstants.LIST_1;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnQCPendingList"), ToolbarDom);
            GridManager.InitMasterTable();
        } else {
            // Receive Page
            ToolbarDom.find("#btnAddNew,#btnEditList,#btnQCPendingList,#btnMrrList,#btnRejectList,#btnAllList").show();
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnEditList"), ToolbarDom);
            GridManager.InitMasterTable();
        }

        // ToolbarDom means Toolbar Buttons

        ToolbarDom.find("#btnEditList").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
        });
        ToolbarDom.find("#btnQCPendingList").on("click", function (e) {
            statusCons = statusConstants.LIST_1;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            //$("#divIndentNo").show();
        });
        ToolbarDom.find("#btnQCPassedList").on("click", function (e) {
            statusCons = statusConstants.LIST_2;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            //$("#divIndentNo").show();
        });

        ToolbarDom.find("#btnQCFailedList").on("click", function (e) {
            statusCons = statusConstants.LIST_3;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            //$("#divIndentNo").show();
        });

        ToolbarDom.find("#btnMrrPendingList").on("click", function (e) {
            statusCons = statusConstants.LIST_4;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            //$("#divIndentNo").show();
        });


        ToolbarDom.find("#btnMrrList").on("click", function (e) {
            statusCons = statusConstants.LIST_5;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            //$("#divIndentNo").show();
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
            $("#divIndentNo").hide();

        });
        $("#btnAddToList").click(function (e) {
            Manager.AddToList();
        });
        //Button
        $("#btnAddNew").on("click", function (e) {
            debugger;
            $("#btnSaveQC,#btnSentToMRR,#btnSaveMRR,#btnSaveForApprove,#btnApproved,#btnRevise,#btnReject,#divReviseNote,#divRejectNote").hide()
            $("#btnSave,#btnSaveNSentToQC,#divIndentNo").show();
            Manager.GetNew();
        });
        $("#btnSave").on("click", function (e) {
            var SaveOption = 0;
            if (isQCPage) {
                // QC Page                
                SaveOption = Helper.GetSaveOption(10);
            } else {
                // Receive Page
                SaveOption = Helper.GetSaveOption(1);
            }
            Manager.Save(SaveOption);
        })
        $("#btnSaveForApprove").on("click", function (e) {
            var SaveOption = Helper.GetSaveOption(6);
            Manager.Save(SaveOption);
        });
        $("#btnApproved").on("click", function (e) {
            var SaveOption = Helper.GetSaveOption(3);
            Manager.Save(SaveOption);
        });
        $("#btnSaveNSentToQC").on("click", function (e) {
            var SaveOption = Helper.GetSaveOption(8);
            Manager.Save(SaveOption);
        })
        $("#btnSentToMRR").on("click", function (e) {
            var SaveOption = Helper.GetSaveOption(11);
            Manager.Save(SaveOption);
        })
        $("#btnSaveMRR").on("click", function (e) {
            var SaveOption = Helper.GetSaveOption(12);
            Manager.Save(SaveOption);
        })
        $("#btnRevise").on("click", function (e) {
            var revReason = $("#ReviseReason").val();
            if (revReason == "") {
                ToastrMessageHelper.ValidationToastr("Please Input Revise Reason.");
                $("#ReviseReason").focus();
                return;
            }
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Revise this Receive?')
            ).then((result) => {
                if (result.isConfirmed) {
                    Manager.Save(5);
                }
            })
        });
        $("#btnReject").on("click", function (e) {
            var rejReason = $("#RejectReason").val();
            if (rejReason == "") {
                ToastrMessageHelper.ValidationToastr("Please Input Reject Reason.");
                $("#RejectReason").focus();
                return;
            }
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
            var piid = $("#ReceiveMasterID").val();
            if (piid) {
                Manager.RptIndent(piid);
            }
        });

        //Selected Row in the kendo Grid
        KendoGridHelper.GridRowSelected(TblChildId);
    },
    GetReceiveBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" },
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#ReceiveBy", "text", "id", columns);
    },
    GetCheckedBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" },
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#CheckedBy", "text", "id", columns);
    },

    GetSaveOption: function (value) {
        var SaveOption = 0;
        var vReceiveMasterID = $("#ReceiveMasterID").val();
        vReceiveMasterID = parseInt(vReceiveMasterID);
        if ((value == 1) && (vReceiveMasterID == 0)) { SaveOption = 1; } // Save New
        if ((value == 1) && (vReceiveMasterID > 0)) { SaveOption = 2; } // Update
        if ((value == 6) && (vReceiveMasterID == 0)) { SaveOption = 6; } // Save & Send To Approve
        if ((value == 6) && (vReceiveMasterID > 0)) { SaveOption = 7; } // Update & Send To Approve
        if ((value == 8) && (vReceiveMasterID == 0)) { SaveOption = 8; } // Save & Send To QC
        if ((value == 8) && (vReceiveMasterID > 0)) { SaveOption = 9; } // Update & Send To QC
        if ((value == 10) && (vReceiveMasterID > 0)) { SaveOption = 10; } // Update QC
        if ((value == 11) && (vReceiveMasterID > 0)) { SaveOption = 11; } // Update QC & Send To MRR
        if ((value == 12) && (vReceiveMasterID > 0)) { SaveOption = 12; } // Generate MRR
        return SaveOption;
    },
    LoadVirtualizationCombo: function () {
        if ($("#cboIndentNo").data("kendoMultiColumnComboBox")) $("#cboIndentNo").kendoMultiColumnComboBox('destroy').empty();

        $("#cboIndentNo").kendoMultiColumnComboBox({
            filter: "contains",
            dataTextField: "IndentNo",
            dataValueField: "IndentMasterID",
            //noDataTemplate: 'No Data!',            
            minLength: 3,
            autoWidth: true,
            height: 420,
            columns: [
                { field: "IndentNo", title: "Indent No", width: "150px" },
                {
                    field: "IndentDate", title: "Indent Date", width: "120px",
                    template: "#= (IndentDate == null)? '' : kendo.toString(kendo.parseDate(IndentDate, 'yyyy-MM-dd'), 'dd-MMM-yyyy') #"
                },
                { field: "IndentQty", title: "Indent Qty", width: "120px" },
                { field: "ReceivedQty", title: "Total Received Qty", width: "150px" },
                { field: "BalanceQty", title: "Balance Qty", width: "130px" },
            ],
            dataSource: {
                type: "json",
                serverFiltering: true,
                transport: {
                    read: {
                        type: "GET",
                        url: `/api/csreceive/GetIndentList`,
                        dataType: "json",
                        contentType: 'application/json',
                        data: {
                            searchKey: Helper.filterValue
                        }
                    },
                }
            },
            index: -1,
        });
    },
    filterValue: function () {
        var itemCombo = $("#cboIndentNo").data("kendoMultiColumnComboBox");
        filter = itemCombo.text();
        return filter;
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
                    url: `/api/csreceive/list/${statusCons}`,
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

        debugger;

        if (isQCPage) {
            // QC Page
            if (statusCons == statusConstants.PROPOSED_FOR_APPROVAL) {
                command =
                    [
                        { name: "edit", text: "", iconClass: "k-icon k-i-plus", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            } else if (statusCons == statusConstants.LIST_1) {
                command =
                    [
                        { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            } else if (statusCons == statusConstants.LIST_2) {
                command =
                    [
                        { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            } else if (statusCons == statusConstants.LIST_3) {
                command =
                    [
                        { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            } else if (statusCons == statusConstants.LIST_4) {
                command =
                    [
                        { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            } else if (statusCons == statusConstants.LIST_5) {
                command =
                    [
                        { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                        { name: "rptMRR", text: "MRR", iconClass: "k-icon k-i-print k-grid-button-rpt", className: "k-success", click: GridHelper.HandleCommands },

                    ];
            } else if (statusCons == statusConstants.ALL) {
                command =
                    [
                        { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                    ];
            }
            else {
                command =
                    [
                        { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },

                    ];
            }
        } else {

            // Receive Page

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
            else if (statusCons == statusConstants.LIST_5) {
                command =
                    [
                        { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                        { name: "rptMRR", text: "MRR", iconClass: "k-icon k-i-print k-grid-button-rpt", className: "k-success", click: GridHelper.HandleCommands },

                    ];
            }
            else {
                command =
                    [
                        { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },

                    ];
            }

            if (statusCons == statusConstants.LIST_1) {
                // QC Pending List
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
                    field: "IndentMasterID", title: "IndentMasterID", hidden: false, width: 120,
                },
                {
                    field: "SSReturnMasterID", title: "SSReturnMasterID", hidden: true,
                },
                {
                    field: "IndentNo", title: "Indent No", width: 120, filterable: true, attributes: { "style": "font-weight:700" },
                },
                {
                    field: "IndentDate", title: "Indent Date", width: 100,
                    template: "#= (IndentDate == null)? '' : kendo.toString(kendo.parseDate(IndentDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "IndentByName", title: "Indent By", width: 100,
                },
                {
                    field: "UnitName", title: "Unit", width: 120,
                },
                {
                    field: "StoreName", title: "Store", width: 120,
                },
                {
                    field: "Remarks", title: "Remarks", width: 120,
                },
            ];
        } else {
            columns = [
                {
                    title: "Action", command: command, width: 60
                },
                {
                    field: "ReceiveMasterID", title: "ReceiveMasterID", hidden: true,
                },
                {
                    field: "ReceiveNo", title: "Receive No", width: 140,
                    filterable: true, attributes: { "style": "font-weight:700" },
                },
                {
                    field: "ReceiveDate", title: "Receive Date", width: 75,
                    template: "#= (ReceiveDate == null)? '' : kendo.toString(kendo.parseDate(ReceiveDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "IndentTypeName", title: "Receive Type", width: 100,
                },
                {
                    field: "IndentNo", title: "Indent No", width: 140,
                    filterable: true, attributes: { "style": "font-weight:700" },
                },
                {
                    field: "ReceiveByName", title: "Receive By", width: 100,
                },
                {
                    field: "CheckedByName", title: "Checked By", width: 120,
                },
            ];
        }

        if (statusCons == statusConstants.PENDING) {
            search =
            {
                fields:
                    [
                        "IndentNo", "IndentDate", "IndentByName"
                    ]
            };
        } else {
            search =
            {
                fields:
                    [
                        "ReceiveNo", "IndentNo", "ReceiveDate", "ReceiveByName", "CheckedByName", "IndentTypeName"
                    ]
            };
        }
        var height = $(window).height() - 120;
        KendoGridHelper.MasterGridLoad(TblMasterId, height + "px", search, columns, true)
    },
    InitChildTable: function (data) {

        var vEnable_Qty = true;
        if (isQCPage) {
            vEnable_Qty = false;
        }


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
                            hidden: childsActionBtn,
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
                                clone.ReceiveChildID = 0;
                                clone.IndentQty = 0;
                                clone.ReceiveQty = '';
                                clone.ReceivedQty = 0;
                                clone.ReceiveQtyPrev = 0;
                                clone.StockQty = '';
                                clone.ChildsRemarks = '';
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
                    field: "IndentChildID", text: "CID", hidden: true, width: 50
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                { field: "IndentMasterID", hidden: true, width: 150 },
                { field: "PRMasterID", hidden: true },
                { field: "UnitID", hidden: true },
                { field: "StoreID", hidden: true },
                { field: "ReceiveChildID", hidden: true },
                {
                    field: "RowSLNo", title: "SL#", template: "#= ++gbGridSLNo #", width: 40,
                    editable: function () { return false; },
                },
                {
                    field: "IndentNo", title: "Indent No", width: 100,
                    editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "objProductList", title: "Chemical Items", width: 140,
                    editor: GridHelper.cboProduct,
                    editable: function () { return false; },
                    template: "#=objProductList != null ? objProductList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "UOM", title: "UOM", width: 50,
                    editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "BatchNo", title: "Batch No", width: 80,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ManufacturingDate", title: "Manufacture</br>Date", width: 80,
                    template: "#= kendo.toString(ManufacturingDate,'dd-MMM-yyyy')#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ExpireDate", title: "Expire</br>Date", width: 80,
                    template: "#= kendo.toString(ExpireDate,'dd-MMM-yyyy')#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "StockQty", title: "Stock Qty", width: 80, editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "IndentQty", title: "Indent Qty", width: 80, editable: function () { return false; },
                    footerTemplate: "<span id='sumIndentQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "ReceivedQty", title: "Previous</br>Received Qty", width: 90,
                    editable: function () { return false; },
                    footerTemplate: "<span id='sumReceivedQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", false, false)
                },
                {
                    field: "ReceiveQty", title: "Receive</br>Qty", width: 70,
                    editable: function () { return vEnable_Qty; },
                    footerTemplate: "<span id='sumReceiveQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "SectionName", title: "Section", width: 80,
                    editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "objLocationList", title: "Location", width: 120,
                    editor: GridHelper.cboLocation,
                    template: "#=objLocationList != null ? objLocationList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },

                {
                    field: "objRackList", title: "Rack", width: 120,
                    editor: GridHelper.cboRack,
                    template: "#=objRackList != null ? objRackList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "ChildsRemarks", title: "Remarks", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "objQCStatusList", title: "QC Status", width: 70,
                    editor: GridHelper.cboQCStatus,
                    template: "#=objQCStatusList != null ? objQCStatusList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objQCByList", title: "QC By", width: 120,
                    editor: GridHelper.cboQCBy,
                    template: "#=objQCByList != null ? objQCByList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "QCDate", title: "QC</br>Date", width: 70,
                    template: "#= kendo.toString(QCDate,'dd-MMM-yyyy')#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "QCRemarks", title: "QC Remarks", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "MRR_Number", title: "MRR Number", width: 120,
                    editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },

            ];

        GridManager.HideColumns(columns, statusCons);

        var schema = {
            model: {
                fields: {
                    IndentMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    IndentChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    UnitID: { type: "number", editable: function () { return false; } },
                    StoreID: { type: "number", editable: function () { return false; } },
                    PRMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    IndentNo: { type: "string", editable: function () { return false; } },
                    objProductList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    UOM: { type: "string", editable: function () { return false; } },
                    BatchNo: { type: "string" },
                    ManufacturingDate: {
                        type: "date",
                        template: '#=kendo.toString("dd-MMM-yyyy")#',
                        editable: true
                    },
                    ExpireDate: {
                        type: "date",
                        template: '#=kendo.toString("dd-MMM-yyyy")#',
                        editable: true
                    },
                    StockQty: { type: "number", editable: function () { return false; } },
                    IndentQty: { type: "number", validation: { min: 0 }, defaultValue: null },
                    ReceiveQtyPrev: { type: "number", validation: { min: 0 }, defaultValue: null },
                    ReceiveQty: { type: "number", validation: { min: 0 }, defaultValue: null },
                    objSectionList: {
                        defaultValue: { id: null, text: "" },
                    },
                    objLocationList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objRackList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    ChildsRemarks: { type: "string" },
                    objQCStatusList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objQCByList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    QCDate: {
                        type: "date",
                        template: '#=kendo.toString("dd-MMM-yyyy")#',
                        editable: true
                    },
                    QCRemarks: { type: "string" },
                    MRR_Number: { type: "string", editable: function () { return false; } },
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId, columns, data, schema, true, true, false);
        GridHelper.InitializeHelper();
        KendoGridHelper.EnterKeyUp(TblChildId);

        $(".k-toolbar", TblChildId).hide();


    },



    HideColumns: function (columns, statusCons) {

        var hideColumns = "";
        var ChildGridColumnsOrTitle = [];
        if (!isQCPage) {
            hideColumns = "objQCStatusList,objQCByList,QCDate,QCRemarks,MRR_Number";
        }

        if ((isQCPage) && ((statusCons == statusConstants.LIST_1) || (statusCons == statusConstants.LIST_2) || (statusCons == statusConstants.LIST_3) || (statusCons == statusConstants.LIST_4))) {
            // btnQCPendingList,btnQCPassedList,btnQCFailedList
            hideColumns = "Action&nbsp;,StockQty,IndentQty,ReceivedQty,ChildsRemarks,MRR_Number";
        }

        if ((isQCPage) && (statusCons == statusConstants.LIST_5)) {
            // btnMrrList
            hideColumns = "Action&nbsp;,StockQty,IndentQty,ReceivedQty,ChildsRemarks,objQCStatusList,objQCByList,QCDate,QCRemarks";
        }
        ChildGridColumnsOrTitle = hideColumns.split(",");

        if (ChildGridColumnsOrTitle.length > 0) {
            for (var i = 0; i < ChildGridColumnsOrTitle.length; i++) {
                var fieldName = ChildGridColumnsOrTitle[i];
                for (var j = 0; j < columns.length; j++) {
                    if (columns[j].field === fieldName) {
                        columns[j].hidden = true;
                        break;
                    }
                    ////Action&nbsp
                    if (columns[j].title === fieldName) {
                        columns[j].hidden = true;
                        break;
                    }
                }
            }
        }

    },
}

var GridHelper = {
    // Actions
    HandleCommands: function (args) {
        args.preventDefault();
        var tr = $(args.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);

        if (selectedItem != null) {
            childsActionBtn = true;
            childsToolsBar = false;

            $("#btnSave,#btnSaveNSentToQC,#btnSaveQC,#btnSentToMRR,#btnSaveMRR,#btnSaveForApprove,#btnApproved,#btnRevise,#btnReject").hide();
            $("#divIndentNo,#divReviseNote,#divRejectNote").hide();
             
            if (args.data.commandName == "rptMRR") {
                Manager.RptMRR(selectedItem.ReceiveMasterID);
                return;
            }


            if (isQCPage) {
                // QC Page

                if (statusCons == statusConstants.PROPOSED_FOR_APPROVAL) {
                    //$("#btnApproved").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                }
                if (statusCons == statusConstants.LIST_1) { //btnQCPendingList
                    $("#btnSave,#btnSentToMRR").show();
                    //$("#divIndentNo").hide();
                    childsActionBtn = false;
                    childsToolsBar = true;
                }
                if (statusCons == statusConstants.LIST_2) { // btnQCPassedList
                    $("#btnSave,#btnSentToMRR").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                }
                if (statusCons == statusConstants.LIST_3) { // btnQCFailedList
                    $("#btnSave,#btnSentToMRR").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                }
                if (statusCons == statusConstants.LIST_4) { // btnMrrPendingList
                    $("#btnSaveMRR").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                }
                if (statusCons == statusConstants.LIST_5) { // btnMrrList
                    childsActionBtn = false;
                    childsToolsBar = true;
                }
                if (statusCons == statusConstants.ALL) { // btnAllList
                    childsActionBtn = true;
                    childsToolsBar = true;
                    //sbs
                }
                Manager.GetDetails(selectedItem.ReceiveMasterID, isQCPage, statusCons);
            }
            else {

                // Receive Page
                if (statusCons == statusConstants.EDIT) {
                    $("#btnSave,#btnSaveNSentToQC").show();
                    $("#divIndentNo").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                } else if (statusCons == statusConstants.PENDING) {
                    //$("#btnSave,#btnSaveForApprove").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                    Manager.GetNew(selectedItem.IndentMasterID);

                } else if (statusCons == statusConstants.LIST_1) {
                    // QC Pending List
                    $("#divReviseNote,#divRejectNote").hide();
                    $("#divIndentNo").hide();
                    childsActionBtn = false;
                    childsToolsBar = true;
                } else if (statusCons == statusConstants.PROPOSED_FOR_APPROVAL) {
                    childsActionBtn = false;
                    childsToolsBar = true;
                } else if (statusCons == statusConstants.APPROVED) {
                    //$("#btnRevise,#btnReject").show();
                    //$("#divReviseNote,#divRejectNote").show();
                } else if (statusCons == statusConstants.REJECT) {
                    //$("#btnSave,#btnApproved").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                    //$("#divRejectNote").show();
                }
                Manager.GetDetails(selectedItem.ReceiveMasterID, isQCPage, statusCons);
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
                if (e.field === "ReceiveQty") {
                    GridHelper.GetSumReceiveQty();
                }
            }
            GridHelper.GetSumReceiveQty();
        });
        GridHelper.GetSumReceiveQty();
    },
    GetSumReceiveQty: function () {
        TblChilds = $(TblChildId).data("kendoGrid");
        TblCDataSource = TblChilds.dataSource.data();
        var totalIndentQty = 0;
        var totalReceivedQty = 0;
        var totalReceiveQty = 0;
        for (var i = 0; i < TblCDataSource.length; i++) {
            totalIndentQty += kendo.parseFloat(TblCDataSource[i].IndentQty);
            totalReceivedQty += kendo.parseFloat(TblCDataSource[i].ReceivedQty);
            totalReceiveQty += kendo.parseFloat(TblCDataSource[i].ReceiveQty);
        }
        $("#sumIndentQty").text(totalIndentQty.toFixed(2));
        $("#sumReceivedQty").text(totalReceivedQty.toFixed(2));
        $("#sumReceiveQty").text(totalReceiveQty.toFixed(2));
    },
    GetSumReceivedQty: function () {
        TblChilds = $(TblChildId).data("kendoGrid");
        TblCDataSource = TblChilds.dataSource.data();
        var totalQty = 0;

        for (var i = 0; i < TblCDataSource.length; i++) {
            totalQty += kendo.parseFloat(TblCDataSource[i].ReceivedQty);
        }
        $("#sumReceivedQty").text(totalQty.toFixed(2));
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
    cboLocation: function (container, options) {
        $('<input id="cmbLocation" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.LocationList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboRack: function (container, options) {
        $('<input id="cmbcboRack" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.RackList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboQCStatus: function (container, options) {
        $('<input id="cmbQCStatus" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.QCStatustList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboQCBy: function (container, options) {
        $('<input id="cmbQCBy" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.QCBytList,
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

