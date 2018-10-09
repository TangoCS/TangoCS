/*
 *contextMenu.js v 1.4.0
 *Author: Sudhanshu Yadav
 *s-yadav.github.com
 *Copyright (c) 2013-2015 Sudhanshu Yadav.
 *Dual licensed under the MIT and GPL licenses
 */
; (function ($, window, document, undefined) {
	"use strict";

	$.single = (function () {
		var single = $({});
		return function (elm) {
			single[0] = elm;
			return single;
		}
	}());

	$.fn.contextMenu = function (method, selector, option) {
		//parameter fix
		if (!methods[method]) {
			option = selector;
			selector = method;
			method = 'popup';
		}

			//need to check for array object
		else if (selector) {
			if (!((selector instanceof Array) || (typeof selector === 'string') || (selector.nodeType) || (selector.jquery))) {
				option = selector;
				selector = null;
			}
		}

		var myoptions = option;
		if (method != 'update') {
			option = iMethods.optionOtimizer(method, option);
			myoptions = $.extend({}, $.fn.contextMenu.defaults, option);
			if (!myoptions.baseTrigger) {
				myoptions.baseTrigger = this;
			}
		}
		methods[method].call(this, selector, myoptions);
		return this;
	};

	$.fn.contextMenu.defaults = {
		triggerOn: 'click', //avaliable options are all event related mouse plus enter option
		displayAround: 'cursor', // cursor or trigger
		mouseClick: 'left',
		verAdjust: 0,
		horAdjust: 0,
		top: 'auto',
		left: 'auto',
		closeOther: true, //to close other already opened context menu
		containment: window,
		winEventClose: true,
		position: 'auto', //allowed values are top, left, bottom and right
		closeOnClick: false, //close context menu on click/ trigger of any item in menu
		delayedTrigger: false,

		//callback
		beforeOpen: function (data, event) { return 0; },
		onOpen: function (data, event) { return $.when(); },
		afterOpen: function (data, event) { },
		onClose: function (data, event) { }
	};

	var methods = {
		menu: function (selector, option) {
			var trigger = $(this);
			iMethods.contextMenuBind.call(this, selector, option, 'menu');
		},
		popup: function (selector, option) {
			$(selector).addClass('iw-contextMenu');
			iMethods.contextMenuBind.call(this, selector, option, 'popup');
		},
		update: function (selector, option) {
			var self = this;
			option = option || {};

			this.each(function () {
				var trgr = $(this),
                    menuData = trgr.data('iw-menuData');
				//refresh if any new element is added
				if (!menuData) {
					self.contextMenu('refresh');
					menuData = trgr.data('iw-menuData');
				}
				//bind event again if trigger option has changed.
				var triggerOn = option.triggerOn;
				if (triggerOn) {
					trgr.unbind('.contextMenu');

					//add contextMenu identifier on all events
					triggerOn = triggerOn.split(" ");
					var events = [];
					for (var i = 0, ln = triggerOn.length; i < ln; i++) {
						events.push(triggerOn[i] + '.contextMenu')
					}

					//to bind event
					trgr.bind(events.join(' '), iMethods.eventHandler);
				}

				//set menu data back to trigger element
				menuData.option = $.extend({}, menuData.option, option);
				trgr.data('iw-menuData', menuData);
			});
		},
		refresh: function () {
			var menuData = this.filter(function () {
				return !!$(this).data('iw-menuData');
			}).data('iw-menuData'),
                newElm = this.filter(function () {
                	return !$(this).data('iw-menuData');
                });
			//to change basetrigger on refresh  
			menuData.option.baseTrigger = this;
			iMethods.contextMenuBind.call(newElm, menuData.menuSelector, menuData.option);
		},
		open: function (sel, data) {
			data = data || {};
			var e = data.event || $.Event('click');
			if (data.top) e.clientY = data.top;
			if (data.left) e.clientX = data.left;
			this.each(function () {
				iMethods.eventHandler.call(this, e);
			});
		},
		//to force context menu to close
		close: function () {
			var menuData = this.data('iw-menuData');
			if (menuData) {
				iMethods.closeContextMenu(menuData.option, this, menuData.menu, null);
			}
		},
		destroy: function () {
			this.each(function () {
				var trgr = $(this),
                    menuId = trgr.data('iw-menuData').menuId,
                    menu = $('.iw-contextMenu[menuId=' + menuId + ']'),
                    menuData = menu.data('iw-menuData');

				//Handle the situation of dynamically added element.
				if (!menuData) return;

				if (menuData.noTrigger == 1) {
					if (menu.hasClass('iw-created')) {
						menu.remove();
					} else {
						menu.removeClass('iw-contextMenu ' + menuId)
                            .removeAttr('menuId').removeData('iw-menuData');
						//to destroy submenus
						//menu.find('li.iw-mTrigger').contextMenu('destroy');
					}
				} else {
					menuData.noTrigger--;
					menu.data('iw-menuData', menuData);
				}
				trgr.unbind('.contextMenu').removeClass('iw-mTrigger').removeData('iw-menuData');
			});
		}
	};
	var timer = null;
	var iMethods = {
		contextMenuBind: function (selector, option, method) {
			var trigger = this,
                menu = $(selector),
                menuData = menu.data('iw-menuData');

			//fallback
			if (menu.length == 0) {
				menu = trigger.find(selector);
				if (menu.length == 0) {
					return;
				}
			}

			//get base trigger
			var baseTrigger = option.baseTrigger;

			if (!menuData) {
				var menuId;
				if (!baseTrigger.data('iw-menuData')) {
					menuId = Math.ceil(Math.random() * 100000);
					baseTrigger.data('iw-menuData', {
						'menuId': menuId
					});
				} else {
					menuId = baseTrigger.data('iw-menuData').menuId;
				}

				menuData = {
					'menuId': menuId,
					'noTrigger': 1,
					'trigger': trigger
				};

				//to set data on selector
				menu.data('iw-menuData', menuData).attr('menuId', menuId);
			} else {
				menuData.noTrigger++;
				menu.data('iw-menuData', menuData);
			}

			//to set data on trigger
			trigger.addClass('iw-mTrigger').data('iw-menuData', {
				'menuId': menuData.menuId,
				'option': option,
				'menu': menu,
				'menuSelector': selector,
				'method': method
			});

			//hover fix
			var triggerOn = option.triggerOn;
			if (triggerOn.indexOf('hover') != -1) {
				triggerOn = triggerOn.replace('hover', 'mouseenter');
				//hover out if display is of context menu is on hover
				if (baseTrigger.index(trigger) != -1) {
					baseTrigger.add(menu).bind('mouseleave.contextMenu', function (e) {
						if ($(e.relatedTarget).closest('.iw-contextMenu').length == 0) {
							$('.iw-contextMenu[menuId="' + menuData.menuId + '"]').fadeOut(100);
						}
					});
				}
			}

			trigger.delegate('input,a,.needs-click', 'click', function (e) {
				e.stopImmediatePropagation()
			});

			//add contextMenu identifier on all events
			triggerOn = triggerOn.split(' ');
			var events = [];
			for (var i = 0, ln = triggerOn.length; i < ln; i++) {
				events.push(triggerOn[i] + '.contextMenu')
			}

			//to bind event
			trigger.bind(events.join(' '), iMethods.eventHandler);

			//to stop bubbling in menu
			menu.bind('click mouseenter', function (e) {
				e.stopPropagation();
			});

			if (option.closeOnClick) {
				menu.bind('click', function (e) {
					iMethods.closeContextMenu(option, trigger, menu, e);
				});
			}

			var existing = document.body.querySelectorAll("#" + menu[0].id);
			for (var i = existing.length - 1; i >= 0; i--) {
				if (existing[i].parentElement == document.body)
					document.body.removeChild(existing[i]);
			}
			document.body.appendChild(menu[0]);
		},
		eventHandler: function (e) {
			e.preventDefault();
			var trigger = $(this),
                trgrData = trigger.data('iw-menuData'),
                menu = trgrData.menu,
                menuData = menu.data('iw-menuData'),
                option = trgrData.option,
                cntnmnt = option.containment,
                clbckData = {
                	trigger: trigger,
                	menu: menu
                },
                //check conditions
                cntWin = cntnmnt == window,
                btChck = option.baseTrigger.index(trigger) == -1;

			var res = option.beforeOpen.call(this, clbckData, e);

			if (res == 0) {
				//to close previous open menu.
				if (!btChck && option.closeOther) {
					$('.iw-contextMenu').not(menu.selector).removeClass('iw-display');
				}

				//to reset already selected menu item
				menu.find('.iw-mSelected').not(menu.selector).removeClass('iw-mSelected');
				$('.iw-opened').not(menu.selector).removeClass('iw-opened');
			}
			else if (res == 1) {
				//to close previous open menu.
				if (!btChck && option.closeOther) {
					$('.iw-contextMenu').removeClass('iw-display');
				}
				//to reset already selected menu item
				menu.find('.iw-mSelected').removeClass('iw-mSelected');
				$('.iw-opened').removeClass('iw-opened');
				return;
			}
			else
				return;

			var openMenu = function () {
				var cObj = $(cntnmnt),
					cHeight = cObj.innerHeight(),
					cWidth = cObj.innerWidth(),
					cTop = 0,
					cLeft = 0,
					va, ha,
					left = 0,
					top = 0,
					bottomMenu,
					rightMenu,
					verAdjust = va = parseInt(option.verAdjust),
					horAdjust = ha = parseInt(option.horAdjust);

				if (!cntWin) {
					cTop = cObj.offset().top;
					cLeft = cObj.offset().left;

					//to add relative position if no position is defined on containment
					if (cObj.css('position') == 'static') {
						cObj.css('position', 'relative');
					}
				}

				if (!menuData.menuHeight) {
					var cloneMenu = menu.clone();
					cloneMenu.appendTo('body');
					menuData.menuWidth = cloneMenu.outerWidth(true);
					menuData.menuHeight = cloneMenu.outerHeight(true);
					cloneMenu.remove();
				}
				var menuHeight = menuData.menuHeight;
				var menuWidth = menuData.menuWidth;
				option.triggerVisibility = trigger[0].style.visibility;
				trigger.css('visibility', 'visible');

				if (option.displayAround == 'cursor') {
					left = cntWin ? e.clientX : e.clientX + $(window).scrollLeft() - cLeft;
					top = cntWin ? e.clientY : e.clientY + $(window).scrollTop() - cTop;
				} else if (option.displayAround == 'trigger') {
					var triggerHeight = trigger.outerHeight(true),
						triggerWidth = trigger.outerWidth(true),
						triggerLeft = cntWin ? trigger.offset().left - cObj.scrollLeft() : trigger.offset().left - cLeft,
						triggerTop = cntWin ? trigger.offset().top - cObj.scrollTop() : trigger.offset().top - cTop,
						leftShift = triggerWidth;

					if (option.position == 'top') {
						top = triggerTop - menuHeight;
						va = verAdjust;
						left = triggerLeft;
					} else if (option.position == 'left') {
						top = triggerTop;
						left = triggerLeft - menuWidth;
						ha = horAdjust;
					} else if (option.position == 'bottom') {
						top = triggerTop + triggerHeight;
						va = verAdjust;
						left = triggerLeft;
					} else if (option.position == 'right') {
						top = triggerTop;
						left = triggerLeft + triggerWidth;
						ha = horAdjust;
					}
				}

				bottomMenu = top + menuHeight;
				rightMenu = left + menuWidth;

				//max height and width of context menu
				if (bottomMenu > cHeight) {
					va = -1 * va;
					top = top - menuHeight;
				}
				if (rightMenu > cWidth) {
					ha = -1 * ha;
					left = cWidth - menuWidth;
				}


				//applying css property
				var cssObj = {
					'position': (cntWin || btChck) ? 'fixed' : 'absolute',
					'height': '',
					'width': ''
				};


				//to get position from offset parent
				if (option.left != 'auto') {
					left = iMethods.getPxSize(option.left, cWidth);
				}
				if (option.top != 'auto') {
					top = iMethods.getPxSize(option.top, cHeight);
				}
				if (!cntWin) {
					var oParPos = trigger.offsetParent().offset();
					if (btChck) {
						left = left + cLeft - $(window).scrollLeft();
						top = top + cTop - $(window).scrollTop();
					} else {
						left = left - (cLeft - oParPos.left);
						top = top - (cTop - oParPos.top);
					}
				}
				cssObj.left = left + ha + 'px';
				cssObj.top = top + va + 'px';

				menu.css(cssObj).addClass('iw-display');

				//to call after open call back
				option.afterOpen.call(this, clbckData, e);
				option.baseTrigger.addClass('iw-opened');

				//to add current menu class
				if (trigger.closest('.iw-contextMenu').length == 0) {
					$('.iw-curMenu').removeClass('iw-curMenu');
					menu.addClass('iw-curMenu');
				}


				var dataParm = {
					trigger: trigger,
					menu: menu,
					option: option,
					method: trgrData.method
				};
				$('html').unbind('click', iMethods.clickEvent).click(dataParm, iMethods.clickEvent);
				//$(document).unbind('keydown', iMethods.keyEvent).keydown(dataParm, iMethods.keyEvent);
				if (option.winEventClose) {
					$(window).bind('scroll resize', dataParm, iMethods.scrollEvent);
				}
			};

			//call open callback
			if (option.delayedTrigger) {
				if (timer) {
					window.clearTimeout(timer);
					timer = null;
				}
				timer = window.setTimeout(function () { option.onOpen.call(this, clbckData, e).done(openMenu); }, 300);
			}
			else {
				option.onOpen.call(this, clbckData, e).done(openMenu);
			}
		},

		scrollEvent: function (e) {
			iMethods.closeContextMenu(e.data.option, e.data.trigger, e.data.menu, e);
		},

		clickEvent: function (e) {
			var button = e.data.trigger.get(0);

			if ((button !== e.target) && ($(e.target).closest('.iw-mTrigger').length == 0)) {
				iMethods.closeContextMenu(e.data.option, e.data.trigger, e.data.menu, e);
			}
		},
		closeContextMenu: function (option, trigger, menu, e) {

			//unbind all events from top DOM
			//$(document).unbind('keydown', iMethods.keyEvent);
			$('html').unbind('click', iMethods.clickEvent);
			$(window).unbind('scroll resize', iMethods.scrollEvent);
			$('.iw-contextMenu').removeClass('iw-display');
			$(document).focus();

			option.baseTrigger.removeClass('iw-opened');
			option.baseTrigger.css('visibility', option.triggerVisibility ? option.triggerVisibility : '');

			//call close function
			option.onClose.call(this, {
				trigger: trigger,
				menu: menu
			}, e);
		},
		getPxSize: function (size, of) {
			if (!isNaN(size)) {
				return size;
			}
			if (size.indexOf('%') != -1) {
				return parseInt(size) * of / 100;
			} else {
				return parseInt(size);
			}
		},
		optionOtimizer: function (method, option) {
			if (!option) {
				return;
			}
			if (method == 'menu') {
				if (!option.mouseClick) {
					option.mouseClick = 'right';
				}
			}
			if ((option.mouseClick == 'right') && (option.triggerOn == 'click')) {
				option.triggerOn = 'contextmenu';
			}

			if ($.inArray(option.triggerOn, ['hover', 'mouseenter', 'mouseover', 'mouseleave', 'mouseout', 'focusin', 'focusout']) != -1) {
				option.displayAround = 'trigger';
			}
			return option;
		}
	};
})(jQuery, window, document);