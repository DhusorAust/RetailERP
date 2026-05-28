/// <reference path="../../common/js/utilitycommon.js" />

var statusConstants;
var masterData;
var menuId, pageName;
var ToolbarId;
var SaveOption = 0;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom;
$(document).ready(function () {
    Helper.Initialize();
});

var Manager = { // API related function which need to operation database 
    Save: function () {
        if (Helper.IsFrmValid()) {
            
            if ($('#MenuID').val() == null || $('#MenuID').val() == "" || $('#MenuID').val() == "0") {
                $('#MenuID').val(0);
            }
            var model = {
                SaveOption: SaveOption,
                MenuID: $('#MenuID').val(),
                ProjectID: $('#ProjectID').val(),
                ProjectModuleID: $("#ProjectModuleID").val(),
                Controller: $("#Controller").val(),
                ViewName: $("#ViewName").val(),
                MenuHead: $("#MenuHead").val(),
                PageName: $("#PageName").val(),
                MenuTitle: $("#MenuTitle").val(),
                MenuTitle2: $("#MenuTitle2").val(),
                MenuTitle3: $("#MenuTitle3").val(),
                MenuTitle4: $("#MenuTitle4").val(),
                MenuIcon: $("#MenuIcon").val(),
                Remarks: $("#Remarks").val(),
                ParentMenuID: $('#MenuID').val()
            };
            $.post("/api/menuRpt_create/save", model, function (data) {
                if (parseInt(data.ResultId) > 0) {
                    MessageHelper.Success("Save Operations", data.Message);
                    Manager.MenuTree();
                    $('#tree').jstree(true).settings.core.data = masterData;
                    $('#tree').jstree(true).refresh();
                    $('#exampleModal').modal('hide');
                    $('#MenuID').val("0");
                } else {
                    MessageHelper.Warning("Save Operations", data.Message);
                }
            });
        }
    },
    Project: function () {
        $.get(`/api/menuRpt_create/project`, function (data) {
            $('#ProjectID').html("");
            var item = "";
            $.each(data.ProjectList, function (i, obj) {
                item += '<option value="' + obj.id + '">' + obj.text + '</option>';
            });
            $('#ProjectID').html(item);
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", data.Message);
        });
    },
    ProjectModule: function (ProjectID) {
        $.get(`/api/menuRpt_create/projectmodule?id=${ProjectID}`, function (data) {
            $('#ProjectModuleID').html("");
            var item = "";
            $.each(data.ProjectModuleList, function (i, obj) {
                item += '<option value="' + obj.id + '">' + obj.text + '</option>';
            });
            $('#ProjectModuleID').html(item);
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", data.Message);
        });
    },
    MenuTree: function () {
        $.get(`/api/menuRpt_create/menulist`, function (data) {
            masterData = data;
            $("#tree").html("");
            $("#tree").removeClass();

            if (data === null)
                MessageHelper.Warning("Error Operations", "Error fetching Data");
            else {
                Helper.CreateTree(masterData);
            }
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", data.Messag);
        });
    },
    MoveMenu: function (MenuID, IsParent, sequence, ParentMenuID, ProjectModuleID) {
        var model = {
            MenuID: MenuID,
            IsParent: IsParent,
            sequence: sequence,
            ParentMenuID: ParentMenuID,
            ProjectModuleID: ProjectModuleID
        };
        $.post("/api/menuRpt_create/menumove", model, function (data) {
            if (parseInt(data[0].ResultId) > 0) {
                MessageHelper.Success("Move Operations", "Move operation sucessful.");
                Manager.MenuTree();
            } else {
                MessageHelper.Warning("Save Operations", "Move operation failed.");
            }
        });
    }
}
var Helper = {
    //Common Function 
    IsFrmValid: function () {
        var isValidItem = true;
        var Elements = ["ProjectID", "ProjectModuleID", "Controller", "ViewName", "MenuHead"];
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

        Manager.MenuTree();
        Manager.Project();
        $("#btnSave").click(function (e) {
            Manager.Save();
        });
        $("#ProjectID").change(function (e) {
            var ProjectID = $(this).val();
            if (ProjectID > 0) {
                Manager.ProjectModule(ProjectID);
            }
        });

        $("#tree").on("move_node.jstree", function (e, data) {
            var par = data.node.parent;
            var parent_node = $('#tree').jstree(true).get_node(par);

            // Check if moves to root application level 
            if (parent_node.parent === null) {
                $('#tree').jstree(true).settings.core.data = masterData;
                $('#tree').jstree(true).refresh();
                return;
            }
            // Check if parent menu is a level-3 menu, if so reinstate menu tree
            var parent_level = parent_node.original.level;

            if (parseInt(parent_level) === 3) {
                $('#tree').jstree(true).settings.core.data = masterData;
                $('#tree').jstree(true).refresh();
                return;
            }

            // Check if the node's highest menu level exceeds 3, if so then rollback changes
            var maxlevel = 0;
            var level = parseInt(data.node.original.level);

            if (data.node.children_d.length > 0) {
                for (var j = 0; j < data.node.children_d.length; j++) {
                    var ch_id = data.node.children_d[j];
                    var this_node = $('#tree').jstree(true).get_node(ch_id);
                    if (this_node.original != undefined) {
                        if (parseInt(this_node.original.level) > maxlevel) {
                            maxlevel = parseInt(this_node.original.level)
                        }
                    }
                }
            }

            if (maxlevel > 0) {
                if (parseInt(parent_level) + (maxlevel - level) >= 3) {
                    $('#tree').jstree(true).settings.core.data = masterData;
                    $('#tree').jstree(true).refresh();
                    return;
                }
            }

            // process other data for server side callback
            var MenuID = data.node.original.MenuID;
            var IsParent = data.node.children.length > 0 ? true : false;

            var sequence = new Array();
            if (parent_node.children.length > 0) {
                for (var i = 0; i < parent_node.children.length; i++) {
                    var child_node = $('#tree').jstree(true).get_node(parent_node.children[i]);
                    try {
                        sequence.push(child_node.original.MenuID);
                    }
                    catch (err) {
                        console.log(err);
                        console.log(sequence);
                    }
                }
            }
            var ParentMenuID = parent_node.original.MenuID;
            var ProjectModuleID = parent_node.original.ProjectModuleID;

            if (ProjectModuleID != null) {
                Manager.MoveMenu(MenuID, IsParent, sequence, ParentMenuID, ProjectModuleID);
            }
        });
    },
    CreateTree: function (menu) {
        $("#tree").jstree({
            "core": { "data": menu, "check_callback": true },
            "check_callback": function (op, node, par, pos, more) {
                return true;
            },
            "contextmenu": {
                "items": function ($node) {
                    var tree = $("#tree").jstree(true);
                    return {
                        "Create": {
                            "separator_before": false,
                            "separator_after": false,
                            "label": "Create Menu",
                            "action": function (obj) {
                                
                                var original = $node.original;
                                if (parseInt(original.level) == 7) {
                                    MessageHelper.Warning("Warning", 'This Level cannot be parent of submenu');
                                    return;
                                }
                                SaveOption = 1;
                                $('#MenuID').val(original.MenuID);
                                $('#ProjectID').val(original.ProjectID).trigger('change');
                                $('#ProjectModuleID').val(original.ProjectModuleID).trigger('change');
                                $('#Controller').val(original.Controller).focus();
                                $('#ViewName').val("");
                                $('#MenuHead').val("");
                                //$('#PageName').val(original.PageName);
                                //$('#MenuTitle').val("");
                                //$('#MenuTitle2').val("");
                                //$('#MenuTitle3').val("");
                                //$('#MenuTitle4').val("");
                                //$('#MenuIcon').val("");
                                //$('#Remarks').val("");
                                $('#exampleModal').modal('show');
                            }
                        },
                        "Edit": {
                            "separator_before": false,
                            "separator_after": false,
                            "label": "Edit Menu",
                            "action": function (obj) {
                                var original = $node.original;
                                if (parseInt(original.level) == 0) {
                                    MessageHelper.Warning("Warning", 'This Level cannot be edited');
                                    return;
                                }
                                SaveOption = 2;
                                
                                $('#MenuID').val(original.MenuID);
                                $('#ProjectID').val(original.ProjectID).trigger('change');
                                $('#ProjectModuleID').val(original.ProjectModuleID).trigger('change');
                                $('#Controller').val(original.Controller).focus();
                                $('#ViewName').val($node.data);
                                $('#MenuHead').val(original.text);
                                //$('#PageName').val(original.PageName);
                                //$('#MenuTitle').val(original.MenuTitle);
                                //$('#MenuTitle2').val(original.MenuTitle2);
                                //$('#MenuTitle3').val(original.MenuTitle3);
                                //$('#MenuTitle4').val(original.MenuTitle4);
                                //$('#MenuIcon').val(original.MenuIcon);
                                //$('#Remarks').val(original.Remarks);
                                $('#exampleModal').modal('show');
                            }
                        }
                    };
                }
            },

            "plugins": ["themes", "json_data", "ui", "contextmenu", "dnd"],
            dnd: {
                is_draggable: function (e) {
                    if (e[0].original.MenuID === null)
                        return false;
                    return true;
                },
                check_while_dragging: true
            }
        });
    },
}











