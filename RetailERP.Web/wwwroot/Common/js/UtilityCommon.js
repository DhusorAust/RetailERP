chnageLocked = 0;
var gbGridSLNo = 0;
var window_height = $(window).height();
var container_height = $(".container").height();
var kEmpFilterBy_InputField;
var kMultiComboBox_ReturnValue = 0;
var comboBoxSearchText = "";
var pageSize_PopUp = 50;
var pageIndex_PopUp = 1;
// #region Constants 
var globalURL = Object.freeze({
    GMTApiUrl: "http://localhost:30472",
    PDLApiUrl: "http://localhost:30470",
    BTLApiUrl: "http://localhost:30471",
});

var statusConstants = Object.freeze({
    ALL: 1,
    NEW: 2,
    EDIT: 3,
    PENDING: 4,
    COMPLETED: 5,
    PARTIALLY_COMPLETED: 6,
    APPROVED: 7,
    PROPOSED_FOR_APPROVAL: 8,
    UN_APPROVE: 9,
    ACKNOWLEDGE: 10,
    PROPOSED_FOR_ACKNOWLEDGE: 11,
    UN_ACKNOWLEDGE: 12,
    ACKNOWLEDGE_ACCEPTENCE: 13,
    PROPOSED_FOR_ACKNOWLEDGE_ACCEPTENCE: 14,
    REJECT: 15,
    REVISE: 16,
    CHECK: 17,
    CHECK_REJECT: 18,
    ACTIVE: 19,
    IN_ACTIVE: 20,
    RECEIVED: 21,
    ISSUED: 22,
    CANCEL: 23,
    RUNNING: 24,
    CLOSE: 25,

    FILE: 26,
    INVOICE: 27,
    GP_NO: 28,
    INT_CHALLAN: 29,

    LIST_1: 40,
    LIST_2: 41,
    LIST_3: 42,
    LIST_4: 43,
    LIST_5: 44,
    LIST_6: 45,
});

var pageConstants = Object.freeze({
    PAGE_ID: "#",
    DIV_TOOLBAR_ID: "#divtoolbar",
    DIV_TBL_ID_PREFIX: "#divtbl",
    TOOLBAR_ID: "#toolbar",
    MASTER_TABLE_ID: "#tbl",
    MASTER_TABLE_ID2: "#tbl2",
    CHILD_TABLE_ID: "#tblChild",
    CHILD_TABLE_ID1: "#tblChild1",
    CHILD_TABLE_ID2: "#tblChild2",
    CHILD_TABLE_ID3: "#tblChild3",
    CHILD_TABLE_ID4: "#tblChild4",
    CHILD_TABLE_ID5: "#tblChild5",
    CHILD_TABLE_ID6: "#tblChild6",

    CHILD_TABLE_VIEW1: "#tblChildId_View1",
    CHILD_TABLE_VIEW2: "#tblChildId_View2",
    CHILD_TABLE_VIEW3: "#tblChildId_View3",
    CHILD_TABLE_VIEW4: "#tblChildId_View4",

    FORM_ID: "#form",
    FORM_ID2: "#form2",
    FORM_ID3: "#form3",
    DIV_DETAILS_ID: "#divDetails",
    DIV_REPORT_ID: "#divReports"
});



var UtilityHelper = {
    EmptyThenZero(fieldValue) {
        if (fieldValue === "" || typeof fieldValue === "undefined" || fieldValue === "0" || fieldValue == null
            || fieldValue === undefined || fieldValue === "NaN" || fieldValue === "Infinity") return 0;
        return fieldValue;
    },
    DigitWiseRoundedDecimal(fieldValue, digit) {
        let factor = Math.pow(10, digit);
        // Floor the number to effectively round down to 3 decimal places
        let roundedDown = Math.floor(fieldValue * factor) / factor;
        return roundedDown
    },
    //Remove Whitespace
    RemoveWhitespace: function (value) {
        return removeWData = value.replace(/\s+/g, " ").trim();
    },
    DaysBetweenTwoDates: function (date1, date2) {

        // The number of milliseconds in one day
        var ONE_DAY = 1000 * 60 * 60 * 24;

        // Convert both dates to milliseconds
        var date1_ms = new Date(date1).getTime();
        var date2_ms = new Date(date2).getTime();

        // Calculate the difference in milliseconds
        /*var difference_ms = Math.abs(date1_ms - date2_ms);*/
        var difference_ms = date2_ms - date1_ms;
        // Convert back to days and return
        return Math.round(difference_ms / ONE_DAY);

    },
    GetDaysBetweenTwoDates: function (date1, date2, SkipFridays) {

        date1 = new Date(date1);
        date2 = new Date(date2);

        if (date1 && date2) {
            const daysDifference1 = Math.round((date2 - date1) / (1000 * 60 * 60 * 24));
            const daysDifference2 = Math.abs(daysDifference1);

            let days = 0;

            for (let i = 0; i <= daysDifference2; i++) {
                const currentDate = new Date(date1);
                currentDate.setDate(date1.getDate() + i);

                //days++;
                let dayName = currentDate.toLocaleString('en-us', { weekday: 'long' });
                if (dayName != "Friday" && SkipFridays == true) {
                    days++;
                }

                //if (currentDate.getDay() !== 5 && SkipFridays == true) {
                //    days++;
                //}
            }
            if (daysDifference1 < 0) {
                return -days;
            }
            return days;

        } else {
            return 0
        }
    },
    GetDateAfterNumberOfDays: function (date, NumberOfDay, SkipFridays) {
        // Format input date using moment.js to "DD-MMM-YYYY"

        const formattedDate = moment(date, "DD-MMM-YYYY").toDate(); // Convert to JavaScript Date object

        if (!formattedDate || !NumberOfDay) {
            return 0;
        }

        const currentDate = new Date(formattedDate);

        for (let i = 1; i <= NumberOfDay; i++) {
            currentDate.setDate(currentDate.getDate() + 1);

            if (SkipFridays && currentDate.getDay() === 5) {
                currentDate.setDate(currentDate.getDate() + 1); // Skip Friday
            }
        }

        return currentDate;
    },
    CountFridays: function (date1, date2) {
        date1 = new Date(date1);
        date2 = new Date(date2);

        if (date1 && date2) {
            const daysDifference = Math.round((date2 - date1) / (1000 * 60 * 60 * 24));

            let fridayCount = 0;

            for (let i = 0; i <= daysDifference; i++) {
                const currentDate = new Date(date1);
                currentDate.setDate(date1.getDate() + i);

                if (currentDate.getDay() == 5) {
                    fridayCount++;
                }
            }
            return fridayCount;
        } else {
            return 0
        }
    },
    Debounce: function (func, wait) {
        let timeout;
        return function (...args) {
            const context = this;
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(context, args), wait);
        };
    },
    FxSpinners: function (Step) {
        if (Step == "1") {
            $('.cls_loading_wrap').addClass('d-inline-block');
            $('.cls_loading_wrap').removeClass('d-none');
            $('.cls_loading_circle').loading('cls_loading_circle');
            $('.correctGIF').removeClass('displayNone');

        }
        else if (Step == "2") {
            $('.cls_loading_circle').loading('');
            $('.cls_loading_wrap').addClass('d-none');
            $('.cls_loading_wrap').removeClass('d-inline-block');
            $('.correctGIF').addClass('displayNone');
        }
    },
    FormatIndianCurrency: function (number) {
        var formatter = new Intl.NumberFormat('en-IN', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
        return formatter.format(number);
    },
    FormatIndianCurrencyNoDecimal: function (number) {
        var formatter = new Intl.NumberFormat('en-IN');
        return formatter.format(number);
    },
};

var ArrayHelper = {
    GroupByProperties: function (dataList, groupByKeys, extraFields = []) {
        return dataList.reduce((acc, item) => {
            const key = groupByKeys.map(k => item[k]).join('_');

            if (!acc[key]) {
                // Dynamically pick the grouping fields
                acc[key] = groupByKeys.reduce((obj, key) => {
                    obj[key] = item[key];
                    return obj;
                }, {});

                // Optionally include extra fields
                extraFields.forEach(field => {
                    acc[key][field] = item[field];
                });
            }

            return acc;
        }, {});
    },
    SortByProperty: function (dataList, property, property_1 = "", property_2 = "", property_3 = "", property_4 = "", property_5 = "") {
        return dataList.sort((a, b) => {
            // If property_1 is the same, sort by property_1
            if (property_1 && a[property_1] !== b[property_1]) {
                return a[property_1] - b[property_1];
            }
            // If property_2 is the same, sort by property_2
            if (property_2 && a[property_2] !== b[property_2]) {
                return a[property_2] - b[property_2];
            }
            // If property_3 is the same, sort by property_3
            if (property_3 && a[property_3] !== b[property_3]) {
                return a[property_3] - b[property_3];
            }
            // If property_3 is the same, sort by property_3
            if (property_4 && a[property_4] !== b[property_4]) {
                return a[property_4] - b[property_4];
            }
            // If property_3 is the same, sort by property_3
            if (property_5 && a[property_5] !== b[property_5]) {
                return a[property_5] - b[property_5];
            }

            // If property is the same, sort by main
            return a[property] - b[property]
        });
    },
    DistinctArrayByProperty: function (arr, key) {
        var unique = {};
        var distinct = [];
        for (var i = 0; i < arr.length; i++) {
            var x = arr[i];
            if (!unique[x[key]]) {
                distinct.push(x);
                unique[x[key]] = true;
            }
        }
        return distinct;
    },
    DistinctArray: function (ArrayList, PropertyName) {
        var arrList = ArrayList;
        var names = arrList.map(function (item) {
            return item[PropertyName];
        });
        var distinctValues = $.grep(names, function (value, index) {
            return index === $.inArray(value, names);
        });
        return distinctValues;
    },
    DuplicateArrayCheck: function (ArrayList) {
        var isDuplicate = false;
        for (var i = 0; i < ArrayList.length; i++) {
            for (var j = i + 1; j < ArrayList.length; j++) {
                if (ArrayList[i] == ArrayList[j]) {
                    isDuplicate = true;
                }
            }
        }
        return isDuplicate;
    }
}

//Ajax Calling
var AjaxManager = {
    GetList: function (serviceUrl, successCallback, errorCallback) {
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
    GetList2: function (sUrl) {
        var result = [];
        if (sUrl.trim() == "") return null;
        $.getJSON(sUrl, "", function (data) {
            result = data;
        });
        return result;
    }
};
//Ajax Calling End

var FormHelper = {
    LoadDatePicker: function (identity) {
        $("#" + identity).kendoDatePicker({
            format: "dd-MMM-yyyy",
            change: function () {

                var inputDate = $("#" + identity).val();
                var date;

                // Determine format based on input length
                if (inputDate.length === 6) {
                    date = kendo.parseDate(inputDate, "ddMMyy");
                } else if (inputDate.length === 8) {
                    date = kendo.parseDate(inputDate, "ddMMyyyy");
                } else if (inputDate.length === 10) {
                    date = kendo.parseDate(inputDate, "dd.MM.yyyy");
                } else {
                    date = kendo.parseDate(inputDate, "dd-MMM-yyyy");
                }

                // Set the parsed date in the correct format
                if (date) {
                    this.value(kendo.toString(date, "dd-MMM-yyyy"));
                } else {
                    this.value(kendo.toString(inputDate, "dd-MMM-yyyy"));
                }
            }
        });
        $("#" + identity).on("change", function () {
            var inputValue = $(this).val();

            $("#" + identity).data("kendoDatePicker").trigger("change");
        });
    },
    LoadDatePicker_Format: function (identity, format) {
        $("#" + identity).kendoDatePicker({
            format: format,
        });
        $("#" + identity).on("keypress", function (e) {
            if (e.which === 13) { // Enter key
                var inputVal = $(this).val().trim();
                var day, month, year;

                if (/^\d{6}$/.test(inputVal)) {
                    // Format: ddMMyy
                    day = inputVal.substring(0, 2);
                    month = inputVal.substring(2, 4);
                    year = inputVal.substring(4, 6);
                }
                else if (/^\d{8}$/.test(inputVal)) {
                    // Format: ddMMyyyy
                    day = inputVal.substring(0, 2);
                    month = inputVal.substring(2, 4);
                    year = inputVal.substring(4, 8);
                }
                else if (/^\d{2}\.\d{2}\.\d{2}$/.test(inputVal)) {
                    // Format: dd.MM.yy
                    let parts = inputVal.split(".");
                    day = parts[0];
                    month = parts[1];
                    year = parts[2];
                }
                else if (/^\d{2}\.\d{2}\.\d{4}$/.test(inputVal)) {
                    // Format: dd.MM.yyyy
                    let parts = inputVal.split(".");
                    day = parts[0];
                    month = parts[1];
                    year = parts[2];
                } else {
                    return; // Invalid format
                }

                // Convert year
                if (year.length === 2) {
                    year = parseInt(year, 10) < 50 ? "20" + year : "19" + year;
                }

                // Build string and parse
                var formattedDate = day + "-" + month + "-" + year;
                var parsedDate = kendo.parseDate(formattedDate, "dd-MM-yyyy");

                if (parsedDate) {
                    var datePicker = $("#" + identity).data("kendoDatePicker");
                    datePicker.value(parsedDate);
                    datePicker.trigger("change");
                }
            }
        });
    },
    KendoCustomDatePicker_Enter: function (identity, currentDate, setValue = "") {
        $("#" + identity).kendoDatePicker({
            format: "dd-MMM-yyyy",
            disableDates: function (date) {
                return date < new Date(currentDate); // Disable past dates
            },
            change: function () {

                var inputDate = $("#" + identity).val();
                var date;

                // Determine format based on input length
                if (inputDate.length === 6) {
                    date = kendo.parseDate(inputDate, "ddMMyy");
                } else if (inputDate.length === 8) {
                    date = kendo.parseDate(inputDate, "ddMMyyyy");
                } else if (inputDate.length === 10) {
                    date = kendo.parseDate(inputDate, "dd.MM.yyyy");
                } else {
                    date = kendo.parseDate(inputDate, "dd-MMM-yyyy");
                }

                // Set the parsed date in the correct format
                if (date) {
                    this.value(kendo.toString(date, "dd-MMM-yyyy"));
                } else {
                    this.value(kendo.toString(inputDate, "dd-MMM-yyyy"));
                }
            }
        });
        $("#" + identity).on("change", function () {
            var inputValue = $(this).val();

            $("#" + identity).data("kendoDatePicker").trigger("change");
        });

        if (setValue) {
            let datePicker = $("#" + identity).data("kendoDatePicker");

            datePicker.setOptions({ disableDates: [] });
            datePicker.value(setValue);
            datePicker.setOptions({
                disableDates: function (date) {
                    return date < new Date(currentDate); // Re-disable past dates
                },
                open: function (e) {
                    datePicker.dateView.calendar.navigate(new Date(currentDate));
                }
            });
        }
    },
    KendoCustomDatePicker: function (identity, currentDate, setValue = "") {
        if ($("#" + identity).data("kendoDatePicker")) $("#" + identity).kendoDatePicker('destroy').empty();

        $("#" + identity).kendoDatePicker({
            format: "dd-MMM-yy",
            disableDates: function (date) {
                return date < new Date(currentDate); // Disable past dates
            }
        });

        if (setValue) {
            let datePicker = $("#" + identity).data("kendoDatePicker");

            datePicker.setOptions({ disableDates: [] });
            datePicker.value(setValue);
            datePicker.setOptions({
                disableDates: function (date) {
                    return date < new Date(currentDate); // Re-disable past dates
                },
                open: function (e) {
                    datePicker.dateView.calendar.navigate(new Date(currentDate));
                }
            });
        }

    },
    FormElToJson: function ($formEl) {
        FormHelper.SetAllFormCheckBoxValue($formEl);
        var disabledInputs = $formEl.find(':disabled');
        disabledInputs.each(function () {
            $(this).removeAttr('disabled');
        });

        var data = $formEl.serializeArray();
        var jsonObj = {};
        for (var i = 0; i < data.length; i++) {
            var prop = data[i];
            var val = prop.value;
            // Check if value is a date string like "26-06-2025" (DD-MM-YYYY)
            var dateRegex = /^\d{2}-\d{2}-\d{4}$/;
            if (dateRegex.test(val)) {
                var parts = val.split("-");
                // Create Date object from DD-MM-YYYY format
                var dt = new Date(parts[2], parts[1] - 1, parts[0]);
                val = kendo.toString(dt, "dd-MMM-yyyy");
            }

            jsonObj[prop.name] = val;
        }

        disabledInputs.each(function () {
            $(this).attr('disabled', 'disabled');
        });

        return jsonObj;
    },
    IsFormValidation: function ($formEl, element) {
        var isValidItem = true;
        var $input;
        $($formEl).find(':input').each(function () {
            if ($.inArray(this.id, element) > -1) {

                $input = $("#" + this.id);
                switch ($input[0].type) {
                    case 'text':
                    case 'textarea':
                    case 'password':
                    case 'hidden':
                    case 'number':
                        if ($input.val() == "" || $input.val() == null) {
                            isValidItem = false;
                            $input.closest('.form-group').find('.' + $input[0].id).show();
                        }
                        else {
                            $input.closest('.form-group').find('.' + $input[0].id).hide();
                        }
                        break;
                    case 'select-multiple':
                    case 'select-one':
                        if ($input.val() == "" || $input.val() == null || $input.val() == "-1") { // || $input.val() == "99") {
                            isValidItem = false;
                            $input.closest('.form-group').find('.' + $input[0].id).show();
                        }
                        else {
                            $input.closest('.form-group').find('.' + $input[0].id).hide();
                        }
                        break;
                    case 'checkbox':
                    case 'radio':
                        if (($input[0].id).is(':checked')) {
                            $input.closest('.form-group').find('.' + $input[0].id).hide();
                        } else {
                            isValidItem = false;
                            $input.closest('.form-group').find('.' + $input[0].id).show();
                        }
                        break;
                    default:
                        break;
                }
            }
        });
        return isValidItem;
    },
    IsNullReturnWhiteSpace: function (value) {
        if (value === null)
            return "";
        else
            return value;
    },
    HtmlDecode: function (input) {
        const e = document.createElement('textarea');
        e.innerHTML = input;
        return e.value;
    },
    SetFormData: function ($formEl, data) {

        if (!data && ! typeof data === 'object') {
            console.error("Your data is not valid.");
            return;
        }

        try {
            $formEl.find("input, select, textarea").each(function () {
                try {
                    var $input = $(this);
                    var value = data[this.name];
                    var controlId = this.name;
                    var controltype = "";
                    var optionListName = "";

                    var kendoControlRole = $input.data('role');
                    var kendoControlRole2 = $input.attr('kendorole');

                    //Kendo Combobox Load
                    if (kendoControlRole2 == "combobox") {
                        optionListName = "";
                        if (this.multiple) {
                            optionListName = this.name.replace("Ids", '');
                            optionListName = optionListName.replace("IDs", '');
                        } else {
                            optionListName = this.name.replace("Id", '');
                            optionListName = optionListName.replace("ID", '');
                        }
                        optionListName += "List";
                        KendoDropDownHelper.ComboBox(data[optionListName], controlId, "text", "id", false);
                        value = value == 0 ? null : value;
                        $input.data("kendoComboBox").value(value);
                    }
                    //kendo Date Picker
                    if (kendoControlRole2 == "kendodatepicker") {
                        optionListName = this.name;
                        KendoDateHelper.DatePicker(controlId);
                        value = value ? value : "";
                        $input.data("kendoDatePicker").value(value);
                        //Format Date
                        FormHelper.KendoDateFormater(controlId);
                    }
                    //kendo Switch
                    if (kendoControlRole2 == "kendoswitch") {
                        KendoSwitchHelper.Switch(controlId);
                        value = (value == 1 ? true : false);
                        $input.data("kendoSwitch").value(value);
                    }
                    if ((kendoControlRole == "datepicker" && kendoControlRole != undefined) || kendoControlRole2 == "kendodatepicker") {
                        value = value ? value : "";
                        $input.data("kendoDatePicker").value(value);
                    }
                    else if (kendoControlRole == "combobox" && kendoControlRole != undefined) {
                        value = value == 0 ? null : value;
                        $input.data("kendoComboBox").value(value);
                    }
                    else if (kendoControlRole == "multicolumncombobox" && kendoControlRole != undefined) {
                        value = value == 0 ? null : value;
                        $input.data("kendoMultiColumnComboBox").value(value);
                    }
                    else if (kendoControlRole == "switch" && kendoControlRole != undefined) {
                        value = value == 1 ? true : false;
                        $input.data("kendoSwitch").value(value);
                    }
                    else if (kendoControlRole == "editor" && kendoControlRole != undefined) {
                        $input.data("kendoEditor").value(FormHelper.HtmlDecode(value));
                    }
                    else {
                        if (this.tagName.toLowerCase() === "textarea") {
                            $input.val(value);
                        }
                        else if (this.tagName.toLowerCase() === "input") {
                            switch (this.type) {
                                case "checkbox":
                                    $input.prop("checked", value);
                                    break;
                                case "radio":
                                    $input.each(function (i) {
                                        if ($(this).val() == value) $(this).attr({ checked: true })
                                        else $(this).attr({ checked: false })
                                    });
                                    break;
                                case "date":
                                    $input.val(FormHelper.SetDateFormat(value));
                                    break;
                                case "file":
                                    break;
                                default:
                                    $("#dl" + this.id).html("");
                                    controltype = "controltype";
                                    if (controltype in this.attributes) {
                                        optionListName = "";
                                        optionListName = this.id.replace("Id", '');
                                        optionListName = optionListName.replace("ID", '');
                                        optionListName += "List";

                                        var content = '';
                                        $.each(data[optionListName], function (i, obj) {
                                            content += '<option id=' + obj.id + '  value="' + obj.text + '">' + obj.text + '</option>';
                                        });
                                        $("#dl" + this.id).append(content);
                                    } else {
                                        $input.val(value);
                                    }

                                    break;
                            }
                        }
                        else if (this.tagName.toLowerCase() === "select") {
                            optionListName = "";
                            if (this.multiple) {
                                optionListName = this.name.replace("Ids", '');
                                optionListName = optionListName.replace("IDs", '');
                            } else {
                                optionListName = this.name.replace("Id", '');
                                optionListName = optionListName.replace("ID", '');
                            }
                            optionListName += "List";
                            FormHelper.InitSelect2($input, data[optionListName], true, "Select Item", false);
                            $input.val(value).trigger("change");
                        }
                    }

                } catch (e) {
                    console.error(e);
                }
            });
        } catch (e) {
            console.error(e);
        }
    },
    InitSelect2: function ($el, data, allowClear = true, placeholder = "Select a Value", showDefaultOption = true) {
        if (showDefaultOption)
            data.unshift({ id: '', text: '' });

        $el.html('').select2({ 'data': data, 'allowClear': allowClear, 'placeholder': placeholder });
    },
    LoadSelectDropDown: function (identity, data, showDefaultOption = true) {
        if (showDefaultOption)
            data.unshift({ id: '', text: '' });

        $("#" + identity).html('').select2({ 'data': data, 'allowClear': true, 'placeholder': "Select" });
        if (data.length == 1 && showDefaultOption == false) {
            $("#" + identity).prop("disabled", true);
        }
    },
    TogToolbarBtn: function (el, $parentEl) {
        $parentEl.find('div').not("#" + el.id).removeClass("btn-primary").addClass("btn-secondary");

        if (el instanceof jQuery)
            el.removeClass("btn-secondary").addClass("btn-primary");
        else
            $(el).removeClass("btn-secondary").addClass("btn-primary");
    },
    SetAllFormCheckBoxValue: function ($formEl) {
        $formEl.find(':checkbox').each(function () {
            this.value = this.checked;
        });
    },
    dateFormat: function (date) {
        if (!date) return "";
        return moment(date).format("DD-MMM-YYYY");
    },
    dotDateFormat: function (date) {
        if (!date) return "";
        return moment(date).format("DD.MM.YYYY");
    },
    DateFormat_Month_Year: function (date) {
        if (!date) return "";
        return moment(date).format("YYYY-MM");
    },
    Month_Year_StartDateFormat: function (monthYear) {
        if (!monthYear) return "";
        return moment(monthYear).format("YYYY-MM-01");
    },
    Month_Year_EndDateFormat: function (monthYear) {
        if (!monthYear) return "";
        return moment(monthYear).format("YYYY-MM-") + moment(monthYear).daysInMonth();
    },
    SetDateFormat: function (element, date) {
        if (!date) return "";
        $("#" + element).datepicker().datepicker('setDate', FormHelper.dateFormat(date));
    },
    SetDotDateFormat: function (element, date) {
        if (!date) return "";
        $("#" + element).datepicker().datepicker('setDate', FormHelper.dotDateFormat(date));
    },
    AddDate: function (element, date, days) {
        if (!isNaN(date.getTime())) {
            date.setDate(date.getDate() + days);
            FormHelper.SetDateFormat(element, date);
        } else {
            MessageHelper.Warning("Warning", 'Invalid Date.');
        }
    },
    SetDateFormat3: function (element, date) {
        var date1 = date;
        if (!date) return "";
        $("#" + element).datepicker().datepicker('setDate', date1);
    },
    FormatDate: function (dateString, format) {
        if (!dateString) return "";

        var date = new Date(dateString);
        var day = ("0" + date.getDate()).slice(-2);
        var month = ("0" + (date.getMonth() + 1)).slice(-2);
        var yearFull = date.getFullYear();
        var yearShort = yearFull.toString().slice(-2);

        var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        var monthNameShort = monthNames[date.getMonth()];

        // Replace format placeholders
        var formattedDate = format
            .replace("dd", day)
            .replace("mm", month)
            .replace("MMM", monthNameShort)
            .replace("yyyy", yearFull)
            .replace("yy", yearShort);

        return formattedDate;
    },
    ConvertToBoolean: function (str) {
        if (!str)
            return false;

        switch (str.toLowerCase()) {
            case "on":
            case "true":
            case "yes":
            case "1":
                return true;
            default: return false;
        }
    },
    KendoDateFormater: function (identity) {
        $("#" + identity).on("keypress", function (e) {
            if (e.which === 13) { // Enter key
                var inputVal = $(this).val().trim();
                var day, month, year;

                if (/^\d{6}$/.test(inputVal)) {
                    // Format: ddMMyy
                    day = inputVal.substring(0, 2);
                    month = inputVal.substring(2, 4);
                    year = inputVal.substring(4, 6);
                }
                else if (/^\d{8}$/.test(inputVal)) {
                    // Format: ddMMyyyy
                    day = inputVal.substring(0, 2);
                    month = inputVal.substring(2, 4);
                    year = inputVal.substring(4, 8);
                }
                else if (/^\d{2}\.\d{2}\.\d{2}$/.test(inputVal)) {
                    // Format: dd.MM.yy
                    let parts = inputVal.split(".");
                    day = parts[0];
                    month = parts[1];
                    year = parts[2];
                }
                else if (/^\d{2}\.\d{2}\.\d{4}$/.test(inputVal)) {
                    // Format: dd.MM.yyyy
                    let parts = inputVal.split(".");
                    day = parts[0];
                    month = parts[1];
                    year = parts[2];
                } else {
                    return; // Invalid format
                }

                // Convert year
                if (year.length === 2) {
                    year = parseInt(year, 10) < 50 ? "20" + year : "19" + year;
                }

                // Build string and parse
                var formattedDate = day + "-" + month + "-" + year;
                var parsedDate = kendo.parseDate(formattedDate, "dd-MM-yyyy");

                if (parsedDate) {
                    var datePicker = $("#" + identity).data("kendoDatePicker");
                    datePicker.value(parsedDate);
                    datePicker.trigger("change");
                }
                else {
                    var datePicker = $("#" + identity).data("kendoDatePicker");
                    datePicker.value("");
                    ToastrMessageHelper.ValidationToastr("Invalid date format");
                }
            }
        });
    }
}

var HRMData = {
    GetEmployeeByFilter: function (employeeName, InputField, IsNeedChangeEvent) {
        kEmpFilterBy_InputField = InputField;

        if ($("#" + InputField).data("kendoMultiColumnComboBox"))
            $("#" + InputField).kendoMultiColumnComboBox('destroy').empty();

        $("#" + InputField).kendoMultiColumnComboBox({
            filter: "contains",
            minLength: 3,
            autoWidth: true,
            dataTextField: "EmployeeName",
            dataValueField: "EmployeeID",
            height: 400,
            //autoBind: false,
            columns: [
                { field: "EmployeeNo", title: "Employee No", width: "110px" },
                { field: "EmployeeName", title: "Employee", width: "220px" },
                { field: "DepartmentName", title: "Department", width: "220" },
                { field: "DesignationName", title: "Designation", width: "200px" },
                { field: "SectionName", title: "Section", width: "160px" },
                { field: "UnitFullName", title: "Project", width: "180px" }
            ],
            dataSource: {
                type: "json",
                serverFiltering: true,
                transport: {
                    read: {
                        type: "GET",
                        url: `/api/helperC/GetEmployeeListBySearchKey?employeeName=${employeeName}&pageIndex=1&pageSize=150`,
                        dataType: "json",
                        contentType: 'application/json',
                        data: {
                            searchKey: HRMData.EmpFilterValue
                        }
                    },
                }
            },
            index: -1,
            change: function (e) {
                if (this.value()) {
                    var model = this.dataItem();

                    if (IsNeedChangeEvent) {
                        $("#DepartmentID").val(model.DepartmentID);
                        $("#SectionID").val(model.SectionID);
                        $("#SubSectionID").val(model.SubSectionID);

                        $("#DepartmentName").val(model.DepartmentName);
                        $("#SectionName").val(model.SectionName);
                        $("#SubSectionName").val(model.SubSectionName);
                    }
                }
            }
        });
    },
    EmpFilterValue: function () {
        var itemCombo = $("#" + kEmpFilterBy_InputField).data("kendoMultiColumnComboBox");
        filter = itemCombo.text();
        return filter;
    },

}

//Start :: Toastr
var ToastrMessageHelper = {
    ValidationToastr: function (Message) {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-bottom-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        toastr.error(Message);
    },
    SucessToastr: function (Message) {
        toastr.options1 = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-bottom-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        toastr.success(Message);
    },
    DisplayError: function (error) {
        console.error(error);
        try {
            if (error.response && error.response.data && error.response.data.Message)
                ToastrMessageHelper.ValidationToastr(error.response.data.Message);
            else
                ToastrMessageHelper.ValidationToastr(error);
        } catch (e) {
            ToastrMessageHelper.ValidationToastr(error);
        }
    },
    FxToastr: function (Message) {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-bottom-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        toastr.error(Message);
    }
}
//End :: Toastr

//Start :: Sweet Alert
var MessageHelper = {
    Success: function (title, operation) {
        Swal.fire(title, operation, "success");
    },
    Error: function (title, operation) {
        Swal.fire(title, operation, "error");
    },
    Warning: function (title, operation) {
        Swal.fire(title, operation, "warning");
    },
    Info: function (title, operation) {
        Swal.fire(title, operation, "info");
    },
    Question: function (title, operation) {
        Swal.fire(title, operation, "question");
    },
    ShowNoRecord: function () {
        Swal.fire("Warning", "Sorry, Data not found!", "warning");
    },
    Delete: function () {
        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, delete it!"
        }).then(function (result) {
            if (result.value) {
                Swal.fire(
                    "Deleted!",
                    "Your file has been deleted.",
                    "success"
                )
            }
        });
    },

    FxSpinners: function (Step) {
        if (Step == "1") {
            $('.cls_loading_wrap').addClass('d-inline-block');
            $('.cls_loading_wrap').removeClass('d-none');
            $('.cls_loading_circle').loading('cls_loading_circle');
            $('.correctGIF').removeClass('displayNone');

        }
        else if (Step == "2") {
            $('.cls_loading_circle').loading('');
            $('.cls_loading_wrap').addClass('d-none');
            $('.cls_loading_wrap').removeClass('d-inline-block');
            $('.correctGIF').addClass('displayNone');
        }
    }

}
//End :: SweetAlert

//Sweet Alert Common Function
var SwalHelper = {
    //For confirmation Settings
    SwalConfirmationConfig: function (msg) {
        var config = {
            title: msg, icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
        }
        return config;
    }
}

//Start :: Email Validation
var EmailValidationHelper = {
    IsEmailValid: function (inputText) {
        var regex = /^([\w-\.]+\u0040([\w-]+\.)+[\w-]{2,4})?$/;
        return regex.test(inputText);
    }
}
//End :: Email Validation

var FormData_File = new FormData();
var FileDataArray = [];
var UploadFiles = "";
var UploadFileNames = "";

var FileUploadHelper = {
    SetUploadedFileArray: function (fileDateArray, FilePath, PrimaryKeyID, FileFormatName, Usql, Dsql = "", IsDeleted = 0, Condition, Is64Byte = false, isMultiple = false, PreviousFilePath = "") {
        // Get the selected files and names from the input elements
        //UploadFiles = $("#" + HTMLTextBoxID).get(0).files;
        //UploadFileNames = $("#" + HTMLTextBoxID);

        //// Populate the fileDataArray with the selected files and names
        //for (var i = 0; i < UploadFiles.length; i++) {
        //    var file = UploadFiles[i];//.files[0];
        //    var name = UploadFileNames[0];//.value;

        //    var fileData = {
        //        Name: name,
        //        File: file
        //    };

        //    FileDataArray.push(fileData);
        //}
        debugger
        //FileUpload [ Controller Function Name]
        FileDataArray = fileDateArray;
        for (var j = 0; j < FileDataArray.length; j++) {
            FormData_File.append("FileUpload[" + j + "].Name", FileDataArray[j].Name);
            FormData_File.append("FileUpload[" + j + "].Files", FileDataArray[j].File);
            FormData_File.append("FileUpload[" + j + "].FilePath", FilePath);
            FormData_File.append("FileUpload[" + j + "].HTMLTextBoxID", "");
            FormData_File.append("FileUpload[" + j + "].IsName", 'false');
            FormData_File.append("FileUpload[" + j + "].PrimaryKeyID", PrimaryKeyID);
            FormData_File.append("FileUpload[" + j + "].FileFormatName", FileFormatName);
            FormData_File.append("FileUpload[" + j + "].Usql", FileDataArray[j].Usql);
            FormData_File.append("FileUpload[" + j + "].Dsql", Dsql);
            FormData_File.append("FileUpload[" + j + "].IsDeleted", 0);
            FormData_File.append("FileUpload[" + j + "].Condition", FileDataArray[j].Condition);
            FormData_File.append("FileUpload[" + j + "].Is64Byte", Is64Byte);
            FormData_File.append("FileUpload[" + j + "].PreviousFilePath", PreviousFilePath);
            FormData_File.append("FileUpload[" + j + "].isMultiple", isMultiple);
        }
    },
    SetUploadedFile: function (HTMLTextBoxID, FilePath, PrimaryKeyID, FileFormatName, Usql, Dsql = "", IsDeleted = 0, Condition, Is64Byte = 0, isMultiple = false, PreviousFilePath = "") {
        // Get the selected files and names from the input elements
        UploadFiles = $("#" + HTMLTextBoxID).get(0).files;
        UploadFileNames = $("#" + HTMLTextBoxID);

        // Populate the fileDataArray with the selected files and names 
        for (var i = 0; i < UploadFiles.length; i++) {
            var file = UploadFiles[i];//.files[0];
            var name = UploadFileNames[0];//.value; 

            var fileData = {
                Name: name,
                File: file
            };

            FileDataArray.push(fileData);
        }

        //FileUpload [ Controller Function Name]
        for (var j = 0; j < FileDataArray.length; j++) {
            FormData_File.append("FileUpload[" + j + "].Name", FileDataArray[j].Name);
            FormData_File.append("FileUpload[" + j + "].Files", FileDataArray[j].File);
            FormData_File.append("FileUpload[" + j + "].FilePath", FilePath);
            FormData_File.append("FileUpload[" + j + "].HTMLTextBoxID", HTMLTextBoxID);
            FormData_File.append("FileUpload[" + j + "].IsName", 'false');
            FormData_File.append("FileUpload[" + j + "].PrimaryKeyID", PrimaryKeyID);
            FormData_File.append("FileUpload[" + j + "].FileFormatName", FileFormatName);
            FormData_File.append("FileUpload[" + j + "].Usql", Usql);
            FormData_File.append("FileUpload[" + j + "].Dsql", Dsql);
            FormData_File.append("FileUpload[" + j + "].IsDeleted", IsDeleted);
            FormData_File.append("FileUpload[" + j + "].Condition", Condition);
            FormData_File.append("FileUpload[" + j + "].Is64Byte", Is64Byte);
            FormData_File.append("FileUpload[" + j + "].PreviousFilePath", PreviousFilePath);
            FormData_File.append("FileUpload[" + j + "].isMultiple", isMultiple);
        }
    },
    SaveFile: function (_fileID, vURL) {
        debugger
        var file = [];
        $.ajax({
            method: 'post',
            processData: false,
            contentType: false,
            cache: false,
            data: FormData_File,
            enctype: 'multipart/form-data',
            url: vURL,
            success: function (response) {

                if (response.length > 0) {
                    file = response[0];
                }

            },
            error: function (err) { }
        });
        return file;

    },
    FileFormat: function (fileExtension, Image) {
        var extension = [];

        if (Image == true) {
            extension = ['jpg', 'jpeg', 'png']
        }
        else {
            extension = ['jpg', 'jpeg', 'png', 'pdf', 'xlsx', 'xls', 'docx', 'doc']
        }

        if (extension.includes(fileExtension)) {
            return true;
        } else {
            return false;
        }
    },
    HandleFiles: function (e, canvasID) {
        const canvas = document.getElementById(canvasID);
        var ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        var size = e.target.files[0].size;
        var fileFormat = e.target.files[0].name.split('.').pop();
        var isValidFileFormat = FileUploadHelper.FileFormat(fileFormat, false);

        if (!isValidFileFormat) {
            MessageHelper.Warning("Invalid file", "Only jpg, jpeg, png, pdf, xlsx, xls, docx, doc files are allowed");
            return;
        }

        if (size > 20000000) {
            MessageHelper.Warning("Large file", "Maximum file size is 20 MB.");
            return;
        }
        var reader = new FileReader();
        reader.onload = function (event) {
            var img = new Image();
            img.onload = function () {
                canvas.width = img.width;
                canvas.height = img.height;
                ctx.drawImage(img, 0, 0);
            }
            img.src = event.target.result;
        }
        reader.readAsDataURL(e.target.files[0]);
    },
    HandleImage: function (e, canvasID) {
        const canvas = document.getElementById(canvasID);
        var ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        var size = e.target.files[0].size;
        var fileFormat = e.target.files[0].name.split('.').pop();
        var isValidFileFormat = FileUploadHelper.FileFormat(fileFormat, true);

        if (!isValidFileFormat) {
            MessageHelper.Warning("Invalid file", "Onluy jpg, jpeg, png files are allowed");
            return false;
        }

        if (size > 20000000) {
            MessageHelper.Warning("Large file", "Maximum file size is 20 MB.");
            return false;
        }

        var reader = new FileReader();
        reader.onload = function (event) {
            var img = new Image();
            img.onload = function () {
                canvas.width = img.width;
                canvas.height = img.height;
                ctx.drawImage(img, 0, 0);
            }
            img.src = event.target.result;
        }
        reader.readAsDataURL(e.target.files[0]);
    },
    SetFile: function (canvasID, RootFilePath) {
        const canvas = document.getElementById(canvasID);
        var ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        var img = new Image();
        img.onload = function () {
            canvas.width = img.width;
            canvas.height = img.height;
            ctx.drawImage(img, 0, 0);
        }

        img.src = null;

        img.src = RootFilePath;
    },
    DownloadFile: function (url) {
        if (url == '') {
            url = null;
        }

        if (url != null) {
            window.open(url);
        } else {
            MessageHelper.Warning("Not Found", 'No file was found against this record.');
        }
    },
    FileConvertBase64Bytes: function (url, callback) {
        var xhr = new XMLHttpRequest();
        xhr.onload = function () {
            var reader = new FileReader();
            reader.onloadend = function () {
                callback(reader.result);
            }
            reader.readAsDataURL(xhr.response);
        };
        xhr.open('GET', url);
        xhr.responseType = 'blob';
        xhr.send();
    },

    //ImageSave: function (txtTextBox, fileTextBox, ModuleName, IdentityID) {
    //    if ($('#' + txtTextBox).val().length > 0) { //txt_UI_Employee_Img
    //        var Files = document.getElementById(fileTextBox).files[0]; //file_Employee_Img

    //        if (Files != undefined) {
    //            if (Files.size > 204800) {
    //                //Warning('File Size Exceeds 200 KB Limit, pls upload image below 200 KB', "Warning");
    //                alert('File Size Exceeds 200 KB Limit, pls upload image below 200 KB');
    //                return;
    //            }
    //            if (!Files.type.match(/image.*/)) {
    //                //Warning('Please ppload image file', "Warning"); //catch message
    //                alert('Please ppload image file'); //catch message
    //                return;
    //            }
    //            else {
    //                var fd = new FormData();
    //                fd.append("Files", Files);
    //                fd.append("ModuleName", ModuleName);
    //                fd.append("IdentityID", IdentityID);
    //                var xhr = new XMLHttpRequest();
    //                xhr.open("POST", "/Common/SaveImage");
    //                xhr.send(fd);
    //            }
    //        }
    //    }
    //}, 
}

//Start :: character restrictions, only interget value allow to the text field
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

$(".IsEmail").blur(function (e) {
    var regex = /^([\w-\.]+\u0040([\w-]+\.)+[\w-]{2,4})?$/;
    if (!regex.test($(this).val())) {
        $(this).closest('.form-group').find('.' + $(this).attr("id")).show().html("Invalid email address");
    } else {
        $(this).closest('.form-group').find('.' + $(this).attr("id")).hide().html("required.");
    }
});


//Inword related functions
var inWordDimension_a = ['', 'One ', 'Two ', 'Three ', 'Four ', 'Five ', 'Six ', 'Seven ', 'Eight ', 'Nine ', 'Ten ', 'Eleven ', 'Twelve ', 'Thirteen ', 'Fourteen ', 'Fifteen ', 'Sixteen ', 'Seventeen ', 'Eighteen ', 'Nineteen '];
var inWordDimension_b = ['', '', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];

var InWordsHelper = {
    InWords: function (num) {
        var vInwords_text = '';
        var vFloor = 0;
        var vFraction_Text = '';
        var vIndex = num.indexOf('.');
        vFloor = Math.floor(num);
        vFraction_Text = num.substring(vIndex + 1, num.length);
        vFraction_Text = Math.abs(vFraction_Text);

        if (vFraction_Text === 0) {
            vIndex = -1;
        }

        if ((vFloor > 0) && (vIndex === -1)) {
            vInwords_text = inWords_Modified(vFloor) + 'Taka Only';
        } else if ((vFloor === 0) && (vIndex > -1)) {
            vInwords_text = inWords_Modified(vFraction_Text) + 'Paisa Only';
        } else if ((vFloor > 0) && (vIndex > -1)) {
            vInwords_text = inWords_Modified(vFloor);
            vInwords_text += 'Taka And ' + inWords_Modified(vFraction_Text) + 'Paisa Only';
        }
        return vInwords_text;
    },
    InWords_Modified: function (num) {
        if ((num = num.toString()).length > 9) return 'overflow';
        n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
        if (!n) return; var str = '';
        str += (n[1] != 0) ? (inWordDimension_a[Number(n[1])] || inWordDimension_b[n[1][0]] + ' ' + inWordDimension_a[n[1][1]]) + 'Crore ' : '';
        str += (n[2] != 0) ? (inWordDimension_a[Number(n[2])] || inWordDimension_b[n[2][0]] + ' ' + inWordDimension_a[n[2][1]]) + 'Lakh ' : '';
        str += (n[3] != 0) ? (inWordDimension_a[Number(n[3])] || inWordDimension_b[n[3][0]] + ' ' + inWordDimension_a[n[3][1]]) + 'Thousand ' : '';
        str += (n[4] != 0) ? (inWordDimension_a[Number(n[4])] || inWordDimension_b[n[4][0]] + ' ' + inWordDimension_a[n[4][1]]) + 'Hundred ' : '';
        str += (n[5] != 0) ? ((str != '') ? ' ' : '') + (inWordDimension_a[Number(n[5])] || inWordDimension_b[n[5][0]] + ' ' + inWordDimension_a[n[5][1]]) + ' ' : '';
        return str;
    },
    InWords_actual: function (num) {
        if ((num = num.toString()).length > 9) return 'overflow';
        n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
        if (!n) return; var str = '';
        str += (n[1] != 0) ? (inWordDimension_a[Number(n[1])] || inWordDimension_b[n[1][0]] + ' ' + inWordDimension_a[n[1][1]]) + 'Crore ' : '';
        str += (n[2] != 0) ? (inWordDimension_a[Number(n[2])] || inWordDimension_b[n[2][0]] + ' ' + inWordDimension_a[n[2][1]]) + 'Lakh ' : '';
        str += (n[3] != 0) ? (inWordDimension_a[Number(n[3])] || inWordDimension_b[n[3][0]] + ' ' + inWordDimension_a[n[3][1]]) + 'Thousand ' : '';
        str += (n[4] != 0) ? (inWordDimension_a[Number(n[4])] || inWordDimension_b[n[4][0]] + ' ' + inWordDimension_a[n[4][1]]) + 'Hundred ' : '';
        str += (n[5] != 0) ? ((str != '') ? ' And ' : '') + (inWordDimension_a[Number(n[5])] || inWordDimension_b[n[5][0]] + ' ' + inWordDimension_a[n[5][1]]) + ' ' : '';
        return str;
    },
    InWords_text: function (num) {
        var vInwords = inWords(num);
        vInwords = $.trim(vInwords);
        if ((vInwords.trim() == null) || (vInwords.trim() == '')) {
            return '';
        }
        else {
            return vInwords + ' Only';
        }
    },
    InWords_paisa_text: function (num) {
        var vInwords_text = '';
        var vFloor = 0;
        var vFraction_Text = '';
        var vIndex = num.indexOf('.');
        vFloor = Math.floor(num);
        vFraction_Text = num.substring(vIndex + 1, num.length);
        vFraction_Text = Math.abs(vFraction_Text);

        if (vFraction_Text === 0) {
            vIndex = -1;
        }

        if ((vFloor > 0) && (vIndex === -1)) {
            vInwords_text = inWords(vFloor) + ' Taka Only';
        } else if ((vFloor === 0) && (vIndex > -1)) {
            vInwords_text = inWords(vFraction_Text) + ' Paisa Only';
        } else if ((vFloor > 0) && (vIndex > -1)) {
            vInwords_text = inWords(vFloor);
            vInwords_text += ' Taka And ' + inWords(vFraction_Text) + ' Paisa Only';
        }
        return vInwords_text;

    },
    Floor_Value: function (num) {
        var val = 0;
        val = Math.floor(num);
        return val;
    },
    Fraction_Value: function (num) {
        var val = 0;
        var vIndex = num.indexOf('.');
        var vFraction_Text = '';
        if (vIndex == -1) {
            val = 0;
        } else {
            vFraction_Text = num.substring(vIndex + 1, num.length);
            if (vFraction_Text === 0) {
                vIndex = -1;
            } else {
                val = Math.abs(vFraction_Text);
            }
        }
        return val;
    }
}

//Date Time Common Function 

function SetTabIndex(IndexValues) {
    $.each(IndexValues, function (i, obj) {
        i = parseInt(i) + 1;
        $("#" + obj).attr('tabindex', i);
    });
}

//Check Available Balance in ICSE Employee

var SpinnerHelper = {
    Spinner: function (Step) {
        if (Step == "1") {
            $('.cls_loading_circle').loading('cls_loading_circle');
            $('.cls_loading_wrap').addClass('d-inline-block');
            $('.cls_loading_wrap').removeClass('d-none');
        }
        else if (Step == "2") {
            $('.cls_loading_circle').loading(false);
            $('.cls_loading_wrap').addClass('d-none');
            $('.cls_loading_wrap').removeClass('d-inline-block');
        }
    }
};

var DateTimeHelper = {
    TotalMinutesCalculation: function (startTime, endTime) {// By Ashraful
        var startTime = moment(startTime, "HH:mm tt");
        var endTime = moment(endTime, "HH:mm tt");
        var duration = moment.duration(endTime.diff(startTime));
        var hours = duration._data.hours;
        if (hours < 0) {
            hours = (duration._data.hours + 12);
        }
        var munites = 0;
        var totalMunites = 0;
        if (duration._data.hours > 0) {
            munites = duration._data.minutes;
            totalMunites = parseInt(hours) * 60 + parseInt(munites);

        } else {
            totalMunites = (hours * 60) + duration._data.minutes;
        }
        return totalMunites;
    },
    GetValidTime: function () {
        var currentTime = new Date();
        var hours = currentTime.getHours();
        var minutes = currentTime.getMinutes();
        var seconds = currentTime.getSeconds();

        var meridiem = hours < 12 ? "AM" : "PM";

        hours = hours % 12;
        hours = hours ? hours : 12; // "0" should be displayed as "12"

        minutes = DateTimeHelper.AddLeadingZero(minutes);
        seconds = DateTimeHelper.AddLeadingZero(seconds);

        var timeString = hours + ":" + minutes + ":" + seconds + " " + meridiem;

        // var tTime = ((tHrs < 10) ? "0" + tHrs : tHrs) + ":" + ((tMin < 10) ? "0" + tMin : tMin) + ":" + ((tSec < 10) ? "0" + tSec : tSec);
        return timeString
    },
    AddLeadingZero: function (number) {
        return (number < 10 ? "0" : "") + number;
    },
    GetSystemDate: function () {
        var today = new Date();
        var dd = String(today.getDate()).padStart(2, '0');
        var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
        var yyyy = today.getFullYear();
        today = dd + '-' + mm + '-' + yyyy;
        return today;
    },
    GetSystemTime: function () {
        var date = new Date();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    },
    GetSystemTime24: function () {
        var date = new Date();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var strTime = hours + ':' + minutes;
        return strTime;
    }
}

//Kendo Grid Common Function
var KendoGridHelper = {
    MasterGridLoad: function (element, height, search, columns, isFilter = false) {
        $(element).kendoGrid({
            dataSource: dataSource,
            //height: height,
            height: window_height - 135,
            editable: false,
            noRecords: true,
            scrollable: true,
            sortable: true,
            filterable: isFilter,
            messages: {
                noRecords: "NO DATA FOUND"
            },
            toolbar: ["search", "excel"],
            excel: {
                fileName: element.substring(4, element.length - 4) + ".xlsx",
            },
            search: search,
            pageSize: 20,
            pageable: {
                refresh: true,
                pageSizes: true,
                buttonCount: 5,
                numeric: true,
            },
            columns: columns,
            dataBinding: function () {
                gbGridSLNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            },
            selectable: "row"
        }).data("kendoGrid");
    },
    MasterEditableGridLoad: function (element, height, search, columns, isFilter = false, isEditable = true) {
        if (isEditable) {
            isEditable = { "createAt": "bottom" }
        }

        $(element).kendoGrid({
            dataSource: dataSource,
            //height: height,
            height: window_height - 135,
            editable: false,
            noRecords: true,
            scrollable: true,
            sortable: true,
            filterable: isFilter,
            editable: isEditable,
            messages: {
                noRecords: "NO DATA FOUND"
            },
            toolbar: ["search", "excel"],
            excel: {
                fileName: element.substring(4, element.length - 4) + ".xlsx",
            },
            search: search,
            pageSize: 20,
            pageable: {
                refresh: true,
                pageSizes: true,
                buttonCount: 5,
                numeric: true,
            },
            columns: columns,
            dataBinding: function () {
                gbGridSLNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            },
            selectable: "row"
        }).data("kendoGrid");
    },
    MasterGridWithoutPagination: function (element, columns, data, schema, height) {
        var dataSource = new kendo.data.DataSource({
            data: data,
            schema: schema
        });
        $(element).kendoGrid({
            noRecords: true,
            messages: { noRecords: "NO DATA FOUND" },
            dataSource: dataSource,
            height: height,
            filterable: false,
            sortable: false,
            columns: columns,
            selectable: "row",
            navigatable: true,
            scrollable: true,
            resizable: false

        });

    },
    MasterGridLoad_VT: function (element, height, search, columns, isFilter = false, pageSize = 20) {
        $(element).kendoGrid({
            dataSource: dataSource,
            //height: height,
            height: window_height - 135,
            editable: false,
            noRecords: true,
            scrollable: true,
            sortable: true,
            filterable: isFilter,
            messages: {
                noRecords: "NO DATA FOUND"
            },
            toolbar: ["search", "excel"],
            excel: {
                fileName: element.substring(4, element.length - 4) + ".xlsx",
            },
            search: search,
            pageSize: pageSize,
            pageable: {
                refresh: true,
                pageSizes: true,
                buttonCount: 5,
                numeric: true,
            },
            height: height,
            scrollable: {
                virtual: true
            },
            columns: columns,
            dataBinding: function () {
                gbGridSLNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
            },
            selectable: "row"
        }).data("kendoGrid");
    },

    ChildGridLoad: function (element, columns, data, schema, isEditable, isToolbar = true, height, isGroupable = false, rowPosition = "bottom", isfilterable = false) {

        var dataSource = new kendo.data.DataSource({
            data: data,
            schema: schema
        });
        if (isEditable) {
            isEditable = { "createAt": rowPosition }
        }

        $(element).kendoGrid({
            noRecords: true,
            messages: { noRecords: "NO DATA FOUND" },
            toolbar: isToolbar ? [{ name: "create", text: "Add New" }] : [],
            dataSource: dataSource,
            height: height,
            filterable: isfilterable,
            sortable: false,
            columns: columns,
            editable: isEditable,
            selectable: "row",
            navigatable: true,
            scrollable: true,
            resizable: false,
            groupable: isGroupable,
            dataBinding: function (e) {
                gbGridSLNo = 0;
            },
            edit: function (e) {
                const dataItem = e.model;
                if (!dataItem.RowSLNo) {
                    const allRows = this.dataSource.data().filter(x => x.RowSLNo);
                    const maxSLNo = allRows.length ? Math.max(...allRows.map(r => r.RowSLNo || 0)) : 0;
                    dataItem.set("RowSLNo", maxSLNo + 1);
                }

                setTimeout(() => {
                    const row = e.container.closest("tr");
                    this.select(row);
                }, 0);



                //expand multi column combo
                let $el = e.container.find("[data-role='multicolumncombobox']").first();
                let mccb = $el.length ? kendo.widgetInstance($el) : null;
                if (mccb) {
                    setTimeout(function () {
                        mccb.open();
                        if (mccb.input) mccb.input.focus();
                    }, 0);
                }

                //expand combo
                let $el2 = e.container.find("[data-role='combobox']").first();
                let mccb2 = $el2.length ? kendo.widgetInstance($el2) : null;
                if (mccb2) {
                    setTimeout(function () {
                        mccb2.open();
                        if (mccb2.input) mccb2.input.focus();
                    }, 0);
                }

            },
            dataBound: function () {
                const grid = this;

                grid.tbody.find("tr").each(function () {
                    const $tr = $(this);
                    const dataItem = grid.dataItem($tr);
                    if (dataItem && dataItem.IsDeleted === 1) {
                        $tr.addClass("undo-row");
                        $tr.find("td:not(.cmd-cell)").each(function () {
                            const t = $(this).text().trim();
                            $(this).html(`<span class="undo-line-text">${t}</span>`);
                        });
                    } else {
                        $tr.removeClass("undo-row");
                    }
                });
            } 
        });

        if (rowPosition == "bottom") {
            $(element).find(".k-grid-toolbar").insertAfter($(element + " .k-grid-content"));
        }

    },
    ChildGridLoad_DataSource: function (element, columns, dataSource, isEditable = true, isToolbar = true, isGroupable = false) {
        if (isEditable) {
            isEditable = { "createAt": "bottom" }
        }

        $(element).kendoGrid({
            noRecords: true,
            messages: { noRecords: "NO DATA FOUND" },
            toolbar: isToolbar ? [{ name: "create", text: "Add New" }] : [],
            dataSource: dataSource,
            filterable: false,
            sortable: false,
            columns: columns,
            //editable: true,
            editable: { "createAt": "bottom" },
            selectable: "row",
            navigatable: false,
            scrollable: true,
            resizable: false,
            groupable: isGroupable,
            edit: function (e) {
                if (e.model.isNew()) {//newly added row will be selected
                    e.sender.select(e.sender.table.find('tr[data-uid="' + e.model.uid + '"]'));
                }
            },
            dataBinding: function () {
                gbGridSLNo = 0;
            }
        });
        // Sets toolbar under the Grid body
        $(element).find(".k-grid-toolbar").insertAfter($(element + " .k-grid-content"));
    },

    ChildGridLoad_Aggregate: function (element, columns, data, schema, isEditable, height, aggregateFields) {

        var dataSource = new kendo.data.DataSource({
            data: data,
            schema: schema,
            aggregate: aggregateFields
        });

        if (isEditable) {
            isEditable = { "createAt": "bottom" }
        }

        $(element).kendoGrid({
            noRecords: true,
            messages: { noRecords: "NO DATA FOUND" },
            dataSource: dataSource,
            filterable: false,
            sortable: false,
            columns: columns,
            editable: isEditable,
            selectable: "row",
            navigatable: true,
            scrollable: true,
            resizable: false,
            height: height,
            toolbar: false
        });
        $(element).find(".k-grid-toolbar").insertAfter($(element + " .k-grid-content"));
    },

    ChildGridLoad_VT: function (element, columns, search, data, height = 400, pageSize = 20) {
        $(element).kendoGrid({
            dataSource: data,
            editable: false,
            noRecords: true,
            scrollable: true,
            sortable: true,
            filterable: true,
            messages: {
                noRecords: "NO DATA FOUND"
            },
            toolbar: ["search"],
            search: search,
            pageSize: pageSize,
            height: height,
            scrollable: {
                virtual: true
            },
            columns: columns,
            selectable: "row"
        }).data("kendoGrid");
    },

    InlineEditGridLoad: function (ctrlId, model, isBlankDs, height = 500, param1 = "") {
        if ($("#" + ctrlId).data("kendoGrid")) $("#" + ctrlId).kendoGrid('destroy').empty();
        var { api, schema, columns } = model;
        var ds = [];
        if (!isBlankDs) {
            ds = new kendo.data.DataSource({
                transport: {
                    read: {
                        url: `/api/${api.controllerName}/${api.listMethod}/${param1}`,
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                    },
                    update: {
                        url: `/api/${api.controllerName}/${api.saveMethod}`,
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                    },
                    destroy: {
                        url: `/api/${api.controllerName}/${api.saveMethod}`,
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                    },
                    create: {
                        url: `/api/${api.controllerName}/${api.saveMethod}`,
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                    },
                    parameterMap: function (options, operation) {

                        const idField = schema.id;
                        if (operation === "read") {
                            return JSON.stringify({
                                page: options.page,
                                take: options.take,
                                skip: options.skip,
                                sort: options.sort,
                                filter: options.filter
                            });
                        }
                        else if (operation === 'create' || operation === 'update') {
                            let model = {
                                ...options,
                                [idField]: options[idField] || 0
                            }
                            return JSON.stringify(model);
                        }
                        else if (operation == "destroy" && options[idField] > 0) {
                            let model = {
                                ...options,
                                IsDeleted: 1
                            }
                            return JSON.stringify(model);
                        }
                        return JSON.stringify(options);



                    }
                },
                type: "json",
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
                allowUnsort: true,
                pageSize: 100,
                schema: {
                    data: "Items", total: "TotalCount",
                    model: schema,

                },
                requestEnd: function (e) {
                    if (e.type === "create" || e.type === "update" || e.type === "destroy") {
                        var resp = e.response || {};
                        if (resp.IsLogin === "1" && resp.redirectUrl) { window.location.href = resp.redirectUrl; return; }
                        if (resp.ResultId && parseInt(resp.ResultId) > 0) {
                            if (resp.Message) MessageHelper.Success("Success", resp.Message);
                            e.sender.read();
                        } else if (resp.ResultId !== undefined) {
                            MessageHelper.Warning("Operation", resp.Message || "Failed.");
                        }
                    }
                },
                error: function (e) {
                    var msg = "Unexpected error.";
                    try {
                        if (e.xhr && e.xhr.responseText) {
                            var j = JSON.parse(e.xhr.responseText);
                            msg = j?.Message || j?.Title || e.xhr.statusText || msg;
                        } else if (e.errors) {
                            msg = Object.values(e.errors).join("\n");
                        }
                    } catch (_) { msg = e.xhr?.responseText || msg; }
                    MessageHelper.Warning("Grid Error", msg);
                }
            });
        }
        //Grid 
        var cols = columns.slice();


        $("#" + ctrlId).kendoGrid({
            dataSource: ds,
            pageable: true,
            height: height,
            scrollable: true,
            selectable: "row",
            toolbar: [{ name: "create", text: "Add New" }],
            columns: cols,
            editable: { mode: "inline", confirmation: "Delete this record?" }
        });


    },

    ChildGridLoad_PopUp: function (element, apiUrl, columns, height = 400) {

        let grid = $(element).kendoGrid({
            dataSource: {
                serverFiltering: true,
                transport: {
                    read: {
                        url: apiUrl,
                        type: "POST",
                        contentType: "application/json",
                        dataType: "json"
                    },
                    parameterMap: function () {
                        const searchKey = $(element).data("kendoGrid").wrapper.find(".k-grid-search input").val() || "";
                        const payload = {
                            searchKey: searchKey,
                            pageIndex: (searchKey && pageIndex_PopUp > 1) ? 1 : pageIndex_PopUp,
                            pageSize: pageSize_PopUp,
                        };
                        return JSON.stringify(payload);
                    }
                },
                schema: {
                    data: function (response) {
                        return response;
                    },
                    total: function (response) {
                        return response.length;
                    }
                }
            },
            toolbar: ["search"],
            noRecords: {
                template: "NO DATA FOUND"
            },
            columns: columns,
            pageable: false,
            sortable: false,
            filterable: false,
            editable: false,
            selectable: "row",
            scrollable: true,
            navigatable: true,
            height: height,
            //dataBound: function () {
            //    // ✅ auto-focus first cell
            //    this.table.find("tr:first").addClass("k-state-selected");
            //    this.current(this.table.find("tr:first td:first"));
            //    this.focus();
            //}
        }).data("kendoGrid");
        grid.table.on("keydown", function (e) {
            let selected = grid.select();
            let rows = grid.tbody.find("tr");
            let currentIndex = selected.index();

            if (e.key === "ArrowDown") {
                e.preventDefault();
                if (currentIndex < rows.length - 1) {
                    grid.select(rows.eq(currentIndex + 1));
                }
            }

            if (e.key === "ArrowUp") {
                e.preventDefault();
                if (currentIndex > 0) {
                    grid.select(rows.eq(currentIndex - 1));
                }
            }
        });
        $(element).off("keydown").on("keydown", function (e) {
            let grid = $(element).data("kendoGrid");
            let selected = grid.select();
            let allRows = grid.tbody.find("tr");
            let currentIndex = selected.index();

            if (e.repeat) return;
            if (allRows.length == 1 && currentIndex == 0) {
                currentIndex = -1;
            }

            // Handle ArrowDown key
            if (e.key === "ArrowDown") {
                e.preventDefault();

                // Deselect the current row
                selected.removeClass("k-state-selected");

                let nextIndex = currentIndex + 1;

                // Make sure the next row exists
                if (nextIndex < allRows.length) {
                    let nextRow = allRows.eq(nextIndex);
                    grid.select(nextRow); // Select the next row
                }
            }

            // Handle ArrowUp key
            if (e.key === "ArrowUp") {
                e.preventDefault();

                // Deselect the current row
                selected.removeClass("k-state-selected");

                let prevIndex = currentIndex - 1;

                // Make sure the previous row exists
                if (prevIndex >= 0) {
                    let prevRow = allRows.eq(prevIndex);
                    grid.select(prevRow); // Select the previous row
                }
            }
        });

    },
    ChildGridLoad_PopUp_Focus: function (element) {
        setTimeout(function () {
            var searchInput = $(element).data("kendoGrid").wrapper.find(".k-grid-search input");
            if (searchInput.length > 0) {
                searchInput.focus();
            }
        }, 500);
    },
    ExpandKendoSubGrid: function (gridId) {
        var childGrid = $(gridId).data("kendoGrid");
        var data = childGrid.dataSource.data();
        var len = data.length;
        for (var i = 0; i < len; i++) {
            var row = data[i];
            childGrid.expandRow("tr[data-uid='" + row.uid + "']");
        }
    },

    EnterKeyUp: function (element) {
        var grid = $(element).data("kendoGrid");
        // Remove existing event to prevent duplicates
        $(element + ' tbody').off('keydown');

        // Add new keydown event
        $(element + ' tbody').on('keydown', function (e) {

            if (e.keyCode == 13) {
                e.preventDefault();
                e.stopPropagation();



                var currentCell = $(e.target).closest('td');
                var currentRow = currentCell.closest('tr');
                if (!currentCell.length || !currentRow.length) return;

                if (!GridNavigationHelper.CommitCurrentCell(grid, currentCell)) {
                    e.preventDefault(); e.stopPropagation();
                    return;
                }

                var inputElement = currentCell.find('input, .k-input-inner, .k-input'); // combo/text both
                if (grid.editable && grid.editable.end && grid.editable.validatable) {
                    var isOk = grid.editable.end();
                    if (!isOk) {
                        // keep focus on the editor
                        if (inputElement.length) {
                            $(inputElement).focus().select();
                        }
                        return;
                    }
                } else {
                    var raw = currentCell.find('input[required]');
                    if (raw.length && !$.trim(raw.val())) {
                        raw.addClass('k-invalid');
                        raw.focus().select();
                        return;
                    }
                }

                var dataItem = grid.dataItem(currentRow);
                var plainInput = currentCell.find('input[name]');
                if (plainInput.length && dataItem) {
                    var fieldName = plainInput.attr('name');   // field name == model field
                    var value = plainInput.val();
                    if (fieldName) dataItem.set(fieldName, value);
                }
                var moveForward = !e.shiftKey; // Shift+Enter = backward


                function getNextEditableCell(startCell, forward) {
                    var selector = 'td:not(:hidden):not(.non-editable-column)';
                    return forward
                        ? startCell.nextAll(selector).first()
                        : startCell.prevAll(selector).first();
                }


                var nextCell = getNextEditableCell(currentCell, moveForward);
                if (nextCell.length) {
                    grid.editCell(nextCell);
                    nextCell.focus().select();
                } else {
                    if (moveForward) {
                        var nextRow = currentRow.next('tr');
                        if (nextRow.length) {
                            nextCell = nextRow.find('td:not(:hidden):not(.non-editable-column)').first();
                            if (nextCell.length) {
                                grid.editCell(nextCell);
                                nextCell.focus().select();
                            };
                        } else {
                            grid.addRow();

                            var $row = grid.select();
                            if (!$row || !$row.length) $row = grid.tbody.children('tr').last();
                            KendoGridHelper.FocusAfterCellCheck(grid, $row);

                            //// old code input focus after new row add
                            //var $inp = grid.wrapper.find(":input:visible:enabled:not(:checkbox)").first();
                            //$inp.focus();
                            //if ($inp.is('input[type=text], textarea, .k-input-inner')) {
                            //    $inp.select();
                            //}

                        }
                    } else {
                        var prevRow = currentRow.prev('tr');
                        if (prevRow.length) {
                            var prevCells = prevRow.find('td:not(:hidden):not(.non-editable-column)');
                            if (prevCells.length) grid.editCell(prevCells.last());
                        }
                    }
                }
            }

        });


    },

    KeyUpDown: function (gridSelector, fieldNames) {
     
        const opt = Object.assign({ skipGroupDetail: true, tryFallbackSameRow: true }, {});
        const $grid = $(gridSelector);
        const grid = $grid.data("kendoGrid");
        if (!grid) return;

        // Normalize to Set for O(1) lookup
        const fieldSet = Array.isArray(fieldNames) ? new Set(fieldNames) :
            (fieldNames instanceof Set ? fieldNames : new Set([String(fieldNames)]));

        // পুরনো হ্যান্ডলার আনবাইন্ড
        $grid.find("tbody").off("keydown.vertNavLockMulti");

        // নতুন হ্যান্ডলার
        $grid.find("tbody").on("keydown.vertNavLockMulti", "td", function (e) {
            if (e.keyCode !== 38 && e.keyCode !== 40) return; // Up/Down only

            const $cell = $(e.target).closest("td");
            const field = GridNavigationHelper.GetCellField(grid, $cell);
            if (!field || !fieldSet.has(field)) return; // তালিকায় না থাকলে ignore

            const $row = $cell.closest("tr");

            // আগে কারেন্ট সেল commit/validate
            if (!GridNavigationHelper.CommitCurrentCell(grid, $cell)) {
                e.preventDefault(); e.stopPropagation();
                return;
            }

            e.preventDefault(); e.stopPropagation();

            // পরের/আগের row বের করুন (group/detail row স্কিপ করে)
            let $nextRow = (e.keyCode === 40) ? $row.next("tr") : $row.prev("tr");
            while (opt.skipGroupDetail && $nextRow.length &&
                ($nextRow.hasClass("k-grouping-row") || $nextRow.hasClass("k-detail-row"))) {
                $nextRow = (e.keyCode === 40) ? $nextRow.next("tr") : $nextRow.prev("tr");
            }
            if (!$nextRow.length) return;

            // একই field-এর সেল নিন
            let $nextCell = GridNavigationHelper.FindCellByField(grid, $nextRow, field);

            // যদি ঐ সেল editable না হয়, আর fallback চালু থাকে—তাহলে একই row-এ fieldSet থেকে প্রথম editable সেল ট্রাই করুন
            if ($nextCell.length) {
                const dataItem = grid.dataItem($nextRow);
                if (!GridNavigationHelper.IsEditableCell(grid, $nextCell, dataItem) && opt.tryFallbackSameRow) {
                    // fieldSet অর্ডার মেইনটেইন করতে Array ব্যবহার করুন
                    const fieldOrder = Array.from(fieldSet);
                    for (const f of fieldOrder) {
                        const $alt = GridNavigationHelper.FindCellByField(grid, $nextRow, f);
                        if ($alt.length && GridNavigationHelper.IsEditableCell(grid, $alt, dataItem)) {
                            $nextCell = $alt;
                            break;
                        }
                    }
                }
            }

            if ($nextCell && $nextCell.length) {
                grid.editCell($nextCell);
            }
        });




    },

    // ---------helper----------
    FocusAfterCellCheck: function (grid, $row) {
        var $cells = $row.children('td:visible')
            .not('.non-editable-column,.k-group-cell,.k-hierarchy-cell,.k-detail-cell');

        var tryOrder = [0, 1, 2, 3, 4, 5, 6]; // প্রথমে 1st, না পেলে 2nd, 3rd, 4th, 5th and 6th

        (function recursiveIndex(k) {
            if (k >= tryOrder.length) return;

            var idx = tryOrder[k];
            var $cell = $cells.eq(idx);
            if (!$cell.length) { recursiveIndex(k + 1); return; }

            // editor রেন্ডার হলে ফোকাস
            grid.one("edit", function (ev) {
                var $inp = $(ev.container)
                    .find(':input:visible:enabled')
                    .not(':checkbox, .k-switch-input')
                    .first();

                if ($inp.length) {
                    $inp.focus();
                    if ($inp.is('input[type=text], textarea, .k-input-inner')) $inp.select();
                } else {
                    recursiveIndex(k + 1);
                }
            });
            grid.editCell($cell);
        })(0);
    },
    EnterKeyUp_Prev: function (element) {
        var grid = $(element).data("kendoGrid");
        // Remove existing event to prevent duplicates
        $(element + ' tbody').off('keydown');
        // Add new keydown event
        $(element + ' tbody').on('keydown', function (e) {
            if (e.keyCode !== 13) return; // Only process Enter key
            e.preventDefault();
            e.stopPropagation();
            var currentCell = $(e.target).closest('td');
            var currentRow = currentCell.closest('tr');
            if (!currentCell.length || !currentRow.length) return;
            var dataItem = grid.dataItem(currentRow);
            var inputElement = currentCell.find('input');
            if (inputElement.length) {
                var inputId = inputElement.attr('id');
                var inputValue = inputElement.val();
                if (inputId) {
                    dataItem.set(inputId, inputValue);
                }
            }
            var moveForward = !e.shiftKey; // Shift + Enter = move backward
            function getNextEditableCell(startCell, forward) {
                var cell = forward ? startCell.nextAll('td:not(:hidden):not(.non-editable-column)').first()
                    : startCell.prevAll('td:not(:hidden):not(.non-editable-column)').first();
                return cell;
            }
            var nextCell = getNextEditableCell(currentCell, moveForward);
            if (nextCell.length > 0) {
                grid.editCell(nextCell);
            } else {
                if (moveForward) {

                    // Move to first cell of next row or add new row
                    var nextRow = currentRow.next('tr');
                    if (nextRow.length > 0) {
                        nextCell = nextRow.find('td:not(:hidden):not(.non-editable-column)').first();
                        if (nextCell.length) {
                            grid.editCell(nextCell);
                        }
                    } else {
                        // Add new row
                        grid.addRow();

                        var $inp = grid.find(":input:visible:enabled").first();
                        if ($inp.length) {
                            $inp.focus().select();                       // plain <input>
                        }
                    }
                } else {
                    // Move to last cell of previous row
                    var prevRow = currentRow.prev('tr');
                    if (prevRow.length > 0) {
                        var prevCells = prevRow.find('td:not(:hidden):not(.non-editable-column)');
                        if (prevCells.length) {
                            grid.editCell(prevCells.last());
                        }
                    }
                }
            }
        });
    },
    EnterKeyUp2: function (element) {
        var $gridEl = $(element);
        var grid = $gridEl.data("kendoGrid");
        if (!grid) return;

        // avoid duplicate binding
        grid.tbody.off(".enterNav");
        grid.tbody.on("keydown.enterNav", "td", function (e) {
            if (e.key !== "Enter" && e.keyCode !== 13) return;

            // if an editor dropdown is open, let Enter select item (don't move)
            var $cell = $(e.currentTarget);
            var openWidget = KendoGridHelper.GetOpenWidget($cell);
            if (openWidget && openWidget.popup && openWidget.popup.visible && openWidget.popup.visible()) {
                return; // let the widget handle Enter
            }

            e.preventDefault();
            e.stopPropagation();

            var $row = $cell.closest("tr");
            if (!$row.length) return;

            var inputElement = $cell.find('input, .k-input-inner, .k-input'); // combo/text both
            if (grid.editable && grid.editable.end && grid.editable.validatable) {
                var isOk = grid.editable.end();
                if (!isOk) {
                    // keep focus on the editor
                    if (inputElement.length) {
                        $(inputElement).focus().select();
                    }
                    return;
                }
            } else {
                var raw = $cell.find('input[required]');
                if (raw.length && !$.trim(raw.val())) {
                    raw.addClass('k-invalid');
                    raw.focus().select();
                    return;
                }
            }

            // Commit current cell edit (if any)
            try { grid.closeCell($cell); } catch (_) { }

            var forward = !e.shiftKey;

            // Try next/prev editable cell within the same row
            var $nextCell = KendoGridHelper.GetSiblingEditableCell(grid, $row, $cell, forward);

            if ($nextCell && $nextCell.length) {
                KendoGridHelper.EditAndFocus($nextCell, grid);
                return;
            }

            if (forward) {
                // move to first editable cell of next row, or add new row
                var $nextRow = $row.next("tr");
                if ($nextRow.length) {
                    var $first = KendoGridHelper.GetFirstEditableCell(grid, $nextRow);
                    if ($first.length) {
                        KendoGridHelper.EditAndFocus($first, grid);
                        return;
                    }
                }

                // Add new row and focus its first editable cell (ComboBox-safe)
                grid.addRow();

                // wait for DOM to render
                setTimeout(function () {
                    var $rows = grid.tbody.find("tr");
                    var $newRow = $rows.last();
                    if (!$newRow.length) return;

                    grid.select($newRow);
                    var $first = KendoGridHelper.GetFirstEditableCell(grid, $newRow);
                    if ($first.length) {
                        KendoGridHelper.EditAndFocus($first, grid, /*preferWidgetFocus*/ true);
                    }
                }, 50);
            } else {
                // Shift+Enter: last editable cell of previous row
                var $prevRow = $row.prev("tr");
                if ($prevRow.length) {
                    var $last = KendoGridHelper.GetLastEditableCell(grid, $prevRow);
                    if ($last.length) {
                        KendoGridHelper.EditAndFocus($last, grid);
                    }
                }
            }
        });
    },
    // ---------- enter key helpers start ----------
    GetFirstEditableCell: function (grid, $tr) {
        var $cells = $tr.children("td");
        for (var i = 0; i < $cells.length; i++) {
            var $td = $($cells[i]);
            if (KendoGridHelper.IsEditableCell(grid, $td)) return $td;
        }
        return $();
    },
    GetLastEditableCell: function (grid, $tr) {
        var $cells = $tr.children("td");
        for (var i = $cells.length - 1; i >= 0; i--) {
            var $td = $($cells[i]);
            if (KendoGridHelper.IsEditableCell(grid, $td)) return $td;
        }
        return $();
    },
    GetSiblingEditableCell: function (grid, $tr, $fromCell, forward) {
        var $cells = $tr.children("td");
        var start = $fromCell[0].cellIndex;
        if (forward) {
            for (var i = start + 1; i < $cells.length; i++) {
                var $td = $($cells[i]);
                if (KendoGridHelper.IsEditableCell(grid, $td)) return $td;
            }
        } else {
            for (var j = start - 1; j >= 0; j--) {
                var $td = $($cells[j]);
                if (KendoGridHelper.IsEditableCell(grid, $td)) return $td;
            }
        }
        return $();
    },
    IsEditableCell: function (grid, $td) {
        // skip hidden, command/non-editable columns
        if (!$td.is(":visible")) return false;
        if ($td.hasClass("non-editable-column")) return false;

        // if grid has columns config, respect editable=false/command columns
        var colIndex = $td[0].cellIndex;
        var col = grid.columns[colIndex];
        if (!col) return true;
        if (col.command) return false;
        if (col.editable === false) return false;
        return true;
    },
    EditAndFocus: function ($cell, grid, preferWidgetFocus) {
        grid.editCell($cell);

        // small delay so the editor template renders
        setTimeout(function () {
            // Prefer focusing Kendo widget if present (e.g., ComboBox in first cell)
            var widget = KendoGridHelper.FindCellWidget($cell);
            if (widget && widget.focus && (preferWidgetFocus || !KendoGridHelper.FindStandardInput($cell).length)) {
                try { widget.focus().select(); return; } catch (_) { }
            }

            var $input = KendoGridHelper.FindStandardInput($cell);
            if ($input.length) {
                $input.focus().select();
                // place caret at end for text input
                try {
                    var el = $input[0];
                    if (el.setSelectionRange && (el.type === "text" || el.type === "search" || el.type === "tel" || el.type === "number")) {
                        var len = el.value ? el.value.length : 0;
                        el.setSelectionRange(len, len);
                    }
                } catch (_) { }
            }
        }, 30);
    },
    FindStandardInput: function ($cell) {
        // Prefer visible focusable input/select/textarea
        return $cell.find('input:not([type="hidden"]), select, textarea')
            .filter(":visible")
            .first();
    },
    GetOpenWidget: function ($cell) {
        var widget = KendoGridHelper.FindCellWidget($cell);
        if (!widget) return null;
        if (typeof widget.popup !== "undefined") return widget;
        if (typeof widget._popup !== "undefined") return widget._popup; // some widgets use _popup
        return null;
    },
    FindCellWidget: function ($cell) {
        // common widgets inside in-cell editors
        var $w = $cell.find(".k-widget,.k-input-inner,.k-combobox,.k-datepicker,.k-numerictextbox").first();
        if (!$w.length) return null;

        // Try known widget getters in order
        return $w.data("kendoComboBox") ||
            $w.data("kendoDropDownList") ||
            $w.data("kendoDatePicker") ||
            $w.data("kendoDateTimePicker") ||
            $w.data("kendoNumericTextBox") ||
            $w.data("kendoAutoComplete") ||
            $w.data("kendoMultiColumnComboBox") ||
            $w.data("kendoMultiSelect") || null;
    },
    // ---------- enter key helpers end ----------

    EnterKey: function (element, firstCellIndex, lastCellIndex) {
        var grid = $(element).data().kendoGrid;
        grid.table.focus();

        $(element + ' tbody').on('keydown', function (e) {
            if (e.keyCode == 13) {
                e.stopPropagation();
                e.preventDefault();
                var currentCell = $(e.target).closest('.enterkeyup');
                var currentCellIndex = currentCell.index();
                var cellToEdit;
                var targetedValue = $(e.target).closest('input').val();
                var field = $(this).children('tr').find('input')[0].name;
                grid.select($(e.target).closest('tr'));
                var rows = grid.dataItem(grid.select());

                rows[field] = targetedValue;
                var nextCellIndex = -1;
                for (var i = currentCellIndex + 1; i <= lastCellIndex; i++) {
                    var nextCell = $(e.target).closest('tr').find('td:eq(' + i + ')');
                    if (nextCell[0].className == "enterkeyup") {
                        nextCellIndex = i;
                        break;
                    }
                }

                if (nextCellIndex != -1) {
                    cellToEdit = currentCell.parent().find('td:eq(' + nextCellIndex + ')');
                }
                else {
                    cellToEdit = currentCell.parent().next().find('td:eq(' + firstCellIndex + ')');
                }
                grid.editCell(cellToEdit);
            }
        })
    },

    EnterKeyUpByGroup: function (element, filedName1 = "", groupColumns = "") {
        var grid = $(element).data().kendoGrid;
        grid.table.focus();

        $(element + ' tbody').on('keydown', function (e) {
            var currentCell = $(e.target).closest('td');
            var currentRow = currentCell.closest('tr');
            var uid = currentRow.data('uid');

            let gridDataTota = 0;
            var cellToEdit;
            var index = grid.tbody.find("tr[data-uid='" + uid + "']").index();
            var currentCellIndex = currentCell[0].cellIndex;


            if (index === -1) {
                return;  // Current row not found
            }

            gridDataTota = grid.dataSource.total();
            if (groupColumns != "") {
                let groupList = ArrayHelper.DistinctArray(grid.dataSource.data(), groupColumns);
                gridDataTota += groupList.length;
            }

            if (index < 0 || index >= gridDataTota || currentCellIndex < 0 || currentCellIndex >= (grid.columns.length + 1)) {
                return;  // No rows or columns to navigate to
            }

            if (e.keyCode == 13) {
                e.stopPropagation();
                e.preventDefault();
                cellToEdit = currentCell.parent().next().find('td:eq(' + currentCellIndex + ')');
                grid.editCell(cellToEdit);
                grid.select("tr:eq(" + (index + 1) + ")");
            }
            else if (e.keyCode == 40) {
                e.preventDefault();
                currentRow.find('input, select, textarea').trigger('change');
                cellToEdit = currentCell.parent().next().find('td:eq(' + currentCellIndex + ')');
                grid.editCell(cellToEdit);
                grid.select("tr:eq(" + (index + 1) + ")");
            }
            else if (e.keyCode == 38) {
                e.preventDefault();
                currentRow.find('input, select, textarea').trigger('change');
                cellToEdit = currentCell.parent().prev().find('td:eq(' + currentCellIndex + ')');
                grid.editCell(cellToEdit);
                grid.select("tr:eq(" + (index - 1) + ")");
            }
            else {
                return;
            }
        });
    },
    EnterKeyUpWithSave_NotNewRow: function (element) {
        var grid = $(element).data().kendoGrid;
        $(element + ' tbody').off('keydown');  // Unbind the keydown event
        grid.table.focus();


        $(element + ' tbody').on('keydown', function (e) {
            var currentCell = $(e.target).closest('td');
            var currentRow = currentCell.closest('tr');
            var cellToEdit;

            grid.select(currentRow);
            var rows = grid.dataItem(currentRow);
            var index = grid.dataSource.indexOf(rows);
            var currentCellIndex = currentCell[0].cellIndex;

            var uid = currentRow.data('uid');
            var rowEdited = grid.tbody.find("tr[data-uid='" + uid + "']").index();


            if (rowEdited === -1) {
                return;  // Current row not found
            }

            if (index < 0 || index >= grid.dataSource.total() /*|| currentCellIndex < 0 || currentCellIndex >= grid.columns.length*/) {
                return;  // No rows or columns to navigate to
            }

            if (e.keyCode == 13) {  // Enter key
                e.stopPropagation();
                e.preventDefault();

                //grid.saveChanges(); 
                var currentCellProperty = currentCell[0].childNodes[0];

                // Find the input element within the current cell
                var inputElement = currentCell.find('input');
                if (inputElement.length) {
                    var inputId = inputElement.attr('id');  // Get the input ID
                    var inputValue = inputElement.val();    // Get the input value

                    // Here you can use inputId and inputValue as needed
                    var dataItem = grid.dataItem(currentRow);
                    if (inputId != null && inputId != "" && inputId != undefined) {
                        dataItem.set(inputId, inputValue);
                    }
                    else {
                        $("#" + currentCell.id).val(inputValue);
                    }
                }

                // Move to the next cell in the current row
                cellToEdit = currentCell.next();
                while (cellToEdit.length && (cellToEdit.is(':hidden') || cellToEdit.hasClass("non-editable-column"))) {
                    cellToEdit = cellToEdit.next();
                }

                if (cellToEdit.length === 0) {
                    // If the current cell is the last cell in the row, move to the first cell of the next row
                    var nextRow = currentRow.next();

                    if (nextRow.length > 0) {
                        cellToEdit = nextRow.find('td').first();
                        while (cellToEdit.length && (cellToEdit.is(':hidden') || cellToEdit.hasClass("non-editable-column"))) {
                            cellToEdit = cellToEdit.next();  // Skip hidden cells
                        }
                    }
                    else {
                        isNewRowCreated = true;
                        if (isExportPage) {
                            var isReject = 0, isAudit = 0;
                            Manager.Save(isReject, isAudit);
                        }
                        //grid.addRow();
                    }
                }

                if (cellToEdit.length > 0) {
                    grid.editCell(cellToEdit);

                    // focus + select the value inside the new editor
                    setTimeout(function () {
                        // first visible, enabled input inside the cell
                        var $inp = cellToEdit.find(":input:visible:enabled").first();
                        if ($inp.length) {
                            $inp.focus().select();                       // plain <input>
                        }
                        //// detect common Kendo editors
                        //var widget =
                        //    $inp.data("kendoNumericTextBox") ||
                        //    $inp.data("kendoDatePicker") ||
                        //    $inp.data("kendoDateTimePicker") ||
                        //    $inp.data("kendoComboBox") ||
                        //    $inp.data("kendoDropDownList") ||
                        //    $inp.data("kendoMaskedTextBox") ||
                        //    $inp.data("kendoTextBox") ||
                        //    null;

                        //if (widget) {
                        //    if (widget.focus) widget.focus();            // put caret in editor
                        //    if (widget.input && widget.input.length) {
                        //        widget.input[0].select();                // select all text for overwrite
                        //    }
                        //} else if ($inp.length) {
                        //    $inp.focus().select();                       // plain <input>
                        //}
                    }, 0);
                }
            }
        });
    },

    EnterKeyUp_UpDown: function (element, columnName) {
        var grid = $(element).data().kendoGrid;
        grid.table.focus();

        grid.tbody.off("keydown").on("keydown", "td", function (e) {
            var key = e.keyCode || e.which;
            var $cell = $(this);
            var colName = $cell.data("field");   // 👈 column name বের হলো

            if ((key === 40 || key === 38) && colName === columnName) {
                debugger
                // 👆 শুধু Qty কলামে থাকলেই কাজ করবে
                e.preventDefault();

                var currentRow = $cell.closest("tr");
                var nextRow = (key === 40) ? currentRow.next() : currentRow.prev();

                if (nextRow.length) {
                    // একই column index-এর cell নিন
                    var cellIndex = $cell.index();
                    var targetCell = nextRow.children().eq(cellIndex);

                    grid.current(targetCell);   // focus ওই cell-এ
                    grid.select(nextRow);       // চাইলে পুরো row select
                }
            }
        });

    },

    MultipleServerDataSource: function (apiUrl, schemaModel) {
        let schema = {
            data: 'Items',
            total: 'TotalCount'
        }
        if (schemaModel) {
            schema = {
                ...schema,
                model: schemaModel
            }
        }
        var virtualDatasource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 80,
            transport: {
                read: {
                    type: "POST",
                    url: apiUrl,
                    dataType: "json",
                    contentType: 'application/json',
                },
                parameterMap: function (options) {
                    return JSON.stringify(options);
                }
            },
            schema: schema,
        });

        return virtualDatasource;
    },
    ComboServerDataSource: function (apiUrl) {
        var virtualDatasource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 80,
            transport: {
                read: {
                    type: "POST",
                    url: apiUrl,
                    dataType: "json",
                    contentType: 'application/json',
                },
                parameterMap: function (options) {
                    return JSON.stringify(options);
                }
            },
            schema: {
                parse: (response) => {
                    var results = response.Items;
                    var items = [];
                    for (var i = 0; i < results.length; i++) {
                        items.push(
                            {
                                id: results[i].id,
                                text: results[i].text,
                            });
                    }
                    return items;
                },
            }
            //schema: schema,
        });

        return virtualDatasource;
    },
    //Always Expand Kendo Grid Nested Child
    ExpandSubGrid: function (gridId) {
        var childGrid = $(gridId).data("kendoGrid");
        var data = childGrid.dataSource.data();
        var len = data.length;
        for (var i = 0; i < len; i++) {
            var row = data[i];
            childGrid.expandRow("tr[data-uid='" + row.uid + "']");
        }
    },

    //Kendo Grid Button Tool tip display
    GridButtonTooltip: function (gridId, filedName, tooltiptext) {
        $(gridId).kendoTooltip({
            filter: ".k-grid-" + filedName,
            content: function (e) {
                return tooltiptext;
            }
        });
    },
    NumberTextBox: function (container, options) {
        var $input = $('<input class="k-input-cell" data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoNumericTextBox({
                spinners: false,
                min: 0,
                decimals: 18,
            });

    },
    NumberTextBox_Required: function (container, options) {
        var $input = $('<input class="k-input-cell" required data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoNumericTextBox({
                spinners: false,
                min: 0,
                decimals: 18,
            });
    },
    NumberTextBox4Digit: function (container, options) {
        $('<input data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoNumericTextBox({
                decimals: 4,
                restrictDecimals: true,
                round: false,
            });
    },
    NumberTextBox2Digit: function (container, options) {
        $('<input data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoNumericTextBox({
                decimals: 2,
                restrictDecimals: true,
                round: false,
            });
    },
    NumberEditor: function (container, options) {
        $('<input type="number" class="k-input k-textbox" data-bind="value:' + options.field + '" />')
            .appendTo(container)
            .attr("min", 0);
    },
    TextBox: function (container, options) {
        $('<input data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoTextBox({

            });
    },
    IsValidItem: function (ctrlId) {
        var cmbBox = $("#" + ctrlId).data("kendoComboBox");
        if (cmbBox && cmbBox.value() != "" && cmbBox.value() == cmbBox.text()) {
            cmbBox.value("");
            cmbBox.text("");
            cmbBox.select(-1);
            cmbBox.refresh();
            cmbBox.focus();
            ToastrMessageHelper.ValidationToastr("Input item is invalid")
            return false;
        } else {
            return true;
        }
    },
    RemoveUndefined: function (e) {
        if (chnageLocked == 0) {
            var combo = e.sender;
            if (!combo.dataItem()) {
                chnageLocked = 1;
                combo.refresh();
                combo.trigger("change");
            }
        } else {
            chnageLocked = 0;
        }
    },
    RemoveUndefined_V2: function (e, options) {
        var comboBox = e.sender;
        var dataItem = comboBox.dataItem();
        if (!dataItem || dataItem.id === undefined || dataItem.id === null) {
            comboBox.value('');
            comboBox.text('');
            options.model.set(options.field, null);
        }
    },
    RemoveUndefined_VT: function (cmbBox) {
        if (cmbBox && cmbBox.value() != "" && cmbBox.value() == cmbBox.text()) {
            cmbBox.value("");
            cmbBox.text("");
            cmbBox.select(-1);
            cmbBox.refresh();
            cmbBox.focus();

            if (!cmbBox.dataItem()) {
                chnageLocked = 1;
                cmbBox.refresh();
                cmbBox.trigger("change");
            }
            ToastrMessageHelper.ValidationToastr("Input item is invalid");
            return false;
        } else {
            return true;
        }
    },
    IsValidItemMultiCombo: function (ctrlId) {
        var cmbBox = $("#" + ctrlId).data("kendoMultiColumnComboBox");
        if (cmbBox.value() != "" && cmbBox.value() == cmbBox.text()) {
            cmbBox.value("");
            cmbBox.text("");
            cmbBox.select(-1);
            cmbBox.refresh();
            cmbBox.focus();
            ToastrMessageHelper.ValidationToastr("Input item is invalid")
            return false;
        } else {
            return true;
        }
    },
    GridRowSelected: function (gridId) {
        $(gridId).on("click", "tr", function (e) {
            var rowElement = this;
            var row = $(rowElement);
            var grid = $(gridId).getKendoGrid();
            grid.select(row)
            e.stopPropagation();
        });
    },
    GridRemoveUndefined: function (e) {
        if (chnageLocked == 0) {
            var combo = e.sender;
            if (!combo.dataItem()) {
                var dataSource = combo.dataSource;
                //dataSource.add({
                //    Value: 0,
                //    Name: ""
                //});
                chnageLocked = 1;
                // combo.select(dataSource.view().length - 1);
                combo.refresh();
                combo.trigger("change");

            }
        } else {
            chnageLocked = 0;
        }
    },
    RemoveUndefined_Final: function (objList, dataItem, options) {
        if (dataItem) {
            options.model.set(objList, dataItem);
        } else {
            options.model.set(objList, null);
            ToastrMessageHelper.FxToastr("Input Item is invalid");
        }
    },
    CheckdGrid: function (data, gbSelectedChkItems) {
        if (gbSelectedChkItems.length > 0) {
            var result = gbSelectedChkItems.filter(function (obj) {
                return obj.CHKID == data.CHKID;//CHKID=For List Id, CHKSAVEDID=For Saved Table Id
            });
            if (result.length > 0) {
                return '<input id="check_row" class="check_row" type="checkbox" checked="checked"/>';
            }
            else {
                if (data.CHKSAVEDID > 0) {//CHKID=For List Id, CHKSAVEDID=For Saved Table Id
                    gbSelectedChkItems.push(data);
                    return '<input id="check_row" class="check_row" type="checkbox" checked="checked"/>';
                } else {
                    return '<input id="check_row" class="check_row" type="checkbox"/>';
                }
            }
        }
        else {
            if (data.CHKSAVEDID > 0) {//CHKID=For List Id, CHKSAVEDID=For Saved Table Id
                gbSelectedChkItems.push(data);
                return '<input id="check_row" class="check_row" type="checkbox" checked="checked"/>';
            } else {
                return '<input id="check_row" class="check_row" type="checkbox"/>';
            }
        }
    },
    VirtualizationDataSource: function (apiUrl, schemaModel) {
        let schema = {
            data: 'Items',
            total: 'TotalCount'
        }
        if (schemaModel) {
            schema = {
                ...schema,
                model: schemaModel
            }
        }

        var virtualDatasource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 80,
            transport: {
                read: {
                    type: "POST",
                    url: apiUrl,
                    dataType: "json",
                    contentType: 'application/json',
                },
                parameterMap: function (options) {
                    return JSON.stringify(options);
                }
            },
            schema: schema,
        });

        return virtualDatasource;
    },
    //Get Grid Checkbox checked value
    //CHKID=For List Id (Primary Key), CHKSAVEDID=For Saved Table Id (id from saved table)
    CheckboxItems: function (GridID, gbSelectedChkItems) {//getCheckboxItems

        $(GridID).on("click", ".check_row", function (e) {
            var $cb = $(this);
            var grid = $(GridID).data("kendoGrid");
            var tr = $(e.currentTarget).closest("tr");
            var dataItem = grid.dataItem($(this).closest('tr'));
            var selectedItem = grid.dataItem(tr);
            grid.select(tr);
            if ($cb.is(":checked")) {
                if (selectedItem != null) {
                    gbSelectedChkItems.push(selectedItem);
                }
            } else {
                for (var j = 0; j < gbSelectedChkItems.length; j++) {
                    if (gbSelectedChkItems[j].CHKID == dataItem.CHKID) {//CHKID=For List Id
                        gbSelectedChkItems.splice(j, 1);
                        break;
                    }
                }
            }
        });//Indivisual row selection

        $(GridID).on('change', '#checkAll' + GridID.slice(1) + '', function (e) {
            gbSelectedChkItems = [];
            var gridSummary = $(GridID).data("kendoGrid");

            var selectAll = document.getElementById("checkAll" + GridID.slice(1) + "");
            if (selectAll.checked == true) {
                $(GridID + " tbody input:checkbox").prop("checked", this.checked);
                $(GridID + "  table tr").addClass('k-state-selected');
                var gridData = gridSummary.dataSource.data();
                for (var i = 0; i < gridData.length; i++) {
                    var obj = gridData[i];
                    gbSelectedChkItems.push(obj);
                }
            }
            else {
                $(GridID + "  tbody input:checkbox").prop("checked", false);
                $(GridID + "  table tr").removeClass('k-state-selected');
                gbSelectedChkItems = [];
            }

        });// All Row Selection
        //return gbSelectedChkItems;
    },

    getRowSLNo(element) {

        var tblChilds = $(element).data("kendoGrid");
        var tblData = tblChilds.dataSource.data();
        for (var i = 0; i < tblData.length; i++) {
            gbRowSLNo = i + 1;
            tblData[i].set("RowSLNo", gbRowSLNo);
        }
    },

    GridCenterAlignAttributes: function (align_header = "", align_body = "", align_footer = "", isInputCell = false, isNonEditable = false, isbold = false) {
        var kInputCell = "";
        var kStyle = "";
        if (isInputCell) {
            kInputCell = " k_input_cell";
        }
        var nonEditable = "";
        if (isNonEditable) {
            nonEditable = " non-editable-column";
        }
        if (isbold) {
            kStyle = "font-weight: 700;";
        }


        return {
            headerAttributes: { "class": align_header + kInputCell + nonEditable + " k-vertical-align" },
            attributes: { "class": align_body + kInputCell + nonEditable, style: kStyle },
            footerAttributes: { "class": align_footer + kInputCell + nonEditable, style: kStyle }
        };
    },

    GridCustomDateEditor: function (container, options) {
        $('<input data-bind="value:' + options.field + '" />')
            .appendTo(container)
            .kendoDatePicker({
                format: "dd-MM-yyyy", // Display format
                parseFormats: ["ddMMyyyy", "dd.MM.yyyy", "ddMMyy", "dd.MM.yy"]
            });
    }
}

//Kendo DropDown/ComboBox common Function
var KendoDropDownHelper = {
    MultiColumnComboBox: function (data, element, textField, valueField, columns, selectedId) {
        if ($(element).data("kendoMultiColumnComboBox")) $(element).kendoMultiColumnComboBox('destroy').empty();

        $(element).kendoMultiColumnComboBox({
            filter: "contains",
            minLength: 2,
            autoWidth: true,
            dataTextField: textField,
            dataValueField: valueField,
            height: 350,
            //autoBind: false,
            columns: columns,
            dataSource: data,
            index: -1,
        });
    },
    MultiSelect: function (selectedId, data) {
        if ($("#" + selectedId).data('kendoMultiSelect')) {
            $("#" + selectedId).data('kendoMultiSelect').destroy();
            $("#" + selectedId).unwrap('.k-multiselect').show().empty();
            $(".k-multiselect-wrap").remove();
        }

        $("#" + selectedId).kendoMultiSelect({
            placeholder: "Multi Select",
            dataTextField: "text",
            dataValueField: "id",
            //autoBind: false,
            dataSource: data
        });
    },
    MultiSelectCombo: function (data, selectedId, textField, valueField, headerTemplate = "", itemTemplate = "", autoClose = true) {
        var $element = $("#" + selectedId);

        if ($element.data('kendoMultiSelect')) {
            $element.data('kendoMultiSelect').destroy();
          
            $element.empty(); 

            var $wrapper = $element.closest('.k-multiselect');
            if ($wrapper.length) {
                $wrapper.replaceWith($element);
            }
        }


        $element.kendoMultiSelect({
            placeholder: "---Select---",
            dataTextField: textField,
            dataValueField: valueField,
            headerTemplate: headerTemplate,
            itemTemplate: itemTemplate,
            //autoBind: false,
            autoClose: autoClose,
            dataSource: data,
            filter: "contains"
        });

  

    },
    ComboBox: function (data, ctrlId, textField, valueField, showDefaultOption = false) {
        if ($("#" + ctrlId).data("kendoComboBox")) ($("#" + ctrlId).kendoComboBox('destroy').empty());
        if (showDefaultOption) {
            var obj = new Object();
            obj[textField] = "Select";
            obj[valueField] = null;
            data.unshift(obj);
        }


        $("#" + ctrlId).kendoComboBox({
            placeholder: "Select or Type...",
            dataTextField: textField,
            dataValueField: valueField,
            dataSource: data,
            //index: 0,
            suggest: true,
            filter: "contains",
            fillMode: "flat",
            autoWidth: true,
        });
    },
    ComboBox2: function (data, ctrlId, textField, valueField, selectedValue) {
        if ($("#" + ctrlId).data("kendoComboBox")) ($("#" + ctrlId).kendoComboBox('destroy').empty());
        $("#" + ctrlId).kendoComboBox({
            placeholder: "Select",
            dataTextField: textField,
            dataValueField: valueField,
            dataSource: data,
            suggest: true,
            filter: "contains",
            fillMode: "flat"
        });
    },
    ComboBox_MultiColumn: function (data, ctrlId, textField, valueField, headerTemplate = "", itemTemplate = "") {
        if ($("#" + ctrlId).data("kendoComboBox")) ($("#" + ctrlId).kendoComboBox('destroy').empty());
        if (data.length < 1) {
            var obj = new Object();
            obj[textField] = "Select";
            obj[valueField] = null;
            data.unshift(obj);
        }


        $("#" + ctrlId).kendoComboBox({
            placeholder: "Select",
            headerTemplate: headerTemplate,
            template: itemTemplate,
            dataTextField: textField,
            dataValueField: valueField,
            dataSource: data,
            //index: 0,
            suggest: true,
            filter: "contains",
            fillMode: "flat"
        });
    },
    HighlightText: function (text, searchText) {
        if (!text) return "";
        const encodedText = kendo.htmlEncode(text);
        if (!searchText) return encodedText;

        const safeSearch = searchText.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
        const regex = new RegExp("(" + safeSearch + ")", "gi");
        return encodedText.replace(regex, "<span class='combo_highlight'>$1</span>");
    },
    KendoMultiColumn_BySearch: function (element, apiUrl, columns, textFiled, valueFiled) {
        if ($(element).data("kendoMultiColumnComboBox")) $(element).kendoMultiColumnComboBox('destroy').empty();
        $(element).kendoMultiColumnComboBox({
            filter: "contains",
            minLength: 2,
            autoWidth: true,
            dataTextField: textFiled,
            dataValueField: valueFiled,
            height: 400,
            autoBind: true,
            columns: columns,
            dataSource: {
                type: "json",
                serverFiltering: true,
                transport: {
                    read: {
                        type: "GET",
                        url: apiUrl,
                        dataType: "json",
                        contentType: "application/json",
                        data: function () {
                            return {
                                searchKey: `${$(element).data("kendoMultiColumnComboBox").input.val()}`,  // Get filter text from input
                                pageIndex: 1,
                                pageSize: 100
                            };
                        }
                    }
                }
            },
            index: -1,
            dataBound: function () {
                comboBoxSearchText = $(element).data("kendoMultiColumnComboBox").input.val(); // Capture current search term
            }

        });
    }
}

var KendoWindoHelper = {
    KendoPopupLoad: function (element, width, title) {
        $("#" + element).kendoWindow({
            title: title,
            resizable: false,
            width: "" + width + "%",
            actions: ["Close"],
            modal: true,
            visible: false,
            position: {
                top: 40,
                left: "30%"
            }
        });
    },
    KendoPopupLoad2: function (element, width, title, top, left) {
        $("#" + element).kendoWindow({
            title: title,
            resizable: false,
            width: "" + width + "%",
            actions: ["Close"],
            modal: true,
            visible: false,
            position: {
                top: top,
                left: left + "%"
            },
            animation: {
                open: {
                    effects: "fade:in",
                    duration: 200
                },
                close: {
                    effects: "fade:out",
                    duration: 100
                }
            },
        });
    }
}

var KendoDateHelper = {
    DatePicker: function (ctrlId, showDefaultDate = false) {
        if ($("#" + ctrlId).data("kendoDatePicker")) ($("#" + ctrlId).kendoDatePicker('destroy').empty());
        $("#" + ctrlId).kendoDatePicker({
            format: "dd-MM-yyyy",

        });
    },
}
var KendoSwitchHelper = {
    Switch2: function (ctrlId) {

        if ($("#" + ctrlId).data("kendoSwitch")) ($("#" + ctrlId).kendoSwitch('destroy').empty());
        $("#" + ctrlId).kendoSwitch({
            messages: {
                checked: "YES",
                unchecked: "NO"
            }
        });
    },
    Switch: function (ctrlId, opts) {
        var $el = $("#" + ctrlId);
        var sw = $el.data("kendoSwitch");
        if (sw) {
            sw.destroy();
            if (sw.wrapper && sw.wrapper.length) {
                sw.wrapper.before($el);
                sw.wrapper.remove();
            }
            $el.removeClass("k-input k-switch k-widget").off();
        }
        $el.kendoSwitch($.extend(true, {
            messages: {
                checked: "YES",
                unchecked: "NO"
            }
        }, opts));
    }
}

var TableHelper = {
    GetDynamicallyTableData: function ($table, arrayPushField = "", conditionField = "", extraAttrFields = []) {
        let detailsDataList = [];
        $table.find("tbody tr").each(function () {
            let detailsData = {};
            let index = 0;
            $(this).find("td").each(function () {
                $(this).find("input, select, textarea").each(function () {
                    let fieldName = $(this).attr('name');
                    let idName = this.id;
                    if (fieldName && idName && idName != "_1" && idName != null) {
                        let value = $(this).val();

                        if ($(this).is(":checkbox")) {
                            value = $(this).is(":checked");
                        }
                        if ($(this).is(":radio")) {
                            value = $(this).is(":checked") ? value : null;
                        }

                        if (value === "" || value === null) {
                            value = 0;
                        }
                        detailsData[fieldName] = value;
                        if (conditionField && idName.toLowerCase().startsWith(conditionField)) {
                            extraAttrFields.forEach(field => {
                                if (index == 0 && field.startsWith("RowSLNo")) {
                                    detailsData[field] = parseInt($(this).attr(field)) + 1;
                                }
                                else if (!field.startsWith("RowSLNo")) {
                                    detailsData[field] = $(this).attr(`${field}`);
                                }

                            });

                            if (arrayPushField == fieldName) {
                                detailsDataList = [...detailsDataList, { ...detailsData }];
                            }
                            index++;
                        }

                    }
                });
            });

            if (!arrayPushField) {
                detailsDataList = [...detailsDataList, { ...detailsData }];
            }

        });
        return detailsDataList;
    },
}

// export file helper
var ExportFileHelper = {
    GetExcelFile: function (base64String, excelName) {
        var binaryData = atob(base64String);

        const blob = new Blob([new Uint8Array(binaryData.length).map((_, index) => binaryData.charCodeAt(index))], {
            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        });

        // Create a URL for the blob
        const url = window.URL.createObjectURL(blob);

        // Create a link element to trigger the download
        const a = document.createElement('a');
        a.href = url;
        a.download = excelName + '.xlsx'; // Set the desired file name
        a.click();

        // Revoke the object URL to free up resources
        window.URL.revokeObjectURL(url);
    }
}

var GridNavigationHelper = {

    CommitCurrentCell: function (grid, $cell) {
        if (grid.editable && typeof grid.editable.end === "function") {
            const ok = grid.editable.end();
            if (ok === false) return false; // invalid => don't move
        }
        const $inp = $cell.find(":input:visible:enabled").first();
        const widget = $inp.length ? kendo.widgetInstance($inp) : null;
        if (widget && typeof widget.trigger === "function") widget.trigger("change");
        if ($inp.length) $inp.trigger("change");
        if (typeof grid.closeCell === "function") grid.closeCell($cell);
        return true;
    },
    GetCellField: function (grid, $td) {
        const idx = $td[0].cellIndex;
        const col = grid.columns[idx];
        return col && col.field ? col.field : null;
    },
    FindCellByField: function (grid, $tr, field) {
        const $cells = $tr.children("td:visible");
        for (let i = 0; i < $cells.length; i++) {
            const $td = $cells.eq(i);
            const f = GridNavigationHelper.GetCellField(grid, $td);
            if (f === field) return $td;
        }
        return $(); // empty
    },
    IsEditableCell: function (grid, $td, dataItem) {
        const idx = $td[0].cellIndex;
        const col = grid.columns[idx];
        if (!col) return false;

        // command/hidden column বাদ
        if (col.command) return false;

        // কাস্টম non-editable class থাকলে বাদ
        if ($td.hasClass("non-editable-column")) return false;

        // column.editable === false
        if (col.editable === false) return false;

        // column.editable যদি function হয়
        if (typeof col.editable === "function") {
            try {
                return !!col.editable.call(grid, { model: dataItem });
            } catch (_) { /* ignore */ }
        }

        return true;
    }
}
