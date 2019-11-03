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

			var defStartDate = startDate.value;
			if (startDate_h)
				defStartDate += ' ' + startDate_h.value + ':' + startDate_m.value;
			var defFinishDate = finishDate.value;
			if (finishDate_h)
				defFinishDate += ' ' + finishDate_h.value + ':' + finishDate_m.value;

			$('#' + args.triggerid).daterangepicker({
				"showDropdowns": true,
				"timePicker": startDate_h != null,
				"timePicker24Hour": startDate_h != null,
				"timePickerIncrement": 30,
				ranges: {
					'Сегодня': [moment().startOf('day'), moment().endOf('day').subtract(30, 'minutes').add(1, 'seconds')],
					'Вчера': [moment().subtract(1, 'days').startOf('day'), moment().subtract(1, 'days').endOf('day').subtract(30, 'minutes').add(1, 'seconds')],
					'Текущий месяц': [moment().startOf('month'), moment().endOf('month').subtract(30, 'minutes').add(1, 'seconds')],
					'Предыдущий месяц': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month').subtract(30, 'minutes').add(1, 'seconds')]
				},
				"locale": {
					"format": startDate_h ? "DD.MM.YYYY HH:mm" : "DD.MM.YYYY",
					"separator": " - ",
					"applyLabel": "Применить",
					"cancelLabel": "Отмена",
					"fromLabel": "С",
					"toLabel": "По",
					"customRangeLabel": "Пользовательский",
					"weekLabel": "W",
					"daysOfWeek": [
						"Вс",
						"Пн",
						"Вт",
						"Ср",
						"Чт",
						"Пт",
						"Сб"
					],
					"monthNames": [
						"Январь",
						"Февраль",
						"Март",
						"Апрель",
						"Май",
						"Июнь",
						"Июль",
						"Август",
						"Сентябрь",
						"Октябрь",
						"Ноябрь",
						"Декабрь"
					],
					"firstDay": 1
				},
				"showCustomRangeLabel": false,
				"alwaysShowCalendars": true,
				"startDate": defStartDate,
				"endDate": defFinishDate
			}, function (start, end, label) {
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


