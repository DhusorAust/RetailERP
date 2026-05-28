var statusConstants;
var masterData;
var menuId, pageName;
var ToolbarId;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom;
var UserRoleID = 0; 
$(document).ready(function () {
    Helper.Initialize();
}); 

var Manager = { // API related function which need to operation database
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
                    url: `/api/menuper_role/list/${statusConstants}`,
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
                { name: "edit", text: "", iconClass: "k-icon k-i-plus", className: "k-success", click: Helper.HandleCommands },
            ];

        var columns = [
            {
                title: "Action", command: command, width: 80,
            },
            {
                field: "UserRoleID", title: "UserRoleID", hidden: true,
            }, 
            {
                field: "UserRoleCode", title: "Role Code",
            },
            {
                field: "UserRoleName", title: "Role Name",
            },
        ];
        var search =
        {
            fields:
                [
                    "UserRoleCode", "UserRoleName" 
                ]
        };
        KendoGridHelper.MasterGridLoad(TblMasterId, "500px", search, columns)
    }, 
    MenuTree: function (UserRoleID) {
        $.get(`/api/menuper_role/GetMenuList?UserRoleID=${UserRoleID}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
            masterData = data;
            UserRoleID = UserRoleID;

            $("#tree").html("");
            $("#tree").removeClass();

            if (data === null)
                MessageHelper.Warning("Error Operations", "Error fetching Data");
            else {
                Helper.CreateTree(masterData);
            }
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    }, 
    Save: function (data) {
        var model = data;
        $.post("/api/menuper_role/save", model, function (data) {
            if (parseInt(data.ResultId) > 0) {
                ToastrMessageHelper.ValidationToastr("Permission operations successful.");
            } else {
                MessageHelper.Warning("Permission operations", data.Message);
            }
        });
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
        statusConstants = statusConstants.EDIT;
        Manager.InitMasterTable();

        // ToolbarDom means Toolbar Buttons
        $("#btnNewList").on("click", function (e) {
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            Manager.InitMasterTable();
        });

        $("#btnBackToList").on("click", Helper.BackToList);

        $("#tree").on("changed.jstree", function (e, data) {
            var action = 2;

            if (data.action == 'select_node')
                action = 1;
            else if (data.action == 'deselect_node')
                action = 0;

            if (action < 2) {
                var menu = data.node;
                var original = menu.original;

                console.log(original);

                if (original.MenuID != null) {
                    var data = {
                        PermissionID: original.PermissionID,
                        MenuID: original.MenuID,
                        UserRoleID: UserRoleID,
                        IsPermission: action
                    }
                    Manager.Save(data);
                }
            }
        }); 
    },
    HandleCommands: function (args) {
        args.preventDefault();
        var tr = $(args.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);

        if (selectedItem != null) {
            UserRoleID = selectedItem.UserRoleID;
            Manager.MenuTree(selectedItem.UserRoleID);
        }
    },
    CreateTree: function (menu) {
        $("#tree").jstree({
            "core": { "data": menu },
            "checkbox": {
                "keep_selected_style": false,
                "three_state": false,
                "real-checkbox": true
            },
            "contextmenu": {
                "items": function ($node) {
                    var tree = $("#tree").jstree(true);
                }
            },
            "plugins": ["themes", "json_data", "ui", "checkbox", "contextmenu"]
        });
    }
} 