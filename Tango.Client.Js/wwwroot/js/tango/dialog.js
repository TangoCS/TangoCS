﻿var dialog = function (au) {
	var instance = {
		instances: {},
		runeventandopen: function (caller, serverEvent, id, callBack) {
			if (!id) id = caller.getAttribute('data-c-id');
			return au.runEventFromElementWithApiResponse(caller, { e: serverEvent, r: id })
			.then(function () {
				instance.open(caller, id);
				if (callBack) callBack(caller, id);
			});
		},
		rundefeventandopen: function (caller, callBack) {
			var id = caller.getAttribute('data-c-id');
			return au.runEventFromElementWithApiResponse(caller)
			.then(function () {
				instance.open(caller, id);
				if (callBack) callBack(caller, id);
			});
		},
		open: function (caller, id) {
			$('#modalOverlay').css('display', 'block');

			var modal = $('#' + id + "_dialog");
			modal.addClass('visible');
			modal.css('zIndex', 101);
			instance.instances[id] = {};

			var parent = getParent(caller);
			if (parent) {
				parent.css('zIndex', 99);
				instance.instances[id].parentDialog = parent.id;
			}
		},
		hide: function (id) {
			var modal = $('#' + id + "_dialog");
			var objOverlay = $('#modalOverlay');
			var modalBody = modal.find('.modal-body');

			modalBody.css('height', '');
			modal.removeClass('visible');

			if (instance.instances[id].parentDialog) {
				var p = $('#' + instance.instances[id].parentDialog);
				p.css('zIndex', 101);
			}
			else {
				objOverlay.css('display', 'none');
			}
			instance.instances[id].parentDialog = undefined;
		},

		getDialog: function (caller) {
			var d = getParent(caller);
			if (d)
				return instance.instances[d.id];
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
}(ajaxUtils);