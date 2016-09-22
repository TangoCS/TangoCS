var tabs = function () {
	var instance = {
		onselect: function (el) {
			var index = [].indexOf.call(el.parentNode.parentNode.children, el.parentNode) + 1;
			var pages = el.parentNode.parentNode.parentNode.children;
			for (i = 1; i < pages.length; i++) {
				pages[i].className = i == index ? 'selected' : '';
			}
		}
	};

	return instance;
}();