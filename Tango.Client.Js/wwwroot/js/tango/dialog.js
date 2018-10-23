var dialog = function (au) {
	var instance = {
		open: function (el) {
			if (!el.nodeType) el = document.getElementById(el);
			el.classList.add('md-show');
			el.style.zIndex = 101;
		},
		close: function (el) {
			if (!el.nodeType) el = document.getElementById(el);
			el.classList.remove('md-show');
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

	return instance;
}(ajaxUtils);
