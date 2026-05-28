/// <reference path="../../common/js/utilitycommon.js" />

var statusCons;
var masterData;
var menuId, pageName;
var ToolbarId;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom;
$(document).ready(function () {
    console.log(CurrentUser);
    Helper.Initialize();
}); 

var Manager = {
    InitMasterTable: function () {
        if ($(TblMasterId).data("kendoGrid")) $(TblMasterId).kendoGrid('destroy').empty();
        dataSource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 25,
            transport: {
                read: {
                    type: "POST",
                    url: `/api/userrole/list/${statusCons}`,
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

        var command =
            [
                { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: Helper.HandleCommands },
            ];
        var columns = [
            {
                title: "Action", command: command, width: 70,
            },
            {
                field: "UserRoleID", title: "UserRoleID", hidden: true,
            },
            {
                field: "UserRoleCode", title: "Role Code",
            }, 
            {
                field: "UserRoleName", title: "User Role",
            },
            {
                field: "ProjectName", title: "Project Name",
            },
            {
                field: "IsActive", title: "IsActive", hidden: true,
            },
            {
                field: "RoleStatus", title: "Status",
            }
        ];
        var search =
        {
            fields:
                [
                    "UserRoleCode", "UserRoleName"
                ]
        };

        KendoGridHelper.MasterGridLoad(TblMasterId, "700px", search, columns)
    },  
    GetNew: function () {
        DivDetailsDom.fadeIn();
        DivToolbarDom.fadeOut();
        $("#UserRoleCode").val("<<--System Value-->>")
        $("#btnSave").show(); 
    },
    GetDetails: function (id) {
        $.get(`/api/userrole/edit?id=${id}`, function (data) {
            if (data.ErrNo > 0) {
                MessageHelper.Warning("System Error", data.Message);
            } else {
                DivDetailsDom.fadeIn();
                DivToolbarDom.fadeOut();
                masterData = data;
                FormHelper.SetFormData(FormDom, masterData);
                $("#IsActive").prop("disabled", false);
            }
        }).catch(function (err) {
            MessageHelper.Warning("System Error", data.Message);
        });
    },
    Save: function () {
        if (Helper.IsFrmValid()) {
            var model = FormHelper.FormElToJson(FormDom);
            $.post("/api/userrole/save", model, function (data) {
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
    }
}
var Helper = { 
    //Common Function
    BackToList: function () {
        DivDetailsDom.fadeOut();
        DivToolbarDom.fadeIn();
        Helper.ResetForm(); 
        Manager.InitMasterTable();
    },
    ResetForm: function () {
        FormDom.trigger("reset"); 
        $('input[type="hidden"]').val("0");
    },
    IsFrmValid: function () {
        var isValidItem = true;
        var Elements = ["UserRoleName", "IsActive"];
        if (!FormHelper.IsFormValidation(FormDom, Elements)) {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
        }
        return isValidItem;
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
        Manager.InitMasterTable();  
        $("#btnNewAdd").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn($("#btnEditList"), ToolbarDom);
            Manager.GetNew();
        });
        $("#btnEditList").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            Manager.InitMasterTable();
        });
        $("#btnSave").click(function (e) {
            Manager.Save();
        });

        $("#btnPivotReportTest").on("click", function (e) {
            window.open('PivotTest');
        });

        $("#btnBackToList").on("click", Helper.BackToList); 
    },
    HandleCommands: function (args) {
        args.preventDefault();
        var tr = $(args.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);

        if (selectedItem != null) {
            if (args.data.commandName == "edit") {
                Manager.GetDetails(selectedItem.UserRoleID);
            }
        } 
    }, 
}

