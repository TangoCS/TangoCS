var nt_listtoolbar_ddmenuitem = 0;
var nt_listtoolbar_ddmenu = 0;
var nt_listtoolbar_skipclose = 0;
function nt_listtoolbar_open(obj) {
    obj.className = 'ms-menubuttonactivehover';
    
    if (nt_listtoolbar_ddmenuitem) {
        nt_listtoolbar_ddmenuitem.style.visibility = 'hidden';
        nt_listtoolbar_ddmenuitem.style.display = 'none';
        nt_listtoolbar_ddmenuitem.style.left = '';
    }

    nt_listtoolbar_ddmenuitem = obj.nextSibling;
    while (nt_listtoolbar_ddmenuitem.tagName != "DIV")
        nt_listtoolbar_ddmenuitem = nt_listtoolbar_ddmenuitem.nextSibling;
    
    var d = document.createElement("div");
    d.style.position = 'absolute';
    obj.appendChild(d);
    var sw = d.offsetParent.scrollWidth;
    obj.removeChild(d);
    
    nt_listtoolbar_ddmenuitem.style.visibility = 'visible';
    nt_listtoolbar_ddmenuitem.style.display = '';
    var w = browserWidth();

    if (nt_listtoolbar_ddmenuitem.scrollWidth + nt_listtoolbar_ddmenuitem.offsetLeft + nt_listtoolbar_ddmenuitem.offsetParent.offsetLeft > w) {
    	//    nt_listtoolbar_ddmenuitem.style.left = sw - nt_listtoolbar_ddmenuitem.clientWidth - 25 + 'px';

    	var offs = obj.clientWidth - nt_listtoolbar_ddmenuitem.clientWidth + 3;
    	nt_listtoolbar_ddmenuitem.style.marginLeft = offs + "px";
    }
       nt_listtoolbar_skipclose = 1;
    window.setTimeout(function() { nt_listtoolbar_skipclose = 0; }, 50);
    window.setTimeout(function() { $(document).one('click', nt_listtoolbar_close); }, 100);
}

function nt_listtoolbar_close() {
	if (nt_listtoolbar_skipclose == 1) {
		nt_listtoolbar_skipclose = 0;
		return;
	}
    if (nt_listtoolbar_ddmenuitem) {
        nt_listtoolbar_ddmenuitem.style.visibility = 'hidden';
        nt_listtoolbar_ddmenuitem.style.display = 'none';
        nt_listtoolbar_ddmenuitem.style.left = '';
        nt_listtoolbar_ddmenuitem = 0;
        if (nt_listtoolbar_ddmenu) {
            nt_listtoolbar_ddmenu.className = 'ms-menubuttoninactivehover';
            nt_listtoolbar_ddmenu = 0;
        }
    }
}

function nt_listtoolbar_mouseover(obj) {
    obj.className = 'ms-menubuttonactivehover';
    if (nt_listtoolbar_ddmenu) {
        nt_listtoolbar_ddmenu.className = 'ms-menubuttoninactivehover';
        nt_listtoolbar_close();
        nt_listtoolbar_open(obj);
    }

    nt_listtoolbar_ddmenu = obj;
}

function nt_listtoolbar_mouseout(obj) {
    //nt_listtoolbar_closetime();
    if (nt_listtoolbar_ddmenuitem == 0)
        obj.className = 'ms-menubuttoninactivehover';
    if (!nt_listtoolbar_ddmenuitem)
        nt_listtoolbar_ddmenu = 0;
}

function nt_listtoolbar_mouseover_tbl(obj) {
    obj.className = 'ms-MenuUIItemTableHover';
    obj.parentNode.style.padding = '1px';
    obj.parentNode.className = 'ms-MenuUIItemTableCellHover';
}

function nt_listtoolbar_mouseout_tbl(obj) {
    obj.className = 'ms-MenuUIItemTable';
    obj.parentNode.style.padding = '2px';
    obj.parentNode.className = 'ms-MenuUIItemTableCell';
}


function browserWidth() {
    var viewportwidth;
    // the more standards compliant browsers (mozilla/netscape/opera/IE7) use window.innerWidth and window.innerHeight
    if (typeof window.innerWidth != 'undefined') {
        viewportwidth = window.innerWidth - 18;
    }
    // IE6 in standards compliant mode (i.e. with a valid doctype as the first line in the document)
    else if (typeof document.documentElement != 'undefined' &&
	 typeof document.documentElement.clientWidth != 'undefined' &&
	 document.documentElement.clientWidth != 0) {
        viewportwidth = document.documentElement.clientWidth - 4;
    }
    // older versions of IE
    else {
        viewportwidth = document.getElementsByTagName('body')[0].clientWidth - 4;
    }
    return viewportwidth;
}
