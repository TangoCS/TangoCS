/// <reference path="../jquery/jquery-1.11.0.min.js"/>
var homePage = function () {
    var instance = {
        splitterClick: function () {
        	if ($("#sidebar").css("display") != "none") {
                $.cookie("nav", "0", { path: '/' });
                hideNav();
            }
            else {
                $.cookie("nav", "", { path: '/' });
                showNav();
                homePage.countNavBodyHeight();
            }
        },
        initNav: function () {
        	if ($.cookie("nav") == "0") {
        		hideNav();
        	}
        	else {
        		showNav();
        	}
        },
        countNavBodyHeight: function () {
        	var h1 = $(window).height() - $(".header-title").outerHeight();
        	var h = h1 - $(".nav-header").outerHeight() - $(".nav-buttonsbar").outerHeight();
        	var h2 = h1 - $(".contentheader").outerHeight() - $(".ms-menutoolbar").outerHeight();
            $(".nav-body").css("height", h + "px");
            $("#contentbody").css("height", h2 + "px");         
        }
    };

    function showNav() {
        $("#splitter").css("margin-left", "240px");
        $("#content").css("margin-left", "248px");
        $("#sidebar").css("display", "block");
    }
    function hideNav() {
        $("#splitter").css("margin-left", "0px");
        $("#content").css("margin-left", "8px");
        $("#sidebar").css("display", "none");
    }

    return instance;
}();