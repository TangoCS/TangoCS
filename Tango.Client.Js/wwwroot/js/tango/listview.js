var listview = function (au) {
	var instance = {
		togglerow: function (el, e, r) {
			var cls = el.firstChild.className;
			if (cls == 'icon flaticon-collapsed') {
				el.firstChild.className = 'icon flaticon-expanded';
				if (el.parentNode.nextSibling && el.parentNode.nextSibling.id == el.parentNode.id + '_content')
					el.parentNode.nextSibling.style.display = '';
				else
					au.postEventFromElementWithApiResponse(el, { e: e, r: r });
			} else {
				el.firstChild.className = 'icon flaticon-collapsed';
				el.parentNode.nextSibling.style.display = 'none';
			}
		}
	}

	return instance;
}(ajaxUtils);