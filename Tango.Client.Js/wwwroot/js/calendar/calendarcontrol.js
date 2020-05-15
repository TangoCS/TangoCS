var calendarcontrol = function () {
    var instance = {
        init: function (elid) {
            const el = document.getElementById(elid);
            el.onkeydown = onkeydown;
            el.onkeypress = onkeypress;
            el.ondrop = ondrop;
            el.onpaste = onpaste;
        }
    };

    function ondrop(event) {
        const str = event.dataTransfer.getData("text");
        const d = Date.parseDate(str, '%d.%m.%Y');
        if (!d) return false;
        $(this).trigger('input');
        return true;
    }
    function onpaste(event) {
        const str = event.clipboardData.getData("text");
        const d = Date.parseDate(str, '%d.%m.%Y');
        if (!d) return false;
        $(this).trigger('input');
        return true;
    }

    function onkeydown(event) {
        const target = event.srcElement;
        const pos = getCaretPos(target);
        const len = target.value.length;

        if (event.keyCode === 46) { // del
            target.value = target.value.substring(0, pos);
            $(this).trigger('input');
            return false;
        }
        if (event.keyCode === 8 && pos !== len) { //backspace
            return false;
        }
    }

    function onkeypress(event) {
        var char = String.fromCharCode(event.charCode);
        const target = event.srcElement;

        if ('0123456789'.indexOf(char) === -1)
            return false;
        $(this).trigger('input');

        var pos = getCaretPos(target);
        var pos0 = pos === 2 || pos === 5 ? pos + 1 : pos;
        const char0 = char;
        var len = target.value.length;
        var val = target.value;

        function next() {
            if (pos === 2 || pos === 5)
                pos++;
            if (pos === pos0)
                val = val.substring(0, pos0) + char0 + val.substring(pos0 + 1);
            pos++;
            if (pos === 2 || pos === 5 || pos === 8)
                pos++;
            char = val.substring(pos, pos + 1);
        }

        function reject() {
            if (pos0 < len)
                target.value = target.value.substring(0, pos0);
            if (pos > pos0)
                target.value = target.value + char0;
            return false;
        }

        if (pos === 0) {
            if ('0123'.indexOf(char) === -1)
                return reject();
            if (pos < len) next();
        }

        if (pos === 1) {
            if (val.substring(0, 1) === '3' && '01'.indexOf(char) === -1)
                return reject();
            if (pos < len) next();
        }

        if (pos === 2 || pos === 3) {
            const day = val.substring(0, 2);
            if (day === '31' && '013578'.indexOf(char) === -1)
                return reject();
            if (day === '30' && char === '2')
                return reject();
            if (pos < len) {
                if ('01'.indexOf(char) === -1) {
                    pos = pos0 = 4;
                    char = char0;
                    target.setSelectionRange(pos0 + 1, pos0 + 1);
                }
                else
                    next();
            }
        }

        if (pos === 4) {
            const day = val.substring(0, 2);
            const m1 = val.substring(3, 4);
            if (day === '31' && m1 === '0' && '13578'.indexOf(char) === -1)
                return reject();
            if (day === '31' && m1 === '1' && char === '1')
                return reject();
            if (day === '30' && m1 === '0' && char === '2')
                return reject();
            if (m1 === '1' && '012'.indexOf(char) === -1)
                return reject();
            if (pos < len) next();
        }

        if (pos === 5 || pos === 6) {
            if ('12'.indexOf(char) === -1)
                return reject();
            if (pos < len) next();
        }

        if (pos === 7) {
            const y1 = val.substring(6, 7);
            if (y1 === '1' && char !== '9')
                return reject();
            if (y1 === '2' && char !== '0')
                return reject();
            if (pos < len) next();
        }

        if (pos === 9 && val.substring(0, 5) === '29.02') {
            const year = parseInt(val.substring(6, 9) + char);
            const isLeap = ((year % 4 === 0) && (year % 100 !== 0)) || (year % 400 === 0);
            if (!isLeap)
                return reject();
        }

        if (pos > 9)
            return reject();

        if (len === 2) {
            target.value += '.';
            if ('01'.indexOf(char0) === -1) {
                target.value += '0';
            }
        }
        else if (len === 3 && '01'.indexOf(char0) === -1) {
            target.value += '0';
        }
        else if (len === 5 && pos0 > len) {
            target.value += '.';
        }


        if (pos0 < len) {
            target.value = target.value.substring(0, pos0) + char0 + target.value.substring(pos0 + 1);
            target.setSelectionRange(pos0 + 1, pos0 + 1);
            return false;
        }

        return true;
    }

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
        else if (oField.selectionStart || oField.selectionStart === '0')
            iCaretPos = oField.selectionStart;

        // Return results
        return (iCaretPos);
    }

    return instance;
}();