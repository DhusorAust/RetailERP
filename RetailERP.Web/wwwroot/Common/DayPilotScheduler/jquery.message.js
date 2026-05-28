(function() {
    var mappings = {
        topright: ['flipInX', 'flipOutX'],
        lowerleft: ['fadeInUp', 'fadeOutLeft'],
        lowerright: ['tada', 'zoomOut']
    };

    var Message = function(opt) {
        opt = $.extend({}, opt);
        var oldEls = $('.jquery-message-' + opt.position); //老的Div
        var box = this.appendBox(opt);
        var boxHeight = box.outerHeight();
        this.bindClose(box, opt);
        this.move(oldEls, boxHeight, opt);
    };

    //移动之前产生的div
    Message.prototype.move = function(oldEls, boxHeight, opt) {
        var that = this;
        oldEls.each(function() {
            var my = $(this);
            if (opt.position === 'lowerright') {
                that.close(my, opt);
            } else {
                moveOne(my, opt);
            }
        });

        function moveOne(my, opt) {
            var top = my.position().top;
            var height = boxHeight + 10;
            height = opt.position === 'topright' ? height : -height;
            my.stop(true).animate({
                top: top + height
            }, 200);
        }
    };

    Message.prototype.appendBox = function(opt) {
        var box = $(this.creatBox(opt)).appendTo('body');
        if (opt.position === 'lowerleft') {
            box.css('top', $(window).height() - box.outerHeight() - 20);
        }
        return box;
    };

    //返回Html字符串
    Message.prototype.creatBox = function(opt) {
        opt.inClass = mappings[opt.position][0];
        opt.fix = 'jquery-message';

        var tpl = '<div class="#{fix} #{fix}-#{position}"><div class="animated #{fix}-box #{fix}-#{color} #{inClass}">';
        //tpl += opt.title ? '<div class="#{fix}-h2">#{title}</div>' : '';
        tpl += opt.content ? '<div class="#{fix}-nav">#{content}</div>' : '';
        tpl += '<div></div>';
        return template(tpl, opt);
    };

    Message.prototype.bindClose = function(box, opt) {
        var that = this;
        setTimeout(function() {
            that.close(box, opt);
        }, 3000);
    };

    //绑定关闭事件
    Message.prototype.close = function(box, opt) {
        box.find('.jquery-message-box').addClass(mappings[opt.position][1]);
        setTimeout(function() {
            box.remove();
        }, 6000);
    };

    function template(template, data) {
        return template.replace(/#\{([\s\S]+?)\}/g, function(a, b) {
            return data[b];
        });
    }

    $.extend({
        message: function(opt) {
            return new Message(opt);
        }
    });
}());