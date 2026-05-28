

var gbUrl = "localhost";

var ipAddress = "";
var officeIpAddress = ["127.0.0.1", "::1"];

var isOutSideOffice = false;

var UserLoginManager = {

};

var UserLoginHelper = {

    InitUserLogin: function () {
       
        $("#divOTP").hide();

        UserLoginHelper.GetIPAddress();

        if (!officeIpAddress.includes(ipAddress)) {
            isOutSideOffice = true;
        }

        var url = new URL(window.location.href).hostname;
      
        if (isOutSideOffice == true && gbUrl == url) {
            $("#divOTP").show();
        }

        $("#txtUserId").bind("keypress", function (e) {
            if (e.keyCode == 13) {
                $('#txtPassword').focus();
            }
        });
        $("#txtPassword").bind("keypress", function (e) {
            if (e.keyCode == 13) {
                if (IsFrmValid()) {
                    UserLoginHelper.UserLogin();
                    //UserLoginHelper.UserLoginWithoutOTP();
                } else {
                    MessageHelper.Warning("Login operation failed.", "Warning");
                }
            }
        });

        $("#txtOTP").bind("keypress", function (e) {
            if (e.keyCode == 13) {
                UserLoginHelper.UserLogin();
                //UserLoginHelper.UserLoginWithoutOTP();
            }
        });

        $('#btnSubmit').click(function (e) {
            e.preventDefault();
            if (IsFrmValid()) {
                UserLoginHelper.UserLogin();
                //UserLoginHelper.UserLoginWithoutOTP();
            } else {
                MessageHelper.Warning("Input User ID & Password", "Warning");
            }
        });
        $('#btnGenerateOTP').click(function (e) {
            var url = new URL(window.location.href).hostname;
            if (isOutSideOffice == true && gbUrl == url) {
                e.preventDefault();
                if (IsFrmValid()) {
                    OTPManager.SaveOTP();
                } else {
                    MessageHelper.Warning("Input User ID & Password", "Warning");
                }
            } else {
                MessageHelper.Warning("You are not allowed to generate OTP", "Warning");
            }
        });
    },

    UserLogin: function () {
        var url = new URL(window.location.href).hostname;
        var usrName = $("#UserID").val();
        if (isOutSideOffice == true && gbUrl == url && usrName.toLowerCase() !="admin") {
            var getOTP = OTPManager.GetOTP();;
            var userOTP = $("#txtOTP").val();

            if (userOTP == "") {
                MessageHelper.Warning("Please Input OTP");
                return
            }
            if (getOTP == null || getOTP.OTP == "") {
                MessageHelper.Warning("Invalid OTP");
                return
            }

            if (!getOTP.IsOTPExpired) {
                if (getOTP.OTP == userOTP) {
                    UserLoginHelper.LoginContinue();
                } else {
                    MessageHelper.Warning("Invalid OTP", "Warning");
                }
            } else {
                MessageHelper.Warning("Your OTP Expired! <br/> Please generate new OTP.");
            }
        }
        else {
            UserLoginHelper.LoginContinue();
        }
    },
    UserLoginWithoutOTP: function () {
        UserLoginHelper.LoginContinue();
    },

    LoginContinue: function () {
        $.ajax({
            type: "POST",
            url: base + "Security/GetLogin",
            data: { UserName: $("#UserID").val(), PasswordH: $("#PasswordH").val() },
            dataType: "json",
            success: function (data) {
                if (data.isCompleted == true) {
                    window.location.href = data.redirectUrl;
                } else {
                    MessageHelper.Warning(data.Message, "Warning");
                }
            },
            error: function (req, status, error) {
                MessageHelper.Warning(error, "Warning");
            }
        });
    },

    GetIPAddress: function () {
        ipAddress = "127.0.0.1";
    }
}
