/// <reference path="daypilot-all.min.js" />
/// <reference path="../../../Script/jquery.validationEngine.js" />

loadAllEvents();

$.ajax({
    type: "POST",
    url: base + "PlanBoard/GetPlanRND",
    dataType: "JSON",
    contentType: "application/json;charset=utf-8",
    success: function (data) {
        if (data.length == 0)
            alert('Error');
        else {
            populateboard(data.lRes, data.lEvents);
        }
    },
    error: function (a, b, c) {
        alert(formatErrorMessage(a, c) + "...populateGrid");
    }
});

function populateboard(resource, events) {
    var dp = $("#planboard").daypilotScheduler({
        startDate: new DayPilot.Date("2014-01-25").firstDayOfMonth(),
        cellGroupBy: "Month",
        days: 365,
        cellDuration: 1440, // one day
        resources: resource
    });
    dp.cssClassPrefix = "scheduler_blue";
    //CONTEXT MENU
    dp.contextMenu = new DayPilot.Menu({ items: [
        { text: "Edit", onclick: function () { alert("Edit Plan ID: " + this.source.id()); } },
        { text: "Delete", onclick: function () { alert("Delete Plan ID text: " + this.source.id()); } },
        { text: "-" },
        { text: "Copy", onclick: function () { alert("Copy Plan ID: " + this.source.id()); } },
        { text: "Select", onclick: function () { alert("Selected Plan ID: " + this.source.id()); } },
        { text: "Color", onclick: function () {
            var item = this.source;
            $("#txtCellColor").val(this.source.data.backColor);
            $("#txtCellFontColor").val(this.source.data.fontColor);
            console.log(item);
            $("#colorDiv").dialog({
                resizable: false,
                modal: true,
                title: "Change Color",
                height: 200,
                width: 300,
                buttons: {
                    "OK": function () {
                        item.data.backColor = $("#txtCellColor").val();
                        item.data.text = '<span style="color:' + $('#txtCellFontColor').val() + '">' + item.text() + '</span>';

                        dp.events.update(item);
                        $.ajax({
                            type: "POST",
                            url: base + "PlanBoard/UpdateColorRND",
                            dataType: "JSON",
                            contentType: "application/json;charset=utf-8",
                            data: JSON.stringify({ "planID": item.id(), "Color": $("#txtCellColor").val(), "FontColor": $('#txtCellFontColor').val() }),
                            success: function (data) {
                            }
                        });
                        $(this).dialog('close');
                    }
                }
            });

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
        args.e.bubbleHtml = "<div> Buyer:" + args.e.buyer + "</div><div>Order No:" + args.e.text + "</div><div>Count:" + args.e.yarnCount + "</div><div>YarnBrand:" + args.e.yarnBrand + "</div><div>Mc Dia:" + args.e.dia + "</div><div>Prog Qty:" + args.e.planQty + "</div><div>Start Date:" + new DayPilot.Date(args.e.start).toString("M/d/yyyy") + "</div><div>End Date:" + new DayPilot.Date(args.e.end).toString("M/d/yyyy");
    };
    dp.events.list = events;
    dp.separators = [
        { color: "Red", location: new Date(), layer: "AboveEvents" }
    ];

    // event moving
    dp.onEventMoved = function (args) {
        dp.message("Moved: " + args.e.text());
    };

    // event resizing
    dp.onEventResized = function (args) {
                $.ajax({
                    type: "POST",
                    url: base + "PlanBoard/UpdateEventDateRND",
                    dataType: "JSON",
                    contentType: "application/json;charset=utf-8",
                    data: JSON.stringify({ "planID": args.e.id(), "Start": args.e.start(), "End": args.e.end() }),
                    success: function (data) {
                    }
                });
        dp.message("Successfully Resized: " + args.e.text());
    };

    // event creating
    dp.onTimeRangeSelected = function (args) {
        loadKnitBuyer();
        $("#planBoardNewSchedule").dialog({
            title: 'New Knit Plan',
            width: 1200,
            height: 600,
            modal: true
        });

        dp.clearSelection();
        //        if (!name) return;
        //        var e = new DayPilot.Event({
        //            start: args.start,
        //            end: args.end,
        //            id: DayPilot.guid(),
        //            resource: args.resource,
        //            text: name
        //        });
        //        dp.events.add(e);
        //dp.message("Created");
    };

    // ON EVENT CLICK
    dp.onEventClicked = function (args) {
        alert("Event id " + args.e.id());
    };

    dp.update();
    dp.scrollTo(new Date() - (86400000 * 15));


    $('div').filter(function () { return $(this).text().toLowerCase() === 'demo'; }).text('Machine No');
}

// KNIT PLAN ENTRY FORM METHODS
function loadAllEvents() {
    //ON ORDER TYPE CHANGE
    $(".OrderType").on('change', function () {
        $("#txtBuyer").val('');
        $("#txtOrder").val('');
        $("#cmbOrder option").remove();
        loadKnitBuyer();
    });

//ON BUYER CHANGE
    $("#txtBuyer").on('input', function () {
       loadKnitOrders();
   });
   //ON ORDER CHANGE
   $("#txtOrder").on('input', function () {
       var order = $(this).val().toUpperCase();
       $("#cmbOrder option").each(function () {
           if ($(this).text().startsWith(order)) {
               $(this).prop('selected', true);
               return false;
           }
       });
   });

   //Order Combo Click
   $("#cmbOrder").click(function () {

       $("#txtOrder").val($(this).val());
       loadUpperGrid();

   });

   //UPPER GRID ROW SELECT
   $(document).on('click', '#tblPlan tr:gt(0)', function () {
       $('#meessageBar').text('Row No ' + $(this).index() + ' selected');
       $('#meessageBar').fadeIn("slow", function () {
           setTimeout(messageBarRemove, 2000);
       });
   });
   function messageBarRemove() {
       $('#meessageBar').fadeOut("slow");
   }
}

function loadKnitBuyer() {
    $("#dlBuyer option").remove();
    var OrderType = $(".OrderType:checked").val();
    $.ajax({
        type: "POST",
        url: base + "PlanBoard/GetBuyerRND",
        dataType: "JSON",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({ "OrderType": OrderType }),
        success: function (data) {
            content = '';
            for (var i = 0; i < data.length; i++) {
                content += '<option>' + data[i] + '</option>';
            }
            
            $("#dlBuyer").append(content);
        },
        error: function (a, b, c) { alert(formatErrorMessage(a, c) + "...loadKnitBuyer"); }
    });
}

// LOAD ORDERS
function loadKnitOrders() {
    $("#cmbOrder option").remove();
    var OrderType = $(".OrderType:checked").val();
    var Buyer = $("#txtBuyer").val();
    $.ajax({
        type: "POST",
        url: base + "PlanBoard/GetOrderRND",
        dataType: "JSON",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({ "OrderType": OrderType, "BuyerName": Buyer, "FloorName": "MKL-KNITTING" }),
        success: function (data) {
            content = '';
            for (var i = 0; i < data.length; i++) {
                content += '<option>' + data[i] + '</option>';
            }
            $("#cmbOrder").append(content);
        },
        error: function (a, b, c) { alert(formatErrorMessage(a, c) + "...loadKnitOrder"); }
    });
}
//LOAD UPPER GRID
function loadUpperGrid() {
    $("#tblPlan tr:gt(0)").remove();
    var order = $("#txtOrder").val();
    $.ajax({
        type: "POST",
        url: base + "PlanBoard/GetUpperGridRND",
        dataType: "JSON",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({ "Order": order, "FloorName": "MKL-KNITTING" }),
        success: function (data) {
            content = '';
            if (data.length === 0) {
                content += '<tr><td colspan="7">No Records Found </td></tr>';
            }
            $.each(data, function (i, row) {
                content += '<tr><td>' + row.ORDER_NO + '</td><td>' + row.QUANTITY + '</td><td>' + row.MC_DIA + '</td><td>' + row.YARN_BRAND + '</td><td>' + row.COLOR + '</td><td>' + row.YARN_COUNT + '</td><td>' + row.LOT + '</td></tr>';
            });
            $("#tblPlan").append(content);
        },
        error: function (a, b, c) { alert(formatErrorMessage(a, c) + "...loadKnitOrder"); }
    });

}