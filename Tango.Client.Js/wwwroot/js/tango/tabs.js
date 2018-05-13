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

				if (el.getAttribute('data-ajax') == "True" && el.getAttribute('data-loaded') != "True") {
					target = { e: "OnPageSelect", r: id, data: {} };
					target.data[id] = el.getAttribute('data-id');
					el.setAttribute('data-loaded', 'True');
				}
				au.setHashFromElement(el, target);
			}
		}
	};

	return instance;
}(ajaxUtils);