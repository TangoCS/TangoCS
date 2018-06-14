var tabs = function (au) {
	var instance = {
		onselect: function (el) {
			var id = el.parentNode.parentNode.parentNode.getAttribute('data-parmname');
			var index = [].indexOf.call(el.parentNode.parentNode.children, el.parentNode);
			var pages = document.getElementById(id.toLowerCase() + '_pages').children;
			for (i = 0; i < pages.length; i++) {
				pages[i].className = i == index ? 'selected' : '';
			}

			if (el.getAttribute('data-useurlparm') == "True") {
				var target = {};

				if (el.getAttribute('data-ajax') == "True") {
					target = { e: "OnPageSelect", r: id, data: {} };
					target.data[id] = el.getAttribute('data-id');

					if (el.getAttribute('data-loaded') != "True") {
						el.setAttribute('data-loaded', 'True');
						au.runHrefWithApiResponse(el, target);
					} else {
						target.changeloc = true;
						au.prepareTarget(target);
					}
				}
			}
		}
	};

	return instance;
}(ajaxUtils);