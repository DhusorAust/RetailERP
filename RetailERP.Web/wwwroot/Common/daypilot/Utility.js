function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function removeCookie(cname) {
    document.cookie = cname + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC";
}

function ConvertToDate(strDate) {
    var dateArray = strDate.split("-");
    var newDate = new Date(dateArray[1] + ' ' + dateArray[0] + ' ' + dateArray[2]);
    return newDate;
}

function DateToStringDDMMYYYY(date) {
    var day = String(date.getDate());
    var month = String(date.getMonth() + 1);
    var year = String(date.getFullYear());
    var strDate = ""; 

    if (day.length == 1) {
        strDate += "0" + day + "/";
    }
    else {
        strDate += day + "/";
    }

    if (month.length == 1) {
        strDate += "0" + month + "/";
    }
    else {
        strDate += month + "/";
    }
    strDate += year;

    return strDate;

}

function DateToStringDDMMYY(date) {
    var day = String(date.getDate());
    var month = String(date.getMonth() + 1);
    var year = String(date.getFullYear());
    year = year.substring(2, 4);
    var strDate = "";

    if (day.length == 1) {
        strDate += "0" + day + "-";
    }
    else {
        strDate += day + "-";
    }

    if (month.length == 1) {
        strDate += "0" + month + "-";
    }
    else {
        strDate += month + "-";
    }
    strDate += year;

    return strDate;

}

Date.prototype.addDays = function (days) {
    this.setDate(this.getDate() + parseInt(days));
    return this;
};

function GetValue(value, dataList) {
    var z = $(dataList);
    var val = $(z).find('option[value="' + value + '"]');
    var endval = val.attr('id');
    return endval;
}
