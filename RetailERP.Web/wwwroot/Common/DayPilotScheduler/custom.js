 $(document).ready(function () {  
    // Textbox Change:: Error validation control Add & Remove
    $('.form-control').bind('input', function () {
        $(this).next('.ibserror').hide();
        $(this).next().next('.ibserror').hide();

        $(this).siblings().children('.ibserror').hide();//added 1.11.16

        if ($(this).next('.ibserror').text() != "" && $(this).val().length == 0)
            $(this).parent().parent().addClass('has-error');
        else
            $(this).parent().parent().removeClass('has-error');
    }); 

    // Dropdown Selected Value:: Error validation control Hide when Dropdown Selected Item Change
    $("select").change(function () {
        if ($(this).val() != '') {
            $(this).parent().nextAll('.ibserror').hide();

            $(this).parent().parent().nextAll('.ibserror').hide();
            $(this).parent().next('.ibserror').hide();

            $(this).siblings().children('.ibserror').hide();//added 1.11.16
            
        }
        else {
            $(this).parent().nextAll('.ibserror').show();
        }
    }).change(); 


    // Datetimepicker:: Error validation control Hide when datepicker date Change
    $(document).on('change', '#datepicker-component', function () {
        $(this).next('.ibserror').hide();
        $(this).next().next('.ibserror').hide();
    }); 

    //Server side:: Error validation control add Error class
    if ($('.field-validation-error').text() != "") {
        $('.field-validation-error').parent().parent().parent().addClass('has-error');
    }
       

    //Textbox Change:: Error validation control text remove
    $(".input-validation-error").keyup(function () {
        $(".input-validation-error").next(".field-validation-error").html('');
    });

    // Load Event:: Error validation control Hide
    $('.ibserror').hide();


    //Added by kamrul 06.09.2016
    $(".IsNumber").keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    $(".thousandSeperate").keyup(function (e) {
        //function addCommas(nStr) {
        var id = e.target.id;
        var nStr = $('#' + id).val();
        nStr = nStr.replace(/,/g, '');
        nStr += '';
        x = nStr.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        //return $(this).val(x1 + x2);
        $('#' + id).val(x1 + x2);
        //return x1 + x2;
    })

});

//When Submit / Edit Successful then page full refresh.
function clear(controllers, functionname) {
    setTimeout(function () {
        window.location.href = '/' + controllers + '/' + functionname;
    }, 1000);
}

//Date Formate 
function date(value) {
    var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
                      "July", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (("0" + (dt.getDay())).slice(-2) + "-" + monthNames[dt.getMonth() + 1]) + "-" + dt.getFullYear();
} 

//Added by kamrul 06.09.2016
function validFromDateToDate(fromDateName, toDateName, fromDateID, toDateID, changeTxtName) {
    if (changeTxtName == 'fromDate') {
        var fromDate = $("#" + fromDateID).datepicker("getDate");
        var toDate = $("#" + toDateID).datepicker("getDate");
        if (toDate != "") {
            if (fromDate > toDate) {
                alert(fromDateName + ' Must Be Less Than ' + toDateName + '');
                $("#" + fromDateID).val('');
                return;
            }
        }
    }
    if (changeTxtName == 'toDate') {
        var fromDate = $("#" + fromDateID).datepicker("getDate");
        var toDate = $("#" + toDateID).datepicker("getDate");
        if (fromDate != "") {
            if (fromDate > toDate) {
                alert(toDateName + ' Must Be Greater Than ' + fromDateName + '');
                $("#" + fromDateID).val('');
                return;
            }
        }
    }
}

 

//******************** Start:: Message Box *******************
//************************************************************
//************************************************************

//*** Start:: Fixed Text 

//Success or Failed Operation
function ShowOpMessage(sOperation, IsSuccess, sFunctionName) {
    if (IsSuccess) {
        $.prompt(sOperation + " Operation Successful.", {
            title: "Success",
            zIndex: 20000,
            buttons: { "Ok": true },
            submit: function (e, v, m, f) {
                eval(sFunctionName + "(" + v + ")");
            }
        })

        $(".jqititle").css({ "color": "#0a7c71", "background-color": "#cff5f2", "border-color": "#0a7c71" });
    }
    else {
        $.prompt(sOperation + " Operation Failed!", {
            title: "Failed",
            zIndex: 20000,
            buttons: { "Ok": true },
            submit: function (e, v, m, f) {
                //eval(sFunctionName + "(" + v + ")");
            }
        })

        $(".jqititle").css({ "color": "#933432", "background-color": "#fddddd", "border-color": "#933432" });
    }
}

//Confirmation Delete Operation
function ShowConfirmDeletion_YESNO(sFunctionName) {
    $.prompt("Sure to DELETE this record?", {
        title: "Confirmation",
        buttons: { "Yes": true, "No": false },
        submit: function (e, v, m, f) {
            eval(sFunctionName + "(" + v + ")");
        }
    })
    $(".jqititle").css({ "color": "#957d32", "background-color": "#fef6dd", "border-color": "#957d32" });
}

//Confirmation Delete Operation
function ShowConfirmDeletion_OKCANCEL(sFunctionName) {
    $.prompt("Sure to DELETE this record?", {
        title: "Confirmation",
        buttons: { "Ok": true, "Cancel": false },
        submit: function (e, v, m, f) {
            eval(sFunctionName + "(" + v + ")");
        }
    })
    $(".jqititle").css({ "color": "#957d32", "background-color": "#fef6dd", "border-color": "#957d32" });
}

//No Record Operation
function ShowNoRecord() {
    $.prompt("Sorry, Data not found!", {
        title: "Warning",
        zIndex: 20000,
        buttons: { "Ok": true },
        submit: function (e, v, m, f) {
            //eval(sFunctionName + "(" + v + ")"); 
            return;
        }
    })

    $(".jqititle").css({ "color": "#933432", "background-color": "#fddddd", "border-color": "#933432" });
}

//*** End:: Fixed Text

//*** Start:: Dynamic Text 

//Start:: Body  
function ShowMessageBox_YESNO(sMessage, title, sFunctionName) {
    $.prompt(sMessage, {
        zIndex: 20000,
        title: title,
        buttons: { "Yes": true, "No": false },
        submit: function (e, v, m, f) {
            eval(sFunctionName + "(" + v + ")");
        }
    })
    $(".jqititle").css({ "color": "#0a7c71", "background-color": "#cff5f2", "border-color": "#0a7c71" });
}

function ShowMessageBox_OKCANCEL(sMessage, title, sFunctionName) {
    $.prompt(sMessage, {
        zIndex: 20000,
        title: title,
        buttons: { "Ok": true, "Cancel": false },
        submit: function (e, v, m, f) {
            eval(sFunctionName + "(" + v + ")");
        }
    })
    $(".jqititle").css({ "color": "#0a7c71", "background-color": "#cff5f2", "border-color": "#0a7c71" });
}

function ShowMessageBox_YES(sMessage, title, sFunctionName) {
    $.prompt(sMessage, {
        zIndex: 20000,
        title: title,
        buttons: { "Yes": true },
        submit: function (e, v, m, f) {
            eval(sFunctionName + "(" + v + ")");
        }
    })
    $(".jqititle").css({ "color": "#0a7c71", "background-color": "#cff5f2", "border-color": "#0a7c71" });
}

function ShowMessageBox_OK(sMessage, title, sFunctionName) {
    $.prompt(sMessage, {
        zIndex: 20000,
        title: title,
        buttons: { "Ok": true},
        submit: function (e, v, m, f) {
            eval(sFunctionName + "(" + v + ")");
        }
    })
    $(".jqititle").css({ "color": "#0a7c71", "background-color": "#cff5f2", "border-color": "#0a7c71" });
}

// End:: Body 

//Start:: Footer  

//Information Operation
function ShowInfoMsg(message) {
    $('body').pgNotification({
        style: 'circle',
        title: 'Warning',
        message: message,
        position: "bottom-right",
        timeout: 3000,
        type: "danger",
        showClose: !0,
        onShown: function () { },
        onClosed: function () { },
        thumbnail: '<img width="40" height="40" style="display: inline-block;" src="/CSS/web_tools/built_in/assets/img/profiles/info.png" data-src="/CSS/web_tools/built_in/assets/img/profiles/info.png" data-src-retina="/CSS/web_tools/built_in/assets/img/profiles/info.png" alt="">'
    }).show();
}

//Warning Operation
function ShowWarningMsg(message) {
    $('body').pgNotification({
        style: 'circle',
        title: 'Warning',
        message: message,
        position: "bottom-right",
        timeout: 3000,
        type: "danger",
        //showClose: !0,
        showClose: 0, 
        onShown: function () { },
        onClosed: function () { },
        thumbnail: '<img width="40" height="40" style="display: inline-block;" src="/CSS/web_tools/built-in/assets/img/profiles/info.png" data-src="/CSS/web_tools/built-in/assets/img/profiles/info.png" data-src-retina="/CSS/web_tools/built-in/assets/img/profiles/info.png" alt="">'
    }).show();
}

// End:: Footer  

//******************* Start:: Dynamic Text *******************

//********************* End:: Message Box ********************
//************************************************************
//************************************************************

// Report Show in Popup
function FxPreviewReport(id, report, url, iframe) { 
    var reportmodal = document.getElementById('myModal');// Get the modal
    var closeBtn = document.getElementsByClassName("close")[0]; // Get the <span> element that closes the modal
    reportmodal.style.display = "block";
   
    
    if (!isNaN(id) && id > 0) {
        var report = report;
        url = url;
        var myFrame = document.getElementById(iframe);
        if (myFrame.src == "") {
            myFrame.src = url;
            $("#"+ iframe).height(620);
        }
        else if (myFrame.contentWindow !== null && myFrame.contentWindow.location !== null) {
            myFrame.contentWindow.location = url;
        }
    }
    closeBtn.onclick = function () {
        reportmodal.style.display = "none";
        
    }

    $("#printReport").click(function () {
        //window.print();
        printDiv();
    });

    function printDiv() {
        var divToPrint = document.getElementById('reportViw');
        var newWin = window.open('', 'Print-Window');

        newWin.document.open();
        newWin.document.write('<html><body onload="window.print()">' + divToPrint.html() + '</body></html>'); //divToPrint.innerHTML

        newWin.document.close();
        setTimeout(function () { newWin.close(); }, 10);
       return

    }
    
}




//Added by kamrul 22.09.2016
//function showOkAlert(message, titleMsg) {
//    new Function(
//    $.prompt(message, {
//        title: titleMsg,
//        buttons: { "Ok": true },
//        submit: function (e, v, m, f) {
//            return v;
//            //if (v == true) {
//            //    return
//            //}
//        }
//    })
//  )
//}

////Added by kamrul 22.09.2016
//function showConfirmAlert(message, titleMsg) {
//    new Function(
//    $.prompt(message, {
//        title: titleMsg,
//        buttons: { "Ok": true, "Cancel": false },
//        submit: function (e, v, m, f) {
//            return v;
//        }
//    })
//  )
//}

//Start::  Message showing Functions


//-- New Message System
// Fully Customize

 

//-- OLD Message System
//Success Operation
//function ShowCustomOpMessage(sMessage, title) {
//    $.prompt(sMessage, {
//        title: title,
//        buttons: { "Ok": true },
//        submit: function (e, v, m, f) {
//            return;
//        }
//    })
//    $(".jqititle").css({ "color": "#0a7c71", "background-color": "#cff5f2", "border-color": "#0a7c71" });
//}

//function ShowCustomOpMessage1(sMessage, title) {
//    $.prompt(sMessage, {
//        title: title,
//        buttons: { "Ok": true },
//        submit: function (e, v, m, f) {
//            return;
//        }
//    })
//    $(".jqititle").css({ "color": "#957d32", "background-color": "#fef6dd", "border-color": "#957d32" });
//}











//Permission Operation
//function ShowPermissionMsg(message, titleMsg, sFunctionName) {
//    new Function(
//    $.prompt(message, {
//        title: titleMsg,
//        buttons: { "Ok": true },
//        submit: function (e, v, m, f) {
//            eval(sFunctionName + "(" + v + ")");
//        }
//    })
//  )
//}

//End::  Message showing Functions





