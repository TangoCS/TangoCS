window.dialog = function (au) {
	var instance = {
		open: function (el, onSubmit) {
			if (!el.nodeType) el = document.getElementById(el);

			const parms = localStorage.getObject(location.pathname + '/' + el.id);
			if (parms) {
				if (parms.width) el.style.width = parms.width;
				if (parms.height) el.style.height = parms.height;
				if (parms.top) el.style.top = parms.top;
				if (parms.left) {
					el.style.left = parms.left;
					el.style.right = 'inherit';
				}
			}

			el.classList.add('md-show');
			el.style.zIndex = 101;

			const header = el.getElementsByClassName("modal-header")[0];
			dragElement(el, header);
			_initResize(el);

			if (onSubmit) {
				const btn = el.querySelector('.modal-footer button[type="submit"]');
				if (btn) {
					btn.onclick = e => {
						onSubmit(e);
						instance.close(el);
					};
				}
			}
		},
		close: function (el) {
			if (!el.nodeType) el = document.getElementById(el);
			el.classList.remove('md-show');
			if (!el.hasAttribute('data-reuse')) {
				au.delay(el, function (d) {
					d.parentNode.removeChild(d);
				});
			};
			_dialog = _dialogs.pop();
		},
		widgetWillMount: function (shadow, state) {
			const el = shadow.getElementById(state.root);
			const parentid = el.getAttribute('data-c-parent');
			if (el.hasAttribute('data-showonrender'))
				instance.open(el);
			if (parentid) {
				const parent = document.getElementById(parentid);
				parent.style.zIndex = 99;
				state.parent = parent;
			}
		},
		onAjaxSend: function (caller, target, state) {
			if (target.data['c-new']) {
				target.data['c-parent'] = state.root;
			}
		},
		onResult: function (res, state) {
			instance.close(document.getElementById(state.root));
			if (state.parent)
				state.parent.style.zIndex = 101;
			if (res == 0) {
				return false;
			}
		}
	};

	function dragElement(el, dragEl) {
		if (!dragEl) return;

		var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;

		dragEl.onmousedown = dragMouseDown;
		dragEl.style.cursor = 'move';

		function dragMouseDown(e) {
			e = e || window.event;
			if (e.target instanceof HTMLButtonElement) return;
			e.preventDefault();

			pos3 = e.clientX;
			pos4 = e.clientY;

			document.onmouseup = closeDragElement;
			document.onmousemove = elementDrag;

			var style = window.getComputedStyle(el);
			el.style.transition = 'inherit';

			if (style.right != '') {
				const left = el.offsetLeft;
				el.style.right = 'inherit';
				el.style.left = left + "px";
			}
		}

		function elementDrag(e) {
			e = e || window.event;
			e.preventDefault();
			// calculate the new cursor position:
			pos1 = pos3 - e.clientX;
			pos2 = pos4 - e.clientY;
			pos3 = e.clientX;
			pos4 = e.clientY;
			// set the element's new position:
			el.style.top = (el.offsetTop - pos2) + "px";
			el.style.left = (el.offsetLeft - pos1) + "px";
		}

		function closeDragElement() {
			localStorage.setObject(location.pathname + '/' + el.id, {
				left: el.style.left,
				top: el.style.top,
				width: el.style.width,
				height: el.style.height
			});

			// stop moving when mouse button is released:
			document.onmouseup = null;
			document.onmousemove = null;
		}
	}

	var _dialog = null,
		_dialogs = [],
		_resizeMode = '',
		_isResize = false,
		_resizePixel = 15,
		_minW = 100,
		_minH = 100,
		_maxX,
		_maxY,
		_startX,
		_startY,
		_startW,
		_startH;
	function _getOffset(elm) {
		var rect = elm.getBoundingClientRect(),
			offsetX = window.scrollX || document.documentElement.scrollLeft,
			offsetY = window.scrollY || document.documentElement.scrollTop;
		return {
			left: rect.left + offsetX,
			top: rect.top + offsetY,
			right: rect.right + offsetX,
			bottom: rect.bottom + offsetY
		}
	}

	function _returnEvent(evt) {
		if (evt.stopPropagation)
			evt.stopPropagation();
		if (evt.preventDefault)
			evt.preventDefault();
		else {
			evt.returnValue = false;
			return false;
		}
	}

	function _onMouseDown(evt) {
		evt = evt || window.event;
		if (evt.target != _dialog)
			return;
		var rect = _getOffset(_dialog);
		_maxX = Math.max(
			document.documentElement["clientWidth"],
			document.body["scrollWidth"],
			document.documentElement["scrollWidth"],
			document.body["offsetWidth"],
			document.documentElement["offsetWidth"]
		);
		_maxY = Math.max(
			document.documentElement["clientHeight"],
			document.body["scrollHeight"],
			document.documentElement["scrollHeight"],
			document.body["offsetHeight"],
			document.documentElement["offsetHeight"]
		);
		if (rect.right > _maxX)
			_maxX = rect.right;
		if (rect.bottom > _maxY)
			_maxY = rect.bottom;

		_startX = evt.pageX;
		_startY = evt.pageY;
		_startW = _dialog.clientWidth;
		_startH = _dialog.clientHeight;
		_leftPos = rect.left;
		_topPos = rect.top;
		if (_resizeMode != '') {
			_isResize = true;
		}
		var r = _dialog.getBoundingClientRect();
		return _returnEvent(evt);
	}
	function _setCursor(cur) {
		_dialog.style.cursor = cur;
	}
	function _onMouseMove(evt) {
		evt = evt || window.event;
		if (evt.target != _dialog && _resizeMode == '')
			return;
		if (_isResize) {
			var dw, dh, w, h;
			if (_resizeMode == 'w') {
				dw = _startX - evt.pageX;
				if (_leftPos - dw < 0)
					dw = _leftPos;
				w = _startW + dw;
				if (w < _minW) {
					w = _minW;
					dw = w - _startW;
				}
				_dialog.style.width = w + 'px';
				_dialog.style.left = (_leftPos - dw) + 'px';
			}
			else if (_resizeMode == 'e') {
				dw = evt.pageX - _startX;
				if (_leftPos + _startW + dw > _maxX)
					dw = _maxX - _leftPos - _startW;
				w = _startW + dw;
				if (w < _minW)
					w = _minW;
				_dialog.style.width = w + 'px';
			}
			else if (_resizeMode == 'n') {
				dh = _startY - evt.pageY;
				if (_topPos - dh < 0)
					dh = _topPos;
				h = _startH + dh;
				if (h < _minH) {
					h = _minH;
					dh = h - _startH;
				}
				_dialog.style.height = h + 'px';
				_dialog.style.top = (_topPos - dh) + 'px';
			}
			else if (_resizeMode == 's') {
				dh = evt.pageY - _startY;
				if (_topPos + _startH + dh > _maxY)
					dh = _maxY - _topPos - _startH;
				h = _startH + dh;
				if (h < _minH)
					h = _minH;
				_dialog.style.height = h + 'px';
			}
			else if (_resizeMode == 'nw') {
				dw = _startX - evt.pageX;
				dh = _startY - evt.pageY;
				if (_leftPos - dw < 0)
					dw = _leftPos;
				if (_topPos - dh < 0)
					dh = _topPos;
				w = _startW + dw;
				h = _startH + dh;
				if (w < _minW) {
					w = _minW;
					dw = w - _startW;
				}
				if (h < _minH) {
					h = _minH;
					dh = h - _startH;
				}
				_dialog.style.width = w + 'px';
				_dialog.style.height = h + 'px';
				_dialog.style.left = (_leftPos - dw) + 'px';
				_dialog.style.top = (_topPos - dh) + 'px';
			}
			else if (_resizeMode == 'sw') {
				dw = _startX - evt.pageX;
				dh = evt.pageY - _startY;
				if (_leftPos - dw < 0)
					dw = _leftPos;
				if (_topPos + _startH + dh > _maxY)
					dh = _maxY - _topPos - _startH;
				w = _startW + dw;
				h = _startH + dh;
				if (w < _minW) {
					w = _minW;
					dw = w - _startW;
				}
				if (h < _minH)
					h = _minH;
				_dialog.style.width = w + 'px';
				_dialog.style.height = h + 'px';
				_dialog.style.left = (_leftPos - dw) + 'px';
			}
			else if (_resizeMode == 'ne') {
				dw = evt.pageX - _startX;
				dh = _startY - evt.pageY;
				if (_leftPos + _startW + dw > _maxX)
					dw = _maxX - _leftPos - _startW;
				if (_topPos - dh < 0)
					dh = _topPos;
				w = _startW + dw;
				h = _startH + dh;
				if (w < _minW)
					w = _minW;
				if (h < _minH) {
					h = _minH;
					dh = h - _startH;
				}
				_dialog.style.width = w + 'px';
				_dialog.style.height = h + 'px';
				_dialog.style.top = (_topPos - dh) + 'px';
			}
			else if (_resizeMode == 'se') {
				dw = evt.pageX - _startX;
				dh = evt.pageY - _startY;
				if (_leftPos + _startW + dw > _maxX)
					dw = _maxX - _leftPos - _startW;
				if (_topPos + _startH + dh > _maxY)
					dh = _maxY - _topPos - _startH;
				w = _startW + dw;
				h = _startH + dh;
				if (w < _minW)
					w = _minW;
				if (h < _minH)
					h = _minH;
				_dialog.style.width = w + 'px';
				_dialog.style.height = h + 'px';
			}
		}
		else {
			if (_dialog.style.left === '') {
				var rect = _getOffset(_dialog);
				_dialog.style.left = (document.documentElement.getBoundingClientRect().width - rect.right + rect.left) / 2 + 'px';
				_dialog.style.right = 'auto';
			}
			var cs, rm = '';
			if (evt.target === _dialog) {
				var rect = _getOffset(_dialog);
				if (evt.pageY < rect.top + _resizePixel)
					rm = 'n';
				else if (evt.pageY > rect.bottom - _resizePixel)
					rm = 's';
				if (evt.pageX < rect.left + _resizePixel)
					rm += 'w';
				else if (evt.pageX > rect.right - _resizePixel)
					rm += 'e';
			}
			if (rm != '' && _resizeMode != rm) {
				if (rm == 'n' || rm == 's')
					cs = 'ns-resize';
				else if (rm == 'e' || rm == 'w')
					cs = 'ew-resize';
				else if (rm == 'ne' || rm == 'sw')
					cs = 'nesw-resize';
				else if (rm == 'nw' || rm == 'se')
					cs = 'nwse-resize';
				_setCursor(cs);
				_resizeMode = rm;
			}
			else if (rm == '' && _resizeMode != '') {
				_setCursor('');
				_resizeMode = '';
			}
		}
		return _returnEvent(evt);
	}

	_onMouseUp = function (evt) {
		evt = evt || window.event;
		if (evt.target != _dialog && _resizeMode == '')
			return;
		if (_isResize) {
			_setCursor('');
			_isResize = false;
			_resizeMode = '';
			localStorage.setObject(location.pathname + '/' + _dialog.id, {
				left: _dialog.style.left,
				top: _dialog.style.top,
				width: _dialog.style.width,
				height: _dialog.style.height
			});
		}
		return _returnEvent(evt);
	}
	function _initResize(dlg) {
		_dialogs.push(_dialog);
		_dialog = dlg;
		_dialog.addEventListener('mousedown', _onMouseDown, false);
		document.addEventListener('mousemove', _onMouseMove, false);
		document.addEventListener('mouseup', _onMouseUp, false);
		window.setTimeout(function () {
			if (_dialog.getBoundingClientRect().height < _dialog.children[0].getBoundingClientRect().height)
				_dialog.style.height = document.documentElement["clientHeight"] + 'px';
		}, 0);		
	}

	return instance;
}(ajaxUtils);


class FieldEdit extends Tango.Component {
	prepareParms() {
		return {
			top: this.root.offsetTop,
			left: this.root.offsetLeft,
			width: this.root.offsetWidth,
			height: this.root.offsetHeight
		}
	}
}

Tango.registerContainer(FieldEdit);
