/// <reference path="../../../common/js/utilitycommon.js" />


var statusCons;
var masterData;
var menuId, pageName;
var ToolbarId;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom;
var gpProductTypeList = [];

$(document).ready(function () {
    Helper.Initialize();
});

var Manager = {
    GetNew: function () {
        $.get(`/api/productInfo/new`, function (data) {
            if (data.ErrNo > 0) {
                MessageHelper.Warning("Error = ", data.Message);
            } else {
                DivDetailsDom.fadeIn();
                DivToolbarDom.fadeOut();
                masterData = data;
                childsActionBtn = false;
                FormHelper.SetFormData(FormDom, masterData);
                $("#IsActive").val("1").trigger('change');
                //  $("#IsActive").prop("disabled", true);

                var columns = [
                    { field: "text", title: "LC No", width: "600" },
                    { field: "valueA", title: "Bale Qty", width: "200" },
                    { field: "valueB", title: "Staple Length", width: "150px" },
                ];


                KendoDropDownHelper.ComboBox(gpProductTypeList, "ProductTypeID", "ProductTypeName", "ProductTypeID", false);
                KendoDropDownHelper.MultiColumnComboBox(masterData.LCMaster_ImpList, "#LCMasterID_Imp", "text", "id", columns);

                GridManager.InitProductTypeGrid();

                GridHelper.LoadProdCategoryGrid([]);

            }
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", data.Message);
        });
    },
    GetDetails: function (id) {
        $.get(`/api/productInfo/edit?id=${id}`, function (data) {
            if (data.ErrNo > 0) {
                MessageHelper.Warning("Error = ", data.Message);
            } else {
                DivDetailsDom.fadeIn();
                DivToolbarDom.fadeOut();
                masterData = data;

                var columns = [
                    { field: "text", title: "LC No", width: "600" },
                    { field: "valueA", title: "Bale Qty", width: "200" },
                    { field: "valueB", title: "Staple Length", width: "150px" },
                ];

                KendoDropDownHelper.ComboBox(gpProductTypeList, "ProductTypeID", "ProductTypeName", "ProductTypeID", false);
                KendoDropDownHelper.MultiColumnComboBox(masterData.LCMaster_ImpList, "#LCMasterID_Imp", "text", "id", columns);
                Helper.LoadProductCategoryCombo(masterData.ProductTypeID);

                FormHelper.SetFormData(FormDom, masterData);

                GridManager.InitProductTypeGrid();

             

             
                GridHelper.LoadProdCategoryGrid([]);

            }
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", data.Message);
        });
    },
    Save: function (Approved) {
        if (Helper.IsFrmValid()) {
            var model = FormHelper.FormElToJson(FormDom);
            model.Approved = Approved;
            $.post("/api/productInfo/save", model, function (data) {
                if (parseInt(data.ResultId) > 0) {
                    MessageHelper.Success("Save Operations", data.Message);
                    Helper.BackToList();
                } else {
                    if (data.IsLogin == "1") {
                        window.location.href = data.redirectUrl;
                    }
                    MessageHelper.Warning("Save Operations", data.Message);
                }
            });
        }
    },
    GetAllProductType: function (prodTypeId) {
        var obj = "";
        var jsonParam = "";
        var serviceUrl = `/api/productType/GetAllProductType`;
        AjaxManager.GetList(serviceUrl, onSuccess, onFailed);
        function onSuccess(jsonData) {
            obj = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return obj;
    },
    GetProductCategory: function (prodTypeId) {
        var obj = "";
        var jsonParam = "";
        var serviceUrl = `/api/productInfo/GetProductCategory/${prodTypeId}`;
        AjaxManager.GetList(serviceUrl, onSuccess, onFailed);
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
    //Common Function
    BackToList: function () {
        DivDetailsDom.fadeOut();
        DivToolbarDom.fadeIn();
        Helper.ResetForm();
        GridManager.InitMasterTable();
    },
    ResetForm: function () {
        FormDom.trigger("reset");
        $('input[type="hidden"]').val("0");
    },
    IsFrmValid: function () {
        var IsValidItem = true;
        var Elements = ["ProductTypeID", "ProductCategoryID"];
        if (!FormHelper.IsFormValidation(FormDom, Elements)) {
            IsValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
        }
        return IsValidItem;
    },

    //Pages Function
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
        statusCons = statusConstants.EDIT;
        GridManager.InitMasterTable();

        $("#btnNewAdd").on("click", function (e) {
            FormDom.find("#btnSave,#btnApproved").show();
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn($("#btnEditList"), ToolbarDom);
            Manager.GetNew();
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
        $("#btnSave").click(function (e) {
            Manager.Save(false);
        });
        $("#btnApproved").click(function (e) {
            Manager.Save(true);
        });

        $("#btnBackToList").on("click", Helper.BackToList);

        gpProductTypeList = Manager.GetAllProductType();

        KendoDropDownHelper.ComboBox([], "ProductCategoryID", "CategoryNameDisplay", "ProductCategoryID", false);

        $("#ProductTypeID").on("change", function (e) {
            var prodTypeId = $("#ProductTypeID").data("kendoComboBox").value();
            Helper.LoadProductCategoryCombo(prodTypeId);
            Helper.ProductNameGenerate();
        });
        $("#ProductCategoryID").on("change", function (e) {
            Helper.ProductNameGenerate();
        });
        $("#LCMasterID_Imp").on("change", function (e) {
            var lcInfo = $("#LCMasterID_Imp").data("kendoMultiColumnComboBox").dataItem();
            $("#ConsignmentNo").val(lcInfo.valueA);
            $("#StapleLength").val(lcInfo.valueB);
            $("#InvoiceNo").val(lcInfo.valueC);
            $("#LCNo_Imp").val(lcInfo.text);
            $("#UOMID").data("kendoComboBox").value(6);
            Helper.ProductNameGenerate();
        });
        $("#btnPrintBinCard").click(function () {
            var firstReceived = $("#FirstReceived").data("kendoDatePicker").value();

            if (firstReceived) {
                var productid = $("#ProductID").val();
                var fromDate = kendo.toString(firstReceived, "dd-MMM-yyyy");
                var today = new Date();
                var toDate = kendo.toString(today, "dd-MMM-yyyy");

                GridHelper.GetBinCard_WasteCotton(fromDate, toDate, productid);
            } else {
                ToastrMessageHelper.FxToastr("Please select a From Date");
            }
        });
        $("#btnPrintBinCardTran").click(function () {
            var firstReceived = $("#FirstReceived").data("kendoDatePicker").value();

            if (firstReceived) {
                debugger;
                var productid = $("#ProductID").val();
                var unitId = $("#UnitID").data("kendoComboBox").value();
                var today = new Date();
                var toDate = kendo.toString(today, "dd-MMM-yyyy");
                var firstReceived = $("#FirstReceived").data("kendoDatePicker").value();
                var fromDate = kendo.toString(firstReceived, "dd-MMM-yyyy");
                GridHelper.GetBinCard_RawCotton(fromDate, toDate, productid, unitId);
            } else {
                ToastrMessageHelper.FxToastr("Please select a From Date");
            }
        });

        
        $("#btnPrintMRR").click(function () {
            debugger;
            var lcInfo = $("#LCMasterID_Imp").data("kendoMultiColumnComboBox").value();
            if (lcInfo) {
                GridHelper.GetBinMRR_Report(lcInfo);
            }
            else {
                ToastrMessageHelper.FxToastr("LC Not Found");
            }
           
        });
         
       
    },

    LoadProductCategoryCombo: function (prodTypeId) {
        var prodCatgList = Manager.GetProductCategory(prodTypeId);
        var prodcatgCombo = $("#ProductCategoryID").data("kendoComboBox");
        prodcatgCombo.value("");
        prodcatgCombo.text("");
        prodcatgCombo.setDataSource(prodCatgList);
    },
    ProductNameGenerate: function () {
        var lcInfo = $("#LCMasterID_Imp").data("kendoMultiColumnComboBox").dataItem();
        var catgCombo = $("#ProductCategoryID").data("kendoComboBox");
        var catgData = catgCombo.dataItem();

        if (catgData && catgData.ProductCategoryID != null && parseInt(catgData.ProductCategoryID) != 0) {
            if (lcInfo != null && lcInfo != undefined && lcInfo.text != "") {
                debugger
                var lcNo = lcInfo.text;
                var consignNo = lcInfo.valueA;

                var catgPart = catgData.CategoryName.substring(0, 3);

                var prodName = catgPart + "-" + consignNo + " LC-" + lcNo.substring(lcNo.length - 5);
                $("#ProductName").val(prodName);
            }
        }
        else {
            if (lcInfo) {
                ToastrMessageHelper.FxToastr("Please Select Product Category");
            }
            $("#LCMasterID_Imp").data("kendoMultiColumnComboBox").value("");
            return;
        }

    }
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
                    url: `/api/productInfo/list/${statusCons}`,
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

        var command = [];
        if (statusCons == statusConstants.EDIT) {
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

        var columns = [
            {
                title: "Action", command: command, width: 20,
            },
            {
                field: "OldProductID", title: "Prod Code", width: 20
            },
            {
                field: "ProductName", title: "Product Name", width: 50, filterable: true,
            },

            {
                field: "ProductTypeName", title: "Product Type", width: 40, filterable: true,
            },
            {
                field: "CategoryName", title: "Product Category", width: 40, filterable: true,
            },
            {
                field: "UOM", title: "Unit", width: 20,
            },
            {
                field: "ProductID", title: "ProductID", hidden: true
            }
        ];
        var search =
        {
            fields:
                [
                    "ProductName", "ProductTypeName", "CategoryName", "OldProductID", "ProductID"
                ]
        };
        var height = $(window).height() - 150 + "px";
        KendoGridHelper.MasterGridLoad(TblMasterId, height, search, columns, true)
    },

    InitProductTypeGrid: function () {
        if ($("#gridProdType").data("kendoGrid")) $("#gridProdType").kendoGrid('destroy').empty();

        dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: `/api/productType/list/${statusCons}`,
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                },
                update: {
                    url: "/api/productType/save",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                },
                destroy: {
                    url: "/api/productType/save",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                },
                create: {
                    url: "/api/productType/save",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                },
                parameterMap: function (options, operation) {
                  
                    if (operation !== "read" && options) {
                        var isDeleted = 0;
                        var m = options;
                        if (operation == "destroy" && m.ProductTypeID > 0) {
                            isDeleted = 1;
                        }

                        let model = {
                            ProductTypeID: m.ProductTypeID || 0,
                            SLNo: m.SLNo,
                            ProductTypeName: m.ProductTypeName,
                            IsDeleted: isDeleted
                        }
                        return JSON.stringify(model);
                    }
                    else {
                        return JSON.stringify(options);
                    }

                }
            },
           /* batch: true,*/
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 10,
            schema: {
                data: "Items", total: "TotalCount",
                model: {
                    id: "ProductTypeID",
                    fields: {
                        ProductTypeID: { editable: false, nullable: false, defaultValue: 0 },
                        SLNo: { type: "text", editable: true, validation: { required: true } },
                        ProductTypeName: { type: "text", validation: { required: true } },
                    }
                },

            },
            requestEnd: function (e) {

                if (e.type === "create" || e.type === "update" || e.type === "destroy") {
                    var resp = e.response || {};
                    if (resp.IsLogin === "1" && resp.redirectUrl) { window.location.href = resp.redirectUrl; return; }
                    if (resp.ResultId && parseInt(resp.ResultId) > 0) {
                        //Load Prod Type Combo
                        gpProductTypeList = Manager.GetAllProductType();
                        KendoDropDownHelper.ComboBox(gpProductTypeList, "ProductTypeID", "ProductTypeName", "ProductTypeID", false);

                        if (resp.Message) MessageHelper.Success("Success", resp.Message);
                        e.sender.read();
                    } else if (resp.ResultId !== undefined) {
                        MessageHelper.Warning("Operation", resp.Message || "Failed.");
                    }
                }
            },
            error: function (e) {
                var msg = "Unexpected error.";
                try {
                    if (e.xhr && e.xhr.responseText) {
                        var j = JSON.parse(e.xhr.responseText);
                        msg = j?.Message || j?.Title || e.xhr.statusText || msg;
                    } else if (e.errors) {
                        msg = Object.values(e.errors).join("\n");
                    }
                } catch (_) { msg = e.xhr?.responseText || msg; }
                MessageHelper.Warning("Grid Error", msg);
            }
        });

        $("#gridProdType").kendoGrid({
            dataSource: dataSource,
            pageable: true,
            height: 250,
            scrollable: true,
            selectable: "row",
            toolbar: [{ name: "create", text: "Add New" }, {
                template: `
                <div style="width:80%; display:flex; justify-content:center; align-items:center;">
                    <h3 style="margin:0;">Product Type List</h3>
                </div>
            `
            }],
            columns: [
                { field: "SLNo", title: "SL", width: 20 },
                { field: "ProductTypeName", title: "Product Type", width: 80 },
                {
                    command: [
                        { name: "edit", text: { edit: "", update: "Save", cancel: "Close" }, iconClass: "k-icon k-i-edit", className: "k-success" },
                        { name: "destroy", text: "", iconClass: "k-icon k-i-trash", className: "k-danger" }
                    ], width: 45
                }],
            editable: { mode: "inline", confirmation: "Delete this record?" },
            change: function (e) {
                var dataItem = this.dataItem(this.select());
                GridHelper.ProdTypeGridChangeEvent(dataItem);
            }
        });

       

    },

    InitProductCategoryGrid: function (prodTypeId) {
        if ($("#gridProdCategory").data("kendoGrid")) $("#gridProdCategory").kendoGrid('destroy').empty();

       var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: `/api/productCategory/list/${statusCons}/${prodTypeId}`,
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                },
                update: {
                    url: "/api/productCategory/save",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                },
                destroy: {
                    url: "/api/productCategory/save",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                },
                create: {
                    url: "/api/productCategory/save",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                },
                parameterMap: function (options, operation) {
                    if (operation !== "read" && options) {
                        var isDeleted = 0;
                        var m = options.models[0];
                        if (operation == "destroy" && m.ProductCategoryID > 0) {
                            isDeleted = 1;
                        }

                        let model = {
                            ProductCategoryID: m.ProductCategoryID || 0,
                            ProductTypeID: m.ProductTypeID,
                            SLNo: m.SLNo,
                            CategoryName: m.CategoryName,
                            IsDeleted: isDeleted
                        }
                        return JSON.stringify(model);
                    }
                    else {
                        return JSON.stringify(options);
                    }

                }
            },
            //batch: true,
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 50,
            schema: {
                data: "Items", total: "TotalCount",
                model: {
                    id: "ProductCategoryID",
                    fields: {
                        ProductCategoryID: { editable: false, nullable: false, defaultValue: 0 },
                        ProductTypeID: { editable: false, nullable: false, defaultValue: 0 },
                        SLNo: { type: "text", editable: true, validation: { required: true } },
                        ProductCategoryName: { type: "text", validation: { required: true } },
                        objProductTypeList: {
                            defaultValue: { ProductTypeID: null, ProductTypeName: "" },
                            validation: { required: true }

                        },
                    }
                },

            },
            requestEnd: function (e) {
                if (e.type === "create" || e.type === "update" || e.type === "destroy") {
                    debugger
                    var resp = e.response || {};
                    if (resp.IsLogin === "1" && resp.redirectUrl) { window.location.href = resp.redirectUrl; return; }
                    if (resp.ResultId && parseInt(resp.ResultId) > 0) {
                        //Load Category Combo Data
                        Helper.LoadProductCategoryCombo(resp.ProductTypeID);
                        $("#ProductCategoryID").data("kendoComboBox").value(resp.ProductCategoryID);

                        if (resp.Message) MessageHelper.Success("Success", resp.Message);
                        e.sender.read();
                    } else if (resp.ResultId !== undefined) {
                        MessageHelper.Warning("Operation", resp.Message || "Failed.");
                    }
                }
            },
            error: function (e) {
                var msg = "Unexpected error.";
                try {
                    if (e.xhr && e.xhr.responseText) {
                        var j = JSON.parse(e.xhr.responseText);
                        msg = j?.Message || j?.Title || e.xhr.statusText || msg;
                    } else if (e.errors) {
                        msg = Object.values(e.errors).join("\n");
                    }
                } catch (_) { msg = e.xhr?.responseText || msg; }
                MessageHelper.Warning("Grid Error", msg);
            }
        });

        GridHelper.LoadProdCategoryGrid(dataSource);

    },

}

var GridHelper = {
    HandleCommands: function (args) {
        args.preventDefault();
        var tr = $(args.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);

        if (selectedItem != null) {
            $("#btnSave,#btnApproved").hide();

            if (args.data.commandName == "edit") {
                $("#btnSave,#btnApproved").show();
            }
            Manager.GetDetails(selectedItem.ProductID);

        }
    },
    cboProdType: function (container, options) {
        $('<input id="cboProdType" data-text-field="ProductTypeName" data-value-field="ProductTypeID" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.ProductTypeList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    ProdCategoryGridBlankDataSource: function () {
        var dataSource1 = new kendo.data.DataSource({
            data: [],
            batch: false,
            serverPaging: false,
            serverSorting: false,
            serverFiltering: false,
            pageSize: 10,
            schema: {
                data: "Items",
                total: "TotalCount",
                model: {
                    id: "ProductCategoryID",
                    fields: {
                        ProductCategoryID: { type: "number", editable: false, nullable: false, defaultValue: 0 },
                        SLNo: { type: "number", validation: { required: true, min: 1 } },
                        ProductCategoryName: { type: "string", validation: { required: true } },
                        objProductTypeList: {
                            defaultValue: { ProductTypeID: null, ProductTypeName: "" },
                            validation: { required: true }
                        }
                    }
                }
            }
        });

        return dataSource1;
    },
    LoadProdCategoryGrid: function (dataSource) {
        if (dataSource.length==0) {
            dataSource = GridHelper.ProdCategoryGridBlankDataSource();
        }
        var g = $("#gridProdCategory").data("kendoGrid");
        if (g) { g.destroy(); $("#gridProdCategory").empty(); }
        $("#gridProdCategory").kendoGrid({
            dataSource: dataSource,
            noRecords: true,
            messages: { noRecords: "NO DATA FOUND" },
            pageable: true,
            height: 250,
            scrollable: true,
            selectable: "row",

            toolbar: [{ name: "create", text: "Add New" }, {
                template: `
                <div style="width:80%; display:flex; justify-content:center; align-items:center;">
                    <h3 style="margin:0;">Product Category List</h3>
                </div>
            `
            }],

            columns: [
                { field: "SLNo", title: "SL", width: 80, attributes: { style: "text-align:center" }, headerAttributes: { style: "text-align:center" } },
                { field: "CategoryName", title: "Category", width: 220 },
                {
                    field: "objProductTypeList", title: "Type", width: 180,
                    editor: GridHelper.cboProdType,                              
                    template: "#= objProductTypeList ? (objProductTypeList.ProductTypeName || '') : '' #"
                },
                {
                    command: [
                        { name: "edit", text: { edit: "", update: "Save", cancel: "Close" }, iconClass: "k-icon k-i-edit" },
                        { name: "destroy", text: "", iconClass: "k-icon k-i-trash" }
                    ],
                    title: "&nbsp;", width: 150
                }
            ],
            editable: { mode: "inline", confirmation: "Delete this record?" },
            change: function (e) {
                var dataItem = this.dataItem(this.select());
                GridHelper.ProdCategoryGridChangeEvent(dataItem);
            }
        });
    },
    ProdTypeGridChangeEvent: function (obj) {
        var prodTypeId = obj.ProductTypeID;
      //$("#ProductTypeID").data("kendoComboBox").value(prodTypeId);
        GridManager.InitProductCategoryGrid(prodTypeId);
    },
    ProdCategoryGridChangeEvent: function (obj) {
        var prodCatgId = obj.ProductCategoryID;
        var prodTypeId = obj.ProductTypeID;
        $("#ProductTypeID").data("kendoComboBox").value(prodTypeId);
        if (parseInt(prodTypeId) > 0) {
            Helper.LoadProductCategoryCombo(prodTypeId);
            $("#ProductCategoryID").data("kendoComboBox").value(prodCatgId);
            GridHelper.ClearAllFields();
        }
       
       
    },
    ClearAllFields: function () {
        $("#ProductID").val("0");
        $("#OldProductID").val("");
        $("#ProductID2").val("");
        $("#LCMasterID_Imp").data("kendoMultiColumnComboBox").value("");
        $("#InvoiceNo").val("");
        $("#InvoiceNo").val("");
        $("#ProductName").val("");
        $("#StapleLength").val("");
        $("#ConsignmentNo").val("");
        $("#Symbol").val("");
        $("#AverageWeight").val("");
        $("#MRRWeight").val("");
        $("#Note").val("");
        $("#UnitID").data("kendoComboBox").value("");
        $("#BuyerID").data("kendoComboBox").value("");
        $("#LocationID").data("kendoComboBox").value("");
        $("#OldLocationID").data("kendoComboBox").value("");
        $("#BuyerID").data("kendoComboBox").value("");
       
    },
    GetBinCard_RawCotton: function (fromDate, toDate, productid, unitId) {
        window.open(`/RptCottonStoreUpd/BinCard_RawCotton?FromDate=${fromDate}&ToDate=${toDate}&ProductID=${productid}&UnitID=${unitId}`, "_blank")
    },
    GetBinCard_WasteCotton: function (fromDate, toDate, productid) {
        window.open(`/RptCottonStoreUpd/BinCard_WasteCotton?FromDate=${fromDate}&ToDate=${toDate}&ProductID=${productid}`, "_blank")
    },
    GetBinMRR_Report: function (lcInfo) {
        debugger;
        window.open(`/RptCottonStoreUpd/RawCottonMRRV2017?LCMasterID=${lcInfo}`, "_blank");
}
}


