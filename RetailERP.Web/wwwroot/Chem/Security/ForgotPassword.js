(function () {
    var status;
    var masterData;
    var menuId, pageName;
    var ToolbarId;
    var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom; 

    $(function () {
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

        FormDom.find("#btnChangePassword").click(function (e) {
            save(); 
        });

        FormDom.find("#btnClear").on("click", backToList); 
    });
    function backToList() { 
        DivDetailsDom.fadeOut();
        resetForm();
        DivToolbarDom.fadeIn(); 
    }
    function resetForm() {
        FormDom.trigger("reset"); 
        FormDom.find("#UserID").val(-1111); 
    } 
    function IsFrmValid() {
        var isValidItem = true;
        var Elements = ["Password_Old", "Password_New", "ConfirmPassword"];
        if (!IsFormValidation(FormDom, Elements)) {
            isValidItem = false;
            FxToastr("Please fill-up mandatory field.");
        } 

        if ($("#Password_New").val() != $("#ConfirmPassword").val()) { 
            Warning("Warning", "New Password & Confirm Password did not match."); 
            $("#Password_New").focus();
            $(".Password_New").show();
            isValidItem = false;
        }

        return isValidItem;
    }

    function save() {
        if (IsFrmValid()) {
            var model = formElToJson(FormDom);
            $.post("/api/chnPassword/save", model, function (data) {
                if (parseInt(data.ResultId) > 0) {
                    Success("Save Operations", data.Message);
                    window.open(`/Security/LogOut`, "_self");
                } else {
                    Warning("Save Operations", data.Message);
                }
            });
        }
    } 
})(); 




