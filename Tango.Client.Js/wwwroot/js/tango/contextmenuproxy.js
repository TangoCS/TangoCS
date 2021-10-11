var contextmenuproxy = function (au) {
	var instance = {
		init: function (args) {
			const parms = instance.parms(args);
			instance.contextMenu(args.triggerid, args.popupid, parms, args.storeparms)
		},
		contextMenu: function (triggerid, popupid, parms, storeparms) {
			if (storeparms == 'true') {
				if (!au.state.ctrl['$contextmenu'])
					au.state.ctrl['$contextmenu'] = {};
				au.state.ctrl['$contextmenu'][triggerid] = {
					popupid: popupid,
					parms: parms
				};
			}
			$('#' + triggerid).contextMenu('#' + popupid, parms);
		},
		parms: function (args) {
			return {
				triggerOn: args.triggeron,
				displayAround: args.displaysaround,
				position: args.position,
				closeOnClick: args.closeonclick,
				closeOnScroll: args.closeonscroll === undefined ? true : args.closeonscroll,
				onOpen: onOpen,
				onMouseLeave: onMouseLeave,
				type: args.type,
				delay: args.delay
			};
		}
	};


	function onOpen(data, event) {
		const menu = data.menu[0];
		if (menu.getAttribute('data-href') || menu.getAttribute('data-e')) {
			return au.runEventFromElementWithApiResponse(menu);
		}

		return $.when();
	}

	function onMouseLeave(data, event) {
		if (data.options.type = 'hover') {
			const menu = data.menu[0];

			data.state.resetTimer();

			if (menu.hasAttribute('data-requestgroup')) {
				const reqGr = menu.getAttribute('data-requestgroup');
				au.state.com.request[reqGr] = null;
			}
		}
	}

	return instance;
}(ajaxUtils);

var contextmenuproxy_closeonlink = function () {
	var instance = {
		init: function (args) {
			var parms = contextmenuproxy.parms(args);
			parms.closeOnClickSelector = function (el) {
				return el instanceof HTMLAnchorElement;
			};
			contextmenuproxy.contextMenu(args.triggerid, args.popupid, parms, args.storeparms)
		}
	};

	return instance;
}();