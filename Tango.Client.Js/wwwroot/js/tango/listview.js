var listview = function (au, cu) {
	var instance = {
		togglerow: function (el, e, r) {
			var tr = getRow(el);
			var content = tr.nextSibling;
			var elcellid = el.id || '';
			var contentcellid = (content && content.getAttribute('data-cellid')) || '';
			var state = tr.getAttribute('data-state') || 'collapsed';
			var isButton = (el.className == 'rowexpandercell');
			var load = false;

			if (state == 'collapsed') {
				tr.setAttribute('data-state', 'expanded');
				if (isButton) el.firstChild.className = 'icon icon-expanded';
				if (content && content.id == tr.id + '_content' && contentcellid == elcellid) {
					content.style.display = '';
					cu.scrollToView(content);
				}
				else
					load = true;
			} else if (content && content.id == tr.id + '_content') {
				if (contentcellid == elcellid) {
					tr.setAttribute('data-state', 'collapsed');
					if (isButton) el.firstChild.className = 'icon icon-collapsed';
					content.style.display = 'none';
				}
				else
					load = true;
			}
			if (contentcellid != '') getCell(document.getElementById(contentcellid)).classList.remove('expandedcell');
			if (elcellid != '' && (load || state == 'collapsed')) getCell(document.getElementById(elcellid)).classList.add('expandedcell');

			if (load) au.postEventFromElementWithApiResponse(el, { e: e, r: r, data: { rowid: tr.id } });
		},
		togglelevel: function (el, e, r) {
			var tr = getRow(el);
			var level = parseInt(tr.getAttribute('data-level'));
			var isCollapsed = tr.classList.contains('collapsed');

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
		init: function (el, s) {

		},
		setstate: function (root, s) {
			const cblist = root.querySelectorAll('.sel');
			const state = au.state.ctrl[root.id];
			const cbhead = document.getElementById(root.id + "_sel_header");

			var j = 0;
			for (var i = 0; i < cblist.length; i++) {
				const tr = getRow(cblist[i]);
				const index = state.selectedvalues.indexOf(tr.getAttribute('data-rowid'));
				if (index > -1) {
					setRowChecked(tr, cblist[i]);
					j++;
				}
			}
			setHeaderSelectorState(cbhead, j, cblist.length);
		},
		setselected: function (el) {
			const tr = getRow(el);
			const root = tr.parentNode.parentNode;
			const state = au.state.ctrl[root.id];
			const cbhead = document.getElementById(root.id + "_sel_header");
			const selected = tr.classList.contains('selected');

			if (selected) {
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
		},
		cbheadclicked: function (cbhead) {
			const tr = getRow(cbhead);
			const root = tr.parentNode.parentNode;
			const state = au.state.ctrl[root.id];
			const cblist = root.querySelectorAll('.sel');
			const headstate = cbhead.getAttribute('data-state') || '0';

			if (headstate == '2' || headstate == '1') {
				for (var i = 0; i < cblist.length; i++) {
					const row = getRow(cblist[i]);
					setRowUnchecked(row, cblist[i]);
				}
				state.selectedvalues = [];
				cbhead.setAttribute('data-state', '0');
				cbhead.firstChild.className = 'icon icon-checkbox-unchecked';
			}
			else if (headstate == '0') {
				for (var i = 0; i < cblist.length; i++) {
					const row = getRow(cblist[i]);
					setRowChecked(row, cblist[i]);
					state.selectedvalues.push(row.getAttribute('data-rowid'));
				}
				cbhead.setAttribute('data-state', '1');
				cbhead.firstChild.className = 'icon icon-checkbox-checked';
			}

		}
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