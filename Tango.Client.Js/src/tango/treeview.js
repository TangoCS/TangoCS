var treeView = function () {
	var instance = {
		defaultInit: function (args) {
			_tvId = args.id;
			$('#' + args.id).jstree({
				'core': {
					'data': {
						'url': ajaxUtils.prepareUrl({ e: 'ongetnode', r: args.id }),
						'data': function (node) {
							return { 'nodeid': node.id };
						}
					},
					'check_callback': true,
					'themes': { 'responsive': false }
				},
				'force_text': true,
				"plugins": ["contextmenu"],
				'contextmenu': {
					'items': getMenu
				}
			}).on("changed.jstree", function (e, data) {
				//data.instance.save_state();
				//location.href = data.instance.get_node(data.node, true).children('a').attr('href');
			});
		}
	}

	var _tvId = null;
	var _menuCache = [];

	function getMenu(node) {
		if (_menuCache[node.id]) return _menuCache[node.id];	
		ajaxUtils.postEvent({ e: 'ongetmenu', r: _tvId }, {
			id: node.id,
			data: node.data,
			children: !node.state.loaded || node.children.length > 0
		}).done(function (data) {
			if (!data) return;
			var items = {};

			for (i = 0; i < data.length; i++) {
				items[data[i].name] = {
					label: data[i].label,
					action: function (obj) {
						location.href = obj.item.url
					},
					url: data[i].url
				}
			}
			_menuCache[node.id] = items;

			$('#' + _tvId).jstree("show_contextmenu", node);
		});
	}

	return instance;
}();