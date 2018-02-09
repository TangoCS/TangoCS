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

			if (load) au.postEventFromElementWithApiResponse(el, { e: e, r: r });
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
		}
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