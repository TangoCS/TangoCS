var tabs = function (au, lv) {
	var instance = {
		onselect: function (el) {

            const fixedHeaders = document.querySelectorAll('.listviewtable.fixedheader');
            if (fixedHeaders.length > 0) lv.fixedHeader(fixedHeaders);

            const isBack = !el.nodeType;
			if (isBack) el = document.getElementById(el);
			const tabs = el.parentNode.parentNode.parentNode;
			const index = [].indexOf.call(el.parentNode.parentNode.children, el.parentNode);
			const pages = document.getElementById(tabs.id + '_pages').children;
			for (i = 0; i < pages.length; i++) {
				pages[i].className = i == index ? 'selected' : '';
			}
			if (isBack)
				el.previousSibling.checked = true;

			if (el.getAttribute('data-useurlparm') == "True") {
				const ctrlid = tabs.getAttribute('data-parmname').toLowerCase();
				var target = {};
				target = { e: "OnPageSelect", r: tabs.id, query: {} };
				target.query[ctrlid] = el.getAttribute('data-id');
				target.url = au.findServiceAction(el);
				target.onBack = { service: "tabs", callChain: [{ method: "onselect", args: el.id }] };
				if (!isBack) target.changeloc = true;

				if (el.getAttribute('data-ajax') == "True" && el.getAttribute('data-loaded') != "True") {
					el.setAttribute('data-loaded', 'True');
					//au.runEventFromElementWithApiResponse(el, target);

					changeUrl(target, ctrlid);
					
					au.postEventFromElementWithApiResponse(el, target);
					return;
				}

				if (!isBack) {
					changeUrl(target, ctrlid);
				}
			}
			
			function changeUrl(target, ctrlid) {
				var args = {
					remove: [ctrlid],
					add: {}
				};
				args.add[ctrlid] = target.query[ctrlid];
				au.changeUrl(args);
			}
		}
	};

	return instance;
}(ajaxUtils, listview);