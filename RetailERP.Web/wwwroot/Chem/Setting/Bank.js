/// <reference path="../../common/js/utilitycommon.js" />

var statusCons;
var masterData;
var menuId, pageName;
var ToolbarId;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom; 
$(document).ready(function () {
    Helper.Initialize();
});

var Manager = { 
    GetNew: function () { 
        $.get(`/api/Bank/new`, function (data) { 
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
            } 
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", data.Message);
        }); 
    },
    GetDetails: function (id) {
        $.get(`/api/Bank/edit?id=${id}`, function (data) {
            if (data.ErrNo > 0) {
                MessageHelper.Warning("Error = ", data.Message);
            } else {
                DivDetailsDom.fadeIn();
                DivToolbarDom.fadeOut();
                masterData = data;
                FormHelper.SetFormData(FormDom, masterData);

                var st = masterData.IsBeneficiaryBank == 1 ? true : false;
                $("#IsBeneficiaryBank").data("kendoSwitch").value(st);

                st = masterData.IsAdvisingBank == 1 ? true : false;
                $("#IsAdvisingBank").data("kendoSwitch").value(st);

                st = masterData.IsNegoBank == 1 ? true : false;
                $("#IsNegoBank").data("kendoSwitch").value(st);

               // $("#IsActive").prop("disabled", false);
            }
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", data.Message);
        });
    },
    Save: function (Approved) {
        if (Helper.IsFrmValid()) {
            var model = FormHelper.FormElToJson(FormDom);
            model.Approved = Approved;

            model.IsBeneficiaryBank = $("#IsBeneficiaryBank").data("kendoSwitch").value() == true ? 1 : 0;
            model.IsAdvisingBank = $("#IsAdvisingBank").data("kendoSwitch").value() == true ? 1 : 0;
            model.IsNegoBank = $("#IsNegoBank").data("kendoSwitch").value() == true ? 1 : 0;

            debugger;
            $.post("/api/Bank/save", model, function (data) {
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
    GetBankReport: function (id) {
        window.open(`/Setting/BankInformation?id=${id}`, "_blank");
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
        var Elements = ["BankName","IsActive"];
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

        $("#IsBeneficiaryBank").kendoSwitch({messages: {checked: "YES",unchecked: "NO"}});
        $("#IsAdvisingBank").kendoSwitch({ messages: { checked: "YES", unchecked: "NO" } });
        $("#IsNegoBank").kendoSwitch({ messages: { checked: "YES", unchecked: "NO" } });

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
            pageSize: 10,
            transport: {
                read: {
                    type: "POST",
                    url: `/api/Bank/list/${statusCons}`,
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
                //{ name: "BankReport", text: "Bank Print", iconClass: "k-icon k-i-print k-grid-button-rpt", className: "k-success", click: GridHelper.HandleCommands },              
                ];
        } else {
            command =
                [
                    { name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands },
                ];
        }
       
        var columns = [
            {
                title: "Action", command: command, width: 90,
            },
            {
                field: "BankID", title: "BankID", hidden: true,
            },               
            {
                field: "BankName", title: "Bank Name", filterable: true,
            }, 
            {
                field: "BankShortName", title: "Short Name",
            },
            {
                field: "BankAddress", title: "Address",
            },
            {
                field: "SwiftCode", title: "Swift Code",
            } 
        ];
        var search =
        {
            fields:
                [
                    "BankName", "BankShortName","BankAddress"
                ]
        };
        KendoGridHelper.MasterGridLoad(TblMasterId, "500px", search, columns)
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
            Manager.GetDetails(selectedItem.BankID);
            if (args.data.commandName == "BankReport") {
                Manager.GetBankReport(selectedItem.BankID);
                return;
            }
        }
    },
}

