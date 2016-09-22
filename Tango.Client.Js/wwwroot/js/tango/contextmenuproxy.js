var contextmenuproxy = function () {
	var instance = {
		init: function (args) {
			$('#' + args.triggerid).contextMenu('#' + args.popupid, {
				triggerOn: args.triggerson,
				displayAround: args.displaysaround,
				position: args.position,
				onOpen: function (data, event) {
					var url = data.menu[0].getAttribute('data-url');
					return url ? ajaxUtils.runEventWithApiResponse({ url: url }, { element: data.menu[0].id }) : $.when();
				}
			});
		}
	};

	return instance;
}();