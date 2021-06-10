/*
 *contextMenu.js v 1.4.0
 *Author: Sudhanshu Yadav
 *s-yadav.github.com
 *Copyright (c) 2013-2015 Sudhanshu Yadav.
 *Dual licensed under the MIT and GPL licenses
 */


; (function ($, window, document) {
	"use strict";

	//$.single = (function () {
	//	var single = $({});
	//	return function (elm) {
	//		single[0] = elm;
	//		return single;
	//	}
	//}());

	function callMethod(el, method, selector, option) {
		var myoptions = option;
		if (method != 'update') {
			option = iMethods.optionOptimizer(method, option);
			myoptions = $.extend({}, $.fn.contextMenu.defaults, option);
			if (!myoptions.baseTrigger) {
				myoptions.baseTrigger = el;
			}
		}
		methods[method].call(el, selector, myoptions);
		return this;
	}

	function getParentOffset(element) {
		var offset = { top: 0, left: 0 };
		while (element && getComputedStyle(element).getPropertyValue('position') != 'fixed') {
			element = element.offsetParent;
		}
		if (element) {
			offset.top = element.offsetTop;
			offset.left = element.offsetLeft;
		}
		return offset;
	}

	$.fn.contextMenu = function (selector, option) {
		return callMethod(this, 'popup', selector, option);
	};

	$.fn.contextMenu.defaults = {
		triggerOn: 'click', //avaliable options are all event related mouse plus enter option
		displayAround: 'cursor', // cursor or trigger
		mouseClick: 'left',
		closeOther: true, //to close other already opened context menu
		closeOnResize: true,
		closeOnScroll: true,
		closeOnClick: false, //close context menu on click/ trigger of any item in menu
		closeOnClickSelector: null,
		type: 'default',

		//callback
		beforeOpen: function (data, event) { return 0; },
		onOpen: function (data, event) { return $.when(); },
		beforeDisplay: function (data, event) { return 1; },
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

			iMethods.eventHandler.call(data.baseTrigger, e);
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
						menu.removeClass('iw-contextMenu ' + menuId).removeAttr('menuId').removeData('iw-menuData');
					}
				} else {
					menuData.noTrigger--;
					menu.data('iw-menuData', menuData);
				}
				trgr.unbind('.contextMenu').removeClass('iw-mTrigger').removeData('iw-menuData');
			});
		}
	};

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
					'trigger': trigger,
					'option': option
				};

				//to set data on selector
				menu.attr('menuId', menuId);
			} else {
				menuData.noTrigger++;
			}

			menu.data('iw-menuData', menuData);

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
				if (baseTrigger.index(trigger) != -1 && !option.closeOnClick) {
					baseTrigger.add(menu).bind('mouseleave.contextMenu', function (e) {
						if ($(e.relatedTarget).closest('.iw-contextMenu').length == 0) {
							iMethods.clearMenuStyle($('.iw-contextMenu'));
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
					var b = true;
					var submenu = false;
					var a = e.target;
					while (a && !(a instanceof HTMLBodyElement) && !(a instanceof HTMLDocument)) {
						if (a.classList.contains('iw-contextMenu')) {
							break;
						} else if (a.classList.contains('iw-mTrigger')) {
							submenu = true;
							break;
						}
						a = a.parentNode;
					}
					if (submenu) return;
					if (option.closeOnClickSelector) b = option.closeOnClickSelector(e.target);
					if (b) iMethods.closeContextMenu(option, trigger, menu, e);
				});
			}

			//var existing = document.body.querySelectorAll("#" + menu[0].id);
			//for (var i = existing.length - 1; i >= 0; i--) {
			//	if (existing[i].parentElement == document.body)
			//		document.body.removeChild(existing[i]);
			//}
			//document.body.appendChild(menu[0]);
		},
		eventHandler: function (e) {
			e.preventDefault();
			var trigger = $(this),
                trgrData = trigger.data('iw-menuData'),
                menu = trgrData.menu,
                menuData = menu.data('iw-menuData'),
                option = trgrData.option,
                clbckData = {
                	trigger: trigger,
                	menu: menu
                };

			var baseEl = option.baseTrigger ? option.baseTrigger : trigger;
			var res = option.beforeOpen.call(this, clbckData, e);

			if (res != 0 && res != 1)
				return;

			var menus = $('.iw-contextMenu').not(menu.selector);
			var a = trigger[0];
			var parentMenu = null;
			var parentMenuData = null;
			while (a && !(a instanceof HTMLBodyElement) && !(a instanceof HTMLDocument)) {
				if (a.classList.contains('iw-contextMenu')) {
					menus = menus.not('#' + a.id);
					if (!parentMenu) parentMenu = a;
				}
				a = a.parentNode;
			}

			if (parentMenu) {
				parentMenuData = $(parentMenu).data('iw-menuData');
				option.type = parentMenuData.option.type;
				option.displayAround = parentMenuData.option.displayAround;
			}

			if (res == 0) {
				//to close previous open menu.
				if (option.closeOther) {
					iMethods.clearMenuStyle(menus);
				}

				//to reset already selected menu item
				menu.find('.iw-mSelected').not(menu.selector).removeClass('iw-mSelected');
				$('.iw-opened').not(menu.selector).removeClass('iw-opened');
			}
			else if (res == 1) {
				//to close previous open menu.
				if (option.closeOther) {
					iMethods.clearMenuStyle($('.iw-contextMenu'));
				}
				//to reset already selected menu item
				menu.find('.iw-mSelected').removeClass('iw-mSelected');
				$('.iw-opened').removeClass('iw-opened');
				return;
			}

			menu.addClass('iw-opening');

			var openMenu = function () {
				if (!menu.hasClass('iw-opening'))
					return;

				var cObj = $(window),
					cHeight = cObj.innerHeight(),
					cWidth = cObj.innerWidth(),
					cTop = 0,
					cLeft = 0,
					left = null,
					top = null;

				menu.addClass('iw-type-' + option.type);

				if (!menuData.menuHeight) {
					var cloneMenu = menu.clone();
					cloneMenu.appendTo('body');
					menuData.menuWidth = cloneMenu.outerWidth(true);
					menuData.menuHeight = cloneMenu.outerHeight(true);
					cloneMenu.remove();
				}
				var menuHeight = menuData.menuHeight;
				var menuWidth = menuData.menuWidth;

				if (option.displayAround == 'cursor') {
					left = e.clientX;
					top = e.clientY;
				} else if (option.displayAround.startsWith('trigger')) {
					var rect = baseEl[0].getBoundingClientRect();
					var parentOffset = getParentOffset(baseEl[0]);

					var triggerHeight = baseEl.outerHeight(true),
						triggerWidth = baseEl.outerWidth(true),
						triggerLeft = rect.left - parentOffset.left,
						triggerTop = rect.top - parentOffset.top,
						leftShift = triggerWidth;

					if (option.displayAround == 'triggertop') {
						top = triggerTop - menuHeight;
						left = triggerLeft;
					} else if (option.displayAround == 'triggerleft') {
						top = triggerTop;
						left = triggerLeft - menuWidth;
					} else if (option.displayAround == 'triggerbottom') {
						top = triggerTop + triggerHeight;
						left = triggerLeft;
					} else if (option.displayAround == 'triggerright') {
						top = triggerTop;
						left = triggerLeft + triggerWidth;
					}
				}

				//applying css property
				var cssObj = { 'opacity': '1' };

				//max height and width of context menu
				if (top) {
					if (top + menuHeight > cHeight && top - menuHeight > 0) {
						top = top - menuHeight;
					}
					cssObj.top = top + 'px';
				}

				if (left) {
					if (left + menuWidth > cWidth && cWidth - menuWidth > 0) {
						left = cWidth - menuWidth;
					}
					cssObj.left = left + 'px';
				}

				if (menuHeight > cHeight - top)
					cssObj.bottom = '5px';
				else
					cssObj.bottom = '';

				var res = option.beforeDisplay.call(this, clbckData, e);
				if (res != 1) {
					iMethods.clearMenuStyle($('.iw-contextMenu'));
					return;
				}

				//if (parentMenu) {
					//cssObj.left = parentMenu.style.left;
					//cssObj.top = parentMenu.style.top;
					//var positionInfo = parentMenu.getBoundingClientRect();
					//if (positionInfo.width > menu.width())
					//	cssObj.width = positionInfo.width;
					//if (positionInfo.height > menu.height())
					//	cssObj.height = positionInfo.height;
					//parentMenu.classList.remove('iw-display');
				//}

				menu.removeClass('iw-opening');
				menu.css(cssObj).addClass('iw-display');

				//to call after open call back
				option.afterOpen.call(this, clbckData, e);
				option.baseTrigger.addClass('iw-opened');

				//to add current menu class
				//if (trigger.closest('.iw-contextMenu').length == 0) {
				//	$('.iw-curMenu').removeClass('iw-curMenu');
				//	menu.addClass('iw-curMenu');
				//}

				var dataParm = {
					trigger: trigger,
					menu: menu,
					option: option,
					method: trgrData.method
				};

				$('html').unbind('click', iMethods.clickEvent).click(dataParm, iMethods.clickEvent);

				if (option.closeOnResize) {
					$(window).bind('resize', dataParm, iMethods.scrollEvent);
				}
				if (option.closeOnScroll) {
					$(window).bind('scroll', dataParm, iMethods.scrollEvent);
				}
			};

			//call open callback
			option.onOpen.call(this, clbckData, e).done(openMenu);
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
			$('html').unbind('click', iMethods.clickEvent);
			$(window).unbind('scroll resize', iMethods.scrollEvent);
			iMethods.clearMenuStyle($('.iw-contextMenu'));
			$(document).focus();

			option.baseTrigger.removeClass('iw-opened');

			//call close function
			option.onClose.call(this, {
				trigger: trigger,
				menu: menu
			}, e);
		},
		optionOptimizer: function (method, option) {
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

			//if ($.inArray(option.triggerOn, ['hover', 'mouseenter', 'mouseover', 'mouseleave', 'mouseout', 'focusin', 'focusout']) != -1) {
			//	option.displayAround = 'trigger';
			//}
			return option;
		},
		clearMenuStyle: function (menu) {
			menu.removeClass('iw-display').removeClass('iw-opening').css('opacity', '0')
				//.removeClass('iw-curMenu')
				.removeClass('iw-type-default').removeClass('iw-type-slidermenu')
				.css('left', '').css('top', '').css('bottom', '').css('width', '').css('height', '');
		}
	};
})(jQuery, window, document);