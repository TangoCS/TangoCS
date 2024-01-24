window.Tango.inlineFieldHelper = function (field) {
	const inp = document.getElementById(field);
	inp.focus();
	inp.select();
	const cancelBtn = document.getElementById(field + '_canceledit');
	const submitBtn = document.getElementById(field + '_submitedit');
	
	const bodyClick = function (e) {
		if (inp.contains(e.target))
			return;

		document.body.removeEventListener('click', bodyClick);

		if (cancelBtn.contains(e.target) || submitBtn.contains(e.target))
			return;

		ajaxUtils.postEventFromElementWithApiResponse(cancelBtn);
	};

	document.body.addEventListener('click', bodyClick);
};