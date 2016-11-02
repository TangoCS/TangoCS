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
			instance.postEventWithApiResponse({ e: 'onsubmit', url: form.action }, new FormData(form));
			return false;
		},
		error: function (xhr, status, e) {
			if (e && e.message)
				document.documentElement.innerHTML = 'Javascript error<br>' + e.message + '<br>' + e.stack;
			else if (e && this.url && xhr.status != '500')
				document.documentElement.innerHTML = 'Ajax error<br>' + this.url + '<br>' + xhr.status + ' ' + e;
			else
				document.documentElement.innerHTML = xhr.responseText;
		},
		delay: function(caller, func) {
			if (timer) {
				window.clearTimeout(timer);
				timer = null;
			}
			timer = window.setTimeout(function () { func(caller); }, 400);
		},
		setHash: function (e, args) {
			if (e) _event = e;
			instance.setHashParms(args);
		},
		setHashFromElement: function (el, e, r) {
			var data = {};
			processElementData(el, data);
			if (el instanceof HTMLInputElement || el instanceof HTMLSelectElement || el instanceof HTMLTextAreaElement)
				data[el.name] = el.value;
			if (e) _event = e;
			if (r) _eventReceiver = r;
			instance.setHashParms(data);
		},
		setHashParms: function (args) {
			if (args) {
				for (key in args) {
					_hash[key] = args[key];
				}
			}
			var hashUrl = '/';
			for (key in _hash) {
				if (_hash[key] && _hash[key] != '' && _hash[key] != 'null')
					hashUrl += key + '=' + _hash[key] + '&';
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
			processElementData(el, data);
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
			processElementData(el, data);
			return instance.postEventWithApiResponse(target, data);
		},
		loadScripts: function (apiResult) {
			if (!apiResult) return $.Deferred().resolve(apiResult);
			var deferreds = [];
			var toLoad = apiResult instanceof Array ? apiResult : apiResult.includes;

			if (toLoad && toLoad.length > 0) {
				for (i = 0; i < toLoad.length; i++) {
					var s = toLoad[i];
					if ($.inArray(s, _requestedJs) < 0) {
						deferreds.push(
							$.ajax({
								type: "GET",
								url: s,
								dataType: "script",
								//cache: true,
								crossDomain: true
							})
						);
						_requestedJs.push(s);
						console.log('requested ' + s);
					}
				}
			}

			if (deferreds.length == 0)
				return $.Deferred().resolve(apiResult);
			else {
				return $.when.apply($, deferreds).then(function () { return $.Deferred().resolve(apiResult); });
			}
		},
		prepareUrl: function (target, args) {
			_event = target.e;
			if (!_event) _event = 'onload';

			var url = target.url ? target.url : '/api' + window.location.pathname;
			if (window.location.search == '')
				url += '?';
			else
				url += window.location.search;

			if (!url.endsWith('?')) url += '&';
			url += 'e=' + _event;
			var p = document.head.getAttribute('data-p');
			if (p) url += '&p=' + p;

			for (key in _hash) {
				if (key != 'e' && key != 'r' && key != 'p' && _hash[key])
					url += '&' + key + '=' + _hash[key];
				else if (key == 'r')
					target.r = _hash[key];
			}
			for (key in args) {
				url += '&' + key + '=' + args[key];
			}
			if (target.r) url += '&r=' + target.r;

			_event = null;
			return url;
		}
	};

	var _requestInProcess = null;
	var _requestedJs = [];

	var _hash = cu.getHashParams();
	var _event = null;
	var _eventReceiver = null;

	var _topMessage = $("#topmessagecontainer");

	function beforeRequest(event, xhr, settings) {
		_requestInProcess = cu.createGuid();
		xhr.setRequestHeader('x-request-guid', _requestInProcess)
		xhr.setRequestHeader('x-csrf-token', document.head.getAttribute('data-x-csrf-token'))
		setTimeout(function () {
			if (_requestInProcess) _topMessage.css('display', 'block');
		}, 100);
	}

	function requestCompleted() {
		_requestInProcess = null;
		_topMessage.css('display', 'none')
	}

	function onRequestResult(data, status, xhr) {
		return xhr.getResponseHeader('X-Request-Guid') == _requestInProcess ?
			$.Deferred().resolve(data) : $.when();
	}

	function processElementData(el, data) {
		for (var attr, i = 0, attrs = el.attributes, n = attrs ? attrs.length : 0; i < n; i++) {
			attr = attrs[i];
			if (attr.name.startsWith('data-p-')) {
				data[attr.name.replace('data-p-', '')] = attr.value;
			} else if (attr.name.startsWith('data-ref-')) {
				var refEl = document.getElementById(attr.name.replace('data-ref-', ''));
				if (refEl) data[refEl.name] = refEl.value;
			}
		}
	}

	function processApiResponse(apiResult) {
		if (!apiResult) return;

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
			_hash = cu.getHashParams();
			instance.runEventWithApiResponse({ e: _event, r: _eventReceiver });
		});

		$(document).ajaxSend(beforeRequest);
		$(document).ajaxStop(requestCompleted);

		var __load = document.getElementById('__load');
		if (__load) {
			var target = {};
		    if (window.location.pathname == '/') {
		        var defAction = __load.getAttribute('data-default');
		        if (defAction) target.url = '/api' + defAction;
		    }
		    instance.runEventWithApiResponse(target);
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