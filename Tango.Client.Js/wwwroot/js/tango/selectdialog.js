var selectSingleObjectDialog = function (au, cu) {
	var instance = {
		clear: function (id, clear) {
			document.getElementById(id).value = '';
			if (clear) {
				const field = document.getElementById(id + '_selected');
				if (field) field.innerHTML = '';
			}
			const state = au.state.ctrl[id + '_str'];
			if (state) state.selectedvalue = '';
		},
		widgetWillMount: function (shadow, state) {
			const root = shadow.getElementById(state.root);

			const checks = root.getElementsByTagName('input');
			for (var i = 0; i < checks.length; i++) {
				const el = checks[i];
				el.checked = state.selectedvalue == el.value;
				el.addEventListener("change", function () {
					state.selectedvalue = el.value;
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
				state.selectedvalue = '';
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
		clear: function (id, clear) {
			document.getElementById(id).value = '';
			if (clear) {
				const field = document.getElementById(id + '_selected');
				if (field) field.innerHTML = '';
			}
			const state = au.state.ctrl[id + '_str'];
			if (state) state.selectedvalues.length = 0;
		},
		widgetWillMount: function (shadow, state) {
			const root = shadow.getElementById(state.root);

			const checks = root.getElementsByTagName('input');
			for (var i = 0; i < checks.length; i++) {
				const el = checks[i];
				el.checked = state.selectedvalues.indexOf(el.value) >= 0;
				el.addEventListener("change", function () {
					if (el.checked) {
						state.selectedvalues.push(el.value);
					}
					else {
						const index = state.selectedvalues.indexOf(el.value);
						state.selectedvalues.splice(index, 1);
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
				state.selectedvalues.length = 0;
		},
		pagingEvent: function (caller, id) {
			au.postEventFromElementWithApiResponse(caller, { e: 'renderlist', r: id }).done(function () {
				instance.setupItems(caller, id);
			});
		}
	}
	return instance;
}(ajaxUtils, commonUtils);