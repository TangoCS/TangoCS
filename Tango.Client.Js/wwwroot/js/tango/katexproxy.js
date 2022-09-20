window.katexproxy = function () {
	var instance = {
		init: function (elid) {
			const el = document.getElementById(elid);
			if (el)
				katex.render(el.textContent, el, { throwOnError: false })
		},
	};

	return instance;
}();
