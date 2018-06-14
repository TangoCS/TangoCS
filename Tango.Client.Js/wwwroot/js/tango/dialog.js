var dialog = function (au) {
	var instance = {
		widgetWillMount: function (shadow, state) {
			var modal = shadow.getElementById(state.root);
			modal.classList.add('md_show');
			modal.style.zIndex = 101;
		},
		open: function (caller, id) {
			//var modal = $('#' + id + "_dialog");
			//modal.addClass('md_show');
			//modal.css('zIndex', 101);

			//var parent = getParent(caller);
			//if (parent) {
			//	parent.css('zIndex', 99);
			//	instance.instances[id].parentDialog = parent.id;
			//}
		},
		onResult: function (res, state) {
			var modal = document.getElementById(state.root);
			modal.classList.remove('md_show');

			//if (instance.instances[modal.id].parentDialog) {
			//	var p = $('#' + instance.instances[modal.id].parentDialog);
			//	p.css('zIndex', 101);
			//}

			//instance.instances[modalid].parentDialog = undefined;

			if (res == 0) {
				return true;
			}
		}
	};

	return instance;
}(ajaxUtils);
