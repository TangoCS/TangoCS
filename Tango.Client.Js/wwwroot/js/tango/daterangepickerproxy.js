var daterangepickerproxy = function (au) {
    const from = '_dperiodfrom';
    const to = '_dperiodto';
    const timehour = 'time_hour';
    const timemin = 'time_minute';

    var instance = {
        init: function (args) {
            const ctrl = document.getElementById(args.triggerid).parentElement;
            const startDate = document.getElementById(ctrl.id + from);
            const startDate_h = document.getElementById(ctrl.id + from + timehour);
            const startDate_m = document.getElementById(ctrl.id + from + timemin);
            const finishDate = document.getElementById(ctrl.id + to);
            const finishDate_h = document.getElementById(ctrl.id + to + timehour);
            const finishDate_m = document.getElementById(ctrl.id + to + timemin);

            const doPostback = ctrl.hasAttribute('data-e');

            const _args = {
                remove: [
                    ctrl.id + from, ctrl.id + from + timehour, ctrl.id + from + timemin,
                    ctrl.id + to, ctrl.id + to + timehour, ctrl.id + to + timemin
                ]
            };

            var postEvent = function (caller) {
                au.changeUrl(_args);
                ajaxUtils.postEventFromElementWithApiResponse(caller);
            }

            if (doPostback) {
                startDate.addEventListener('keyup', function (e) {
                    au.delay(ctrl, function (caller) { postEvent(caller); });
                });
                finishDate.addEventListener('keyup', function (e) {
                    au.delay(ctrl, function (caller) {
                        postEvent(caller);
                    });
                });
                if (startDate_h) {
                    startDate_h.addEventListener('change', function (e) {
                        postEvent(ctrl);
                    });
                }
                if (startDate_m) {
                    startDate_m.addEventListener('change', function (e) {
                        postEvent(ctrl);
                    });
                }
                if (finishDate_h) {
                    finishDate_h.addEventListener('change', function (e) {
                        postEvent(ctrl);
                    });
                }
                if (finishDate_m) {
                    finishDate_m.addEventListener('change', function (e) {
                        postEvent(ctrl);
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
                    postEvent(ctrl);
            });
        },
        setStartDate: function (args) {
            $('#' + args.triggerid).data('daterangepicker').setStartDate(args.date);
        },
        setFinishDate: function (args) {
            $('#' + args.triggerid).data('daterangepicker').setEndDate(args.date);
        },
        getValue: function (id, formData) {
            return {
                from: moment(formData[id + from] + ' ' + formData[id + from + timehour] + ':' + formData[id + from + timemin], 'DD.MM.YYYY HH:mm').toDate(),
                to: moment(formData[id + to] + ' ' + formData[id + to + timehour] + ':' + formData[id + to + timemin], 'DD.MM.YYYY HH:mm').toDate(),
            }
        },
        setValue: function (id, sd, fd) {
            const startDate = document.getElementById(id + from);
            const startDate_h = document.getElementById(id + from + timehour);
            const startDate_m = document.getElementById(id + from + timemin);
            const finishDate = document.getElementById(id + to);
            const finishDate_h = document.getElementById(id + to + timehour);
            const finishDate_m = document.getElementById(id + to + timemin);

            startDate.value = sd.getDate() + '.' + (sd.getMonth() + 1).toString().padStart(2, '0') + '.' + sd.getFullYear();
            startDate_h.value = sd.getHours();
            startDate_m.value = sd.getMinutes();
            finishDate.value = fd.getDate() + '.' + (fd.getMonth() + 1).toString().padStart(2, '0') + '.' + fd.getFullYear();
            finishDate_h.value = fd.getHours();
            finishDate_m.value = fd.getMinutes();
        }
    };

    return instance;
}(ajaxUtils);