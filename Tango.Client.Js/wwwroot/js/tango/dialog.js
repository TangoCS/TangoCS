var dialog = function (au) {
	var instance = {
		widgetWillMount: function (shadow, state) {
			var modal = shadow.getElementById(state.root);
			modal.classList.add('md-show');
			modal.style.zIndex = 101;
			//var parent = getParent(caller);
			//if (parent) {
			//	parent.css('zIndex', 99);
			//	instance.instances[id].parentDialog = parent.id;
			//}
		},
		onResult: function (res, state) {
			var modal = document.getElementById(state.root);
			modal.classList.remove('md-show');

			//if (instance.instances[modal.id].parentDialog) {
			//	var p = $('#' + instance.instances[modal.id].parentDialog);
			//	p.css('zIndex', 101);
			//}

			if (res == 0) {
				return false;
			}
		}
	};

	return instance;
}(ajaxUtils);
