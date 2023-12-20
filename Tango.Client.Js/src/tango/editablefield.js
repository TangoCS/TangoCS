window.Tango.inlineFieldHelper = function (field) {
	const imp = document.getElementById(field);
	imp.focus();
	
	const bodyClick = function () {
		const cancelBtn = document.getElementById(field + '_canceledit');
		ajaxUtils.postEventFromElementWithApiResponse(cancelBtn);
		document.body.removeEventListener('click', bodyClick);
	};

	document.body.addEventListener('click', bodyClick);
};