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
        $.get(`/api/BuyerRef/new`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();

            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);  
            Helper.GetProduct(masterData.ProductList);
            Helper.GetMarketingPerson(masterData.MarketingPersonList);
            KendoDropDownHelper.MultiSelectCombo(masterData.ColorList, "ColorID", "text", "id", "", "", false);
             
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id) {
        $.get(`/api/BuyerRef/edit?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();

            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);

            Helper.GetProduct(masterData.ProductList);
            Helper.GetMarketingPerson(masterData.MarketingPersonList);
            KendoDropDownHelper.MultiSelectCombo(masterData.ColorList, "ColorID", "text", "id", "", "", false); 
            if (masterData.ColorIDs) {
                var colorValues = masterData.ColorIDs.toString().split(",");
                $("#ColorID").data("kendoMultiSelect").value(colorValues);
            }  
            $("#ProductCategoryName").val(masterData.ProductCategoryName);
            $("#ProductTypeName").val(masterData.ProductTypeName); 
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    Save: function (IsApproved, IsRevise, IsReject) {
        if (Helper.IsFrmValid()) {

            var model = FormHelper.FormElToJson(FormDom);

            var multiSelect = $("#ColorID").data("kendoMultiSelect");
            if (multiSelect && multiSelect.value().length > 0) {
                model.ColorIDs = multiSelect.value().join(",");
                model.ColorName = multiSelect.dataItems().map(x => x.text).join(",");
            } else {
                $(".ColorID").show();
                $(".ColorID").html("required.");
            }
            model.Approved = IsApproved;
            model.IsRevise = IsRevise;
            model.Reject = IsReject; 
            debugger;
            $.post("/api/BuyerRef/save", model, function (data) {
                if (parseInt(data.ResultId) > 0) {
                    MessageHelper.Success("Save Operations", data.Message);
                    //Helper.BackToList();

                    // Upload file
                    Helper.FileUpload(data.ResultId);
                } else {
                    if (data.IsLogin == "1") {
                        window.location.href = data.redirectUrl;
                    }
                    MessageHelper.Warning("Save Operations", data.Message);
                }
            });
        }
    }, 
    RptPR: function (id) {
        window.open(`/RptCommercialExport/BuyerRef?id=${id}`, "_blank");
    },
}

var Helper = {
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
        var isValidItem = true;
        var Elements = ["BuyerRef", "BuyerID", "FinishedProductID", "BuyerRefDate", "SeasonID", "MarketingPersonID"];
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

            $("#btnSave,#btnApproved").show();
            $("#btnRevise,#btnReject").hide();
            $("#divReviseNote,#divRejectNote").hide();
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
        $("#btnSave").on("click", function () {
            IsApproved = false, IsRevise = false, IsReject = false;
            Manager.Save(IsApproved, IsRevise, IsReject);
        });
        $("#btnApproved").on("click", function () { 
            IsApproved = true, IsRevise = false, IsReject = false;
            Manager.Save(IsApproved, IsRevise, IsReject);
        });
        $("#btnRevise").on("click", function () {  
            var revReason = $("#ReviseReason").val();
            if (revReason == "") {
                ToastrMessageHelper.FxToastr("Please Input Revise Reason.");
                $("#ReviseReason").focus();
                return;
            }
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Revise this Reference?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsApproved = false, IsRevise = true, IsReject = false;
                    Manager.Save(IsApproved, IsRevise, IsReject);
                }
            })
        });
        $("#btnReject").on("click", function () {   
            var rejReason = $("#RejectReason").val();
            if (rejReason == "") {
                ToastrMessageHelper.FxToastr("Please Input Reject Reason.");
                $("#RejectReason").focus();
                return;
            }

            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Reject this Reference?')
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
        $("#btnPrintPI").on("click", function () {  
            var piid = $("#PINo").val();
            if (piid) {
                Manager.GetPIReport(piid);
            }
        });

        //Change Event
        $("#FinishedProductID").on("change", function () { 
            var cboProductID = $(this).data("kendoMultiColumnComboBox");
            var ProductID = cboProductID ? cboProductID.dataItem() : null;

            if (!ProductID) {
                $("#FinishedProductCategoryID").val('');
                $("#ProductCategoryName").val('');

                $("#FinishedProductTypeID").val('');
                $("#ProductTypeName").val(''); 
                return;
            }

            $("#FinishedProductCategoryID").val(ProductID.valueA);
            $("#ProductCategoryName").val(ProductID.valueB);

            $("#FinishedProductTypeID").val(ProductID.valueC);
            $("#ProductTypeName").val(ProductID.valueD); 
        });
        $("#MarketingPersonID").on("change", function () {
            var cboMarketingPersonID = $(this).data("kendoMultiColumnComboBox");
            var MarketingPersonID = cboMarketingPersonID ? cboMarketingPersonID.dataItem() : null;

            if (!MarketingPersonID) {
                $("#DepartmentID").val('');
                $("#SectionID").val('');
                $("#SubSectionID").val('');
                return;
            }

            $("#DepartmentID").val(MarketingPersonID.valueA);
            $("#SectionID").val(MarketingPersonID.valueB);
            $("#SubSectionID").val(MarketingPersonID.valueC);
        });
        $("#BuyerRefFiles").change(function (e) {
            var IsUpload = FileUploadHelper.HandleFiles(e, 'canvBuyerRefFiles');
            if (IsUpload == false) {
                $("#BuyerRefFiles").val(null);
            }
        });
        //Selected Row in the kendo Grid
        KendoGridHelper.GridRowSelected(TblChildId);
    },
    GetProduct: function (data) {  
        var columns = [
            { field: "text", title: "Product Name", width: "200" },
            { field: "valueB", title: "Category Name", width: "200" },
            { field: "valueD", title: "Product Type", width: "200" }, 
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#FinishedProductID", "text", "id", columns); 

        var oldValue = $("#FinishedProductID").val();   
        if (oldValue && oldValue == "0") {
            $("#FinishedProductID").data("kendoMultiColumnComboBox").value(null);  
        }   
    },
    GetMarketingPerson: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" },
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#MarketingPersonID", "text", "id", columns);

        var oldValue = $("#MarketingPersonID").val();
        if (oldValue && oldValue == "0") {
            $("#MarketingPersonID").data("kendoMultiColumnComboBox").value(null);
        }
    },
    FileUpload: function (ResultId) { 
        var HTMLTextBoxID = "",
            FilePath = "",
            PrimaryKeyID = 0,
            FileFormatName = "",
            Usql = "",
            Dsql = "",
            IsDeleted = 0,
            Condition = "",
            Is64Byte = false,
            PreviousFilePath = "",
            IsMultiple = false;

        if ($("#BuyerRefFiles").val() != "") {
            HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", IsMultiple = true;

            HTMLTextBoxID = "BuyerRefFiles";
            FilePath = "\\Files\\Marketing\\BuyerRef\\";
            PrimaryKeyID = ResultId;
            FileFormatName = "BF_";
            IsDeleted = 1;
            Dsql = " Delete From mkt_BuyerRefFiles Where BuyerRefMasterID = " + PrimaryKeyID;
            Usql = " Insert into mkt_BuyerRefFiles (FileName, BuyerRefMasterID, IsDeleted) ";
            FileUploadHelper.SetUploadedFile(HTMLTextBoxID, FilePath, PrimaryKeyID, FileFormatName, Usql, Dsql, IsDeleted, Condition, Is64Byte, IsMultiple,); 
        }  
        FileUploadHelper.SaveFile(ResultId, "/api/helperC/FileUpload");
        Helper.BackToList();
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
                    url: `/api/BuyerRef/list/${statusCons}`,
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
                ];
        } else {
            command =
                [
                    { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands }, 
                ];
        }
        var columns = [
            {
                title: "Action", command: command, width: 60
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "BuyerRefMasterID", title: "BuyerRefMasterID", hidden: true,
            }, 
            {
                field: "download", title: "Files",
                filterable: false, width: 135,
                template: "#=GridHelper.BtnFileTemplate(data) #"
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "ChemicalCode", title: "Chemical</br>Code", width: 120 
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
            },
            {
                field: "BuyerRef", title: "Brand Ref.", width: 120,
                filterable: true 
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
            },
            {
                field: "BuyerRefDate", title: "Brand Ref.</br>Date", width: 120,
                template: "#= (BuyerRefDate == null)? '' : kendo.toString(kendo.parseDate(BuyerRefDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            }, 
            {
                field: "BuyerName", title: "Brand Name", width: 130,
                filterable: true 
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "ProductName", title: "Product</br>Name", width: 130, filterable: true
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "ProductCategoryName", title: "Product</br>Category", width: 110, filterable: true
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "ProductTypeName", title: "Product</br>Type", width: 120, filterable: true
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "ColorName", title: "Color</br>Name", width: 120,
                filterable: true
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "SeasonName", title: "Season", width: 100, filterable: true
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            },
            {
                field: "MktPersonName", title: "Marketing</br>Person", width: 120, filterable: true
                , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
            }, 
        ];
        var search =
        {
            fields:
                [
                    "ChemicalCode", "BuyerRef", "BuyerRefDate", "BuyerName", "ProductName",
                    "ColorName", "SeasonName", "MktPersonName"
                ]
        };
        var height = $(window).height() - 120;
        KendoGridHelper.MasterGridLoad(TblMasterId, height + "px", search, columns, true)
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

            Manager.GetDetails(selectedItem.BuyerRefMasterID);
        }
    }, 
    BtnFileTemplate: function (data) {
        var btnContent = "";
        
        //TechPackFile
        if (data.BuyerRefFiles1 != "" && data.BuyerRefFiles1 != null) {
            btnContent += '<button type="button" title="BuyerRef" class="k-button k-button-icontext k-success" onClick="GridHelper.BtnFileClick(event,' + "'BuyerRefFiles1'" + ')"><i class="k-grid-button-rpt fa fa-file-pdf" aria-hidden="true" style="margin-right: 3px;"></i> Brand Ref Files 1</button>';
        }
        if (data.BuyerRefFiles2 != "" && data.BuyerRefFiles2 != null) {
            btnContent += '<button type="button" title="BuyerRef" class="k-button k-button-icontext k-success" onClick="GridHelper.BtnFileClick(event,' + "'BuyerRefFiles2'" + ')"><i class="k-grid-button-rpt fa fa-file-pdf" aria-hidden="true" style="margin-right: 3px;"></i> Brand Ref Files 2</button>';
        }
        if (data.BuyerRefFiles3 != "" && data.BuyerRefFiles3 != null) {
            btnContent += '<button type="button" title="BuyerRef" class="k-button k-button-icontext k-success" onClick="GridHelper.BtnFileClick(event,' + "'BuyerRefFiles3'" + ')"><i class="k-grid-button-rpt fa fa-file-pdf" aria-hidden="true" style="margin-right: 3px;"></i> Brand Ref Files 3</button>';
        }
        if (data.BuyerRefFiles4 != "" && data.BuyerRefFiles4 != null) {
            btnContent += '<button type="button" title="BuyerRef" class="k-button k-button-icontext k-success" onClick="GridHelper.BtnFileClick(event,' + "'BuyerRefFiles4'" + ')"><i class="k-grid-button-rpt fa fa-file-pdf" aria-hidden="true" style="margin-right: 3px;"></i> Brand Ref Files 4</button>';
        }
        if (data.BuyerRefFiles5 != "" && data.BuyerRefFiles5 != null) {
            btnContent += '<button type="button" title="BuyerRef" class="k-button k-button-icontext k-success" onClick="GridHelper.BtnFileClick(event,' + "'BuyerRefFiles5'" + ')"><i class="k-grid-button-rpt fa fa-file-pdf" aria-hidden="true" style="margin-right: 3px;"></i> Brand Ref Files 5</button>';
        }
        if (data.BuyerRefFiles6 != "" && data.BuyerRefFiles6 != null) {
            btnContent += '<button type="button" title="BuyerRef" class="k-button k-button-icontext k-success" onClick="GridHelper.BtnFileClick(event,' + "'BuyerRefFiles6'" + ')"><i class="k-grid-button-rpt fa fa-file-pdf" aria-hidden="true" style="margin-right: 3px;"></i> Brand Ref Files 6</button>';
        }
        if (data.BuyerRefFiles7 != "" && data.BuyerRefFiles7 != null) {
            btnContent += '<button type="button" title="BuyerRef" class="k-button k-button-icontext k-success" onClick="GridHelper.BtnFileClick(event,' + "'BuyerRefFiles7'" + ')"><i class="k-grid-button-rpt fa fa-file-pdf" aria-hidden="true" style="margin-right: 3px;"></i> Brand Ref Files 7</button>';
        }
        if (data.BuyerRefFiles8 != "" && data.BuyerRefFiles8 != null) {
            btnContent += '<button type="button" title="BuyerRef" class="k-button k-button-icontext k-success" onClick="GridHelper.BtnFileClick(event,' + "'BuyerRefFiles8'" + ')"><i class="k-grid-button-rpt fa fa-file-pdf" aria-hidden="true" style="margin-right: 3px;"></i> Brand Ref Files 8</button>';
        }
        if (data.BuyerRefFiles9 != "" && data.BuyerRefFiles9 != null) {
            btnContent += '<button type="button" title="BuyerRef" class="k-button k-button-icontext k-success" onClick="GridHelper.BtnFileClick(event,' + "'BuyerRefFiles9'" + ')"><i class="k-grid-button-rpt fa fa-file-pdf" aria-hidden="true" style="margin-right: 3px;"></i> Brand Ref Files 8</button>';
        }
        if (data.BuyerRefFiles10 != "" && data.BuyerRefFiles10 != null) {
            btnContent += '<button type="button" title="BuyerRef" class="k-button k-button-icontext k-success" onClick="GridHelper.BtnFileClick(event,' + "'BuyerRefFiles10'" + ')"><i class="k-grid-button-rpt fa fa-file-pdf" aria-hidden="true" style="margin-right: 3px;"></i> Brand Ref Files 10</button>';
        } 
        return btnContent;
    },
    BtnFileClick: function (e, btnAction) {
        var grid = $(TblMasterId).data("kendoGrid"); 
        var tr = $(e.currentTarget).closest("tr");
        var dataItem = grid.dataItem($(this).closest('tr'));
        var selectedItem = grid.dataItem(tr);
        grid.select(tr);

        if (selectedItem != null) { 
            if (btnAction == "BuyerRefFiles1") {
                FileUploadHelper.DownloadFile(selectedItem.BuyerRefFiles1);
            }
            else if (btnAction == "BuyerRefFiles2") {
                FileUploadHelper.DownloadFile(selectedItem.BuyerRefFiles2);
            }
            else if (btnAction == "BuyerRefFiles3") {
                FileUploadHelper.DownloadFile(selectedItem.BuyerRefFiles3);
            }
            else if (btnAction == "BuyerRefFiles4") {
                FileUploadHelper.DownloadFile(selectedItem.BuyerRefFiles4);
            }
            else if (btnAction == "BuyerRefFiles5") {
                FileUploadHelper.DownloadFile(selectedItem.BuyerRefFiles5);
            }
            else if (btnAction == "BuyerRefFiles6") {
                FileUploadHelper.DownloadFile(selectedItem.BuyerRefFiles6);
            }
            else if (btnAction == "BuyerRefFiles7") {
                FileUploadHelper.DownloadFile(selectedItem.BuyerRefFiles7);
            }
            else if (btnAction == "BuyerRefFiles8") {
                FileUploadHelper.DownloadFile(selectedItem.BuyerRefFiles8);
            }
            else if (btnAction == "BuyerRefFiles9") {
                FileUploadHelper.DownloadFile(selectedItem.BuyerRefFiles9);
            }
            else if (btnAction == "BuyerRefFiles10") {
                FileUploadHelper.DownloadFile(selectedItem.BuyerRefFiles10);
            }
        }
    },
}


