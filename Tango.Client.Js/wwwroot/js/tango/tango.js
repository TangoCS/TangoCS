/// <reference path="/js/jquery-1.11.0.min.js"/>
/// ver. 29-08-2016
var commonUtils = function ($) {
	var instance = {
		getHashParams: function () {
			var hashParams = {};
			var e,
				a = /\+/g, // Regex for replacing addition symbol with a space
				r = /([^&;=]+)=?([^&;]*)/g,
				d = function (s) { return decodeURIComponent(s.replace(a, " ")); },
				q = window.location.hash.substring(2);

			while (e = r.exec(q))
				hashParams[d(e[1])] = d(e[2]);

			return hashParams;
		},
		defaultFor: function (arg, val) { return typeof arg !== 'undefined' ? arg : val; },
		setFocus: function (id) {
			var el = $('#' + id);
			el.focus();
			if (el.val()) {
				var strLength = el.val().length * 2;
				el[0].setSelectionRange(strLength, strLength);
			}
		},
		createGuid: function () {
			return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
				var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
				return v.toString(16);
			});
		},
		hideShow: function(id) {
			var x = document.getElementById(id);
			if (x.style.display === 'none') {
				x.style.display = 'block';
			} else {
				x.style.display = 'none';
			}
		}
	}

	return instance;
}($);

var domActions = function ($) {
	var instance = {
		setValue: function (args) {
			var e = document.getElementById(args.elName);
			if (e) {
				if (e instanceof HTMLInputElement || e instanceof HTMLSelectElement)
					e.value = args.value;
				else
					e.innerHTML = args.value;
			}
		},
		setAttribute: function (args) {
			var e = document.getElementById(args.elName);
			e.setAttribute(args.attrName, args.attrValue);
		},
		removeAttribute: function (args) {
			var e = document.getElementById(args.elName);
			e.removeAttribute(args.attrName);
		},
		setVisible: function (args) {
			var e = document.getElementById(args.elName);
			if (args.visible) $(e).show(); else $(e).hide();
		},
		setClass: function (args) {
			var e = document.getElementById(args.elName);
			$(e).addClass(args.clsName);
		},
		removeClass: function (args) {
			var e = document.getElementById(args.elName);
			$(e).removeClass(args.clsName);
		}
	}

	return instance;
}($);

var ajaxUtils = function ($, cu) {
	var timer = null;

	var instance = {
		initForm: function (args) {
			var form = $('#' + args.id);
			form.on('submit', { el: form[0] }, function (e) {
				return instance.formSubmit(e.data.el);
			});
			if (!args.submitOnEnter) {
				form.on("keypress", ":input:not(textarea):not([type=submit])", function (e) {
					return e.keyCode != 13;
				});
			}
		},
		formSubmit: function (form) {
			var fd = new FormData(form);
			var els = form.elements;
			for (var i = 0, el; el = els[i++];) {
				processElementDataOnFormSubmit(el, fd);
			}
			instance.postEventWithApiResponse({ e: 'onsubmit', url: form.action }, fd);
			return false;
		},
		error: function (xhr, status, e) {
			if (e && e.message)
				document.documentElement.innerHTML = 'Javascript error<br/>' + e.message + '<br>' + e.stack;
			else if (e && this.url && xhr.status != '500')
				document.documentElement.innerHTML = 'Ajax error<br/>' + this.url + '<br>' + xhr.status + ' ' + e;
			else
				document.documentElement.innerHTML = xhr.responseText;
		},
		delay: function (caller, func) {
			if (timer) {
				window.clearTimeout(timer);
				timer = null;
			}
			timer = window.setTimeout(function () { func(caller); }, 400);
		},
		setHash: function (target, args) {
			if (target.e)
				state.loc.event = target.e;
			else
				state.loc.event = '#';
			if (target.r) state.loc.receiver = target.r;
			instance.setHashParms(args);
		},
		setHashFromElement: function (el) {
			var data = {};
			processElementDataOnAction(el, data);
			if (el instanceof HTMLInputElement || el instanceof HTMLSelectElement || el instanceof HTMLTextAreaElement)
				data[el.name] = el.value;
			instance.setHashParms(data);
		},
		setHashParms: function (args) {
			if (args) {
				for (key in args) {
					if (key == 'e')
						state.loc.event = args[key];
					else
						state.loc.hash[key] = args[key];
				}
			}
			var hashUrl = '/';
			for (key in state.loc.hash) {
				if (state.loc.hash[key] && state.loc.hash[key] != '' && state.loc.hash[key] != 'null')
					hashUrl += key + '=' + state.loc.hash[key] + '&';
			}
			window.location.hash = hashUrl == '/' ? hashUrl : hashUrl.substring(0, hashUrl.length - 1);
		},
		bindevent: function (args) {
			$('#' + args.id).on(args.clientEvent, { serverEvent: args.serverEvent, receiver: args.serverEventReceiver, method: args.method }, function (e) {
				if (e.data.method && e.data.method == 'get')
					instance.runEventFromElementWithApiResponse(this, { e: e.data.serverEvent, r: e.data.receiver });
				else
					instance.postEventFromElementWithApiResponse(this, { e: e.data.serverEvent, r: e.data.receiver });
			});
		},
		runEvent: function (target, args) {
			return $.ajax({
				url: instance.prepareUrl(target, args),
				type: 'GET'
			}).fail(instance.error).then(onRequestResult);
		},
		runEventWithApiResponse: function (target, args) {
			return instance.runEvent(target, args).then(instance.loadScripts).then(processApiResponse);
		},
		runEventFromElementWithApiResponse: function (el, target) {
			var data = {};
			processElementDataOnEvent(el, data);
			if (el instanceof HTMLInputElement || el instanceof HTMLSelectElement || el instanceof HTMLTextAreaElement)
				data[el.name] = el.value;
			return instance.runEventWithApiResponse(target, data);
		},
		postEvent: function (target, args) {
			var isForm = args instanceof FormData;
			return $.ajax({
				url: instance.prepareUrl(target),
				type: 'POST',
				processData: !isForm,
				contentType: isForm ? false : "application/json; charset=utf-8",
				dataType: 'json',
				data: isForm ? args : JSON.stringify(args)
			}).fail(instance.error).then(onRequestResult);
		},
		postEventWithApiResponse: function (target, args) {
			return instance.postEvent(target, args).then(instance.loadScripts).then(processApiResponse);
		},
		postEventFromElementWithApiResponse: function (el, target) {
			var form = $(el).closest('form')[0];
			var data = {};
			if (form) data = $(form).serializeObject();
			processElementDataOnEvent(el, data);
			return instance.postEventWithApiResponse(target, data);
		},
		loadScripts: function (apiResult) {
			if (!apiResult) return $.Deferred().resolve(apiResult);
			var deferreds = [];
			var toLoad = apiResult instanceof Array ? apiResult : apiResult.includes;

			var r = $.Deferred();
			if (toLoad && toLoad.length > 0) {
				loadScript(r, toLoad, 0);
				return r.then(function () {
					console.log('loadScripts done');
					return $.Deferred().resolve(apiResult);
				});
			}
			else
				return r.resolve(apiResult);
		},
		prepareUrl: function (target, args) {
			state.loc.event = target.e;
			if (!state.loc.event) state.loc.event = 'onload';
			state.loc.hash = cu.getHashParams();

			var url = target.url ?
				target.url :
				('/api' + (window.location.pathname == '/' ? state.loc.defAction : window.location.pathname));
			if (window.location.search == '')
				url += '?';
			else
				url += window.location.search;

			if (!url.endsWith('?')) url += '&';
			url += 'e=' + state.loc.event;
			var p = document.head.getAttribute('data-p');
			if (p) url += '&p=' + p;

			for (key in state.loc.hash) {
				if (key != 'e' && key != 'r' && key != 'p' && state.loc.hash[key])
					url += '&' + key + '=' + encodeURIComponent(state.loc.hash[key]);
				else if (key == 'r')
					target.r = state.loc.hash[key];
			}
			for (key in args) {
				url += '&' + key + '=' + encodeURIComponent(args[key]);
			}
			if (target.r) url += '&r=' + target.r;

			state.loc.event = null;
			return url;
		},
		stopRequest: function () {
			requestCompleted();
		}
	};

	var state = {
		com: {
			requestId: null,
			message: $("#topmessagecontainer"),
			requestedJs: []
		},
		loc: {
			defAction: null,
			hash: cu.getHashParams(),
			event: null,
			receiver: null
		}
	};

	function loadScript(def, toLoad, cur) {
		if ($.inArray(toLoad[cur], state.com.requestedJs) >= 0) {
			if (toLoad.length - 1 > cur)
				return loadScript(def, toLoad, cur + 1);
			else
				return def.resolve();
		}
		state.com.requestedJs.push(toLoad[cur]);
		return $.ajax({
			type: "GET",
			url: toLoad[cur],
			dataType: "script",
			beforeSend: function () { console.log('requested ' + this.url); },
			success: function () {
				console.log('loaded ' + this.url);
				if (toLoad.length - 1 > cur)
					loadScript(def, toLoad, cur + 1);
				else
					def.resolve();
			},
			crossDomain: true
		});
	}

	function beforeRequest(event, xhr, settings) {
		state.com.requestId = cu.createGuid();
		xhr.setRequestHeader('x-request-guid', state.com.requestId)
		xhr.setRequestHeader('x-csrf-token', document.head.getAttribute('data-x-csrf-token'))
		setTimeout(function () {
			if (state.com.requestId) state.com.message.css('display', 'block');
		}, 100);
	}

	function requestCompleted() {
		state.com.requestId = null;
		state.com.message.css('display', 'none')
	}

	function onRequestResult(data, status, xhr) {
		return xhr.getResponseHeader('X-Request-Guid') == state.com.requestId ?
			$.Deferred().resolve(data) : $.Deferred().reject();
	}

	function processElementDataOnEvent(el, data) {
		for (var attr, i = 0, attrs = el.attributes, n = attrs ? attrs.length : 0; i < n; i++) {
			attr = attrs[i];
			if (attr.name.startsWith('data-p-')) {
				data[attr.name.replace('data-p-', '')] = attr.value;
			} else if (attr.name.startsWith('data-format')) {
				data['__format_' + el.name] = attr.value;
			} else if (attr.name.startsWith('data-ref-')) {
				var refEl = document.getElementById(attr.name.replace('data-ref-', ''));
				if (refEl) data[refEl.name] = refEl.value;
			}
		}
	}

	function processElementDataOnAction(el, data) {
		state.loc.event = null;
		state.loc.receiver = null;
		for (var attr, i = 0, attrs = el.attributes, n = attrs ? attrs.length : 0; i < n; i++) {
			attr = attrs[i];
			if (attr.name.startsWith('data-p-')) {
				data[attr.name.replace('data-p-', '')] = attr.value;
			} else if (attr.name == 'data-e') {
				state.loc.event = attr.value;
			} else if (attr.name == 'data-r') {
				state.loc.receiver = attr.value;
			} else if (attr.name.startsWith('data-ref-')) {
				var refEl = document.getElementById(attr.name.replace('data-ref-', ''));
				if (refEl) data[refEl.name] = refEl.value;
			}
		}
	}

	function processElementDataOnFormSubmit(el, data) {
		for (var attr, i = 0, attrs = el.attributes, n = attrs ? attrs.length : 0; i < n; i++) {
			attr = attrs[i];
			if (attr.name.startsWith('data-format')) {
				data.append('__format_' + el.name, attr.value);
			}
		}
	}

	function processApiResponse(apiResult) {
		console.log('processApiResponse');
		if (!apiResult) return;

		if (apiResult.error) {
			document.documentElement.innerHTML = 'Server error<br/>' + apiResult.error;
			return;
		}

		if (apiResult.url) {
			window.location = apiResult.url;
			return;
		}

		if (apiResult.widgets) {
			for (var w in apiResult.widgets) {
				var el = w == 'body' ? document.body : document.getElementById(w);
				var obj = apiResult.widgets[w];
				if (obj != null && typeof (obj) == "object") {
					var el2 = document.body;
					if (obj.parent && obj.parent != 'body') el2 = document.getElementById(obj.parent);
					if (el)
						el.outerHTML = obj.content;
					else
						el2.insertAdjacentHTML(obj.position, obj.content);
				}
				else {
					if (el) el.innerHTML = obj;
				}
			}
		}

		if (apiResult.widgetsforremove) {
			for (var w in apiResult.widgetsforremove) {
				var el = document.getElementById(apiResult.widgetsforremove[w]);
				if (el) el.remove();
			}
		}

		if (apiResult.clientactions) {
			var ca;
			for (var i = 0; i < apiResult.clientactions.length; i++) {
				ca = apiResult.clientactions[i];
				runClientAction(ca.service, ca.callChain, 0);
			}
		}
		if (window.homePage) homePage.countNavBodyHeight();
	}

	function runClientAction(service, callChain, iter) {
		if (iter > 10) return;
		var caller = window[service];
		if (caller) {
			for (var j = 0; j < callChain.length; j++) {
				step = callChain[j];
				if (step.method == 'apply')
					caller = caller[step.method](caller, step.args);
				else
					caller = caller[step.method](step.args);
			}
		}
		else {
			console.log("wait for " + service);
			setTimeout(function () { runClientAction(service, callChain, iter++); }, 50);
		}
	}

	$(document).ready(function () {
		document.body.className = '';

		$(window).on('hashchange', function () {
			if (!window.location.hash.startsWith('#/')) return;
			if (!state.loc.event) state.loc.event = 'onload';
			if (state.loc.event != '#') instance.runEventWithApiResponse({ e: state.loc.event, r: state.loc.receiver });
		});

		$(document).ajaxSend(beforeRequest);
		$(document).ajaxStop(requestCompleted);

		var __load = document.getElementById('__load');
		if (__load) {
			if (window.location.pathname == '/') {
				state.loc.defAction = __load.getAttribute('data-default');
				//if (state.loc.defAction) target.url = '/api' + state.loc.defAction;
			}
			instance.runEventWithApiResponse({});
		}
	});

	return instance;
}($, commonUtils);

if (!String.prototype.endsWith) {
	String.prototype.endsWith = function (searchString, position) {
		var subjectString = this.toString();
		if (typeof position !== 'number' || !isFinite(position) || Math.floor(position) !== position || position > subjectString.length) {
			position = subjectString.length;
		}
		position -= searchString.length;
		var lastIndex = subjectString.indexOf(searchString, position);
		return lastIndex !== -1 && lastIndex === position;
	};
}

if (!String.prototype.startsWith) {
	String.prototype.startsWith = function (str) {
		return this.lastIndexOf(str, 0) === 0;
	}
}