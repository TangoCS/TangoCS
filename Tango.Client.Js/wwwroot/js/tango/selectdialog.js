/// <reference path="homewindow.js"/>
/// <reference path="dialog.js"/>
/// ver. 26-08-2016
var selectSingleObjectDialog = function () {
	var instance = {
		submit: function (id) {
			var d = dialog.instances[id];
			var el = $("#" + $("#" + id).attr('data-value-id'));
			el.val(d.selectedObj);
			dialog.hide(id);
			ajaxUtils.runEventFromElementWithApiResponse(el[0], 'submitdialog', id);
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
				ajaxUtils.delay(this, function (caller) {
					ajaxUtils.runEventFromElementWithApiResponse(caller, 'renderlist', id)
						.done(function () { selectSingleObjectDialog.setupItems(caller, id); });
				});
			});

			commonUtils.setFocus(id + "_filter");
		},
		pagingEvent: function (caller, id) {
			ajaxUtils.runEventFromElementWithApiResponse(caller, 'renderlist', id).done(function () {
				selectSingleObjectDialog.setupItems(caller, id);
			});
		}
	}

	return instance;
}();


var selectMultipleObjectsDialog = function () {
	var instance = {
		submit: function (id) {
			var d = dialog.instances[id];
			var valueelid = $("#" + id).attr('data-value-id');
			$("#" + valueelid).val(d.selectedObjs.join());
			dialog.hide(id);

			var data = {};
			data[valueelid] = d.selectedObjs;
			ajaxUtils.postEventWithApiResponse('submitdialog', id, data);
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
				ajaxUtils.delay(this, function (caller) {
					ajaxUtils.runEventFromElementWithApiResponse(caller, 'renderlist', id)
						.done(function () { selectMultipleObjectsDialog.setupItems(caller, id); });
				});
			});

			commonUtils.setFocus(id + "_filter");
		},
		pagingEvent: function (caller, id) {
			ajaxUtils.runEventFromElementWithApiResponse(caller, 'renderlist', id).done(function () {
				selectMultipleObjectsDialog.setupItems(caller, id);
			});
		}
	}

	return instance;
}();