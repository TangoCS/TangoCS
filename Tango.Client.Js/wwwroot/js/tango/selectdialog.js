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

var selectSingleObjectDialog2 = function (au, cu) {
	var instance = {
		placeholderInit: function (id) {
			const elPh = document.getElementById(id + '_placeholder');
			const elFilter = document.getElementById(id + '_filter');
			const elSel = document.getElementById(id + '_selected');
			const elPopup = document.getElementById(id + '_popup');

			const opt = {
				triggerOn: 'keyup',
				displayAround: 'trigger',
				position: 'bottom',
				delayedTrigger: true,
				onOpen: function (data, event) {
					return au.postEventFromElementWithApiResponse(data.menu[0]);
				},
				closeOnClick: true,
				closeOnClickSelector: function (el) {
					return el.hasAttribute('data-res-postponed') && el.getAttribute('data-res-postponed') == '1';
				},
				baseTrigger: $('#' + id + '_placeholder'),
				beforeDisplay: function (data, event) {
					return data.menu.text() == '' ? 0 : 1;
				}
			};
			opt.beforeOpen = function (data, event) {
				if (event.type == 'click') return 0;
				var v = data.trigger.val();
				if (event.keyCode == 8) return v == '' ? 1 : 0;
				if (event.keyCode == 27 || event.keyCode == 13) return 1;
				if (event.keyCode == 33 || event.keyCode == 34 || event.keyCode == 37 || event.keyCode == 38 || event.keyCode == 39) return 2;
				if (event.keyCode == 40) return elPh.classList.contains('iw-opened') ? 2 : 0;
				if (v == '') return 1;

				return 0;
			}

			$('#' + id + '_filter').contextMenu('#' + id + '_popup', opt);
			opt.triggerOn = 'click';
			$('#' + id + '_btn').contextMenu('#' + id + '_popup', opt);

			elPh.addEventListener('click', function (e) {
				if (e.target.id != elSel.id) elFilter.focus();
			});

			elFilter.addEventListener('click', function (e) {
				e.stopPropagation();
			});

			elFilter.addEventListener('keydown', function (e) {
				if (e.keyCode == 8 && elFilter.value == '') {
					instance.clear(id, true);
					return;
				}

				if (elPh.classList.contains('iw-opened')) {
					var cur = elPopup.querySelector('label.selected');
					if (!cur) cur = elPopup.querySelector('label');

					if (e.keyCode == 38) {
						e.preventDefault();
						if (cur.previousSibling instanceof HTMLLabelElement) {
							cur.classList.remove('selected');
							cur.previousSibling.classList.add('selected');
						}
					}
					else if (e.keyCode == 40) {
						e.preventDefault();
						if (cur.nextSibling instanceof HTMLLabelElement) {
							cur.classList.remove('selected');
							cur.nextSibling.classList.add('selected');
						}
					}
					else if (e.keyCode == 33) {
						e.preventDefault();
						var pg = elPopup.querySelector('#' + id + '_str_pgup');
						if (pg) au.postEventFromElementWithApiResponse(pg);
					}
					else if (e.keyCode == 34) {
						e.preventDefault();
						var pg = elPopup.querySelector('#' + id + '_str_pgdown');
						if (pg) au.postEventFromElementWithApiResponse(pg);
					}
					else if (e.keyCode == 13) {
						cur.firstChild.checked = true;
						cur.firstChild.onchange();
					}
				}
			});

			elFilter.addEventListener('input', function (w) {
				var spanElm = this.nextElementSibling;
				spanElm.textContent = this.value; // the hidden span takes the value of the input; 
				var w = spanElm.offsetWidth + 15;
				if (w < 25) w = 25;
				if (w > this.parentElement.offsetWidth - 26) return;
				this.style.width = w + 'px'; // apply width of the span to the input
			});
		},
		clear: function (id) {
			document.getElementById(id).value = '';
			const field = document.getElementById(id + '_selected');
			if (field) {
				field.innerText = '';
				if (!field.classList.contains('empty'))
					field.classList.add('empty');
			}
			const state = au.state.ctrl[id];
			if (state) state.selectedvalue = '';
		},
		widgetWillMount: function (shadow, state) {
			const root = shadow.getElementById(state.root + '_str');

			const checks = root.getElementsByTagName('input');
			for (var i = 0; i < checks.length; i++) {
				const el = checks[i];
				el.checked = state.selectedvalue == el.value;
				el.addEventListener("change", function () {
					state.selectedvalue = el.value;
				});
			}
		},
		widgetDidMount: function (state) {
			const elPopup = document.getElementById(state.root + '_popup');
			const rows = elPopup.getElementsByTagName('label');
			if (rows.length > 0) rows[0].classList.add('selected');

			for (var i = 0; i < rows.length; i++) {
				const el = rows[i];
				el.addEventListener('mousemove', function (e) {
					const cur = elPopup.querySelector('label.selected');
					if (cur == e.target) return;
					if (cur) cur.classList.remove('selected');
					e.target.classList.add('selected');
				});
			}

			const elFilter = document.getElementById(state.root + '_filter');
			elFilter.focus();
		},
		onResult: function (res, state) {
			if (res == 0)
				state.selectedvalue = '';
			else if (res == 1) {
				const elFilter = document.getElementById(state.root + '_filter');
				elFilter.value = '';
			}
		}
	}

	return instance;
}(ajaxUtils, commonUtils);
