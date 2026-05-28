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
var IsSendForApproval = false,IsApproved = false, IsRevise = false, IsReject = false; 
var isApprovePage = false;

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
            //Helper.GetProduct(masterData.ProductList);
            //alert(1)
            //GridManager.InitChildTable([]);
            GridManager.InitChildTable_Ingredients([]);
            GridManager.InitChildTable_ReferenceProducts([]);

            Helper.GetMarketingPerson(masterData.MarketingPersonList);
            KendoDropDownHelper.MultiSelectCombo(masterData.ColorList, "ColorID", "text", "id", "", "", false);

        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    //GetNew: function (BuyerRefMasterID, BuyerRefColorID) {  
    //    $.get(`/api/sdrfentry/new?BuyerRefMasterID=${BuyerRefMasterID}&BuyerRefColorID=${BuyerRefColorID}`, function (data) {
    //        DivDetailsDom.fadeIn();
    //        DivToolbarDom.fadeOut(); 
            
    //        masterData = data; 
    //        FormHelper.SetFormData(FormDom, masterData);

    //        GridManager.InitChildTable([]); 
    //        //GridManager.InitChildTable(masterData.Childs); 
    //        Helper.GetSDRFBy(masterData.SDRFByList);
    //        KendoDropDownHelper.MultiSelectCombo(masterData.ClaimList, "SDRFClaimID", "text", "id", "", "", false);
    //        KendoDropDownHelper.MultiSelectCombo(masterData.FeelList, "SDRFFeelID", "text", "id", "", "", false);
    //        KendoDropDownHelper.MultiSelectCombo(masterData.FragranceList, "SDRFFragranceID", "text", "id", "", "", false);
    //        KendoDropDownHelper.MultiSelectCombo(masterData.RestrictionList, "SDRFRestrictionID", "text", "id", "", "", false);

    //    }).catch(function (err) {
    //        MessageHelper.Warning("Error = ", err.response.data.Message);
    //    });
    //},
    GetDetails: function (id) {
        $.get(`/api/sdrfentry/edit?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut(); 
         
            masterData = data; 
            //Helper.GetPRBy(masterData.PRByList);



            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable(masterData.Childs); 

            Helper.GetSDRFBy(masterData.SDRFByList);
            KendoDropDownHelper.MultiSelectCombo(masterData.ClaimList, "SDRFClaimID", "text", "id", "", "", false);
            KendoDropDownHelper.MultiSelectCombo(masterData.FeelList, "SDRFFeelID", "text", "id", "", "", false);
            KendoDropDownHelper.MultiSelectCombo(masterData.FragranceList, "SDRFFragranceID", "text", "id", "", "", false);
            KendoDropDownHelper.MultiSelectCombo(masterData.RestrictionList, "SDRFRestrictionID", "text", "id", "", "", false);


            $("input[type='radio'][id='DEV_ID'][value='" + masterData.DevelopmentTypeID + "']").prop("checked", true);
            $("input[type='radio'][id='TEAMID'][value='" + masterData.MktTeamID + "']").prop("checked", true);
            $("input[type='radio'][id='RND_TEAM_ID'][value='" + masterData.RnDTeamID + "']").prop("checked", true);


            var values = "";

            KendoDropDownHelper.MultiSelectCombo(masterData.ClaimList, "SDRFClaimID", "text", "id", "", "", false);
            if (masterData.SDRFClaimIDs) {
                values = masterData.SDRFClaimIDs.toString().split(",");
                $("#SDRFClaimID").data("kendoMultiSelect").value(values);
            }
            KendoDropDownHelper.MultiSelectCombo(masterData.FeelList, "SDRFFeelID", "text", "id", "", "", false);
            if (masterData.SDRFFeelIDs) {
                values = masterData.SDRFFeelIDs.toString().split(",");
                $("#SDRFFeelID").data("kendoMultiSelect").value(values);
            }
            KendoDropDownHelper.MultiSelectCombo(masterData.FragranceList, "SDRFFragranceID", "text", "id", "", "", false);
            if (masterData.SDRFFragranceIDs) {
                values = masterData.SDRFFragranceIDs.toString().split(",");
                $("#SDRFFragranceID").data("kendoMultiSelect").value(values);
            }
            KendoDropDownHelper.MultiSelectCombo(masterData.RestrictionList, "SDRFRestrictionID", "text", "id", "", "", false);
            if (masterData.SDRFRestrictionIDs) {
                values = masterData.SDRFRestrictionIDs.toString().split(",");
                $("#SDRFRestrictionID").data("kendoMultiSelect").value(values);
            }


            $("#btnPrintPR").show();   
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    }, 
    Save: function (IsSendForApproval,IsApproved, IsRevise, IsReject, CrudValue) { 


        if (Helper.IsFrmValid()) {

            var DevelopmentTypeID = $("input[name='DEV_ID']:checked").val();
            var MktTeamID = $("input[name='TEAMID']:checked").val();
            var RnDTeamID = $("input[name='RND_TEAM_ID']:checked").val();


            debugger;
            var SaveOption = Helper.GetSaveOption(CrudValue);
            var model = FormHelper.FormElToJson(FormDom);
            model.SaveOption = SaveOption; 
            model.Childs = JSON.parse(JSON.stringify($(TblChildId).data("kendoGrid").dataSource.data()));
            model.Approved = IsApproved;
            model.IsRevise = IsRevise;
            model.Reject = IsReject;
            model.SendForApproval = IsSendForApproval;

            model.DevelopmentTypeID = DevelopmentTypeID;
            model.MktTeamID = MktTeamID;
            model.RnDTeamID = RnDTeamID;




            var multiSelect = $("#SDRFClaimID").data("kendoMultiSelect");
            if (multiSelect && multiSelect.value().length > 0) {
                model.SDRFClaimIDs = multiSelect.value().join(",");
                //model.ColorName = multiSelect.dataItems().map(x => x.text).join(",");
            }
            var multiSelect = $("#SDRFFeelID").data("kendoMultiSelect");
            if (multiSelect && multiSelect.value().length > 0) {
                model.SDRFFeelIDs = multiSelect.value().join(",");
                //model.ColorName = multiSelect.dataItems().map(x => x.text).join(",");
            }
            var multiSelect = $("#SDRFFragranceID").data("kendoMultiSelect");
            if (multiSelect && multiSelect.value().length > 0) {
                model.SDRFFragranceIDs = multiSelect.value().join(",");
                //model.ColorName = multiSelect.dataItems().map(x => x.text).join(",");
            }
            var multiSelect = $("#SDRFRestrictionID").data("kendoMultiSelect");
            if (multiSelect && multiSelect.value().length > 0) {
                model.SDRFRestrictionIDs = multiSelect.value().join(",");
                //model.ColorName = multiSelect.dataItems().map(x => x.text).join(",");
            }




            




            //var selectedValue = ("input[name='DEV_ID']:checked").val();

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

            //Child Validation
            if (!Helper.IsFrmChildsValid(model.Childs)) {
                return;
            }
            $.post("/api/sdrfentry/save", model, function (data) {
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
        window.open(`/RptCommercialExport/sdrfentry?id=${id}`, "_blank");
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
                return;
            }
            if (obj.objUOMList.id == 0 || obj.objUOMList.text == null) {
                MessageHelper.Warning("Warning", "UOM is required.");
                isValidItem = false;
                return;
            }

            obj.UOMID = obj.objUOMList.id;
            obj.PackagingTypeID = obj.objPackagingTypeList.id;


        });

        //Childs Length Check
        if (ChildsList.length === 0) {
            MessageHelper.Warning("Warning", "At least 1 packaging type is required.");
            return isValidItem = false;
        }
        return isValidItem;
    },
    IsFrmValid: function () {
        var isValidItem = true;
        var Elements = ["SDRFDate", "RequiredDate", "SDRFBy", "PriorityID"];
        if (!FormHelper.IsFormValidation(FormDom, Elements)) {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
        }

        if ($("#SDRFBy").val() == 0) {
            $(".SDRFBy").show();
            isValidItem = false;
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
        TblChildId_Ingredients = pageConstants.CHILD_TABLE_ID2 + pageId; 
        TblChildId_ReferenceProducts = pageConstants.CHILD_TABLE_ID3 + pageId; 
        FormDom = $(pageConstants.FORM_ID + pageId);
        DivDetailsDom = $(pageConstants.DIV_DETAILS_ID + pageId);  

        //Development Code 
        //isApprovePage = FormHelper.ConvertToBoolean($(`#${pageId}`).find("#ApprovePage").val());
       
        //if (isApprovePage) {
        ToolbarDom.find("#btnNewAdd,#btnEditList,#btnApprovedList,#btnRejectList,#btnAllList").show();
            //ToolbarDom.find("#btnPendingList,#btnEditList").hide();
            statusCons = statusConstants.PROPOSED_FOR_APPROVAL;
        FormHelper.TogToolbarBtn(ToolbarDom.find("#btnEditList"), ToolbarDom);
        Manager.GetNew();
        GridManager.InitMasterTable();
        
        //} else {
        //    ToolbarDom.find("#btnPendingList,#btnEditList,#btnPendingApproveList,#btnApprovedList,#btnRejectList,#btnAllList").show();
        //    statusCons = statusConstants.PENDING;
        //    FormHelper.TogToolbarBtn(ToolbarDom.find("#btnPendingList"), ToolbarDom);
        //    GridManager.InitMasterTable(); 
        //}  
        ToolbarDom.find("#btnNewAdd").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(ToolbarDom.find("#btnEditList"), ToolbarDom);
            Manager.GetNew();

            $("#btnSave,#btnApproved").show();
            $("#btnRevise,#btnReject").hide();
            $("#divReviseNote,#divRejectNote").hide();
        });
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
        $("#btnSave").click(function (e) {

            //alert('clicked.');
            //return;
            debugger;
            IsSendForApproval = false,IsApproved = false, IsRevise = false, IsReject = false; 

            Manager.Save(IsSendForApproval,IsApproved, IsRevise, IsReject,1);
        });

        $("#btnSaveForApprove").click(function (e) {
            IsSendForApproval = true, IsApproved = false, IsRevise = false, IsReject = false;
            Manager.Save(IsSendForApproval,IsApproved, IsRevise, IsReject, 1);
        });  

        $("#btnAcknowledgement").click(function (e) {
            IsSendForApproval = false,IsApproved = true, IsRevise = false, IsReject = false; 
            Manager.Save(IsSendForApproval,IsApproved, IsRevise, IsReject,4);
        });  
        $("#btnRevise").click(function (e) {
            //var revReason = $("#ReviseReason").val();
            //if (IsRevise == true && revReason == "") {
            //    MessageHelper.Warning("Please Input Revise Reason.");
            //    return;
            //}


            //Swal.fire(
            //    SwalHelper.SwalConfirmationConfig('Are you sure want to Revise this Requisition?')
            //).then((result) => {
            //    if (result.isConfirmed) {
            //        IsSendForApproval = false,IsApproved = false, IsRevise = true, IsReject = false;
            //        Manager.Save(IsSendForApproval,IsApproved, IsRevise, IsReject,4);
            //    }
            //})
        }); 

        $("#btnReject").click(function (e) {
            var rejReason = $("#RejectReason").val();
            if (rejReason == "") {
                ToastrMessageHelper.FxToastr("Please Input Reject Reason.");
                $("#RejectReason").focus();
                return;
            } 
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Reject this SDRF ?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsSendForApproval = false,IsApproved = false, IsRevise = false, IsReject = true;
                    Manager.Save(IsSendForApproval,IsApproved, IsRevise, IsReject,3);
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
    GetSDRFBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" }, 
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#SDRFBy", "text", "id", columns);
    }, 

    GetSaveOption: function (value) {
        var SaveOption = 0;
        var pk_id = $("#SDRFMasterID").val();
        pk_id = parseInt(pk_id);
        if ((value == 1) && (pk_id == 0)) { SaveOption = 1; } // Save New
        if ((value == 1) && (pk_id > 0)) { SaveOption = 2; } // Update
        if ((value == 3) && (pk_id > 0)) { SaveOption = 3; } // Reject
        if ((value == 4) && (pk_id > 0)) { SaveOption = 4; } // Approve

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
                    url: `/api/sdrfentry/list/${statusCons}`,
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

        if (isApprovePage) {
            if (statusCons == statusConstants.PROPOSED_FOR_APPROVAL) {
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
        } else { 
            if (statusCons == statusConstants.PENDING || statusCons == statusConstants.EDIT || statusCons == statusConstants.REJECT) {
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
        } 

        if (statusCons == statusConstants.PENDING) {
            columns = [
                {
                    title: "Action", command: command, width: 60
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "BuyerRefMasterID", title: "BuyerRefMasterID", hidden: true,
                },
                {
                    field: "BuyerRefColorID", title: "BuyerRefColorID", hidden: true,
                },
                {
                    field: "ChemicalCode", title: "Chemical</br>Code", width: 160,
                    filterable: true 
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "BuyerRef", title: "Brand Ref.", width: 120,
                    filterable: true 
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "BuyerRefDate", title: "Brand Ref.</br>Date", width: 120,
                    template: "#= (BuyerRefDate == null)? '' : kendo.toString(kendo.parseDate(BuyerRefDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #",
                    filterable: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "BuyerName", title: "Brand", width: 110, filterable: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ProductName", title: "Product</br>Name", width: 110, filterable: true
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
                    field: "ColorName", title: "Color</br>Name", width: 120, filterable: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true) 
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

            search =
            {
                fields:
                    [
                        "ChemicalCode", "BuyerRef", "BuyerName", "ProductName", "ProductCategoryName", "ProductTypeName",
                        "ColorName", "SeasonName", "MktPersonName"
                    ]
            };
        } else {
            columns = [
                {
                    title: "Action", command: command, width: 60
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
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
                    field: "ChemicalCode", title: "Chemical Code", width: 160
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "SDRFNo", title: "SDRF No", width: 160
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "SDRFDate", title: "SDRF Date", width: 110,
                    template: "#= (SDRFDate == null)? '' : kendo.toString(kendo.parseDate(SDRFDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "RequiredDate", title: "Required</br>Date", width: 110,
                    template: "#= (RequiredDate == null)? '' : kendo.toString(kendo.parseDate(RequiredDate, 'yyyy-MM-dd'), 'dd-MMM-yy') #"
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "BuyerName", title: "Brand</brName", width: 100
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "BuyerRef", title: "Brand</br>Ref", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ColorName", title: "Color</brName", width: 100
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true, true)
                },
                {
                    field: "SDRFByName", title: "SDRF By", width: 100
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "DevelopmentType", title: "Development</br>Type", width: 150
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "MktTeamName", title: "Mkt. Team", width: 110
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "RnDTeamName", title: "R & D Team", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "PriorityName", title: "Priority", width: 90
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "ClaimName", title: "Claim", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "FeelName", title: "Feel", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "FragranceName", title: "Fragrance", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "RestrictionName", title: "Restriction", width: 120
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
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
                    field: "SDRFChildID", hidden: true, width: 50,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "SDRFMasterID", hidden: true,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
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
                    field: "Qty", title: "Qty Per Sample", width: 80,
                    footerTemplate: "<span id='sumQty'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "Remarks", title: "Remarks", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
                }, 
            ];

        var schema = {
            model: {
                fields: {
                    SDRFChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    SDRFMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    objPackagingTypeList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    SizeGram: { type: "number", editable: function () { return false; } },
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

    InitChildTable_Ingredients: function (data) {
        if ($(TblChildId_Ingredients).data("kendoGrid")) $(TblChildId_Ingredients).kendoGrid('destroy').empty();
        dataSource = data;
        var columns =
            [
                {
                    command: [{
                        name: "delete2", text: "", iconClass: "k-icon k-i-delete", className: "k-success",
                        click: GridHelper.ChildCommandClick,
                    }], title: "Action &nbsp;", width: 40,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "SDRFChildID", hidden: true, width: 50,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "SDRFMasterID", hidden: true,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "objPackagingTypeList", title: "Type", width: 120,
                    editor: GridHelper.cboPackagingType,
                    template: "#=objPackagingTypeList != null ? objPackagingTypeList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "objPackagingTypeList", title: "Ingredient", width: 120,
                    editor: GridHelper.cboPackagingType,
                    template: "#=objPackagingTypeList != null ? objPackagingTypeList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "Remarks", title: "Required %", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "Remarks", title: "Note", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
                }, 
            ];

        var schema = {
            model: {
                fields: {
                    SDRFChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    SDRFMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    objPackagingTypeList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    SizeGram: { type: "number", editable: function () { return false; } },
                    objUOMList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    Qty: { type: "number", editable: function () { return false; } },
                    Remarks: { type: "string" },
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId_Ingredients, columns, data, schema, true, true, false);
        //GridHelper.InitializeHelper();
        KendoGridHelper.EnterKeyUp(TblChildId_Ingredients);
    },
    InitChildTable_ReferenceProducts: function (data) {
        if ($(TblChildId_ReferenceProducts).data("kendoGrid")) $(TblChildId_ReferenceProducts).kendoGrid('destroy').empty();
        dataSource = data;
        var columns =
            [
                {
                    command: [{
                        name: "delete3", text: "", iconClass: "k-icon k-i-delete", className: "k-success",
                        click: GridHelper.ChildCommandClick,
                    }], title: "Action &nbsp;", width: 40,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "SDRFChildID", hidden: true, width: 50,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                }, 
                {
                    field: "Remarks", title: "Product Name", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "Remarks", title: "Brand", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "Remarks", title: "Remarks", width: 120,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-left", "k-text-left", "k-text-left", true, true)
                }, 
            ];

        var schema = {
            model: {
                fields: {
                    SDRFChildID: { type: "number", validation: { min: 0 }, defaultValue: 0 },
                    SDRFMasterID: { type: "number", validation: { min: 0 }, defaultValue: 0 }, 
                    Remarks: { type: "string" },
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId_ReferenceProducts, columns, data, schema, true, true, false);
        //GridHelper.InitializeHelper();
        KendoGridHelper.EnterKeyUp(TblChildId_ReferenceProducts);
    },
}

var GridHelper = {
    // Actions
    HandleCommands: function (args) {
        args.preventDefault();
        var tr = $(args.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);

        if (selectedItem != null) {
            $("#btnSave,#btnSaveForApprove,#btnAcknowledgement,#btnRevise,#btnReject").hide();
            $("#divReviseNote,#divRejectNote").hide();

            childsActionBtn = true;
            childsToolsBar = false; 

            if (isApprovePage) {
                if (statusCons == statusConstants.PROPOSED_FOR_APPROVAL) {
                    $("#btnAcknowledgement,#btnReject").show();
                    $("#divRejectNote").show();
                    childsActionBtn = false;
                    childsToolsBar = true;
                }
            }
            else {
                if (statusCons == statusConstants.PENDING) {
                    $("#btnSave,#btnSaveForApprove").show();
                    //$("#divRejectNote").show();
                    childsActionBtn = false;
                    childsToolsBar = true;

                    Manager.GetNew(selectedItem.BuyerRefMasterID, selectedItem.BuyerRefColorID);
                    return;
                }
                if (statusCons == statusConstants.EDIT) {
                    $("#btnSave,#btnSaveForApprove").show(); 
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
            } 

            Manager.GetDetails(selectedItem.SDRFMasterID);
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


