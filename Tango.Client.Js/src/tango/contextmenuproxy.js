window.contextmenuproxy = function (au) {
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
			const popup = document.getElementById(popupid);
			if (popup) {
				const tr = popup.closest("tr");
				if (tr && window.getComputedStyle(tr)["position"] == "sticky") {
					const tbl = tr.closest('table');
					const tbodymenu = tbl.querySelector('tbodymenu');
					if (!tbodymenu) {
						tbodymenu = document.createElement("tbody");
						tbodymenu.classList.add("tbodymenu");
						tbl.insertBefore(tbodymenu, tbl.firstElementChild);
						tbodymenu.appendChild(document.createElement("tr"));
						tbodymenu.firstElementChild.appendChild(document.createElement("td"));
					}
					tbodymenu.firstElementChild.firstElementChild.appendChild(popup);
				}
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
				onClose: onClose,
				onMouseLeave: onMouseLeave,
				type: args.type,
				delay: args.delay,
				showOverlay: args.showoverlay && args.showoverlay == "true",
				width: args.width
			};
		}
	};


	function onOpen(data, event) {
		const menu = data.menu[0];
		if (menu.getAttribute('data-href') || menu.getAttribute('data-e')) {
			return au.runEventFromElementWithApiResponse(menu);
		}
		if (data.options.showOverlay) {
			document.documentElement.classList.add('lock-position-html');
			body.classList.add('lock-position-body');
		}

		return $.when();
	}

	function onClose(data, event) {
		if (data.options.showOverlay) {
			document.documentElement.classList.remove('lock-position-html');
			body.classList.remove('lock-position-body');
		}
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

window.contextmenuproxy_closeonlink = function () {
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

window.sidebarmenuproxy = function () {
	var instance = {
		init: function (args) {
			var parms = contextmenuproxy.parms(args);
			parms.closeOnClickSelector = function (el) {
				return el instanceof HTMLAnchorElement || el.classList.contains('sidebarmenu-background');
			};
			contextmenuproxy.contextMenu(args.triggerid, args.popupid, parms, args.storeparms)
		}
	};

	return instance;
}();