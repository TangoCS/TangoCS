/// <reference path="homewindow.js"/>
/// <reference path="dialog.js"/>
/// ver. 26-08-2016
var selectSingleObjectDialog = function (au, cu, dialog) {
	var instance = {
		submit: function (id) {
			var d = dialog.instances[id];
			var el = $("#" + $("#" + id).attr('data-value-id'));
			el.val(d.selectedObj);
			dialog.hide(id);
			au.runEventFromElementWithApiResponse(el[0], { e: 'submitdialog', r: id });
		},
		setupItems: function (caller, id) {
			var d = dialog.instances[id];
			if (!d.selectedObj)
				d.selectedObj = $("#" + $("#" + id).attr('data-value-id')).val();

			$("#" + id + "_list :input").each(function () {
				var el = $(this);
				if (el.val() == d.selectedObj) {
					el.prop('checked', true);
				}
				el.on('change', { dialog: d }, function (e) {
					e.data.dialog.selectedObj = this.value;
				});
			});

			$("#" + id + "_filter").on('keyup', { id: id }, function (e) {
				au.delay(this, function (caller) {
					au.runEventFromElementWithApiResponse(caller, { e: 'renderlist', r: id })
						.done(function () { instance.setupItems(caller, id); });
				});
			});

			cu.setFocus(id + "_filter");
		},
		pagingEvent: function (caller, id) {
			au.runEventFromElementWithApiResponse(caller, { e: 'renderlist', r: id }).done(function () {
				instance.setupItems(caller, id);
			});
		}
	}

	return instance;
}(ajaxUtils, commonUtils, dialog);


var selectMultipleObjectsDialog = function (au, cu, dialog) {
	var instance = {
		submit: function (id) {
			var d = dialog.instances[id];
			var valueelid = $("#" + id).attr('data-value-id');
			$("#" + valueelid).val(d.selectedObjs.join());
			dialog.hide(id);

			var data = {};
			data[valueelid] = d.selectedObjs;
			au.postEventWithApiResponse({ e: 'submitdialog', r: id }, data);
		},
		setupItems: function (caller, id) {
			var d = dialog.instances[id];
			if (!d.selectedObjs)
				d.selectedObjs = $("#" + $("#" + id).attr('data-value-id')).val().split(",");

			$("#" + id + "_list :input").each(function () {
				var el = $(this);
				if ($.inArray(el.val(), d.selectedObjs) >= 0) {
					el.prop('checked', true);
				}
				el.on('change', { dialog: d }, function (e) {
					if (this.checked) {
						e.data.dialog.selectedObjs.push(this.value);
					}
					else {
						var index = e.data.dialog.selectedObjs.indexOf(this.value);
						e.data.dialog.selectedObjs.splice(index, 1);
					}
				});
			});

			$("#" + id + "_filter").on('keyup', { id: id }, function (e) {
				au.delay(this, function (caller) {
					au.runEventFromElementWithApiResponse(caller, { e: 'renderlist', r: id })
						.done(function () { instance.setupItems(caller, id); });
				});
			});

			cu.setFocus(id + "_filter");
		},
		pagingEvent: function (caller, id) {
			au.runEventFromElementWithApiResponse(caller, { e: 'renderlist', r: id }).done(function () {
				instance.setupItems(caller, id);
			});
		}
	}

	return instance;
}(ajaxUtils, commonUtils, dialog);