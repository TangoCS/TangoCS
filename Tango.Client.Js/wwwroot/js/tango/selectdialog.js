var selectSingleObjectDialog = function (au, cu) {
	var instance = {
		clear: function (id) {
			document.getElementById(id).value = '';
			document.getElementById(id + '_selected').innerHTML = '';
			const state = au.state.ctrl[id + '_str'];
			if (state) state.selectedObj = [];
		},
		widgetWillMount: function (shadow, state) {
			const root = shadow.getElementById(state.root);
			const valid = root.getAttribute('data-val');

			const checks = root.getElementsByTagName('input');
			for (var i = 0; i < checks.length; i++) {
				const el = checks[i];
				el.checked = state.selectedObj == el.value;
				el.addEventListener("change", function () {
					state.selectedObj = el.value;
				});
			}

			const filter = (shadow.getElementById(root.id + '_filter') || document.getElementById(root.id + '_filter'));

			filter.addEventListener('keyup', function () {
				au.delay(filter, function (caller) {
					au.postEventFromElementWithApiResponse(caller, { e: 'renderlist', r: root.id });
				});
			});

			cu.setFocus(filter);
		},
		onResult: function (res, state) {
			if (res == 0)
				delete state.selectedObjs;
			else if (res == 1) {
				const root = document.getElementById(state.root);
				const val = document.getElementById(root.getAttribute('data-val'));

				val.value = state.selectedObj;

				var data = {};
				data[val.id] = state.selectedObj;
				au.postEventWithApiResponse({ e: 'submitdialog', r: root.id, data: data });
				return false;
			}
		},
		pagingEvent: function (caller, id) {
			au.runEventFromElementWithApiResponse(caller, { e: 'renderlist', r: id }).done(function () {
				instance.setupItems(caller, id);
			});
		}
	}

	return instance;
}(ajaxUtils, commonUtils);


var selectMultipleObjectsDialog = function (au, cu) {
	var instance = {
		clear: function (id) {
			document.getElementById(id).value = '';
			document.getElementById(id + '_selected').innerHTML = '';
			const state = au.state.ctrl[id + '_str'];
			if (state) state.selectedObjs = [];
		},
		widgetWillMount: function (shadow, state) {
			const root = shadow.getElementById(state.root);
			const valid = root.getAttribute('data-val');

			if (!state.selectedObjs) {
				const val = (shadow.getElementById(valid) || document.getElementById(valid)).value;
				if (val && val != '')
					state.selectedObjs = val.split(",");
				else
					state.selectedObjs = [];
			}

			const checks = root.getElementsByTagName('input');
			for (var i = 0; i < checks.length; i++) {
				const el = checks[i];
				el.checked = state.selectedObjs.indexOf(el.value) >= 0;
				el.addEventListener("change", function () {
					if (el.checked) {
						state.selectedObjs.push(el.value);
					}
					else {
						const index = state.selectedObjs.indexOf(el.value);
						state.selectedObjs.splice(index, 1);
					}
				});
			}

			const filter = (shadow.getElementById(root.id + '_filter') || document.getElementById(root.id + '_filter'));

			filter.addEventListener('keyup', function () {
				au.delay(filter, function (caller) {
					au.postEventFromElementWithApiResponse(caller, { e: 'renderlist', r: root.id });
				});
			});

			cu.setFocus(filter);
		},
		onResult: function (res, state) {
			if (res == 0)
				delete state.selectedObjs;
			else if (res == 1) {
				const root = document.getElementById(state.root);
				const val = document.getElementById(root.getAttribute('data-val'));

				val.value = state.selectedObjs.join();

				var data = {};
				data[val.id] = state.selectedObjs;
				au.postEventWithApiResponse({ e: 'submitdialog', r: root.id, data: data });
				return true;
			}
		},
		pagingEvent: function (caller, id) {
			au.postEventFromElementWithApiResponse(caller, { e: 'renderlist', r: id }).done(function () {
				instance.setupItems(caller, id);
			});
		}
	}
	return instance;
}(ajaxUtils, commonUtils);