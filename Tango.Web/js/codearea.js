function createCodeArea(id, height) {
    var el = document.createElement('div');
    var ta = document.getElementById(id);
    var string = '';
    for (var no = 1; no < 999; no++) {
        if (string.length > 0) string += '<br>';
        string += no;
    }
    el.className = 'textAreaWithLines';
    el.style.height = height;
    el.style.width = "30px";
    el.style.position = "absolute";
    el.style.overflow = 'hidden';
    el.style.textAlign = 'center';
    el.style.fontFamily = 'Consolas,Courier New Cyr';
    el.style.fontSize = '14px';
    el.innerHTML = string;
    el.value = string;
    el.style.zIndex = 0;
    el.id = 'lines';
    ta.style.zIndex = 1;
    ta.style.position = "relative";
    ta.parentNode.insertBefore(el, ta.nextSibling);
    setLine();

    ta.onkeydown = function () { setLine(); }
    ta.onmousedown = function () { setLine(); }
    ta.onscroll = function () { setLine(); }
    ta.onblur = function () { setLine(); }
    ta.onfocus = function () { setLine(); }
    ta.onmouseover = function () { setLine(); }
    ta.onmouseup = function () { setLine(); }

    function setLine() {
        el.scrollTop = ta.scrollTop;
        el.style.top = (ta.offsetTop) + "px";
        el.style.left = (ta.offsetLeft - 30) + "px";
    }
}