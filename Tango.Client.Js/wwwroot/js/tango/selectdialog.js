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
			if (state) state.selectedvalues.splice(0, state.selectedvalues.length);
		},
		widgetDidMount: function (state) {
			const root = document.getElementById(state.root);

			if (!state.dialogvalues) state.dialogvalues = state.selectedvalues.slice();

			const checks = root.getElementsByTagName('input');
			for (var i = 0; i < checks.length; i++) {
				const el = checks[i];
				el.checked = state.dialogvalues.indexOf(el.value) >= 0;
				el.addEventListener("change", function () {
					if (el.checked) {
						state.dialogvalues.push(el.value);
					}
					else {
						const index = state.dialogvalues.indexOf(el.value);
						state.dialogvalues.splice(index, 1);
					}
				});
			}

			const filter = document.getElementById(root.id + '_filter');

			filter.addEventListener('keyup', function () {
				au.delay(filter, function (caller) {
					au.postEventFromElementWithApiResponse(caller, { e: 'renderlist', r: root.id });
				});
			});

			cu.setFocus(filter);
		},
		onResult: function (res, state) {
			if (res == 1) {
				state.selectedvalues.splice(0, state.selectedvalues.length);
				for (var i = 0; i < state.dialogvalues.length; i++) {
					state.selectedvalues.push(state.dialogvalues[i]);
				}
			}
			delete state.dialogvalues;
		},
		pagingEvent: function (caller, id) {
			au.postEventFromElementWithApiResponse(caller, { e: 'renderlist', r: id }).done(function () {
				instance.setupItems(caller, id);
			});
		},
		selectAll: function(id) {
			const container = document.querySelector("#" + id);
			const checkBoxes = container.getElementsByTagName('input');
			const state = au.state.ctrl[id];

			if (!state.dialogvalues) state.dialogvalues = state.selectedvalues.slice();
			checkBoxes.forEach(function(element) {
				element.checked = true;
				if (!state.dialogvalues.includes(element.value)) {
					state.dialogvalues.push(element.value);
				}
			});
        },
        selectNone: function (id) {
            const container = document.querySelector("#" + id);
            const checkBoxes = container.getElementsByTagName('input');
            const state = au.state.ctrl[id];

            if (!state.dialogvalues) state.dialogvalues = state.selectedvalues.slice();
            checkBoxes.forEach(function (element) {
                element.checked = false;
            });

            state.dialogvalues.splice(0, state.dialogvalues.length);
			//this.clear(id.replace("_str", ""), true);
        }
	}
	return instance;
}(ajaxUtils, commonUtils);

var selectObjectDropDownField = function (au, cu, cbcell) {
	var instance = {
		widgetDidMount: function (state) {
			const id = state.root;
			const elPh = document.getElementById(id + '_placeholder');
			const elFilter = document.getElementById(id + '_filter');			
			const elPopup = document.getElementById(id + '_popup');

			const opt = {
				triggerOn: 'keyup paste',
				displayAround: 'triggerbottom',
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
				if (elPh.hasAttribute('data-disabled')) return 2;
				else if (event.type == 'paste') {
					var data = event.originalEvent.clipboardData
					if (!data) data = window.clipboardData;
					elFilter.value = data.getData('Text');
					adjustInputWidth(elFilter);
					return 0;
				}
				var v = data.trigger.val();
				const sso = elPopup.querySelector('.radiobuttonlist') != null;
				if (event.keyCode == 8) return v == '' ? 1 : 0;
				if (event.keyCode == 27 || (event.keyCode == 13 && sso)) return 1;
				if (event.keyCode == 33 || event.keyCode == 34 || event.keyCode == 37 ||
					event.keyCode == 38 || event.keyCode == 39 || (event.keyCode == 13 && !sso)) return 2;
				if (event.keyCode == 40) return elPh.classList.contains('iw-opened') ? 2 : 0;
				if (v == '') return 1;

				return 0;
			}

			$('#' + id + '_filter').contextMenu('#' + id + '_popup', opt);
			opt.triggerOn = 'click';
			$('#' + id + '_btn').contextMenu('#' + id + '_popup', opt);

			elPh.addEventListener('click', function (e) {
				const elSel = document.getElementById(id + '_selected');
				if (e.target.id != elSel.id && !e.target.closest('.selected')) elFilter.focus();
			});

			elFilter.addEventListener('click', function (e) {
				e.stopPropagation();
			});

			elFilter.addEventListener('keydown', function (e) {
					const elSel = document.getElementById(id + '_selected');
					const isSingleMode = state && state.hasOwnProperty('selectedvalue');

					if (e.keyCode == 8 && elFilter.value == '' && !elFilter.hasAttribute('readonly')) {
						instance.clear(elSel.lastChild);
						return;
					}

					if (elPh.classList.contains('iw-opened')) {
						var cur = elPopup.querySelector('.row.selected');
						if (!cur) cur = elPopup.querySelector('.row');

						if (e.keyCode == 38) {
							e.preventDefault();
							if (cur && cur.previousSibling && cur.previousSibling.classList.contains('row')) {
								cur.classList.remove('selected');
								cur.previousSibling.classList.add('selected');
							}
						}
						else if (e.keyCode == 40) {
							e.preventDefault();
							if (cur && cur.nextSibling && cur.nextSibling.classList.contains('row')) {
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
							e.preventDefault();
							if (isSingleMode) {
								cur.firstChild.checked = true;
								state.selectedvalue = cur.firstChild.value;
								instance.setselected_singlemode(elPh, cur);
								if (cur.firstChild.onchange) cur.firstChild.onchange();
							}
							else {
								instance.setselected(elPh, cur);
							}
							elFilter.value = '';
							adjustInputWidth(elFilter);
						}
					}
				});

			elFilter.addEventListener('input', function () {
				adjustInputWidth(this);
			});
		},
		clear: function (el) {
			if (!el) return;

			const field = cu.getThisOrParent(el, function (n) { return n.classList.contains('selected'); });
			if (!field) return;
			if (field.classList.contains('hide')) return;
			if (!field.classList.contains('object')) return;

			const rowid = field.getAttribute('data-rowid');
			const c = au.findControl(field);
			const cins = c.instance;
			const isSingleMode = cins && cins.hasOwnProperty('selectedvalue');

			document.getElementById(c.id).value = '';

			const container = field.parentElement;

			if (isSingleMode) {
				field.firstChild.innerText = '';
				if (!field.classList.contains('hide'))
					field.classList.add('hide');
				cins.selectedvalue = '';
			}
			else {
				field.parentElement.removeChild(field);
				const index = cins.selectedvalues.indexOf(rowid);
				if (index > -1) {
					cins.selectedvalues.splice(index, 1);
				}
			}

			const selectedCnt = container.querySelectorAll('.selected.object:not(.hide)').length;
			if (selectedCnt == 0) {
				const empty = container.querySelector('.nothingselectedtext');
				if (empty) empty.classList.remove('hide');
			}

			if (el.hasAttribute('data-e'))
				au.postEventFromElementWithApiResponse(el);
		},
		setselected: function (elPh, elSel) {
			cbcell.setselected(elSel);

			const cont = elPh.querySelector('.selectedcontainer');
			const rowid = elSel.getAttribute('data-rowid');
			const existing = cont.querySelector('[data-rowid="' + rowid + '"]');

			if (elSel.classList.contains('checked')) {
				if (!existing) {
					const textel = elSel.querySelector('.text');
					const template = elPh.querySelector('.template');
					const div = template.cloneNode(true);
					div.className = "selected object";
					div.setAttribute('data-rowid', rowid);
					div.firstChild.innerHTML = textel.innerHTML;
					cont.appendChild(div);
				}
			}
			else {
				existing.parentElement.removeChild(existing);
			}

			const empty = cont.querySelector('.nothingselectedtext');
			if (empty) {
				const selectedCnt = cont.querySelectorAll('.selected.object:not(.hide)').length;
				if (selectedCnt == 0) {
					empty.classList.remove('hide');
				}
				else {
					empty.classList.add('hide');
				}
			}
		},
		setselected_singlemode: function (elPh, elSel) {
			const selected = elPh.querySelector('.selected.object');
			selected.classList.remove('hide');
			selected.firstChild.innerText = elSel.innerText;

			const empty = elPh.querySelector('.nothingselectedtext');
			if (empty) empty.classList.add('hide');
		}
	}

	function adjustInputWidth(el) {
		var spanElm = el.nextElementSibling;
		spanElm.textContent = el.value; // the hidden span takes the value of the input; 
		var w = spanElm.offsetWidth + 15;
		if (w < 25) w = 25;
		const maxWidth = el.parentElement.offsetWidth - 26 - 2;
		if (w > maxWidth) w = maxWidth;
		el.style.width = w + 'px'; // apply width of the span to the input
	}

	return instance;
}(ajaxUtils, commonUtils, checkBoxCell);

var selectObjectDropDown = function (au, cu, cbcell, field) {
	var instance = {	
		widgetWillMount: function (shadow, state) {
			const root = shadow.getElementById(state.root + '_str');
			const isSingleMode = state && state.hasOwnProperty('selectedvalue');

			const rows = root.querySelectorAll('.row');
			for (var i = 0; i < rows.length; i++) {
				const el = rows[i];

				if (isSingleMode) {
					el.addEventListener('mousedown', function (e) {
						e.preventDefault();
						const input = el.querySelector('input');
						state.selectedvalue = input.value;
						const elPh = document.getElementById(state.root + '_placeholder');
						field.setselected_singlemode(elPh, el);
						instance.onResult(1, state);
					});
				}
				else {
					const rowid = el.getAttribute('data-rowid');
					if (state.selectedvalues.indexOf(rowid) >= 0)
						cbcell.setselected(el);

					el.addEventListener('mousedown', function (e) {
						e.preventDefault();
						const elPh = document.getElementById(state.root + '_placeholder');
						field.setselected(elPh, el);
					});
				}
			}
		},
		widgetDidMount: function (state) {
			const elPopup = document.getElementById(state.root + '_popup');
			const rows = elPopup.querySelectorAll('.row');
			if (rows.length > 0) rows[0].classList.add('selected');

			for (var i = 0; i < rows.length; i++) {
				const el = rows[i];
				el.addEventListener('mousemove', function (e) {
					const cur = elPopup.querySelector('.row.selected');
					if (cur == e.currentTarget) return;
					if (cur) cur.classList.remove('selected');
					e.currentTarget.classList.add('selected');
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
}(ajaxUtils, commonUtils, checkBoxCell, selectObjectDropDownField);
