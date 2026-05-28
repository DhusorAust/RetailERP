/// <reference path="../../common/js/utilitycommon.js" />

var statusConstants;
var masterData;
var menuId, pageName;
var ToolbarId;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom;
$(document).ready(function () {
    Helper.Initialize();
}); 
var Manager = { // API related function which need to operation database 
    Save: function () {
        debugger
        if (Helper.IsFrmValid()) {
            var model = FormHelper.FormElToJson(FormDom);
            $.post("/api/chnPassword/save", model, function (data) {
                if (parseInt(data.ResultId) > 0) {
                    MessageHelper.Success("Save Operations", data.Message);
                    window.open(`/Security/LogOut`, "_self");
                } else {
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
    },
    ResetForm: function () {
        FormDom.trigger("reset");
        $('input[type="hidden"]').val("0");
    },
    IsFrmValid: function () {
        var isValidItem = true;
        var Elements = ["OldPassword", "NewPassword", "ConfirmPassword"];
        if (!FormHelper.IsFormValidation(FormDom, Elements)) {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
        }

        if ($("#NewPassword").val() != $("#ConfirmPassword").val()) {
            MessageHelper.Warning("Warning", "New Password & Confirm Password did not match.");
            $("#NewPassword").focus();
            $(".NewPassword").show();
            isValidItem = false;
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
        FormDom = $(pageConstants.FORM_ID + pageId); 
       $("#btnChangePassword").click(function (e) {
            Manager.Save();
        });

        $("#btnClear").on("click", Manager.BackToList); 
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




 



