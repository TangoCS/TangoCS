﻿isIE=document.all;

function ddInit(e){
  topDog="HTML";
  hotDog=isIE ? event.srcElement : e.target;  
  while (hotDog.getAttribute('titleBar')!="titleBar"&&hotDog.tagName!=topDog){
    hotDog=isIE ? hotDog.parentElement : hotDog.parentNode;
  }  
  if (hotDog.getAttribute('titleBar')=="titleBar"){
    offsetx=isIE ? event.clientX : e.clientX;
    offsety=isIE ? event.clientY : e.clientY;
    whichDog=isIE ? hotDog.parentElement.parentElement.parentElement.parentElement : hotDog.parentNode.parentNode.parentNode.parentNode;
    nowX=parseInt(whichDog.style.left);
    nowY=parseInt(whichDog.style.top);
    ddEnabled=true;
    document.onmousemove=dd;
  }
}

function dd(e){
  if (!ddEnabled) return;
  whichDog.style.left=(isIE ? nowX+event.clientX-offsetx : nowX+e.clientX-offsetx) + 'px'; 
  whichDog.style.top=(isIE ? nowY+event.clientY-offsety : nowY+e.clientY-offsety) + 'px';
  return false;  
}

document.onmousedown=ddInit;
document.onmouseup=Function("ddEnabled=false");

function getPageScroll(){

	var yScroll;

	if (self.pageYOffset) {
		yScroll = self.pageYOffset;
	} else if (document.documentElement && document.documentElement.scrollTop){	 // Explorer 6 Strict
		yScroll = document.documentElement.scrollTop;
	} else if (document.body) {// all other Explorers
		yScroll = document.body.scrollTop;
	}

	arrayPageScroll = new Array('',yScroll) 
	return arrayPageScroll;
}

function getPageSize(){
	
	var xScroll, yScroll;
	
	if (window.innerHeight && window.scrollMaxY) {	
		xScroll = document.body.scrollWidth;
		yScroll = window.innerHeight + window.scrollMaxY;
	} else if (document.body.scrollHeight > document.body.offsetHeight){ // all but Explorer Mac
		xScroll = document.body.scrollWidth;
		yScroll = document.body.scrollHeight;
	} else { // Explorer Mac...would also work in Explorer 6 Strict, Mozilla and Safari
		xScroll = document.body.offsetWidth;
		yScroll = document.body.offsetHeight;
	}
	
	var windowWidth, windowHeight;
	if (self.innerHeight) {	// all except Explorer
		windowWidth = self.innerWidth;
		windowHeight = self.innerHeight;
	} else if (document.documentElement && document.documentElement.clientHeight) { // Explorer 6 Strict Mode
		windowWidth = document.documentElement.clientWidth;
		windowHeight = document.documentElement.clientHeight;
	} else if (document.body) { // other Explorers
		windowWidth = document.body.clientWidth;
		windowHeight = document.body.clientHeight;
	}	
	
	// for small pages with total height less then height of the viewport
	if(yScroll < windowHeight){
		pageHeight = windowHeight;
	} else { 
		pageHeight = yScroll;
	}

	// for small pages with total width less then width of the viewport
	if(xScroll < windowWidth){	
		pageWidth = windowWidth;
	} else {
		pageWidth = xScroll;
	}


	arrayPageSize = new Array(pageWidth,pageHeight,windowWidth,windowHeight) 
	return arrayPageSize;
}

function showModalPopup()
{
	// prep objects
	var objOverlay = document.getElementById('overlay1');
	var objLoadingImage = document.getElementById('loadingImage');
	
	
	var arrayPageSize = getPageSize();
	var arrayPageScroll = getPageScroll();

	// center loadingImage if it exists
	if (objLoadingImage) {
		objLoadingImage.style.top = (arrayPageScroll[1] + ((arrayPageSize[3] - 35 - objLoadingImage.height) / 2) + 'px');
		objLoadingImage.style.left = (((arrayPageSize[0] - 20 - objLoadingImage.width) / 2) + 'px');
		objLoadingImage.style.display = 'block';
		objLoadingImage.style.visibility = 'visible';
	}

	// set height of Overlay to take up whole page and show
	objOverlay.style.height = (arrayPageSize[1] + 'px');
	objOverlay.style.display = 'block';
	objOverlay.style.zIndex = 61;
	
	var arVersion = navigator.appVersion.split("MSIE")
    var version = parseFloat(arVersion[1])

    if (version < 7)
    {
        var selects = document.getElementsByTagName('select');
        for(var i = 0; i < selects.length; i++)
        {
            selects[i].style.display = 'none';
        }
	}
}

function hideModalPopup()
{
	document.getElementById('overlay1').style.display = 'none';
	
	var arVersion = navigator.appVersion.split("MSIE")
    var version = parseFloat(arVersion[1])

    if (version < 7)
    {
        var selects = document.getElementsByTagName('select');
        for(var i = 0; i < selects.length; i++)
        {
            selects[i].style.display = 'block';
        }
	}
}

function initModalPopup()
{
	var objBody = document.getElementsByTagName("body").item(0);
	
	// create overlay div and hardcode some functional styles (aesthetic styles are in CSS file)
	var objOverlay = document.getElementById('overlay1');
	if (objOverlay == null)
	{
		objOverlay = document.createElement("div");
		objOverlay.setAttribute('id','overlay1');
		objOverlay.style.display = 'none';
		var mdp = document.getElementById('ModalDialogPlace');
		mdp.insertBefore(objOverlay, mdp.firstChild);
		//objBody.insertBefore(objOverlay, objBody.firstChild);
		
		var objLoadingImage = document.createElement("img");
		objLoadingImage.src = '/i/n/loading.gif';
		objLoadingImage.setAttribute('id','loadingImage');
		objLoadingImage.setAttribute('alt','Идет обращение к серверу. Пожалуйста, подождите...');
		objLoadingImage.style.position = 'absolute';
		objLoadingImage.style.zIndex = '150';
		objOverlay.appendChild(objLoadingImage);
	}
}


var mdm_hfOpenDialogs;
function mdm_showmodalpopup() {
	var od = parseInt(document.getElementById(mdm_hfOpenDialogs).value);
	if (od == 0) {
		document.getElementById('ModalDialogPlace').style.display = 'block';
		showModalPopup();
	}
	else {
		document.getElementById('loadingImage').style.visibility = 'visible';
		document.getElementById('overlay1').style.zIndex = 61 + 2 * od;
	}
	document.getElementById(mdm_hfOpenDialogs).value = od + 1;
}
function mdm_hidemodalpopup() {
	var od = parseInt(document.getElementById(mdm_hfOpenDialogs).value);
	document.getElementById(mdm_hfOpenDialogs).value = od - 1;
	if (od == 1) {
		hideModalPopup();
		document.getElementById('ModalDialogPlace').style.display = 'none';
	}
	else {
		document.getElementById('loadingImage').style.visibility = 'hidden';
		document.getElementById('overlay1').style.zIndex = 57 + 2 * od;
	}
}
function mdm_getzindex() {
	return 60 + 2 * parseInt(document.getElementById(mdm_hfOpenDialogs).value);
}
function mdm_disable() {
	document.getElementById('ModalDialogPlace').style.zIndex = 10;
}
function mdm_enable() {
	document.getElementById('ModalDialogPlace').style.zIndex = 90;
}


/*function addLoadEvent(func)
{	
	var oldonload = window.onload;
	if (typeof window.onload != 'function'){
    	window.onload = func;
	} else {
		window.onload = function(){
		oldonload();
		func();
		}
	}
}

addLoadEvent(initModalPopup);*/