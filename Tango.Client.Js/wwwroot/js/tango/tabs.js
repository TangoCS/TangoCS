var tabs = function (au) {
	var instance = {
		onselect: function (el) {
			var index = [].indexOf.call(el.parentNode.parentNode.children, el.parentNode) + 1;
			var pages = el.parentNode.parentNode.parentNode.children;
			for (i = 1; i < pages.length; i++) {
				pages[i].className = i == index ? 'selected' : '';
			}

			if (el.getAttribute('data-useurlparm') == "True") {
				var args = {}, target = {};

				var id = el.parentNode.parentNode.parentNode.getAttribute('data-parmname');
				args[id] = el.getAttribute('data-id');

				if (el.getAttribute('data-ajax') == "True" && el.getAttribute('data-loaded') != "True") {
					target = { e: "OnPageSelect", r: id };
					el.setAttribute('data-loaded', 'True');
				}
				au.setHash(target, args);
			}
		}
	};

	return instance;
}(ajaxUtils);