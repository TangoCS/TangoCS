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
			const tr = popup.closest("tr");
			if (tr && window.getComputedStyle(tr)["position"] == "sticky") {
				const tbl = tr.closest('table');
				if (tbl.getElementsByClassName("tbodymenu").length == 0) {
					const tbodymenu = document.createElement("tbody");
					tbodymenu.classList.add("tbodymenu");
					tbl.insertBefore(tbodymenu, tbl.firstElementChild);
					tbodymenu.appendChild(document.createElement("tr"));
					tbodymenu.firstElementChild.appendChild(document.createElement("td"));
				}
				tbl.getElementsByClassName("tbodymenu")[0].firstElementChild.firstElementChild.appendChild(popup);
			}
			window.setTimeout(function () { $('#' + triggerid).contextMenu('#' + popupid, parms); }, 1);
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