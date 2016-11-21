var calendarcontrol = function () {
	var instance = {
		keypress: function (event) {
			event = event || window.event;
			var target = event.srcElement || event.target;
			var code = (event.charCode) ? event.charCode : event.keyCode;
			if (code < 47) return true;
			if ((code >= 48 && code <= 57) || code == 46 || code == 0 || code == 32 || code == 58) {
				var pos = getCaretPos(target);
				if (code == 46 && pos != 2 && pos != 5)
					return false;
				if (code == 32 && pos != 10)
					return false;
				if (code == 58 && pos != 13)
					return false;
				if (pos == 0 && target.value.length > 0)
					target.value = '';
				if (pos == 0 && code > 51) {
					target.value = '0' + (target.value.length > 2 ? target.value.substring(0, target.length - 1) : '');
				}
				if (pos == 2 && code != 46) {
					target.value = target.value.substring(0, 2) + '.' + (target.value.length > 4 ? target.value.substring(3, target.length - 1) : '');
				}
				if (pos == 2 && code > 49) {
					target.value = target.value.substring(0, 2) + '.0';
				}
				if (pos == 5 && code != 46) {
					target.value = target.value.substring(0, 5) + '.' + (target.value.length > 7 ? target.value.substring(6, target.length - 1) : '');
				}
				if (pos == 10 && code != 32) {
					target.value = target.value.substring(0, 10) + ' ';
				}
				if (pos == 13 && code != 58) {
					target.value = target.value.substring(0, 13) + ':';
				}
				return true;
			}
			return false;
		}
	};

	function getCaretPos(oField) {
		// Initialize
		var iCaretPos = 0;

		// IE Support
		if (document.selection) {

			// Set focus on the element
			oField.focus();

			// To get cursor position, get empty selection range
			var oSel = document.selection.createRange();

			// Move selection start to 0 position
			oSel.moveStart('character', -oField.value.length);

			// The caret position is selection length
			iCaretPos = oSel.text.length;
		}

		// Firefox support
		else if (oField.selectionStart || oField.selectionStart == '0')
			iCaretPos = oField.selectionStart;

		// Return results
		return (iCaretPos);
	}

	return instance;
}();