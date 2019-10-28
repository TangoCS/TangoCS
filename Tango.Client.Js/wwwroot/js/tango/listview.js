var checkBoxCell = function (au, cu) {
	var instance = {
		setselected: function (el, onCheckChangeDelegate) {
			const tr = getRow(el);
			const root = cu.getThisOrParent(el, function (n) { return n.hasAttribute && n.hasAttribute('data-ctrl'); });
			const ctrlid = root.hasAttribute('data-ctrl-id') ? root.getAttribute('data-ctrl-id') : root.id;
			const state = au.state.ctrl[ctrlid];
			const cbhead = document.getElementById(ctrlid + "_sel_header");
			const selected = tr.classList.contains('checked');

			if (selected) {
				if (state.selectedvalues[0] == -1) {
					state.selectedvalues = [];
					if (cbhead) instance.setPageChecked(root, state, cbhead);
				}
				instance.setRowUnchecked(tr, el);
				const index = state.selectedvalues.indexOf(tr.getAttribute('data-rowid'));
				if (index > -1) {
					state.selectedvalues.splice(index, 1);
				}
			}
			else {
				instance.setRowChecked(tr, el);
				state.selectedvalues.push(tr.getAttribute('data-rowid'));
			}

			const cblist = root.querySelectorAll('.sel');
			var j = 0;
			for (var i = 0; i < cblist.length; i++) {
				if (cblist[i].getAttribute('data-state') == 1) j++;
			}
			if (cbhead) instance.setHeaderSelectorState(cbhead, j, cblist.length);
			if (onCheckChangeDelegate) onCheckChangeDelegate(document, root, state, j != 0);
		},

		cbheadclicked: function (cbhead, onCheckChangeDelegate) {
			const root = cu.getThisOrParent(cbhead, function (n) { return n.hasAttribute && n.hasAttribute('data-ctrl'); });
			const ctrlid = root.hasAttribute('data-ctrl-id') ? root.getAttribute('data-ctrl-id') : root.id;
			const state = au.state.ctrl[ctrlid];
			const cblist = root.querySelectorAll('.sel');
			const headstate = cbhead.getAttribute('data-state') || '0';

			if (headstate == '2' || headstate == '1') {
				instance.setPageUnchecked(root, state, cbhead);
			}
			else if (headstate == '0') {
				instance.setPageChecked(root, state, cbhead);
			}
			if (onCheckChangeDelegate) onCheckChangeDelegate(document, root, state);
		},

		setHeaderSelectorState: function (cbhead, j, cnt) {
			if (j == cnt) {
				cbhead.setAttribute('data-state', '1');
				cbhead.firstChild.className = 'icon icon-checkbox-checked';
			} else if (j == 0) {
				cbhead.setAttribute('data-state', '0');
				cbhead.firstChild.className = 'icon icon-checkbox-unchecked';
			} else {
				cbhead.setAttribute('data-state', '2');
				cbhead.firstChild.className = 'icon icon-minus-box';
			}
		},

		setRowChecked: function (tr, el) {
			tr.classList.add('checked');
			el.querySelector('i').className = 'icon icon-checkbox-checked';
			//el.firstChild.className = 'icon icon-checkbox-checked';
			el.setAttribute('data-state', 1);
		},

		setRowUnchecked: function (tr, el) {
			tr.classList.remove('checked');
			el.querySelector('i').className = 'icon icon-checkbox-unchecked';
			//el.firstChild.className = 'icon icon-checkbox-unchecked';
			el.setAttribute('data-state', 0);
		},

		setPageChecked: function (root, state, cbhead) {
			const cblist = root.querySelectorAll('.sel');
			for (var i = 0; i < cblist.length; i++) {
				const tr = getRow(cblist[i]);
				instance.setRowChecked(tr, cblist[i]);
				state.selectedvalues.push(tr.getAttribute('data-rowid'));
			}
			cbhead.setAttribute('data-state', '1');
			cbhead.firstChild.className = 'icon icon-checkbox-checked';
		},

		setPageUnchecked: function (root, state, cbhead) {
			const cblist = root.querySelectorAll('.sel');
			for (var i = 0; i < cblist.length; i++) {
				const tr = getRow(cblist[i]);
				instance.setRowUnchecked(tr, cblist[i]);
				const index = state.selectedvalues.indexOf(tr.getAttribute('data-rowid'));
				if (index > -1) {
					state.selectedvalues.splice(index, 1);
				}
			}
			cbhead.setAttribute('data-state', '0');
			cbhead.firstChild.className = 'icon icon-checkbox-unchecked';
		}
	};

	function getRow(caller) {
		return cu.getThisOrParent(caller, function (el) { return el instanceof HTMLTableRowElement; });
	};

	return instance;
}(ajaxUtils, commonUtils);

var listview = function (au, cu, cbcell) {
	var instance = {
		togglerow: function (el) {
			const tr = getRow(el);
			const root = tr.parentNode.parentNode;
			const level = parseInt(tr.getAttribute('data-level')) || 0;
			const elcellid = el.id || '';
			const state = el.getAttribute('data-state') || 'collapsed';
			const isButton = (el.className == 'rowexpandercell');

			if (state == 'collapsed') {
				el.setAttribute('data-state', 'expanded');
				if (isButton) el.firstChild.className = 'icon icon-expanded';

				var load = true;

				var row = tr.nextElementSibling;
				while (row && parseInt(row.getAttribute('data-level')) > level) {
					if (elcellid == (row.getAttribute('data-cellid') || '')) {
						row.style.display = '';
						load = false;
					}
					row = row.nextElementSibling;
				}

				const hideOthers = function () {
					row = tr.nextElementSibling;
					while (row && parseInt(row.getAttribute('data-level')) > level) {
						if (elcellid != (row.getAttribute('data-cellid') || '')) {
							row.style.display = 'none';
							document.getElementById(row.getAttribute('data-cellid')).setAttribute('data-state', 'collapsed');
						}
						row = row.nextElementSibling;
					}
				};

				const e = el.getAttribute('data-e');
				if (load && e)
					au.postEventFromElementWithApiResponse(el, { data: { rowid: tr.id, level: level } }).then(hideOthers);
				else
					hideOthers();
			} else {
				el.setAttribute('data-state', 'collapsed');
				if (isButton) el.firstChild.className = 'icon icon-collapsed';

				var row = tr.nextElementSibling;
				while (row && parseInt(row.getAttribute('data-level')) > level) {
					if (elcellid == (row.getAttribute('data-cellid') || '')) {
						row.style.display = 'none';
					}
					row = row.nextElementSibling;
				}
			}

			tr.querySelectorAll('.expandedcell').forEach(function (n) {
				n.classList.remove('expandedcell');
			});

			if (elcellid != '' && state == 'collapsed') getCell(el).classList.add('expandedcell');
		},
		togglelevel: function (el) {
			const tr = getRow(el);
			const level = parseInt(tr.getAttribute('data-level'));
			const isCollapsed = tr.classList.contains('collapsed');

			var row = tr.nextElementSibling;

			while (row && parseInt(row.getAttribute('data-level')) > level) {
				if (isCollapsed) {
					tr.classList.remove('collapsed');
					if (parseInt(row.getAttribute('data-collapsedby')) == level) {
						row.classList.remove('hide');
						row.setAttribute('data-collapsedby', '');
					}
				}
				else if (!row.classList.contains('hide')) {
					tr.classList.add('collapsed');
					row.setAttribute('data-collapsedby', level);
					row.classList.add('hide');
				}

				row = row.nextElementSibling;
			}
		},
		widgetWillMount: function (shadow, state) {
			if (!state.selectedvalues) return;

			const root = shadow.getElementById(state.root);
			const cblist = root.querySelectorAll('.sel');
			const cbhead = root.querySelector('.sel_header');

			var j = 0;

			if (state.selectedvalues[0] == -1) {
				cbcell.setPageChecked(root, state, cbhead);
				j = cblist.length
			} else {
				for (var i = 0; i < cblist.length; i++) {
					const tr = getRow(cblist[i]);
					const index = state.selectedvalues.indexOf(tr.getAttribute('data-rowid'));
					if (index > -1) {
						cbcell.setRowChecked(tr, cblist[i]);
						j++;
					}
				}
			}

			for (var i = 0; i < cblist.length; i++) {
				const el = cblist[i];
				el.addEventListener('click', function (e) { cbcell.setselected(el, onCheckChange); });
			}
			cbhead.addEventListener('click', function (e) { cbcell.cbheadclicked(cbhead, onCheckChange); });

			cbcell.setHeaderSelectorState(cbhead, j, cblist.length);
			onCheckChange(shadow, root, state);
		},
		widgetDidMount: function (state) {
			var el = $('#' + state.root);
			if (!el.tableDnD || !el.hasClass("draggablerows")) return;
			el.tableDnD({
				_oldpos: null,
				dragHandle: ".dragHandle",
				onDragStart: function (table, row) {
					const next = row.nodeName == 'TD' ? row.parentNode.nextElementSibling : row.nextElementSibling;
					this._oldpos = next ? next.getAttribute('data-id') : -1;
				},
				onDragStop: function (table, row) {
					const next = row.nodeName == 'TD' ? row.parentNode.nextElementSibling : row.nextElementSibling;

					const target = {
						e: 'OnRowMove',
						r: table.id,
						data: {
							newid: next ? next.getAttribute('data-id') : -1,
							oldid: row.getAttribute('data-id')
						}
					};
					if (target.data.newid == this._oldpos) return;

					au.postEventFromElementWithApiResponse(el[0], target);
				}
			});
		},
		
		
		selectall: function (rootid) {
			const root = document.getElementById(rootid);
			const state = au.state.ctrl[rootid];
			const cbhead = document.getElementById(root.id + "_sel_header");

			state.selectedvalues = [];
			state.selectedvalues.push(-1);

			cbcell.setPageChecked(root, state, cbhead);
			onCheckChange(document, root, state);
		},
		clearselection: function (rootid) {
			const root = document.getElementById(rootid);
			const state = au.state.ctrl[rootid];
			const cbhead = document.getElementById(root.id + "_sel_header");

			state.selectedvalues = [];

			cbcell.setPageUnchecked(root, state, cbhead);
			onCheckChange(document, root, state);
		},
	}

	function onCheckChange(document, root, state, keepInfoBlockState) {
		setBulkOpsState(document, root, state);
		initInfoBlock(document, root, state, keepInfoBlockState);
	}

	function setBulkOpsState(doc, root, state) {
		const bulkops = doc.querySelectorAll('.bulkop');
		for (var i = 0; i < bulkops.length; i++) {
			if (bulkops[i].getAttribute('data-owner') != root.id) continue;
			if (state.selectedvalues.length == 0) {
				bulkops[i].classList.add('hide');
			} else {
				bulkops[i].classList.remove('hide');
			}
		}
	}

	function initInfoBlock(doc, root, state, keepBlockState) {
		const trInfo = doc.getElementById(root.id + '_sel_info');
		if (trInfo) {
			if (state.selectedvalues.length > 0) {
				if (!keepBlockState) trInfo.style.display = '';
			}
			else {
				trInfo.style.display = 'none';
			}
			if (trInfo.style.display == '') {
				const elCnt = doc.getElementById(root.id + '_sel_info_cnt');
				const elAll = doc.getElementById(root.id + '_sel_info_all');
				if (state.selectedvalues[0] != -1) {
					elCnt.style.display = '';
					elAll.style.display = 'none';
					elCnt.innerHTML = state.selectedvalues.length;
				}
				else {
					elCnt.style.display = 'none';
					elAll.style.display = '';
				}
			}
		}
	}

	function getRow(caller) {
		return cu.getThisOrParent(caller, function (el) { return el instanceof HTMLTableRowElement; });
	};

	function getCell(caller) {
		return cu.getThisOrParent(caller, function (el) { return el instanceof HTMLTableCellElement; });
	};

	return instance;
}(ajaxUtils, commonUtils, checkBoxCell);