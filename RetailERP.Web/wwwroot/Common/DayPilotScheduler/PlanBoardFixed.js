/// <reference path="daypilot-all.min.js" />
/// <reference path="../../../Script/jquery.validationEngine.js" />



$.ajax({
    type: "POST",
    url: base + "PlanBoard/GetPlanFixed",
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
        startDate: new Date() - (86400000 * 4),
        cellGroupBy: "Month",
        days: 40,
        cellDuration: 1440, // one day
        resources: resource,
        durationBarMode:"PercentComplete"
    });
   // dp.cssClassPrefix = "scheduler_blue";
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
                            url: base + "PlanBoard/UpdateColorFixed",
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
      
        dp.message("Successfully Resized: " + args.e.text());
    };

    // event creating
    dp.onTimeRangeSelected = function (args) {
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
        console.log(args);

    };

    dp.update();
    dp.scrollTo(new Date() - (86400000 * 4));


    $('div').filter(function () { return $(this).text().toLowerCase() === 'demo'; }).text('Machine No');
}