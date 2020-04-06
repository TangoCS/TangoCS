var daterangepickerproxy = function () {
	var instance = {
		init: function (args) {
			const ctrl = document.getElementById(args.triggerid).parentElement;
			const startDate = document.getElementById(ctrl.id + '_dperiodfrom');
			const startDate_h = document.getElementById(ctrl.id + '_dperiodfromtime_hour');
			const startDate_m = document.getElementById(ctrl.id + '_dperiodfromtime_minute');
			const finishDate = document.getElementById(ctrl.id + '_dperiodto');
			const finishDate_h = document.getElementById(ctrl.id + '_dperiodtotime_hour');
			const finishDate_m = document.getElementById(ctrl.id + '_dperiodtotime_minute');

			$('#' + args.triggerid).daterangepicker(args.pickerparms, function (start, end, label) {
				startDate.value = start.format('DD.MM.YYYY');
				finishDate.value = end.format('DD.MM.YYYY');
				if (startDate_h) startDate_h.value = start.hour();
				if (startDate_m) startDate_m.value = start.minute();
				if (finishDate_h) finishDate_h.value = end.hour();
				if (finishDate_m) finishDate_m.value = end.minute();
			});
		}
	};

	return instance;
}();


