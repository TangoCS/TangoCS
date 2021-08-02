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
				type: args.type,
				delay: args.delay
			};
		}
	};


	function onOpen (data, event) {
		if (data.menu[0].getAttribute('data-href') || data.menu[0].getAttribute('data-e'))
			return au.runEventFromElementWithApiResponse(data.menu[0]);

		return $.when();
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