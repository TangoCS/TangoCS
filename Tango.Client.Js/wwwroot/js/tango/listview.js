var listview = function (au, cu) {
	var instance = {
		togglerow: function (el, e) {
			const tr = getRow(el);
			const root = tr.parentNode.parentNode;
			const level = parseInt(tr.getAttribute('data-level')) || 0;
			const elcellid = el.id || '';
			const state = el.getAttribute('data-state') || 'collapsed';
			const isButton = (el.className == 'rowexpandercell');
			var load = true;

			if (state == 'collapsed') {
				el.setAttribute('data-state', 'expanded');
				if (isButton) el.firstChild.className = 'icon icon-expanded';

				var row = tr.nextElementSibling;
				while (row && parseInt(row.getAttribute('data-level')) > level) {
					if (elcellid == (row.getAttribute('data-cellid') || '')) {
						row.style.display = '';
						load = false;
					}
					row = row.nextElementSibling;
				}

				if (!load) cu.scrollToView(el);
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
				load = false;
			}

			tr.querySelectorAll('.expandedcell').forEach(function (n) {
				n.classList.remove('expandedcell');
			});

			if (elcellid != '' && (load || state == 'collapsed')) getCell(el).classList.add('expandedcell');

			if (load && e) au.postEventFromElementWithApiResponse(el, { e: e, r: root.id, data: { rowid: tr.id } });
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
			const root = shadow.getElementById(state.root);
			const cblist = root.querySelectorAll('.sel');
			const cbhead = shadow.getElementById(root.id + "_sel_header");

			var j = 0;

			if (state.selectedvalues[0] == -1) {
				setPageChecked(root, state, cbhead);
				j = cblist.length
			} else {
				for (var i = 0; i < cblist.length; i++) {
					const tr = getRow(cblist[i]);
					const index = state.selectedvalues.indexOf(tr.getAttribute('data-rowid'));
					if (index > -1) {
						setRowChecked(tr, cblist[i]);
						j++;
					}
				}
			}

			setHeaderSelectorState(cbhead, j, cblist.length);
			setMassOpsState(shadow, root, state);
			initInfoBlock(shadow, root, state);
		},
		setselected: function (el) {
			const tr = getRow(el);
			const root = tr.parentNode.parentNode;
			const state = au.state.ctrl[root.id];
			const cbhead = document.getElementById(root.id + "_sel_header");
			const selected = tr.classList.contains('selected');

			if (selected) {
				if (state.selectedvalues[0] == -1) {
					state.selectedvalues = [];
					setPageChecked(root, state, cbhead);
				}
				setRowUnchecked(tr, el);
				const index = state.selectedvalues.indexOf(tr.getAttribute('data-rowid'));
				if (index > -1) {
					state.selectedvalues.splice(index, 1);
				}
			}
			else {
				setRowChecked(tr, el);
				state.selectedvalues.push(tr.getAttribute('data-rowid'));
			}

			const cblist = root.querySelectorAll('.sel');
			var j = 0;
			for (var i = 0; i < cblist.length; i++) {
				if (cblist[i].getAttribute('data-state') == 1) j++;
			}
			setHeaderSelectorState(cbhead, j, cblist.length);
			setMassOpsState(document, root, state);
			initInfoBlock(document, root, state, j != 0);
		},
		cbheadclicked: function (cbhead) {
			const tr = getRow(cbhead);
			const root = tr.parentNode.parentNode;
			const state = au.state.ctrl[root.id];
			const cblist = root.querySelectorAll('.sel');
			const headstate = cbhead.getAttribute('data-state') || '0';

			if (headstate == '2' || headstate == '1') {
				setPageUnchecked(root, state, cbhead);
			}
			else if (headstate == '0') {
				setPageChecked(root, state, cbhead);
			}
			setMassOpsState(document, root, state);
			initInfoBlock(document, root, state);
		},
		selectall: function (rootid) {
			const root = document.getElementById(rootid);
			const state = au.state.ctrl[rootid];
			const cbhead = document.getElementById(root.id + "_sel_header");

			state.selectedvalues = [];
			state.selectedvalues.push(-1);

			setPageChecked(root, state, cbhead);
			setMassOpsState(document, root, state);
			initInfoBlock(document, root, state);
		},
		clearselection: function (rootid) {
			const root = document.getElementById(rootid);
			const state = au.state.ctrl[rootid];
			const cbhead = document.getElementById(root.id + "_sel_header");

			state.selectedvalues = [];

			setPageUnchecked(root, state, cbhead);
			setMassOpsState(document, root, state);
			initInfoBlock(document, root, state);
		},
	}

	function setHeaderSelectorState(cbhead, j, cnt) {
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
	}

	function setMassOpsState(doc, root, state) {
		const massops = doc.querySelectorAll('.massop');
		for (var i = 0; i < massops.length; i++) {
			if (state.selectedvalues.length == 0) {
				massops[i].style.display = 'none';
			} else {
				massops[i].style.display = '';
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

	function setRowChecked(tr, el) {
		tr.classList.add('selected');
		el.firstChild.className = 'icon icon-checkbox-checked';
		el.setAttribute('data-state', 1);
	}

	function setRowUnchecked(tr, el) {
		tr.classList.remove('selected');
		el.firstChild.className = 'icon icon-checkbox-unchecked';
		el.setAttribute('data-state', 0);
	}

	function setPageChecked(root, state, cbhead) {
		const cblist = root.querySelectorAll('.sel');
		for (var i = 0; i < cblist.length; i++) {
			const tr = getRow(cblist[i]);
			setRowChecked(tr, cblist[i]);
			state.selectedvalues.push(tr.getAttribute('data-rowid'));
		}
		cbhead.setAttribute('data-state', '1');
		cbhead.firstChild.className = 'icon icon-checkbox-checked';
	}

	function setPageUnchecked(root, state, cbhead) {
		const cblist = root.querySelectorAll('.sel');
		for (var i = 0; i < cblist.length; i++) {
			const tr = getRow(cblist[i]);
			setRowUnchecked(tr, cblist[i]);
			const index = state.selectedvalues.indexOf(tr.getAttribute('data-rowid'));
			if (index > -1) {
				state.selectedvalues.splice(index, 1);
			}
		}
		cbhead.setAttribute('data-state', '0');
		cbhead.firstChild.className = 'icon icon-checkbox-unchecked';
	}

	function getFirstRowNo(table) {
		var i = 0;
		var tr = table.firstChild.firstChild;
		while (tr.firstChild.nodeName == 'TH') {
			tr = tr.nextSibling;
			i++;
		}
		return i;
	}

	function getRow(caller) {
		var el = caller;
		while (el) {
			if (el instanceof HTMLTableRowElement) return el;
			el = el.parentNode;
		}
		return undefined;
	};

	function getCell(caller) {
		var el = caller;
		while (el) {
			if (el instanceof HTMLTableCellElement) return el;
			el = el.parentNode;
		}
		return undefined;
	};

	return instance;
}(ajaxUtils, commonUtils);