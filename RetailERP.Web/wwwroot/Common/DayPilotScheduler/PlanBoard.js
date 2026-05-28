var OrderNo = '';
var resource_id = '';
var NewLineID = 0;
var Quantity = 0;
var IsTextHide = 0;
var StripQTY = 0;
var ID = 0;
var Msg = '';
var HolidayArray = new Array();
$(document).ready(function () {
    Get_Holidays();
    Load_OrderList();
    PlanBoard(0);
    RefreshPlanBoard(IsTextHide);


   
   
    $("#myModal").modal({
        show: false,
        backdrop: 'static',
        resizable: false,
        modal: true,
        height: 660,
        width: 1200,
        autoOpen: false
    });

    $("#myModalSplit").modal({
        show: false,
        backdrop: 'static',
        resizable: false,
        modal: true,
        height: 660,
        width: 1200,
        autoOpen: false
    });

    $("#dlgPlanSchedule").modal({
        show: false,
        backdrop: 'static',
        resizable: false,
        modal: true,
        height: 660,
        width: 1200,
        autoOpen: false
    });


    $("#dlgMultipleStrip").modal({
        show: false,
        backdrop: 'static',
        resizable: false,
        modal: true,
        height: 660,
        width: 1200,
        autoOpen: false
    });

    $('#btnTextHide').click(function () {
       
        Msg = '';
        if (IsTextHide == 0) {
            IsTextHide = 1;
            Msg= 'Planboard Text Hide'
        }
        else {
            IsTextHide = 0;
            Msg = 'Planboard Text Show'
        }
        RefreshPlanBoard(IsTextHide);
        //dp.message(Msg, 5000);
        Show_Alert(Msg, 'Success');
    });

    $('#btnRefresh').click(function () {
        dp.message("Plan Board Refreshed!", 5000);
        RefreshPlanBoard(IsTextHide);
       
    });

    $("#cellwidth")[0].oninput = function (ev) {
        var cellwidth = parseInt(this.value);
        $("#label").html(cellwidth);
        var start = dp.getViewPort().start;  // save the original start
        dp.cellWidth = cellwidth;
        dp.update();
        dp.scrollTo(start); // reset the start position
    };


    $('#btnOrderSearch').click(function () {
        Get_Orders('');
        $('#myModal').modal('show');
        $('.modal-dialog').css({ 'width': '1200px' })


    });

    $("#myModalSplit").modal('close');

    $("#dlgPlanSchedule").modal('close');
    $("#dlgMultipleStrip").modal('close');


    var base = '@Url.Content("~/")';
    var perm = new Array();
    var barID = '', planID = '', kid = '', knit_unit = '', yc = '', fldid = '';

   
});

$(document).on('click', '.loadcss', function () {
    var OrderID = $(this).closest('tr').children('td:eq(0)').text();
    Select_A_Order_ForDashBoard(OrderID);
});

function bootstrap_alert(elem, message, timeout) {
    $(elem).show().html('<div class="alert"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button><span>' + message + '</span></div>');

    if (timeout || timeout === 0) {
        setTimeout(function () {
            $(elem).alert('close');
        }, timeout);
    }
};

function messageBarRemove() {
    $(barID).fadeOut("slow");
}

function Load_OrderList() {
    $("#dlOrdersNo option").remove();
    $.ajax({
        type: "POST",
        url: base + "Planning/OrderList",
        dataType: "JSON",
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            var content = '';
            $.each(data, function (i, obj) {
                content += '<option value="' + obj[1] + '" id="' + obj[0] + '"></option>';
            });
            $("#dlOrdersNo").append(content);
        },
        error: function (a, b, c) { alert(formatErrorMessage(a, c) + "...LoadDia"); }
    });
}


$('#txtOrderNoSearch').on('input', function () {
    if ($(this).length > 0) {
        Get_Orders($(this).val());
    }
    else {

    }

});


function Load_SinglePlanDetails(ID, Type) {
    $('#lblStripIDPLOD').text('0');

    $("#tblSingleOrderInfo tr:gt(0)").remove();
    var content = '';
    $.ajax({
        async: false,
        url: base + "Planning/GetSingle_Plan_ScheduleInfo",
        type: "POST",
        data: JSON.stringify({ "ID": ID, "Type": Type }),
        datatype: "JSON",
        contentType: "application/Json;charset=UTF-8",
        success: function (data) {
            console.log(data);

           
            content = '<tr><td>Order:</td><td>' + data[0].OrderRefNo + '</td><tr>' +
                       '<tr><td>Product:</td><td>' + data[0].ProductName + '</td><tr>' +
                        '<tr><td>Customer:</td><td>' + data[0].BuyerName + '</td><tr>' +
                        '<tr><td>Description:</td><td>' + data[0].OrderRefNo + '</td><tr>' +
                         '<tr><td>Delivery Date:</td><td>' + data[0].FirstOrder_Del_Date + '</td><tr>' +
                         '<tr><td>Start:</td><td>' + data[0].StartDate + '</td><tr>' +
                         '<tr><td>End:</td><td>' + data[0].EndDate + '</td><tr>' +
                         '<tr><td>SMV:</td><td>' + data[0].SMV + '</td><tr>' +
                         '<tr><td>This  Strip (Qty):</td><td>' + data[0].OrderQty + '</td><tr>' +
                         '<tr><td>Total  (Qty):</td><td>' + data[0].Total_Order_QTY + '</td><tr>' +
                         '<tr><td>Completed:</td><td>' + data[0].Completed + '</td><tr>' +
                         '<tr><td>Balance:</td><td>' + data[0].Balance + '</td><tr>' +
                         '<tr><td>Show Type:</td> <td > <select  class="form-control" data-Type="Type"><option value="1">This Strip Only</option><option value="2">Whole Order</option></select></td></tr>';
            $("#tblSingleOrderInfo").append(content);
            $('#lblStripIDPLOD').text(data[0].ID);

          

           
        }
    })
   
}

function Load_SinglePlanDetails_MultipleStrip(ID, Type) {
    $('#lblStripIDPLOD').text('0');
    $("#tblSingleOrderInfo1 tr:gt(0)").remove();
    $('#lblMultiplestrip_StripID').text('');
    $('#lblMultiplestrip_LineID').text('');
    $('#lblMultiplestrip_StartDate').text('');
    var content = '';
    $.ajax({
        async: false,
        url: base + "Planning/GetSingle_Plan_ScheduleInfo",
        type: "POST",
        data: JSON.stringify({ "ID": ID, "Type": Type }),
        datatype: "JSON",
        contentType: "application/Json;charset=UTF-8",
        success: function (data) {
            console.log(data);
            content = '<tr><td>Order:</td><td>' + data[0].OrderRefNo + '</td><tr>' +
                       '<tr><td>Product:</td><td>' + data[0].ProductName + '</td><tr>' +
                        '<tr><td>Customer:</td><td>' + data[0].BuyerName + '</td><tr>' +
                        '<tr><td>Description:</td><td>' + data[0].OrderRefNo + '</td><tr>' +
                         '<tr><td>Start:</td><td>' + data[0].StartDate + '</td><tr>' +
                         '<tr><td>End:</td><td>' + data[0].EndDate + '</td><tr>' +
                         '<tr><td>SMV:</td><td>' + data[0].SMV + '</td><tr>' +
                         '<tr><td>This  Strip (Qty):</td><td>' + data[0].OrderQty + '</td><tr>' +
                         '<tr><td>Total  (Qty):</td><td>' + data[0].Total_Order_QTY + '</td><tr>';
            $("#tblSingleOrderInfo1").append(content);
            $('#lblStripIDPLOD').text(data[0].ID);

            var id = data[0].BuildUpCurveID;

            Load_CurveList();

            $("#ddlCurve").val(id);

            //$('#ddlCurve option').each(function () {
            //    if ($(this).attr('id') == id) {
            //        $(this).attr('selected', true);
            //    }
            //});
            Get_BuildCurve_Details(data[0].BuildUpCurveID, ID);
            $('#lblMultiplestrip_StripID').text(data[0].StripID);
            $('#lblMultiplestrip_LineID').text(data[0].LineID);
            $('#lblMultiplestrip_StartDate').text(data[0].StartDate);
        }
    })

}

$(document).on('change', 'select[data-Type=Type]', function () {
    var indx = $(this).closest('tr').index();
    var Type = $(this).val();
    GetPlan_Schedule($('#lblStripIDPLOD').text(), Type);
});

function GetPlan_Schedule(StripID, Type) {
    $("#tblPlanSchedule tr:gt(0)").remove();
    $.ajax({
        async: false,
        url: base + "Planning/GetPlan_Schedule",
        type: "POST",
        data: JSON.stringify({ "ID": StripID, "Type": Type }),
        datatype: "JSON",
        contentType: "application/Json;charset=UTF-8",
        success: function (data) {
            var content = '';
            if (data.length == 0) {
                content += '<tr><td colspan="14">No Records Found</td></tr>';
            }
            else {
                var tdColor = "";

                $.each(data, function (i, obj) {
                    if (obj.Status == "Complete") {
                        tdColor = "background-color: #e6dc95 !important;";
                    }
                    else {
                        tdColor = "";
                    }
                    content += '<tr>' +
                    '<td style="display:none">' + obj.ID + '</td>' +
                    '<td style ="'+ tdColor +'">' + obj.ProductionStartDate + '</td>' +
                    '<td style ="' + tdColor + '">' + obj.PerDay_TGT_QTY + '</td>' +
                    '<td style ="' + tdColor + '">' + obj.CumulativeTGT_QTY + '</td>' +
                    '<td style ="' + tdColor + '">' + obj.ProductionUpdateQTY + '</td>' +
                    '<td style ="' + tdColor + '">' + obj.Efficiency + '</td>' +
                    '<td style ="' + tdColor + '">' + obj.Capacity + '</td>' +
                    '<td style ="' + tdColor + '">' + obj.CapacityFactor + '</td>' +
                    '<td style ="' + tdColor + '">' + obj.WorkingHour + '</td>' +
                    '<td style ="' + tdColor + '">' + obj.Status + '</td>' +
                    ' </tr>';
                });
                $("#tblPlanSchedule").append(content);
            }
        },
        error: function (a, b, c) {

        }

    });
}

// Fill DayPilot Information in Day Pilot Control
function PlanBoard(IsTextHide) {
    $.ajax({
        type: "POST",
        url: base + "Planning/GetPlanFixed",
        dataType: "JSON",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({ "IsTextHide": IsTextHide }),
        success: function (data) {
            if (data.length == 0)
                alert('Error');
            else {
                populateboard(data.lRes, data.lEvents);
            }
        },
        error: function (a, b, c) {

        }
    });
}


function RefreshPlanBoard(IsTextHide) {
    $.ajax({
        type: "POST",
        url: base + "Planning/GetPlanFixed",
        dataType: "JSON",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({ "IsTextHide": IsTextHide }),
        success: function (data) {
            dp.resources = data.lRes;
            dp.events.list = data.lEvents;
           
            dp.clearSelection();
            dp.update();
            $('div').filter(function () { return $(this).text().toLowerCase() === 'demo'; }).text(' Line No ');
        },
        error: function (a, b, c) {
            alert(formatErrorMessage(a, c) + "...populateGrid");
        }
    });
}

function populateboard(resource, events) {
    dp = $("#planboard").daypilotScheduler({
        startDate: new Date() - (86400000 * 4),
        cellGroupBy: "Month",
        days: 365,
        cellWidth: 40,
        cellDuration: 1440, // one day
        DurationBarVisible: false,
        resources: resource,
        rawBlankCells: false,
        cellSweepingCacheSize: 50,
        dynamicEventRenderingCacheSweeping: true,
        dynamicEventRenderingCacheSize : 50,
        autoScroll: "Always",
        heightSpec : "Max",
        height: 500,
    });


    // ...
    dp.onEventFilter = function (args) {

        // display only events that contain the specified text, case insensitive
        if (args.e.text().toUpperCase().indexOf(args.filter.toUpperCase()) === -1) {
            args.visible = false;
        }
    };
    dp.init();

    $("#filter").keyup(function () {
        var query = $(this).val();
        dp.events.filter(query);
    });

    $("#clear").click(function () {
        $("#filter").val("");
        dp.events.filter(null);
        return false;
    });

   
    // dp.cssClassPrefix = "scheduler_blue";
    //CONTEXT MENU
    dp.contextMenu = new DayPilot.Menu({
        items: [
            {
                text: "Split", onclick: function () {

                    OrderNo = this.source.data.text;
                    resource_id = this.source.data.value;
                    NewLineID = this.source.data.resource;
                    StripQTY = this.source.data.StripQTY;


                    $('#txtQuantity').val(parseInt(parseInt(StripQTY)/2));


                    $('#txtSplitStart').val(this.source.data.start.toString("M/d/yyyy"));
                    $('#txtSplitEnd').val(this.source.data.end.toString("M/d/yyyy"));

                    $('#txtAvlbQuantity').val(StripQTY);

                    $('#txtOrderNo').val(OrderNo);
                    $('#myModalSplit').modal('show');
                   

                }
            },

            {
                text: "Plan Schedule", onclick: function () {
                    var StripID = this.source.data.id;
                
                    Load_SinglePlanDetails(StripID, '3');
                    GetPlan_Schedule(StripID, '1')
                    $('#dlgPlanSchedule').modal('show');

                    

                }
            },

             {
                 text: "Multiple Strip", onclick: function () {
                     ID = this.source.data.id;


                     Load_SinglePlanDetails_MultipleStrip(ID, '3');
                     $('#dlgMultipleStrip').modal('show');



                 }
             },

          
           
        ]
    });

    dp.bubble = new DayPilot.Bubble();
    dp.treeEnabled = true;

    dp.dynamicEventRenderingCacheSweeping = true;

    dp.eventHoverHandling = "Bubble";

    dp.eventMovingStartEndEnabled = true;
    dp.eventResizingStartEndEnabled = true;
    dp.timeRangeSelectingStartEndEnabled = true;

    dp.onBeforeEventRender = function (args) {
        args.e.bubbleHtml = "<div><b>Start Date: </b>" + new DayPilot.Date(args.e.start).toString("M/d/yyyy hh:mm") + "</div><div><b>End Date: </b>" + new DayPilot.Date(args.e.end).toString("M/d/yyyy hh:mm") + "</div>" +
            "<div><b>Delivery Date: </b>" + args.e.FirstOrder_Del_Date + "</div><div><b>Order No: </b>" + args.e.text + "</div><div><b>SMV: </b>" + args.e.SMV + "</div>";
        
    };

    dp.events.list = events;
   
    dp.separators = [
        { color: "Red", location: new Date(), layer: "AboveEvents" }
    ];

    dp.onBeforeCellRender = function (args) {
        var str = '';
        str = args.cell.start;
        if (IsHoliday($.trim(str), HolidayArray) == true)
        {
            
            args.cell.backColor = "#ecdbdb";
        }
       
        //if (
        //    //args.cell.start.getDayOfWeek() === 5
        //    args.cell.start <= DayPilot.Date.today() && DayPilot.Date.today() < args.cell.end
        //    )
        //{
        //    args.cell.backColor = "#ffcccc";
        //}
    };
    // event moving
    dp.onEventMove = function (args) {
        var planStDate = args.newStart.toString().substring(0, 16).replace('T', ' ');
        var planEndDate = args.newEnd.toString().substring(0, 16).replace('T', ' ');
        var value = args.e.data.value;
        var NewResource = args.newResource;
        var BuildUpCurveID = args.e.data.BuildUpCurveID;
        //var PlanId = args.e.data.id;
        //
        //var mcNoArray = args.newResource.split('-');
        //
        //var McId = mcNoArray[0];
        //var result = 1;

        //if (new Date(args.newEnd) < new Date()) {
        //    barID = '#messageBarY';
        //    $(barID).text(' Plan End Date Can Not Be Smaller Than Current Date !!!');
        //    $(barID).fadeIn("slow", function () {
        //        $(barID).fadeOut(3000);
        //    });
        //    args.preventDefault();
        //}
     
        //else {
        result = PlanMoved(value, NewResource, planStDate, BuildUpCurveID);
        //}

        if (result == '1') {
            args.preventDefault();
        }
    };
    $('div').filter(function () { return $(this).text().toLowerCase() === 'demo'; }).text('Line No');
    dp.update();
    dp.scrollTo(new Date().toString("s"));
    //RefreshPlanBoard(IsTextHide);

    // event resizing
    //dp.onEventResize = function (args) {
    //    var planStDate = args.newStart.toString().substring(0, 16).replace('T', ' ');
    //    var planEndDate = args.newEnd.toString().substring(0, 16).replace('T', ' ');
    //    var MachineProductionPlanId = args.e.data.id;

    //    var result = 1;

    //    var mcNoArray = args.e.data.resource.split('-');
    //    var McId = mcNoArray[0];
    //    var McNo = mcNoArray[1];

    //    alert(McId);
    //    alert(McNo);

    //    if (new Date(args.newEnd) < new Date()) {
    //        barID = '#messageBarY';
    //        $(barID).text(' Plan End Date Can Not Be Smaller Than Current Date !!!');
    //        $(barID).fadeIn("slow", function () {
    //            $(barID).fadeOut(3000);
    //        });
    //        args.preventDefault();
    //    }
    //    else if (McNo.length == 0) {
    //        barID = '#messageBarY';
    //        $(barID).text('Error: Invalid Machine No !!!');
    //        $(barID).fadeIn("slow", function () {
    //            $(barID).fadeOut(3000);
    //        });
    //        args.preventDefault();
    //    }
    //    else {
    //        result = PlanResized(MachineProductionPlanId, McId, planStDate, planEndDate);
    //    }

    //    if (result == '1') {
    //        args.preventDefault();
    //    }
    //};

    //Drag and Drop Event Creating
    dp.onTimeRangeSelected = function (args) {

        dp.clearSelection();
        //FrmMasterClear();


    };

    // ON EVENT CLICK
    dp.onEventClicked = function (args) {
        console.log(args);
    };

    dp.update();
    dp.scrollTo(new Date() - (86400000 * 4));

    $('div').filter(function () { return $(this).text().toLowerCase() === 'demo'; }).text('Line No');
}


$('#btnSplit').click(function () {

    if(parseInt(StripQTY) <= parseInt($('#txtQuantity').val()))
    {
        alert('Strip Quatity :' + StripQTY + ' Is Greater Then ' + $('#txtQuantity').val());
    }
else
{
        PlanSplit(resource_id, NewLineID, $('#txtQuantity').val(), $('#txtSplitStart').val());

    RefreshPlanBoard(IsTextHide);
}
});



$('#btnImplement').click(function () {
    if (confirm('Are You Sure Unload This Strip?')) {
        Unload_Strip(ID);
    } else {
     
    }

    RefreshPlanBoard(IsTextHide);
});


function Unload_Strip(value) {
    var ret = 0;
    $.ajax({
        async: false,
        type: "POST",
        url: base + "Planning/Unload_Strip",
        dataType: "JSON",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({ "ID": value }),
        success: function (data) {
            var result = data.ResultID;
            var errorMsg = data.Message;

            if (result == '0') {
                Show_Alert('Item Saved Failed', 'Fail');
            }

            else {
                Show_Alert('Strip Unloaded Successfully', 'Success');
            }
        }
    });

    return ret;
}



function PlanSplit(value, resource, StripQTY,start) {
    var ret = 0;
    $.ajax({
        async: false,
        type: "POST",
        url: base + "Planning/Split_Strip",
        dataType: "JSON",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({ "value": value, "resource": resource, "StripQTY": StripQTY, "start": start }),
        success: function (data) {
            var result = data.ResultID;
            var errorMsg = data.Message;

            if (result == '0') {
              
                Show_Alert('Item Saved Failed', 'Fail');
            }

            else {
                Show_Alert('Strip Splitted Successfully', 'Success');
            }
        }
    });

    return ret;
}

function PlanMoved(value, NewResource, planStDate, BuildUpCurveID) {

    var ret = 0;
    $.ajax({
        async: false,
        type: "POST",
        url: base + "Planning/Plan_Move",
        dataType: "JSON",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({ "value": value, "NewResource": NewResource, "start": planStDate, "BuildUpCurveID": BuildUpCurveID }),
        success: function (data) {
            var result = data.Result;
            var errorMsg = data.Message;

            if (data == '0') {
                Show_Alert('Plan Moved Failed!!', 'Fail');
                //dp.message("Plan Moved Failed!!", 5000);
                //ret = 1;
            }
            else if (data == '-1') {
                Show_Alert('Plan Moved Failed!! Already Planned Another Order In This Line Between Start & End Date!!', 'Fail');
                //dp.message("Plan Moved Failed!! Already Planned Another Order In This Line Between Start & End Date!!", 5000);
                //ret = 1;
            }

            else {
                Show_Alert('Plan Moved Successfully', 'Success');
                //dp.message("Plan Moved Successfully", 5000);
                //ret = 0;
            }
        }
    });

    RefreshPlanBoard(0);
    return ret;
   
}


function Get_Orders(OrderRefNo) {
    $("#tblOrder tr:gt(0)").remove();
    $.ajax({
        async: false,
        url: base + "Planning/Load_Order_List",
        type: "POST",
        data: JSON.stringify({ "OrderRefNo": OrderRefNo, "StatusID": '0' }),
        datatype: "JSON",
        contentType: "application/Json;charset=UTF-8",
        success: function (data) {
            var content = '';
            if (data.length == 0) {
                content += '<tr><td colspan="14">No Records Found</td></tr>';
            }
            else {
                $.each(data, function (i, obj) {
                    content += '<tr><td style="display:none">' + obj.OrderID + '</td>' +
                    '<td >' + obj.OrderRefNo + '</td>' +
                    '<td >' + obj.ProductName + '</td>' +
                    '<td >' + obj.ProductTypeName + '</td>' +
                    '<td >' + obj.CustomerName + '</td>' +
                    '<td >' + obj.FirstDeliveryDate + '</td>' +
                    '<td >' + obj.OrderQty + '</td>' +
                    '<td style="display:none">' + obj.ProductDescription + '</td>' +
                    '<td style="display:none">' + obj.OrderStatus + '</td>' +
                    '<td style="display:none">' + obj.OrderReceiveDate + '</td>' +
                    '<td style="display:none">' + obj.OrderReference + '</td>' +
                    '<td ><input type="button" value="Load" id="btnLoad" class="btn btn-success loadcss" style="padding: 3px 5px;" /></td>' +
                    ' </tr>';
                });
                $("#tblOrder").append(content);
            }
        },
        error: function (a, b, c) {

        }

    });
}

function Select_A_Order_ForDashBoard(OrderID) {

    if (OrderID == '0') {
        Show_Alert('Please Select A Order First..', 'Fail');
    }

    else {
        var data = {
            FactoryType: '3',
            OrderID: OrderID,
            RowID: '1',
            OperationOptions: '1',
            BuildUpCurveID: '1'
        }
        $.ajax({
            type: "POST",
            url: base + "Planning/Select_A_Order_ForDashBoard",
            dataType: "JSON",
            data: JSON.stringify(data),
            contentType: "application/json;charset=utf-8",
            success: function (data) {
                if (data != '0') {
                    //bootstrap_alert('#form_errors', 'Order Not Loaded!!', 1000)
                    Show_Alert('Order Not Loaded!!', 'Fail');
                }

                else {
  
                    Show_Alert('Order Loaded Succesfully..', 'Success');
                    Get_Orders();
                    RefreshPlanBoard(0);
                   
                }

            },
            error: function (a, b, c) {
                alert(a + '..' + c.statusCode);
            }
        });
    }
}


function Load_CurveList() {
    $('#ddlCurve option').remove();
    $.ajax({
        async: false,
        type: "POST",
        url: base + "Planning/LoaCurveList",
        dataType: "JSON",
        contentType: "application/json;charset=utf-8",
        success: function (data) {

            if (data.length != undefined) {
                var content = '';
                $.each(data, function (i, obj) {
                    content += '<option value="' + obj[0] + '">' + obj[1] + '</option>';
                });
                $('#ddlCurve').append(content);
            }
        }
    });
}


function Get_BuildCurve_Details(BuildUpCurveID,ID) {
    var PerDay_TGT_QTY = '0';
    var tdColor = '';
    $("#tblBuildCurveDetails tr:gt(0)").remove();
    $.ajax({
        async: false,
        url: base + "Planning/Load_BuildCurve_Details",
        type: "POST",
        data: JSON.stringify({ "BuildUpCurveID": BuildUpCurveID, "ID": ID }),
        datatype: "JSON",
        contentType: "application/Json;charset=UTF-8",
        success: function (data) {
            var content = '';
            if (data.length == 0) {
                content += '<tr><td colspan="2">No Records Found</td></tr>';
            }
            else {
                $.each(data, function (i, obj) {
                    PerDay_TGT_QTY = parseInt(obj.PerDay_TGT_QTY);
                    if (PerDay_TGT_QTY != 0) {
                        tdColor = "background-color: #e6e6e6 !important; font-weight:bold !important";
                    }
                    else {
                        tdColor = "";
                    }

                    content += '<tr>'+
                    '<td style ="' + tdColor + '">' + obj.Days + '</td>' +
                    '<td style ="' + tdColor + '">' + obj.EfficiencyPercentage + '</td>' +
                    '<td style ="' + tdColor + '">' + obj.PerDay_TGT_QTY + '</td>' +
                    ' </tr>';
                });
                $("#tblBuildCurveDetails").append('<tbody>' + content + '</tbody>');
            }
        },
        error: function (a, b, c) {

        }

    });
}



$("#btnLinkStrip").click(function ()
{
    if (confirm('Are You Sure Want To Change Build Curve For This Strip?'))
    {
        PlanMoved($('#lblMultiplestrip_StripID').text(), $('#lblMultiplestrip_LineID').text(), $('#lblMultiplestrip_StartDate').text(), $('#ddlCurve').val());
        RefreshPlanBoard(IsTextHide);
    }
    else
    {

    }

});

$("#ddlCurve").change(function () {
    $("#tblBuildCurveDetails tr:gt(0)").remove();

    Get_BuildCurve_Details($('#ddlCurve').val(), $('#lblStripIDPLOD').text());
  
});

function Get_Holidays() {
    $.ajax({
        async: false,
        url: base + "Planning/Load_Holiday_List",
        type: "POST",
        datatype: "JSON",
        contentType: "application/Json;charset=UTF-8",
        success: function (data) {
            var content = '';
            if (data.length == 0) {
                content += '';
            }
            else {
                $.each(data, function (i, obj) {
                    //var HolidayObj = new Object();
                    //HolidayObj.ProductionDate = obj.ProductionDate;
                    HolidayArray.push(obj.ProductionDate);
                });
            
            }
        },
        error: function (a, b, c) {

        }

    });

}

function IsHoliday(Date, HolidayList)
{
    return HolidayList.indexOf(Date)>-1
}