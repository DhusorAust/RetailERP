/// <reference path="daypilot-all.min.js" />

$.ajax({
    type: "POST",
    url: base + "PlanBoard/GetPlan",
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

    //CONTEXT MENU
    dp.contextMenu = new DayPilot.Menu({ items: [
        { text: "Edit", onclick: function () { alert("Edit Plan ID: " + this.source.id()); } },
        { text: "Delete", onclick: function () { alert("Delete Plan ID text: " + this.source.id()); } },
        { text: "-" },
        { text: "Copy", onclick: function () { alert("Copy Plan ID: " + this.source.id()); } },
        { text: "Select", onclick: function () { alert("Selected Plan ID: " + this.source.id()); } },
        { text: "Color", onclick: function () {
            var item = this.source;
            $("#colorDiv").dialog({
                resizable: false,
                modal: true,
                title: "Change Color",
                height: 150,
                width: 300,
                buttons: {
                    "OK": function () {
                        item.data.backColor = $("#txtCellColor").val();
                        dp.events.update(item);
                        $.ajax({
                            type: "POST",
                            url: base + "PlanBoard/UpdateColor",
                            dataType: "JSON",
                            contentType: "application/json;charset=utf-8",
                            data: JSON.stringify({ "planID": item.id(), "Color": $("#txtCellColor").val() }),
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
        args.e.bubbleHtml = "<div> Buyer:"+ args.e.buyer+ "</div><div>Order No:"+args.e.text+"</div><div>Count:"+args.e.yarnCount+"</div><div>YarnBrand:"+args.e.yarnBrand+"</div><div>Mc Dia:"+args.e.dia+"</div><div>Prog Qty:"+args.e.planQty+"</div><div>Start Date:"+new DayPilot.Date(args.e.start).toString("M/d/yyyy")+"</div><div>End Date:"+ new DayPilot.Date(args.e.end).toString("M/d/yyyy");     
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
        //        $.ajax({
        //            type: "POST",
        //            url: base + "PlanBoard/UpdateEventDate",
        //            dataType: "JSON",
        //            contentType: "application/json;charset=utf-8",
        //            data: JSON.stringify({ "planID": args.e.id(), "start": args.e.start(), "end": args.e.end() }),
        //            success: function (data) {
        //            }
        //        });
        dp.message("Successfully Resized: " + args.e.text());
    };

    // event creating
    dp.onTimeRangeSelected = function (args) {
        // var name = prompt("New Plan Name:", "KnitPlan");

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
