
var OTPManager = {
    SaveOTP: function () {
        var model = {};
        model["UserID"] = $("#UserID").val();
        model["Password"] = $("#PasswordH").val();
        $.post("/api/chnPassword/SaveOTP", model, function (data) {
            if (parseInt(data.ResultId) > 0 && data.MobileNo != "") {
                OTPManager.SendOTP(data);
                MessageHelper.Success("Success", 'Login OTP has sent to your Mobile No. & email address<br/> After 60 Seconds your OTP will be expired!');

            } else {
                MessageHelper.Warning("Warning", data.Message);
            }
        });
    },
    SendOTP: function (data) {
        console.log("Local OTP generated for Dhusor only:", data.ResultId);
    },

    GetOTP: function () {
        var userId = $("#UserID").val();
        var obj = "";
        var url = `/api/chnPassword/GetOTP/${userId}`;
        OTPManager.GetAPI(url, onSuccess, onFailed);
        function onSuccess(jsonData) {
            obj = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return obj;
    },
    GetAPI: function (serviceUrl, successCallback, errorCallback) {
        $.ajax({
            type: "GET",
            async: false,
            cache: false,
            url: serviceUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: successCallback,
            error: errorCallback
        });
    },

    
};

