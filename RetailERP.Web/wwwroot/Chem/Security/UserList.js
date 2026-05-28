/// <reference path="../../common/js/utilitycommon.js" />

var statusCons;
var masterData;
var menuId, pageName;
var ToolbarId;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom; 
$(document).ready(function () {
    Helper.Initialize();
}); 

var Manager = { // API related function which need to operation database
    
    GetNew: function () {
        $.get("/api/userlist/new", function (data) {
            if (data.ErrNo > 0) {
                MessageHelper.Warning("System Error", data.Message);
            } else {
                DivDetailsDom.fadeIn();
                DivToolbarDom.fadeOut();
                masterData = data;
                FormHelper.SetFormData(FormDom, masterData);
                $("#IsActive").val("1").trigger('change');
            } 
        }).catch(function (err) { 
            MessageHelper.Warning("System Error", err.Message);
        });
    },
    GetDetails: function (id) {
        $.get(`/api/userlist/edit?id=${id}`, function (data) {
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
            MessageHelper.Warning("System Error", err.Message);
        });
    },
    Save: function () {
        if (Helper.IsFrmValid()) {
            var model = FormHelper.FormElToJson(FormDom);
            $.post("/api/userlist/save", model, function (data) {
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
        GridManager.InitMasterTable();
        $("#IsActive").prop("disabled", true);
    },
    ResetForm: function () {
        FormDom.trigger("reset");
        $('input[type="hidden"]').val("0"); 
    },
    IsFrmValid: function () {
        var isValidItem = true;
        var Elements = ["UserName", "PasswordH", "EmployeeID", "UserRoleID", "IsActive"];
        if (!FormHelper.IsFormValidation(FormDom, Elements)) {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
        }

        if ($("#PasswordH").val() != $("#ConfirmPassword").val()) {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Password & Confirm Password not match.");
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
        GridManager.InitMasterTable(); 
        $("#btnNewAdd").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn($("#btnEditList"), ToolbarDom);
            Manager.GetNew();
        });
        $("#btnEditList").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
        }); 
        $("#btnSave").click(function (e) {
            Manager.Save();
        });

        $("#btnBackToList").on("click", Helper.BackToList);
        $("#EmployeeID").change(function () {
            var EmployeeID = $(this).val();
            if (EmployeeID != null) {
                
                $.get(`/api/helperC/GetEmployeeList?id=${EmployeeID}`, function (data) {
                    $("#DepartmentName").val(data.DepartmentName);
                    $("#DesignationName").val(data.DesignationName);
                    $("#SectionName").val(data.SectionName);
                    $("#SubSectionName").val(data.SubSectionName);
                }).catch(function (err) {
                    MessageHelper.Warning("Error = ", data.Message);
                });
            }
        });
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
            pageSize: 25,
            transport: {
                read: {
                    type: "POST",
                    url: `/api/userlist/list/${statusCons}`,
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
                { name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands },
            ];
        var columns = [
            {
                title: "Action", command: command, width: 70,
            },
            {
                field: "UserID", title: "UserID", hidden: true,
            },
            {
                field: "UserName", title: "User Name",
            },
            {
                field: "UserRoleName", title: "User Roll Name",
            },
            {
                field: "PasswordExpireDate", title: "Password Expire Date", template: "#= (PasswordExpireDate == null)? '' : kendo.toString(kendo.parseDate(PasswordExpireDate, 'yyyy-MM-dd'), 'dd-MMM-yyyy') #"
            },
            {
                field: "EmployeeName", title: "Employee Name",
            },
            {
                field: "UserStatus", title: "Status",
            }
        ];
        var search =
        {
            fields:
                [
                    "UserName", "UserRoleName", "EmployeeName"
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
            if (args.data.commandName == "edit") {
                Manager.GetDetails(selectedItem.UserID);
            }
        }
    },
}



