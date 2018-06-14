var contextmenuproxy = function () {
	var instance = {
		init: function (args) {
			$('#' + args.triggerid).contextMenu('#' + args.popupid, {
				triggerOn: args.triggerson,
				displayAround: args.displaysaround,
				position: args.position,
				onOpen: function (data, event) {
					if (data.menu[0].getAttribute('data-s') || data.menu[0].getAttribute('data-e'))
						return ajaxUtils.runEventFromElementWithApiResponse(data.menu[0]);

					return $.when();
				}
			});
		}
	};

	return instance;
}();