var dialog = function () {
	var instance = {
		instances : {},
		open: function (caller, serverEvent, id, callBack) {
			return ajaxUtils.runEventWithApiResponse(serverEvent, id).then(function () {
				$('#modalOverlay').css('display', 'block');

				var modal = $('#' + id + "_dialog");
				var modalBody = modal.find('.modal-body');
				modal.css('display', 'block');

				if (modalBody.height() > $(window).height() - 200)
					modalBody.css("height", ($(window).height() - 200) + "px");
				modal.css('zIndex', 101);

				dialog.instances[id] = {};

				var parent = getParent(caller);
				if (parent) {
					parent.css('zIndex', 99);
					dialog.instances[id].parentDialog = parent.id;
				}

				if (callBack) callBack(caller, id);
			});
		},
		hide: function (id) {
			var modal = $('#' + id + "_dialog");
			var objOverlay = $('#modalOverlay');
			var modalBody = modal.find('.modal-body');

			modalBody.css('height', '');
			modal.css('display', 'none');

			if (dialog.instances[id].parentDialog) {
				var p = $('#' + dialog.instances[id].parentDialog);
				p.css('zIndex', 101);
			}
			else {
				objOverlay.css('display', 'none');
			}
			dialog.instances[id].parentDialog = undefined;
		},

		

		getDialog: function (caller) {
			var d = getParent(caller);
			if (d)
				return dialog.instances[d.id];
			else
				return undefined;
		}
	};

	function getParent(caller) {
		var el = caller;
		while (el.parentNode) {
			if (el.attributes.role && el.attributes.role.value == 'dialog') return el;
			el = el.parentNode;
		}
		return undefined;
	};

	return instance;
}();
