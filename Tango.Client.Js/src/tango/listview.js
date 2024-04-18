window.checkBoxCell = function (au, cu) {
	var instance = {
		setselected: function (el, onCheckChangeDelegate) {
			const row = { el: el, tr: cu.getRow(el) };
			instance.selSelectedRange([row], onCheckChangeDelegate);
			return row;
		},
		selSelectedRange: function (rows, onCheckChangeDelegate) {
			if (!rows || rows.length == 0)
				return;

			const c = au.findControl(rows[0].el);
			const cins = c.instance;
			const cbhead = document.getElementById(c.id + "_sel_header");

			for (var i = 0; i < rows.length; i++) {
				const el = rows[i].el;
				const tr = rows[i].tr;
				if (!tr) tr = cu.getRow(el);
				const setUnchecked = 'newState' in rows[i] ?
					rows[i].newState == 0 :
					tr.classList.contains('checked') || tr.hasAttribute('data-checked');

				if (setUnchecked) {
					if (cins.selectedvalues[0] == -1) {
						cins.selectedvalues = [];
						if (cbhead) instance.setPageChecked(c.root, cins, cbhead);
					}
					instance.setRowAndValueUnchecked(tr, el, cins);
				}
				else {
					instance.setRowAndValueChecked(tr, el, cins);
				}
			}

			const cblist = c.root.querySelectorAll('.sel');
			var j = 0;
			for (var i = 0; i < cblist.length; i++) {
				if (cblist[i].getAttribute('data-state') == 1) j++;
			}
			if (cbhead) instance.setHeaderSelectorState(cbhead, j, cblist.length);
			if (onCheckChangeDelegate) onCheckChangeDelegate(document, c.root, cins, j != 0);
		},

		cbheadclicked: function (cbhead, onCheckChangeDelegate) {
			const c = au.findControl(cbhead);
			const cins = c.instance;
			const cblist = c.root.querySelectorAll('.sel');
			const headstate = cbhead.getAttribute('data-state') || '0';

			if (headstate == '2' || headstate == '1') {
				instance.setPageUnchecked(c.root, cins, cbhead);
			}
			else if (headstate == '0') {
				instance.setPageChecked(c.root, cins, cbhead);
			}
			if (onCheckChangeDelegate) onCheckChangeDelegate(document, c.root, cins);
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

		setRowChecked: function (tr, el, state) {
			const ctx = { tr: tr };
			if (state.onRowChecking)
				if (state.onRowChecking.invoke(ctx) == false)
					return;
			tr.classList.add('checked');
			el.querySelector('i').className = 'icon icon-checkbox-checked';
			el.setAttribute('data-state', 1);
			if (state.onRowChecked)
				state.onRowChecked.invoke(ctx);
		},
		setRowAndValueChecked: function (tr, el, state) {
			instance.setRowChecked(tr, el, state);
			const rowid = tr.getAttribute('data-rowid');
			if (state.selectedvalues.indexOf(rowid) == -1)
				state.selectedvalues.push(rowid);
		},

		setRowUnchecked: function (tr, el, state) {
			const ctx = { tr: tr };
			if (state.onRowUnchecking)
				if (!state.onRowUnchecking.invoke(ctx))
					return;
			tr.classList.remove('checked');
			el.querySelector('i').className = 'icon icon-checkbox-unchecked';
			el.setAttribute('data-state', 0);
			if (state.onRowUnchecked)
				state.onRowUnchecked.invoke(ctx);
		},
		setRowAndValueUnchecked: function (tr, el, state) {
			instance.setRowUnchecked(tr, el, state);
			const rowid = tr.getAttribute('data-rowid');
			const index = state.selectedvalues.indexOf(rowid);
			if (index > -1) {
				state.selectedvalues.splice(index, 1);
			}
		},

		setPageChecked: function (root, state, cbhead) {
			const cblist = root.querySelectorAll('.sel');
			for (var i = 0; i < cblist.length; i++) {
				const tr = cu.getRow(cblist[i]);
				instance.setRowChecked(tr, cblist[i], state);
				state.selectedvalues.push(tr.getAttribute('data-rowid'));
			}
			cbhead.setAttribute('data-state', '1');
			cbhead.firstChild.className = 'icon icon-checkbox-checked';
		},

		setPageUnchecked: function (root, state, cbhead) {
			const cblist = root.querySelectorAll('.sel');
			for (var i = 0; i < cblist.length; i++) {
				const tr = cu.getRow(cblist[i]);
				instance.setRowUnchecked(tr, cblist[i], state);
				const index = state.selectedvalues.indexOf(tr.getAttribute('data-rowid'));
				if (index > -1) {
					state.selectedvalues.splice(index, 1);
				}
			}
			if (cbhead) {
				cbhead.setAttribute('data-state', '0');
				cbhead.firstChild.className = 'icon icon-checkbox-unchecked';
			}
		}
	};

	return instance;
}(ajaxUtils, commonUtils);

window.listview = function (au, cu, cbcell, menu) {
	const w = new Tango.HtmlWriter();
	const instance = {
		togglerow: function (el) {
			const tr = cu.getRow(el);
			const level = parseInt(tr.getAttribute('data-level')) || 0;
			const elcellid = el.id || '';
			const state = el.getAttribute('data-state') || 'collapsed';
			const isButton = el.classList.contains('rowexpandercell');
			const expandedrows = document.getElementById(el.getAttribute('data-r') + "_expandedrows");
			const expandedValue = tr.getAttribute('data-rowid') + ";";

			if (state == 'collapsed') {
				el.setAttribute('data-state', 'expanded');
				if (isButton) {
					var icon = el.querySelector('.icon.icon-collapsed')
					if (icon) {
						icon.className = 'icon icon-expanded';
					}
				}

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
					au.postEventFromElementWithApiResponse(el, { data: { rowid: tr.id, level: level, dataid: tr.getAttribute('data-rowid') } }).then(hideOthers);
				else
					hideOthers();

				if (expandedrows != null && !expandedrows.value.includes(expandedValue))
					expandedrows.value += expandedValue;
			} else {
				el.setAttribute('data-state', 'collapsed');
				if (isButton) {
					var icon = el.querySelector('.icon.icon-expanded')
					if (icon) {
						icon.className = 'icon icon-collapsed';
					}
				}

				var row = tr.nextElementSibling;
				while (row && parseInt(row.getAttribute('data-level')) > level) {
					if (elcellid == (row.getAttribute('data-cellid') || '')) {
						row.style.display = 'none';
					}
					row = row.nextElementSibling;
				}

				if (expandedrows != null && expandedrows.value.includes(expandedValue)) {
					expandedrows.value = expandedrows.value.replace(expandedValue, '');
				}
			}

			tr.querySelectorAll('.expandedcell').forEach(function (n) {
				n.classList.remove('expandedcell');
			});

			if (elcellid != '' && state == 'collapsed') cu.getCell(el).classList.add('expandedcell');
		},
		togglelevel: function (el, afterLoad) {
			const tr = cu.getRow(el);
			const level = parseInt(tr.getAttribute('data-level'));
			const isCollapsed = tr.classList.contains('collapsed');

			const expand = function () {
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

				if (afterLoad) afterLoad();
			}

			if (tr.hasAttribute('data-e') && !tr.hasAttribute('data-loaded')) {
				au.postEventFromElementWithApiResponse(tr).then(function () {
					tr.setAttribute('data-loaded', '');
					expand();
				});
			}
			else {
				expand();
			}
		},
		widgetWillMount: function (shadow, ctrl) {
			const root = shadow.getElementById(ctrl.root);
			//initHighlight(root);

			if (!ctrl.selectedvalues) return;

			const cblist = root.querySelectorAll('.sel:not(.initialized)');
			const cbhead = root.querySelector('.sel_header');

			var j = 0;

			if (ctrl.selectedvalues[0] == -1) {
				cbcell.setPageChecked(root, ctrl, cbhead);
				j = cblist.length
			} else {
				for (var i = 0; i < cblist.length; i++) {
					const tr = cu.getRow(cblist[i]);
					const index = ctrl.selectedvalues.indexOf(tr.getAttribute('data-rowid'));
					if (index > -1) {
						cbcell.setRowChecked(tr, cblist[i], ctrl);
						j++;
					}
				}
			}

			initCheckBoxes(ctrl, cblist);

			if (cbhead) {
				cbhead.addEventListener('click', function (e) { cbcell.cbheadclicked(e.currentTarget, onCheckChange); });
				cbcell.setHeaderSelectorState(cbhead, j, cblist.length);
			}
			onCheckChange(shadow, root, ctrl);


		},
		widgetDidMount: function (ctrl) {
			const root = document.getElementById(ctrl.root);

			const highlight = root instanceof HTMLTableElement && root.classList.contains('highlight') ?
				root : root.querySelector('.listviewtable.highlight');
			if (highlight) initHighlight(highlight);

			instance.initFixedHeader(root);

			if (ctrl.props && ctrl.props.listSettingsPopupID)
				instance.initListSettings(root);

			var el = $('#' + ctrl.root);
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
		widgetContentChanged: function (state) {
			const root = document.getElementById(state.root);
			const cblist = root.querySelectorAll('.sel:not(.initialized)');
			initCheckBoxes(state, cblist);
			for (var i = 0; i < cblist.length; i++) {
				const tr = cu.getRow(cblist[i]);
				const index = state.selectedvalues.indexOf(tr.getAttribute('data-rowid'));
				if (index > -1) {
					cbcell.setRowChecked(tr, cblist[i], state);
				}
			}
			initHighlight(root);
		},
		selectall: function (rootid) {
			const root = document.getElementById(rootid);
			const state = au.state.ctrl[rootid];
			const cbhead = document.getElementById(root.id + "_sel_header");

			state.selectedvalues.splice(0);
			state.selectedvalues.push(-1);

			cbcell.setPageChecked(root, state, cbhead);
			onCheckChange(document, root, state);
		},
		clearselection: function (rootid) {
			const state = au.state.ctrl[rootid];
			if (state) {
				state.selectedvalues.splice(0);

				const root = document.getElementById(rootid);
				if (root) {
					const cbhead = document.getElementById(root.id + "_sel_header");
					cbcell.setPageUnchecked(root, state, cbhead);
					onCheckChange(document, root, state);
				}
			}
		},
		onlevelsetpage: function (el) {
			const tr = cu.getRow(el);
			const level = parseInt(tr.getAttribute('data-level'));

			var row = tr.previousElementSibling;
			var toremove = [];
			while (row && parseInt(row.getAttribute('data-level')) >= level) {
				toremove.push(row);
				row = row.previousElementSibling;
			}

			for (var i = 0; i < toremove.length; i++) {
				toremove[i].parentNode.removeChild(toremove[i]);
			}

			row = tr.previousElementSibling;

			var target = { data: {} };
			for (var attr, i = 0, attrs = row.attributes, n = attrs ? attrs.length : 0; i < n; i++) {
				attr = attrs[i];
				var val = attr.value == '' ? null : attr.value;
				if (attr.name.startsWith('data-p-')) {
					target.data[attr.name.replace('data-p-', '')] = val || '';
				}
			}

			target.data['sender'] = row.id;

			tr.parentNode.removeChild(tr);

			au.postEventFromElementWithApiResponse(el, target);
		},
		openlevel: function (args, counter) {

			if (!counter) counter = 0;

			if (args && args[counter]) {

				var rowid = '[data-rowid="' + args[counter] + '"]';
				var el = document.querySelector(rowid)
				var level = parseInt(el.getAttribute('data-level'))
				el.attributes['class'] = '';
				//Если обновляем дерево, необходимо удалить страные элементы
				var rows = el.parentNode.children;
				var toremove = [];

				for (var i = 0; i < rows.length; i++) {
					if (parseInt(rows[i].getAttribute('data-level')) > level) {
						toremove.push(rows[i]);
					}
				}

				for (var i = 0; i < toremove.length; i++) {
					toremove[i].parentNode.removeChild(toremove[i]);
				}


				ajaxUtils.postEventFromElementWithApiResponse(el, { data: { rowid: args[counter], level: level, selectedRow: args[args.length - 1] } }).done(function () {

					var rowid = '[data-rowid="' + args[counter] + '"]';
					var el = document.querySelector(rowid)

					el.removeAttribute("class");
					el.setAttribute('data-loaded', '')

					instance.openlevel(args, counter + 1)
				});
			}
		},
		initFixedHeader: function (root) {
			if (typeof root === 'string' || root instanceof String)
				root = document.getElementById(root);
			const table = root instanceof HTMLTableElement && root.classList.contains('fixedheader') ?
				root : root.querySelector('.listviewtable.fixedheader');
			if (table) initFixedHeader(table);
		},
		onRemoveIconClick: function (e) {
			var seltr = cu.getRow(e.currentTarget);
			const rootsel = au.findControl(seltr);
			const state = au.state.ctrl[rootsel.id.replace('_selected', '')];

			function unSelect(el) {
				const origel = document.getElementById(el.id.replace('_selected', ''));
				if (origel) {
					var origcb = origel.querySelector('.sel');
					if (origcb && el.hasAttribute('data-checked'))
						cbcell.setselected(origcb, onCheckChange);
				}
				else {
					const rowid = el.getAttribute('data-rowid');
					const index = state.selectedvalues.indexOf(rowid);
					if (index > -1) {
						state.selectedvalues.splice(index, 1);
					}
				}
			}

			unSelect(seltr);

			var tocopy = [];
			var level = parseInt(seltr.getAttribute('data-level'));

			var childEl = seltr.nextElementSibling;
			if (childEl) {
				var childLevel = parseInt(childEl.getAttribute('data-level'));
				while (level < childLevel) {
					var el = childEl;
					unSelect(el);
					childEl = childEl.nextElementSibling;
					el.parentElement.removeChild(el);
					if (!childEl) break;
					childLevel = parseInt(childEl.getAttribute('data-level'))
				}
			}

			while (seltr) {
				if (parseInt(seltr.getAttribute('data-level')) == level) {
					tocopy.push(seltr);
					level--;
				}
				seltr = seltr.previousElementSibling;
			}
			removeSelected(tocopy);
			setObjectSetBackgroundColor();
		},
		initListSettings: function (root) {
			const ctrl = au.state.ctrl[root.id];
			if (ctrl.props.listSettingsPopupID) {
				const popup = document.getElementById(ctrl.props.listSettingsPopupID);
				const btn = document.getElementById(ctrl.props.listSettingsBtnID);
				const cbHideColumns = popup.querySelectorAll('input[type="checkbox"]');
				const map = initMapHead(root);
				const fh = root.classList.contains('fixedheader');
				btn.setAttribute('data-colcnt', cbHideColumns.length);

				for (var i = 0; i < cbHideColumns.length; i++) {
					cbHideColumns[i].checked = true;
					cbHideColumns[i].addEventListener('click', function (e) {
						const cb = e.currentTarget;
						const colIdx = parseInt(cb.getAttribute('data-colidx'));
						const data = map[colIdx];
						var colcnt = parseInt(btn.getAttribute('data-colcnt'));

						if (cb.checked)
							colcnt++;
						else
							colcnt--;

						btn.setAttribute('data-colcnt', colcnt);

						if (colcnt < cbHideColumns.length) {
							btn.classList.add('listsettings-enabled');
							w.ChangeIcon(btn.firstChild, 'listsettings-mono');
						}
						else {
							btn.classList.remove('listsettings-enabled');
							w.ChangeIcon(btn.firstChild, 'listsettings');
						}

						if (fh)
							root.classList.remove('fixedheader');

						hideColumns(data.ths);

						for (var i = 0; i < data.columns.length; i++) {
							var column = data.columns[i] + 1;
							const cells = root.querySelectorAll('tr > td:nth-child(' + column + ')');
							hideColumns(cells);
						}

						var columns = '';
						for (var i = 0; i < cbHideColumns.length; i++) {
							if (cbHideColumns[i].checked) {
								columns = columns + '"' + cbHideColumns[i].nextSibling.innerText + '",';
							}
						}
						if (ctrl.props.formID)
							sessionStorage.setItem(ctrl.props.formID + '.columns', columns.substring(0, columns.length - 1))

						if (fh) {
							setTimeout(function () {
								if (!root.classList.contains('fixedheader'))
									root.classList.add('fixedheader');
							}, 400);
						}

						function hideColumns(cells) {
							for (var j = 0; j < cells.length; j++) {
								if (cb.checked) {
									cells[j].classList.remove('hide');
								}
								else {
									cells[j].classList.add('hide');
								}
							}
						}
					});
				}
			}
		},
		scrollToCurrentNode: function (root) {
			root = document.getElementById(root);
			const node = root.querySelector('.selected');
			if (node) node.scrollIntoView({ block: 'center' });
		}
	}

	function initMapHead(root) {
		const map = [];
		const ths = Object.values(root.querySelectorAll('tr > th'));
		if (ths) {
			var idx = 0;
			var curRow = 0;
			var masterRowIdx = 0;

			for (let i = 0; i < ths.length; i++) {
				const th = ths[i];

				if (th.parentElement.rowIndex > curRow) {
					curRow = th.parentElement.rowIndex;
					idx = 0;
					masterRowIdx = 0;
				}

				if (th.parentElement.rowIndex == 0) {
					const mapObj = { columns: [], ths: [] };
					for (var j = idx; j < idx + th.colSpan; j++) {
						mapObj.columns.push(j);
					}
					mapObj.ths.push(th);
					map.push(mapObj);

					idx += th.colSpan;
				}
				else {
					while (map[masterRowIdx].ths[0].rowSpan > curRow) {
						masterRowIdx++;
						idx = map[masterRowIdx].columns[0];
					}

					map[masterRowIdx].ths.push(th);

					idx += th.colSpan;

					while (map.length > masterRowIdx && map[masterRowIdx].columns[map[masterRowIdx].columns.length - 1] < idx) {
						masterRowIdx++;
					}
				}
			}
			return map;
		}
	}

	function initCheckBoxes(state, cblist) {
		for (var i = 0; i < cblist.length; i++) {
			const el = cblist[i];
			el.addEventListener('mousedown', function (e) {
				e.preventDefault();
			});
			el.addEventListener('click', function (e) {
				const curRow = { tr: cu.getRow(e.currentTarget), el: e.currentTarget };
				if (e.shiftKey) {
					const actRow = document.activeElement;
					if (actRow.parentElement == curRow.tr.parentElement) {
						cbcell.selSelectedRange([curRow], onCheckChange);

						if (curRow.tr != actRow) {
							const rows = [];
							var tr = curRow.tr.rowIndex >= actRow.rowIndex ? actRow : curRow.tr.nextElementSibling;
							const cnt = Math.abs(curRow.tr.rowIndex - actRow.rowIndex);
							for (var j = 0; j < cnt; j++) {
								const el = tr.querySelector('td.sel');
								rows.push({ tr: tr, el: el, newState: curRow.newState });
								tr = tr.nextElementSibling;
							}
							cbcell.selSelectedRange(rows, onCheckChange);
						}
					}
				}
				else {
					const curel = e.currentTarget;
					cbcell.setselected(curel, onCheckChange);
					updateSelected(curel);

					const currow = cu.getRow(curel);
					const level = parseInt(currow.getAttribute('data-level'));
					var tr = currow.nextElementSibling;
					const isChecked = currow.classList.contains('checked');

					function setChildrenState() {
						tr = currow.nextElementSibling;
						while (tr && parseInt(tr.getAttribute('data-level')) == level + 1) {
							var cb = tr.querySelector('.sel');
							if (cb) {
								if (isChecked)
									cbcell.setRowAndValueChecked(tr, cb, state);
								else
									cbcell.setRowAndValueUnchecked(tr, cb, state);
								updateSelected(cb);
							}
							tr = tr.nextElementSibling;
						}
					}

					if (curel.hasAttribute('data-strategy') && curel.getAttribute('data-strategy') == 'WithChildren') {
						if (!currow.hasAttribute('data-loaded')) {
							instance.togglelevel(curel, setChildrenState);
						}
						else {
							setChildrenState();
						}
					}

					var allChildrenChecked = true;
					tr = currow.previousElementSibling;
					while (tr && parseInt(tr.getAttribute('data-level')) == level) {
						if (isChecked) allChildrenChecked = allChildrenChecked && tr.classList.contains('checked');
						tr = tr.previousElementSibling;
					}

					if (tr) {
						var cb = tr.querySelector('.sel');
						if (cb && cb.hasAttribute('data-strategy') && cb.getAttribute('data-strategy') == 'WithChildren') {
							if (isChecked) {
								var ntr = currow.nextElementSibling;
								while (ntr && parseInt(ntr.getAttribute('data-level')) == level) {
									allChildrenChecked = allChildrenChecked && ntr.classList.contains('checked');
									ntr = ntr.nextElementSibling;
								}
								if (allChildrenChecked) {
									cbcell.setRowAndValueChecked(tr, cb, state);
									
									updateSelected(cb);
								}
							}
							else {
								cbcell.setRowAndValueUnchecked(tr, cb, state);
								updateSelected(cb);
							}
						}
					}
				}
				curRow.tr.focus();
				setObjectSetBackgroundColor();
			});
			el.classList.add('initialized');
		}
	}

	function initFixedHeader(el) {
		const th = el.querySelector('th:not(.hide)');
		if (!th)
			return;
		const tableHeaderTop = th.getBoundingClientRect().top;
		if (tableHeaderTop === 0)
			return;
		const ths = el.querySelectorAll('th');
		for (let i = 0; i < ths.length; i++) {
			const th = ths[i];
			let padding = window.getComputedStyle(th, null).getPropertyValue('padding-top');
			let paddingParent = calculatePadding(th);
			if (paddingParent != null) {
				padding = padding.replace("px", "");
				paddingParent = paddingParent.replace("px", "");
				const currentTop = th.getBoundingClientRect().top - tableHeaderTop;
				const offsetTop = (((parseInt(padding) / 2) + parseInt(paddingParent)) * -1) + 2;
				th.style.top = offsetTop + currentTop + "px";
			} else {
				th.style.top = th.getBoundingClientRect().top - tableHeaderTop + "px";
			}
		}
	}

	function calculatePadding(node) {
		node = getScrollParent(node);
		if (node != null)
			return window.getComputedStyle(node, null).getPropertyValue('padding-bottom');
		return null;
	}

	function getScrollParent(node) {
		const isElement = node instanceof HTMLElement;
		const overflowY = isElement && window.getComputedStyle(node).overflowY;
		const isScrollable = overflowY !== 'visible' && overflowY !== 'hidden';

		if (!node) {
			return null;
		} else if (isScrollable && node.scrollHeight >= node.clientHeight) {
			return node;
		}

		return getScrollParent(node.parentNode) || document.body;
	}

	function initHighlight(root) {
		const highlightlist = root.querySelectorAll('*[data-highlight]:not(.initialized)');

		for (var i = 0; i < highlightlist.length; i++) {
			const el = highlightlist[i];
			el.addEventListener('click', function (e) {
				const tr = cu.getRow(e.currentTarget);
				const table = tr.parentNode.parentNode;
				const selEl = table.querySelector('.treerow-content .selected');
				if (selEl) selEl.classList.remove('selected');
				e.currentTarget.classList.add('selected');
				table.setAttribute('data-highlighted', tr.getAttribute('data-rowid'));
				e.stopPropagation();
			});
			el.classList.add('initialized');
		}

		// obsolete
		const currentSelected = root.getAttribute('data-highlighted');
		if (currentSelected) {
			const selEl = root.querySelector("tr[data-rowid='" + currentSelected + "'] *[data-highlight]");
			if (selEl) selEl.classList.add('selected');
		}
		/////
		const currentSelectedid = root.getAttribute('data-highlightedid');
		if (currentSelectedid) {
			const selEl = root.querySelector("#" + currentSelectedid + " *[data-highlight]");
			if (selEl) selEl.classList.add('selected');
		}

		const kb = root.classList.contains('kb');
		if (kb) {
			const trlist = root.querySelectorAll('tr');
			for (var i = 0; i < trlist.length; i++) {
				const el = trlist[i];

				if (el.classList.contains('initialized'))
					continue;

				el.addEventListener('keydown', function (e) {
					var cur = e.currentTarget;

					if (e.keyCode == 38) {
						e.preventDefault();
						cur = cur.previousSibling;
						while (cur && (cur.classList.contains('hide') || !cur.hasAttribute('tabindex')))
							cur = cur.previousSibling;

						if (cur) {
							cur.focus();
						}
					}
					else if (e.keyCode == 40) {
						e.preventDefault();
						cur = cur.nextSibling;
						while (cur && (cur.classList.contains('hide') || !cur.hasAttribute('tabindex')))
							cur = cur.nextSibling;

						if (cur) {
							cur.focus();
						}
					}
					else if (e.keyCode == 33) { //pgup
						e.preventDefault();

						var i = 0;
						while (cur && cur.previousSibling && i < 20) {
							var next = cur.previousSibling;
							while (next && (next.classList.contains('hide') || !next.hasAttribute('tabindex'))) {
								next = next.previousSibling;
							}
							i++;
							if (next)
								cur = next;
						}

						if (cur) {
							cur.focus();
						}
					}
					else if (e.keyCode == 34) { //pgdn
						e.preventDefault();

						var i = 0;
						while (cur && cur.nextSibling && i < 20) {
							var next = cur.nextSibling;
							while (next && (next.classList.contains('hide') || !next.hasAttribute('tabindex'))) {
								next = next.nextSibling;
							}
							i++;
							if (next)
								cur = next;
						}

						if (cur) {
							cur.focus();
						}
					}
					else if (e.keyCode == 37) { // left
						var rowexpander = cur.querySelector('.rowexpandercell');
						if (rowexpander)
							instance.togglerow(rowexpander);
						else {
							// tree
							if (!cur.classList.contains('collapsed'))
								instance.togglelevel(cur);
							else {
								var level = parseInt(cur.getAttribute('data-level'));
								while (cur && parseInt(cur.getAttribute('data-level')) >= level) {
									cur = cur.previousSibling;
								}
								if (cur && cur.hasAttribute('tabindex'))
									cur.focus();
							}
						}
					}
					else if (e.keyCode == 39) { // right
						if (cur.classList.contains('collapsed'))
							// tree
							instance.togglelevel(cur);
						else {
							var rowexpander = cur.querySelector('.rowexpandercell');
							if (rowexpander) instance.togglerow(rowexpander);
						}
					}
					else if (e.keyCode == 32) { //space
						e.preventDefault();
						var cb = cur.querySelector('.sel');
						if (cb) {
							cbcell.setselected(cb, onCheckChange);
							updateSelected(cb);
						}
					}
				});

				el.classList.add('initialized');
			}
		}
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

	function addDelIcon(el) {
		var del = w.Icon('delete');
		el.appendChild(del);
		del.addEventListener('click', instance.onRemoveIconClick);
	}

	function removeSelected(tocopy) {
		for (var i = 0; i < tocopy.length; i++) {
			const el = document.getElementById(tocopy[i].id);
			const isLastLeaf = !el.nextElementSibling || parseInt(el.nextElementSibling.getAttribute('data-level')) <= parseInt(el.getAttribute('data-level'));
			const clickedEl = i == 0;
			const unchecked = clickedEl || !el.hasAttribute('data-checked');
			if (isLastLeaf && unchecked)
				el.parentElement.removeChild(el);
			else if (clickedEl) {
				el.removeAttribute('data-checked');
				//var del = el.querySelector('.icon-delete');
				//if (del) del.parentElement.removeChild(del);
			}
			else if (isLastLeaf && !unchecked) {
				var arr = el.querySelector('.togglelevel > span');
				if (arr) arr.classList.add('hide');
			}
		}
	}

	function updateSelected(el) {
		var tr = cu.getRow(el);
		const root = au.findControl(el);
		const rootsel = document.getElementById(root.id + '_selected');
		var level = parseInt(tr.getAttribute('data-level'));

		const remove = !tr.classList.contains('checked');
		var tocopy = [];
		var toinitmenu = [];

		var i = 0;
		while (tr) {
			if (parseInt(tr.getAttribute('data-level')) == level) {
				var copyTr = tr.cloneNode(true);
				var isChecked = false;
				copyTr.id = copyTr.id + '_selected';
				if (copyTr.classList.contains('checked')) {
					copyTr.classList.remove('checked');
					copyTr.setAttribute('data-checked', '');
					isChecked = true;
				}
				copyTr.removeAttribute('data-e');

				var cb = copyTr.querySelector('.sel');
				if (cb) {
					//if (isChecked) addDelIcon(cb.parentElement);
					cb.parentNode.removeChild(cb);
				}

				var content = copyTr.querySelector('.treerow-content');
				addDelIcon(content);

				var ddm = copyTr.querySelector('.dropdownimage');
				if (ddm) {
					var menuData = null;
					if (au.state.ctrl['$contextmenu'])
						menuData = au.state.ctrl['$contextmenu'][ddm.id];
					if (menuData) {
						ddm.id = ddm.id + '_sel';
						ddm.nextElementSibling.id = ddm.nextElementSibling.id + '_sel';
						toinitmenu.push({
							triggerid: ddm.id,
							popupid: ddm.nextElementSibling.id,
							parms: menuData.parms
						});
					}
					else {
						ddm.parentNode.removeChild(ddm.nextElementSibling);
						ddm.parentNode.removeChild(ddm);
					}
				}

				if (copyTr.classList.contains('collapsed') || i == 0) {
					var arr = copyTr.querySelector('.togglelevel > span');
					if (arr) arr.classList.add('hide');
				}

				tocopy.push(copyTr);
				level--;
				i++;
			}
			tr = tr.previousElementSibling;
		}

		if (remove) {
			removeSelected(tocopy);
		}
		else {
			var parent = rootsel;
			var pos = 'beforeend';
			var hide = false;
			var collapsedby = '';

			for (var i = tocopy.length - 1; i >= 0; i--) {
				const el = document.getElementById(tocopy[i].id);
				if (!el) {
					if (hide) {
						tocopy[i].classList.add('hide');
						tocopy[i].setAttribute('data-collapsedby', collapsedby);
					}
					const level = parseInt(tocopy[i].getAttribute('data-level'));
					while (parent.nextElementSibling && parseInt(parent.nextElementSibling.getAttribute('data-level')) == level) {
						parent = parent.nextElementSibling;
					}
					while (parent.nextElementSibling && parseInt(parent.nextElementSibling.getAttribute('data-level')) > level) {
						parent = parent.nextElementSibling;
					}
					parent.insertAdjacentElement(pos, tocopy[i]);
				}
				else {
					if (el.classList.contains('hide')) {
						hide = true;
						collapsedby = el.getAttribute('data-collapsedby');
					}
					if (i == 0) {
						el.setAttribute('data-checked', '');
						//var content = el.querySelector('.treerow-content');
						//addDelIcon(content);
					}

					var arr = el.querySelector('.togglelevel > span');
					if (arr && arr.classList.contains('hide')) {
						arr.classList.remove('hide');
						el.classList.remove('collapsed');
					}
				}
				parent = el ? el : tocopy[i];
				pos = 'afterend';
			}

			for (var i = toinitmenu.length - 1; i >= 0; i--) {
				menu.contextMenu(toinitmenu[i].triggerid, toinitmenu[i].popupid, toinitmenu[i].parms);
			}
		}
	}
	function setObjectSetBackgroundColor() {
		const el = document.querySelector('.objectsetmanager .objectset-select > div');
		if (el) {
			if (el.classList.contains('objectset-green')) {
				el.classList.remove('objectset-green');
				el.classList.add('objectset-yellow')
			}
			const elsel = document.querySelector('.sidebar-panel .contentbody-selected > table');
			const elsave = document.querySelector('.objectsetmanager .objectset-save > button');
			const elsaveas = document.querySelector('.objectsetmanager .objectset-saveas > button');
			if (elsel && elsave && elsaveas) {
				const elseltr = elsel.querySelector('tr');
				if (elseltr) {
					elsaveas.classList.remove('disabled')
					const elseldel = el.querySelector('div.object.hide');
					if (!elseldel)
						elsave.classList.remove('disabled')
				}
				else {
					elsave.classList.add('disabled')
					elsaveas.classList.add('disabled')
				}
			}
		}
	}
	return instance;
}(ajaxUtils, commonUtils, checkBoxCell, contextmenuproxy);

window.sidebar = function () {
	var instance = {
		widgetWillMount: function (shadow, state) {
			var key = function (id) {
				return (location.pathname + '/' + root.id + '/collapsed').toLowerCase();
			}

			var handler = function (e) {
				const root = document.getElementById(state.root);

				if (root.classList.contains('collapsed')) {
					root.classList.remove('collapsed');
					localStorage.removeItem(key(root.id));
				}
				else {
					root.classList.add('collapsed');
					localStorage.setItem(key(root.id), '1');
				}
			};

			const root = shadow.getElementById(state.root);
			var val = localStorage.getItem(key(root.id));
			if (val == '1') root.classList.add('collapsed');

			var btn = root.getElementsByClassName('sidebar-close')[0];
			var menu = root.getElementsByClassName('sidebar-menu')[0];
			btn.addEventListener('click', handler);
			menu.addEventListener('click', handler);
		}
	}

	return instance;
}();

window.filterHelper = function (da) {
	var instance = {
		setValue: function (args) {
			const last = sessionStorage.getItem(args.id);
			if (last != '[]')
				sessionStorage.setItem(args.id + '_last', last);
			da.setStorageArg(args.id, args.val);
		}
	}

	return instance;
}(domActions);

