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
var IsApproved = false, IsRevise = false, IsReject = false, IsFinalApproved = false;
var isApprovePage = false;


$(document).ready(function () {
    Helper.Initialize();
});

var Manager = {
    GetNew: function (id) {
        $.get(`/api/fback/new?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);
            debugger
            GridManager.InitChildTable([]);
            Helper.GetRnDBy(masterData.RnDByList);
            $("input[type='radio'][id='DEV_ID'][value='" + masterData.DevelopmentTypeID + "']").prop("checked", true);
            $("input[type='radio'][id='TEAMID'][value='" + masterData.MktTeamID + "']").prop("checked", true);
            $("input[type='radio'][id='RND_TEAM_ID'][value='" + masterData.RnDTeamID + "']").prop("checked", true);
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDetails: function (id) {
        $.get(`/api/fback/edit?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
            masterData = data;
            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs);
            Helper.GetRnDBy(masterData.RnDByList);
            $("input[type='radio'][id='DEV_ID'][value='" + masterData.DevelopmentTypeID + "']").prop("checked", true);
            $("input[type='radio'][id='TEAMID'][value='" + masterData.MktTeamID + "']").prop("checked", true);
            $("input[type='radio'][id='RND_TEAM_ID'][value='" + masterData.RnDTeamID + "']").prop("checked", true);
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    Save: function (IsApproved, IsRevise, IsReject, IsFinalApproved, SaveOption) {

        if (Helper.IsFrmValid()) {
            debugger;
            if (Helper.IsFrmValidCustom()) {
                var DevelopmentTypeID = $("input[name='DEV_ID']:checked").val();
                var RnDTeamID = $("input[name='RND_TEAM_ID']:checked").val();
                var model = FormHelper.FormElToJson(FormDom);
                model.Childs = JSON.parse(JSON.stringify($(TblChildId).data("kendoGrid").dataSource.data()));
                model.Approved = IsApproved;
                model.Revise = IsRevise;
                model.Reject = IsReject;
                model.SaveOption = SaveOption;
                model.DevelopmentTypeID = DevelopmentTypeID;
                model.RnDTeamID = RnDTeamID;
                model.FinalApproved = IsFinalApproved;
                //model.SDRFClaimIDs = "1,2";
                //model.SDRFFeelIDs = "1,2";
                //model.SDRFFragranceIDs = "1,2";
                //model.SDRFRestrictionIDs = "1,2";
                //Child Validation
                debugger;
                if (!Helper.IsFrmChildsValid(model.Childs)) {
                    return;
                }
                $.post("/api/fback/save", model, function (data) {
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
        }
    },
    RptPR: function (id) {
        window.open(`/RptCommercialExport/fback?id=${id}`, "_blank");
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
                MessageHelper.Warning("Warning", "Packaging type is required.");
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
        /*var Elements = ["FeedbackDate", "AbsorptionScoreID", "ResidueScoreID", "FragranceScoreID", "OverallScoreID"];*/
        var Elements = ["FeedbackDate", "RnDBy"];
        if (!FormHelper.IsFormValidation(FormDom, Elements)) {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
        }
        return isValidItem;
    },

    IsFrmValidCustom: function () {
        var isValidItem = true;

        var DevelopmentTypeID = $("input[name='DEV_ID']:checked").val();
        var RnDTeamID = $("input[name='RND_TEAM_ID']:checked").val();

        if (typeof DevelopmentTypeID === "undefined") {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field - Development Type.");
        }

        if (typeof RnDTeamID === "undefined") {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field - RND Team.");
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
        isApprovePage = FormHelper.ConvertToBoolean($(`#${pageId}`).find("#ApprovePage").val());
        if (isApprovePage) {
            ToolbarDom.find("#btnPendingList,#btnFinalList,#btnAllList").show();
            ToolbarDom.find("#btnEditList,#btnApprovedList,#btnRejectList").hide();
            statusCons = statusConstants.CHECK;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnPendingList"), ToolbarDom);
            GridManager.InitMasterTable();
        } else {
            ToolbarDom.find("#btnPendingList,#btnEditList,#btnPendingApproveList,#btnApprovedList,#btnRejectList,#btnFinalList,#btnAllList").show();
            statusCons = statusConstants.PENDING;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnPendingList"), ToolbarDom);
            GridManager.InitMasterTable();
        }

        //Development Code 
        //ToolbarDom.find("#btnPendingList,#btnEditList,#btnApprovedList,#btnRejectList,#btnAllList").show();
        $("#divReviseNote").hide();
        $("#divRejectNote").hide();
        GridManager.InitMasterTable();

        // ToolbarDom means Toolbar Buttons  
        ToolbarDom.find("#btnPendingList").on("click", function (e) {
            if (isApprovePage) {
                statusCons = statusConstants.CHECK;
            } else {
                statusCons = statusConstants.PENDING;
            }
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnPendingList"), ToolbarDom);
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

        ToolbarDom.find("#btnFinalList").on("click", function (e) {
            statusCons = statusConstants.COMPLETED;
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
            debugger;
            var SaveOption = Helper.GetSaveOption();
            IsApproved = false, IsRevise = false, IsReject = false, IsFinalApproved = false;
            if (isApprovePage) {
                SaveOption = 6;
            }
            Manager.Save(IsApproved, IsRevise, IsReject, IsFinalApproved, SaveOption);

        });
        $("#btnApproved").click(function (e) {
            debugger;
            var SaveOption = Helper.GetSaveOption();
            if (isApprovePage) {
                IsApproved = false, IsRevise = false, IsReject = false, IsFinalApproved = true, SaveOption = 7;
            } else {
                IsApproved = true, IsRevise = false, IsReject = false, IsFinalApproved = false;
            }
            Manager.Save(IsApproved, IsRevise, IsReject, IsFinalApproved, SaveOption);
        });
        $("#btnRevise").click(function (e) {
            var revReason = $("#ReviseReason").val();
            if (revReason == "") {
                ToastrMessageHelper.ValidationToastr("Please Input Revise Reason.");
                $("#ReviseReason").focus();
                return;
            }
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Revise this FeedBack?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsApproved = false, IsRevise = true, IsReject = false, IsFinalApproved = false;
                    Manager.Save(IsApproved, IsRevise, IsReject, IsFinalApproved, 5);
                }
            })
        });
        $("#btnReject").click(function (e) {
            var rejReason = $("#RejectReason").val();
            if (rejReason == "") {
                ToastrMessageHelper.ValidationToastr("Please Input Reject Reason.");
                $("#RejectReason").focus();
                return;
            }
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Reject this FeedBack?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsApproved = false, IsRevise = false, IsReject = true, IsFinalApproved = false;
                    Manager.Save(IsApproved, IsRevise, IsReject, IsFinalApproved, 3);
                }
            })
        });

        $("#btnBackToList").on("click", function () {
            Helper.BackToList();
        });

        //Selected Row in the kendo Grid
        KendoGridHelper.GridRowSelected(TblChildId);
    },
    GetRnDBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" },
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#RnDBy", "text", "id", columns);
    },

    GetSaveOption: function () {
        var SaveOption = 0;
        var pk_id = $("#FeedbackMasterID").val();
        pk_id = parseInt(pk_id);
        if (pk_id == 0) { SaveOption = 1; }
        if (pk_id > 0) { SaveOption = 2; }
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
                    url: `/api/fback/list/${statusCons}`,
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

        if (statusCons == statusConstants.PENDING) {
            command =
                [
                    { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                ];
        }
        else if (statusCons == statusConstants.EDIT || statusCons == statusConstants.REJECT) {
            command =
                [
                    { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
                ];

        } else if (statusCons == statusConstants.CHECK) {
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

        if (statusCons == statusConstants.PENDING) {
            columns = [
                {
                    title: "Action", command: command, width: 60
                },
                {
                    field: "SDRFMasterID", title: "SDRFMasterID", hidden: true,
                },
                {
                    field: "BuyerRefMasterID", title: "BuyerRefMasterID", hidden: true,
                },
                {
                    field: "BuyerRefColorID", title: "BuyerRefColorID", hidden: true,
                },
                {
                    field: "ChemicalCode", title: "Chemical Code", width: 160, attributes: { "style": "font-weight:700" }
                },
                {
                    field: "SDRFNo", title: "SDRF No", width: 160, attributes: { "style": "font-weight:700" }
                },
                {
                    field: "SDRFDate", title: "SDRF Date", width: 110,
                    template: "#= (SDRFDate == null)? '' : kendo.toString(kendo.parseDate(SDRFDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                },
                {
                    field: "RequiredDate", title: "Required</br>Date", width: 110,
                    template: "#= (RequiredDate == null)? '' : kendo.toString(kendo.parseDate(RequiredDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                },
                {
                    field: "BuyerName", title: "Brand</brName", width: 100,
                },
                {
                    field: "BuyerRef", title: "Brand</br>Ref", width: 120,
                },
                {
                    field: "ColorName", title: "Color</brName", width: 100,
                },
                {
                    field: "SDRFByName", title: "SDRF By", width: 100,
                },
                {
                    field: "DevelopmentType", title: "Development</br>Type", width: 130,
                },
                {
                    field: "MktTeamName", title: "Mkt. Team", width: 110,
                },
                {
                    field: "RnDTeamName", title: "R & D Team", width: 120,
                },
                {
                    field: "PriorityName", title: "Priority", width: 90,
                },
                {
                    field: "ClaimName", title: "Claim", width: 120,
                },
                {
                    field: "FeelName", title: "Feel", width: 120,
                },
                {
                    field: "FragranceName", title: "Fragrance", width: 120,
                },
                {
                    field: "RestrictionName", title: "Restriction", width: 120,
                },
            ];

            search =
            {
                fields:
                    [
                        "ChemicalCode", "SDRFNo", "SDRFDate", "RequiredDate", "BuyerName", "BuyerRef", "ColorName", "SDRFByName",
                        "DevelopmentType", "MktTeamName", "RnDTeamName", "PriorityName", "ClaimName", "FeelName", "FragranceName",
                        "RestrictionName"
                    ]
            };
        } else {
            columns = [
                {
                    title: "Action", command: command, width: 60
                },
                {
                    field: "FeedbackMasterID", title: "FeedbackMasterID", hidden: true,
                },
                {
                    field: "BuyerRefMasterID", title: "BuyerRefMasterID", hidden: true,
                },
                {
                    field: "FeedbackNo", title: "Feedback No", width: 120,
                },
                {
                    field: "FeedbackDate", title: "Feedback Date", width: 70,
                    template: "#= (FeedbackDate == null)? '' : kendo.toString(kendo.parseDate(FeedbackDate, 'yyyy-MM-dd'), 'dd-MM-yyyy') #"
                },
                {
                    field: "ChemicalCode", title: "Chemical Code", width: 120,
                },
                {
                    field: "BuyerRef", title: "Brand Ref", width: 100,
                },
                {
                    field: "ProductName", title: "Product Name", width: 100,
                },
                {
                    field: "BuyerName", title: "Brand", width: 100,
                },
                {
                    field: "RnDByName", title: "R&D By Name", width: 100,
                },
                {
                    field: "DevelopmentType", title: "Development Type", width: 100,
                },
            ];

            search =
            {
                fields:
                    [
                        "FeedbackNo", "FeedbackDate", "ChemicalCode", "BuyerRef", "ProductName", "BuyerName", "RnDByName", "DevelopmentType"
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
                    }], title: "Action &nbsp;", width: 70, hidden: childsActionBtn,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "FeedbackMasterID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "FeedbackChildID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "SDRFChildID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "SDRFMasterID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "objVariantList", title: "Variant", width: 80, editable: isApprovePage === true,
                    editor: GridHelper.cboVariant,
                    template: "#=objVariantList != null ? objVariantList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "objFormulationMasterList", title: "Formulation", width: 150, editable: isApprovePage === true,
                    editor: GridHelper.cboFormulation,
                    template: "#=objFormulationMasterList != null ? objFormulationMasterList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objPackagingTypeList", title: "Packaging</br>Type", width: 100, editable: isApprovePage === true,
                    editor: GridHelper.cboPackagingType,
                    template: "#=objPackagingTypeList != null ? objPackagingTypeList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "SizeGram", title: "Size</br>Gram", width: 80, editable: isApprovePage === true,
                    footerTemplate: "<span id='sumSizeGram'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "objUOMList", title: "UOM", width: 80, editable: isApprovePage === true,
                    editor: GridHelper.cboUOM,
                    template: "#=objUOMList != null ? objUOMList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "BatchNo", title: "Batch No", width: 100, editable: isApprovePage === true,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "MultipleClaimList", title: "Claim", width: 160, editable: isApprovePage === true,
                    editor: GridHelper.cboMultipleClaim,
                    template: $("#ClaimTemplate").html(),
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "MultipleFeelList", title: "Feel", width: 160, editable: isApprovePage === true,
                    editor: GridHelper.cboMultipleFeel,
                    template: $("#FeelTemplate").html(),
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "MultipleFragranceList", title: "Fragrance", width: 160, editable: isApprovePage === true,
                    editor: GridHelper.cboMultipleFragrance,
                    template: $("#FragranceTemplate").html(),
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "MultipleRestrictionList", title: "Restriction", width: 160, editable: isApprovePage === true,
                    editor: GridHelper.cboMultipleRestriction,
                    template: $("#RestrictionTemplate").html(),
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "ChildRemarks", title: "Remarks", width: 120, editable: isApprovePage === true,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "BuyerApproved", title: "Is Approve?", width: 110, hidden: isApprovePage === false,
                    template: "<input class='customClass' data-field='BuyerApproved' #if (BuyerApproved) { # checked='checked' # } # type='checkbox' />",
                    headerTemplate: "<input class='approveAll' type='checkbox' id='approveAll" + TblMasterId.slice(1) + "'/><span> Approve All</span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-center", true, true)
                },
                {
                    field: "BuyerRejected", title: "Is Reject?", width: 110, hidden: isApprovePage === false,
                    template: "<input class='customClass' data-field='BuyerRejected' #if (BuyerRejected) { # checked='checked' # } # type='checkbox' />",
                    headerTemplate: "<input class='rejectAll' type='checkbox' id='rejectAll" + TblMasterId.slice(1) + "'/><span> Reject All</span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-center", true, true)
                },
                {
                    field: "BuyerRejectReason", title: "Brand</br>Reject Reason", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
            ];



        //// Dynamicall Hide Columns - Start
        //var GridColumnsHide = [];
        //var GridColumnsDisable = [];
        //if (isApprovePage) {
        //    GridColumnsHide = [""];
        //    GridColumnsDisable = ["ChildRemarks"];

        //} else {
        //    GridColumnsHide = ["BuyerRejected", "BuyerApproved"];
        //    GridColumnsDisable = [];
        //}
        //for (var i = 0; i < GridColumnsHide.length; i++) {
        //    var fieldName = GridColumnsHide[i];
        //    for (var j = 0; j < columns.length; j++) {
        //        if (columns[j].field === fieldName) {
        //            columns[j].hidden = true;
        //            break;
        //        }
        //    }
        //}
        //for (var i = 0; i < GridColumnsDisable.length; i++) {
        //    var fieldName = GridColumnsDisable[i];
        //    //alert(fieldName);
        //    for (var j = 0; j < columns.length; j++) {
        //        if (columns[j].field === fieldName) {
        //            columns[j].editable = false;
        //            break;
        //        }
        //    }
        //}

        // Dynamicall Hide Columns - End


        var schema = {
            model: {
                fields: {
                    FeedbackMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    FeedbackChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    SDRFChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    SDRFMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },

                    objVariantList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objFormulationMasterList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objPackagingTypeList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    SizeGram: { type: "number", editable: function () { return false; } },
                    objUOMList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    BatchNo: { type: "string" },
                    MultipleClaimList: {
                        defaultValue: { id: null, text: "" },
                    },
                    MultipleFeelList: {
                        defaultValue: { id: null, text: "" },
                    },
                    MultipleFragranceList: {
                        defaultValue: { id: null, text: "" },
                    },
                    MultipleRestrictionList: {
                        defaultValue: { id: null, text: "" },
                    },
                    ChildRemarks: { type: "string" },
                    BuyerApproved: { editable: false, defaultValue: false },
                    BuyerRejected: { editable: false, defaultValue: false },
                    BuyerRejectReason: { type: "string", editable: isApprovePage === true }
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId, columns, data, schema, true, true, false);
        GridHelper.InitializeHelper();
        KendoGridHelper.EnterKeyUp(TblChildId);
        GridHelper.RegisterEvents();
        GridHelper.ApproveAll();
        GridHelper.RejectAll();
        //GridHelper.ResetAll();
        if ((statusCons == statusConstants.PENDING) || (statusCons == statusConstants.EDIT) || (statusCons == statusConstants.APPROVED)) {
            $(TblChildId + " .k-toolbar").show();
        } else {
            $(TblChildId + " .k-toolbar").hide();
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

            $("#btnSave,#btnApproved,#btnRevise,#btnReject").hide();
            $("#divReviseNote,#divRejectNote").hide();

            childsActionBtn = true;
            childsToolsBar = false;

            if (args.data.commandName == "rptPR") {
                Manager.RptPR(selectedItem.PRMasterID);
                return;
            }

            if (statusCons == statusConstants.PENDING) {
                $("#btnSave,#btnApproved").show();
                childsActionBtn = false;
                childsToolsBar = true;
                Manager.GetNew(selectedItem.SDRFMasterID);
                return;
            } else {
                if (statusCons == statusConstants.EDIT) {
                    $("#btnSave,#btnApproved").show();
                    childsActionBtn = false;
                    childsToolsBar = false;
                }
                else if (statusCons == statusConstants.APPROVED) {
                    $("#btnRevise,#btnReject").show();
                    $("#divReviseNote,#divRejectNote").show();
                }
                else if (statusCons == statusConstants.REJECT) {
                    $("#btnSave,#btnApproved").show();
                    childsActionBtn = false;
                    childsToolsBar = false;
                    $("#divRejectNote").show();
                } else if (statusCons == statusConstants.CHECK) {
                    $("#btnSave,#btnApproved").show();
                    $("#FeedbackDate,#RnDBy,#AbsorptionScoreID,#ResidueScoreID,#FragranceScoreID,#OverallScoreID,#Remarks").prop("disabled", true);
                    childsActionBtn = true;
                    childsToolsBar = true;
                }

                Manager.GetDetails(selectedItem.FeedbackMasterID);
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

    cboVariant: function (container, options) {
        $('<input id="cmbVariant" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.VariantList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },
    cboFormulation: function (container, options) {
        $('<input id="cmbFormulation" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.FormulationMasterList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
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
    cboMultipleClaim: function (container, options) {
        $('<input id="cmbMultipleClaim" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoMultiSelect({
                filter: "contains",
                suggest: true,
                dataTextField: "text",
                dataValueField: "id",
                optionLabel: '',
                placeholder: "Select",
                autoWidth: true,
                dataSource: masterData.ClaimList,
                change: function (e) {
                    //KendoGridHelper.IsValidItem("cmbColor")
                    //KendoGridHelper.RemoveUndefined(e);

                    var ClaimList = $("#cmbMultipleClaim").data("kendoMultiSelect").dataItems();
                    var itemGrid = $(TblChildId).data("kendoGrid");
                    var selectedItem = itemGrid.dataItem(itemGrid.select());

                    if (selectedItem) {
                        let ClaimIDs = ClaimList.map(f => f.id).join(',');
                        selectedItem.set('ClaimIDs', ClaimIDs);
                    }
                }
            });
    },

    cboMultipleFeel: function (container, options) {
        $('<input id="cmbMultipleFeel" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoMultiSelect({
                filter: "contains",
                suggest: true,
                dataTextField: "text",
                dataValueField: "id",
                optionLabel: '',
                placeholder: "Select",
                autoWidth: true,
                dataSource: masterData.FeelList,
                change: function (e) {
                    //KendoGridHelper.IsValidItem("cmbColor")
                    //KendoGridHelper.RemoveUndefined(e);

                    var FeelList = $("#cmbMultipleFeel").data("kendoMultiSelect").dataItems();
                    var itemGrid = $(TblChildId).data("kendoGrid");
                    var selectedItem = itemGrid.dataItem(itemGrid.select());

                    if (selectedItem) {
                        let FeelIDs = FeelList.map(f => f.id).join(',');
                        selectedItem.set('FeelIDs', FeelIDs);
                    }
                }
            });
    },

    cboMultipleFragrance: function (container, options) {
        $('<input id="cmbMultipleFragrance" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoMultiSelect({
                filter: "contains",
                suggest: true,
                dataTextField: "text",
                dataValueField: "id",
                optionLabel: '',
                placeholder: "Select",
                autoWidth: true,
                dataSource: masterData.FragranceList,
                change: function (e) {
                    //KendoGridHelper.IsValidItem("cmbColor")
                    //KendoGridHelper.RemoveUndefined(e);

                    var FragranceList = $("#cmbMultipleFragrance").data("kendoMultiSelect").dataItems();
                    var itemGrid = $(TblChildId).data("kendoGrid");
                    var selectedItem = itemGrid.dataItem(itemGrid.select());

                    if (selectedItem) {
                        let FragranceIDs = FragranceList.map(f => f.id).join(',');
                        selectedItem.set('FragranceIDs', FragranceIDs);
                    }
                }
            });
    },

    cboMultipleRestriction: function (container, options) {
        $('<input id="cmbMultipleRestriction" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoMultiSelect({
                filter: "contains",
                suggest: true,
                dataTextField: "text",
                dataValueField: "id",
                optionLabel: '',
                placeholder: "Select",
                autoWidth: true,
                dataSource: masterData.RestrictionList,
                change: function (e) {
                    //KendoGridHelper.IsValidItem("cmbColor")
                    //KendoGridHelper.RemoveUndefined(e);

                    var RestrictionList = $("#cmbMultipleRestriction").data("kendoMultiSelect").dataItems();
                    var itemGrid = $(TblChildId).data("kendoGrid");
                    var selectedItem = itemGrid.dataItem(itemGrid.select());

                    if (selectedItem) {
                        let RestrictionIDs = RestrictionList.map(f => f.id).join(',');
                        selectedItem.set('RestrictionIDs', RestrictionIDs);
                    }
                }
            });
    },

    GridCenterAlignAttributes: function (align_header = "", align_body = "", align_footer = "") {
        return {
            headerAttributes: { "class": align_header + " k_input_cell" },
            attributes: { "class": align_body + " k_input_cell" },
            footerAttributes: { "class": align_footer + " k_input_cell" }
        };
    },
    RegisterEvents: function () {
        $(TblChildId).on("change", ".customClass", function () {
            var grid = $(TblChildId).data("kendoGrid");
            var uid = $(this).closest("tr").data("uid");
            var dataItem = grid.dataSource.getByUid(uid);
            var field = $(this).data("field");
            var isChecked = $(this).is(":checked");
            if (field && dataItem) {
                dataItem[field] = isChecked;
                if (field === "BuyerApproved") {
                    dataItem["BuyerRejected"] = false;
                    $(this).closest("tr").find("input[data-field='BuyerRejected']").prop("checked", false);
                }
                if (field === "BuyerRejected") {
                    dataItem["BuyerApproved"] = false;
                    $(this).closest("tr").find("input[data-field='BuyerApproved']").prop("checked", false);
                }
            }
            $(TblChildId).find(".approveAll").prop("checked", false);
            $(TblChildId).find(".rejectAll").prop("checked", false);

        });
    },
    ApproveAll: function () {
        $(TblChildId).on("change", ".approveAll", function () {
            var grid = $(TblChildId).data("kendoGrid");
            var isChecked = $(this).is(":checked");
            grid.tbody.find("tr").each(function (index, item) {
                var uid = $(item).data("uid");
                var dataItem = grid.dataSource.getByUid(uid);
                dataItem["BuyerApproved"] = isChecked ? true : false;
                dataItem["BuyerRejected"] = false;
                $(item).find("input[data-field='BuyerApproved']").prop("checked", isChecked);
                $(item).find("input[data-field='BuyerRejected']").prop("checked", false);
            });
            $(TblChildId).find(".rejectAll").prop("checked", false);
        });
    },
    RejectAll: function () {
        $(TblChildId).on("change", ".rejectAll", function () {
            var grid = $(TblChildId).data("kendoGrid");
            var isChecked = $(this).is(":checked");
            grid.tbody.find("tr").each(function (index, item) {
                var uid = $(item).data("uid");
                var dataItem = grid.dataSource.getByUid(uid);
                dataItem["BuyerRejected"] = isChecked ? true : false;
                dataItem["BuyerApproved"] = false;
                $(item).find("input[data-field='BuyerRejected']").prop("checked", isChecked);
                $(item).find("input[data-field='BuyerApproved']").prop("checked", false);
            });
            $(TblChildId).find(".approveAll").prop("checked", false);
        });
    },

}



