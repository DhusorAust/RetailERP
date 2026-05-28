
(function ($) {
	$.fn.confettiButton = function(options) {
		
		var settings = $.extend({
			num: 60,
			colorArray: ['#FF5566', '#FFFFFF', '#F1C40F', '#2EBFF6', '#2ECC71'],
			minX: 5,
			minY: 5,
			maxX: 120,
			maxY: 120,
			minScale: 50,
			maxScale: 150,
			speed: 1,
			rotation: true,
			hoverOnly: false,
		}, options);
		
		var num = settings.num;
		var colorArray = settings.colorArray;
		var minX = settings.minX;
		var minY = settings.minY;
		var maxX = settings.maxX;
		var maxY = settings.maxY;
		var minScale = settings.minScale / 10;
		var maxScale = settings.maxScale / 10;
		var speed = settings.speed;
		var rotation = settings.rotation;
		var hover = settings.hoverOnly;
		
		var hoverClass = (hover == true) ? ' only-hover' : '';
		
		this.each(function() {
			
			var _this = $(this);
			_this.addClass('vnm-confetti-button');
			var confettiContainer = $('<span class="vnm-confetti" />').appendTo(_this);
			
			for (var i = 0; i < num; i++) {
				var bgColor = colorArray[Math.floor(Math.random() * colorArray.length)];
				
				var translateX = Math.floor(Math.random() * (maxX - minX)) + minX;
				var translateY = Math.floor(Math.random() * (maxX - minX)) + minX;
				
				var scale = (Math.floor(Math.random() * (maxScale - minScale)) + minScale) / 10;
				var rotate = (rotation == true) ? Math.floor(Math.random() * 90) : 0;
				
				var xRand = Math.random() >= 0.5;
				var yRand = Math.random() >= 0.5;
				
				if (!xRand) {
					translateX = -translateX;
				}
				
				if (!yRand) {
					translateY = -translateY;
				}
				
				var time = speed + Math.floor(Math.random() * 100) * 0.01;
				var delay = -((Math.floor(Math.random() * 100) * 0.01) * time);
				
				var confetto = $('<span class="confetto' + hoverClass + '" />').appendTo(confettiContainer);
				
				confetto.css({
					'background-color': bgColor,
					'transform': 'translate(' + translateX + 'px, ' + translateY + 'px)' + ' rotate(' + rotate + 'deg) scale(' + scale + ')',
					'animation-duration': time + 's',
					'animation-delay': delay + 's',
				});
			}
		});
	};
}(jQuery));