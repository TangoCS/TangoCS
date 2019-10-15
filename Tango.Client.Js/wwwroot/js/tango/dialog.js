var dialog = function (au) {
	var instance = {
		open: function (el) {
			if (!el.nodeType) el = document.getElementById(el);
			el.classList.add('md-show');
			el.style.zIndex = 101;

			const header = el.getElementsByClassName("modal-header")[0];
			dragElement(el, header);
		},
		close: function (el) {
			if (!el.nodeType) el = document.getElementById(el);
			el.classList.remove('md-show');
			if (!el.getAttribute('data-reuse')) {
				au.delay(el, function (d) {
					d.parentNode.removeChild(d);
				});
			};
		},
		widgetWillMount: function (shadow, state) {
			const el = shadow.getElementById(state.root);
			const parentid = el.getAttribute('data-c-parent');
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
		if (dragEl) return;

		var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;

		dragEl.onmousedown = dragMouseDown;
		dragEl.style.cursor = 'move';

		function dragMouseDown(e) {
			e = e || window.event;
			e.preventDefault();
			// get the mouse cursor position at startup:
			pos3 = e.clientX;
			pos4 = e.clientY;
			document.onmouseup = closeDragElement;
			// call a function whenever the cursor moves:
			document.onmousemove = elementDrag;

			var style = window.getComputedStyle(el);
			if (style.transition != '') {
				el.style.transition = 'inherit';
			}
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
			// stop moving when mouse button is released:
			document.onmouseup = null;
			document.onmousemove = null;
		}
	}

	return instance;
}(ajaxUtils);
