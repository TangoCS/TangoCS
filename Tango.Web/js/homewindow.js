var buttonsCount = 0;
var buttonHeight = 0;
var navOffset = 0;
var navTitleHeight = 0;
var wsdivOffset = 0;


function resize_maindiv() {
    var delta = 17;
    if (navigator.appName == 'Microsoft Internet Explorer')
        delta = 2;
    var hdelta = 8;

    if (document.getElementById('navmenuexcurrentgrouplist') != null) {
        var v = $(window).height() - navOffset - buttonHeight * buttonsCount - navTitleHeight - delta;
        if (v < 0) v = 1;
        document.getElementById('navmenuexcurrentgrouplist').style.height = v + 'px';

        hdelta = 230;
    }

    if ($(window).width() > 210) {
        var tables = document.getElementsByTagName('table');
        for (var i = 0; i < tables.length; i++) {
            var c = tables.item(i).attributes.getNamedItem('class');
            if (c != null) {
                if (c.nodeValue == 'ms-menutoolbar') {
                    tables.item(i).style.width = '0px';
                    document.getElementById('toolbarhead').style.width = '0px';
                }
            }
        }
        var paddingOffset = document.getElementById('toolbarhead') != null ?
                parseInt(document.getElementById('toolbarhead').parentNode.style.paddingLeft.replace('px', ''), 10) +
                parseInt(document.getElementById('toolbarhead').parentNode.style.paddingRight.replace('px', ''), 10) : 0;
        if (paddingOffset == 16)
            paddingOffset = 10;
        else
            paddingOffset = 0;

        var v2 = $(window).height() - wsdivOffset;
        if (v2 < 0) v2 = 1;
        document.getElementById('wsdiv').style.height = v2 + 'px';

        var sidebar = document.getElementById('sidebar');
        if (sidebar != null) {
            if (sidebar.style.display != 'none')
                document.getElementById('wsdiv').style.width = $(window).width() - hdelta + 'px';
            else
                document.getElementById('wsdiv').style.width = $(window).width() - 8 + 'px';
        }
        else
            document.getElementById('wsdiv').style.width = $(window).width() - hdelta + 'px';


        for (var i = 0; i < tables.length; i++) {
            var c = tables.item(i).attributes.getNamedItem('class');
            if (c != null) {
                if (c.nodeValue == 'ms-menutoolbar') {
                    tables.item(i).style.width = document.getElementById('wsdiv').scrollWidth - paddingOffset - paddingOffset + 'px';
                    document.getElementById('toolbarhead').style.width = tables.item(i).style.width;
                }
            }
        }
    }
}

$(document).ready(
	        function () {
	            buttonsCount = $(".navmenubutton").length;
	            buttonHeight = $(".navmenubutton").height();
	            navOffset = $("#navmenuex").offset().top;
	            wsdivOffset = $("#wsdiv").offset().top;
	            navTitleHeight = $("#navmenutitle").height();

	            resize_maindiv();
	            var tr = document.getElementById('__asptrace');
	            if (tr != null) {
	                var log = document.getElementById('tracelog');
	                log.appendChild(tr);
	            }
	        }
	    );
$(window).resize(function () {
    resize_maindiv();
});
