var daterangepickerproxy = function (au) {
    var instance = {
        init: function (args) {
            const ctrl = document.getElementById(args.triggerid).parentElement;
            const startDate = document.getElementById(ctrl.id + '_dperiodfrom');
            const startDate_h = document.getElementById(ctrl.id + '_dperiodfromtime_hour');
            const startDate_m = document.getElementById(ctrl.id + '_dperiodfromtime_minute');
            const finishDate = document.getElementById(ctrl.id + '_dperiodto');
            const finishDate_h = document.getElementById(ctrl.id + '_dperiodtotime_hour');
            const finishDate_m = document.getElementById(ctrl.id + '_dperiodtotime_minute');

            const doPostback = ctrl.hasAttribute('data-e');

            if (doPostback) {
            	startDate.addEventListener('keyup', function (e) {
            		au.delay(ctrl, function (caller) { ajaxUtils.postEventFromElementWithApiResponse(caller); });
            	});
            	finishDate.addEventListener('keyup', function (e) {
            		au.delay(ctrl, function (caller) { ajaxUtils.postEventFromElementWithApiResponse(caller); });
            	});
            	if (startDate_h) {
            		startDate_h.addEventListener('change', function (e) {
            			ajaxUtils.postEventFromElementWithApiResponse(ctrl);
            		});
            	}
            	if (startDate_m) {
            		startDate_m.addEventListener('change', function (e) {
            			ajaxUtils.postEventFromElementWithApiResponse(ctrl);
            		});
            	}
            	if (finishDate_h) {
            		finishDate_h.addEventListener('change', function (e) {
            			ajaxUtils.postEventFromElementWithApiResponse(ctrl);
            		});
            	}
            	if (finishDate_m) {
            		finishDate_m.addEventListener('change', function (e) {
            			ajaxUtils.postEventFromElementWithApiResponse(ctrl);
            		});
            	}
            };

            $('#' + args.triggerid).daterangepicker(args.pickerparms, function (start, end, label) {
                if (startDate.value !== start.format('DD.MM.YYYY')) {
                    startDate.value = start.format('DD.MM.YYYY');
                    $(startDate).trigger('input');
                }
                if (finishDate.value !== end.format('DD.MM.YYYY')) {
                    finishDate.value = end.format('DD.MM.YYYY');
                    $(finishDate).trigger('input');
                }
                if (startDate_h && startDate_h.value !== start.hour()) {
                    startDate_h.value = start.hour();
                    $(startDate_h).trigger('change');
                }
                if (startDate_m && startDate_h.value !== start.minute()) {
                    startDate_m.value = start.minute();
                    $(startDate_m).trigger('change');
                }
                if (finishDate_h && finishDate_h.value !== end.hour()) {
                    finishDate_h.value = end.hour();
                    $(finishDate_h).trigger('change');
                }
                if (finishDate_m && finishDate_m.value !== end.minute()) {
                    finishDate_m.value = end.minute();
                    $(finishDate_m).trigger('change');
                }

                if (doPostback)
                	au.postEventFromElementWithApiResponse(ctrl);
            });
        },
        setStartDate: function(args) {
            $('#' + args.triggerid).data('daterangepicker').setStartDate(args.date);
        },
        setFinishDate: function(args) {
            $('#' + args.triggerid).data('daterangepicker').setEndDate(args.date);
        }
    };

    return instance;
}(ajaxUtils);