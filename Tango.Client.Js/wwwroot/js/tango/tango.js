/// <reference path="/js/jquery-1.11.0.min.js"/>
/// ver. 26-08-2016
var commonUtils = function () {
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
}();

var domActions = function () {
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
}();

var ajaxUtils = function () {
	var timer = null;

	var instance = {
		initForm: function (args) {
			var form = $('#' + args.id);
			form.on('submit', { el: form[0] }, function (e) {
				return ajaxUtils.formSubmit(e.data.el);
			});
			if (!args.submitOnEnter) {
			    form.on("keypress", ":input:not(textarea):not([type=submit])", function (e) {
			        return e.keyCode != 13;
			    });
			}
		},
		formSubmit: function (form) {
			if (form.action) _baseUrl = form.action;
			var data = new FormData(form);
			ajaxUtils.postEventWithApiResponse('onsubmit', null, data);
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
		delay(caller, func) {
			if (timer) {
				window.clearTimeout(timer);
				timer = null;
			}
			timer = window.setTimeout(function () { func(caller); }, 400);
		},
		setHash: function (e, args) {
			if (e) _event = e;
			ajaxUtils.setHashParms(args);
		},
		setHashFromElement: function (el, e, r) {
			var data = {};
			processElementData(el, data);
			if (el instanceof HTMLInputElement || el instanceof HTMLSelectElement || el instanceof HTMLTextAreaElement)
				data[el.name] = el.value;
			if (e) _event = e;
			if (r) _eventReceiver = r;
			ajaxUtils.setHashParms(data);
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
					ajaxUtils.runEventFromElementWithApiResponse(this, e.data.serverEvent, e.data.receiver);
				else
					ajaxUtils.postEventFromElementWithApiResponse(this, e.data.serverEvent, e.data.receiver);
			});
		},
		runEvent: function (e, r, args) {
			return $.ajax({
				url: ajaxUtils.prepareUrl(e, r, args),
				type: 'GET'
			}).fail(ajaxUtils.error).then(onRequestResult);
		},
		runEventWithApiResponse: function (e, r, args) {
			return ajaxUtils.runEvent(e, r, args).then(loadScripts).then(processApiResponse);
		},
		runEventFromElementWithApiResponse: function (el, e, r) {
			var data = {};
			processElementData(el, data);
			if (el instanceof HTMLInputElement || el instanceof HTMLSelectElement || el instanceof HTMLTextAreaElement)
				data[el.name] = el.value;
			return ajaxUtils.runEventWithApiResponse(e, r, data);
		},
		postEvent: function (e, r, args) {
			var isForm = args instanceof FormData;
			return $.ajax({
				url: ajaxUtils.prepareUrl(e, r),
				type: 'POST',
				processData: !isForm,
				contentType: isForm ? false : "application/json; charset=utf-8",
				dataType: 'json',
				data: isForm ? args : JSON.stringify(args)
			}).fail(ajaxUtils.error).then(onRequestResult);
		},
		postEventWithApiResponse: function (e, r, args) {
			return ajaxUtils.postEvent(e, r, args).then(loadScripts).then(processApiResponse);
		},
		postEventFromElementWithApiResponse: function (el, e, r) {	
			var form = $(el).closest('form')[0];
			var data = {};
			if (form) data = $(form).serializeObject();
			processElementData(el, data);
			return ajaxUtils.postEventWithApiResponse(e, r, data);
		},
		prepareUrl: function(e, r, args) {
			_event = e;
			if (!_event) _event = 'onload';

			var url = _baseUrl ? _baseUrl : '/api' + window.location.pathname;
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
					r = _hash[key];
			}
			for (key in args) {
				url += '&' + key + '=' + args[key];
			}
			if (r) url += '&r=' + r;

			_event = null;
			return url;
		}
	};

	var _hash = commonUtils.getHashParams();
	var _event = null;
	var _eventReceiver = null;
	var _baseUrl = null;
	var _loadedJs = [];
	var _apiResponse = null;

	var _topMessage = $("#topmessagecontainer");
	var _requestInProcess = null;

	function beforeRequest(event, xhr, settings) {
		_requestInProcess = commonUtils.createGuid();
		xhr.setRequestHeader('x-request-guid', _requestInProcess)
		xhr.setRequestHeader('x-csrf-token', document.head.getAttribute('data-x-csrf-token'))
		setTimeout(function () {
			if (_requestInProcess) _topMessage.css('display', 'block');
		}, 100);
	}

	function onRequestResult(data, status, xhr) {
		if (xhr.getResponseHeader('X-Request-Guid') == _requestInProcess) {
			_apiResponse = data;
			return $.Deferred().resolve(data);
		}
	}

	function requestCompleted() {
		_requestInProcess = null;
		_topMessage.css('display', 'none')
	}

	function loadScript(url) {
		return $.ajax({
			type: "GET",
			url: url,
			dataType: "script",
			//cache: true,
			crossDomain: true
		});
	}

	function loadScripts() {
		if (_apiResponse && _apiResponse.includes && _apiResponse.includes.length > 0) {
			for (i = 0; i < _apiResponse.includes.length; i++) {
				var s = _apiResponse.includes[i];				
				if ($.inArray(s, _loadedJs) < 0) {
					loadScript(s);
					_loadedJs.push(s);
					console.log('loaded ' + s);
				}
			}
		}
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

	function processApiResponse() {
		if (!_apiResponse) return;

		if (_apiResponse.url) {
			window.location = _apiResponse.url;
			return;
		}

		if (_apiResponse.widgets) {
			for (var w in _apiResponse.widgets) {
				var el = w == 'body' ? document.body : document.getElementById(w);
				var obj = _apiResponse.widgets[w];
				if (obj != null && typeof (obj) == "object") {
					var el2 = document.body;
					if (obj.parent && obj.parent != 'body') el2 = document.getElementById(obj.parent);
					if (el)
						el.outerHTML = obj.content;
					else
						el2.insertAdjacentHTML('beforeend', obj.content);
				}
				else {
					if (el) el.innerHTML = obj;
				}
			}
		}

		if (_apiResponse.widgetsforremove) {
			for (var w in _apiResponse.widgetsforremove) {
				var el = document.getElementById(_apiResponse.widgetsforremove[w]);
				if (el) el.remove();
			}
		}

		if (_apiResponse.clientactions) {
			var ca;
			for (var i = 0; i < _apiResponse.clientactions.length; i++) {
				ca = _apiResponse.clientactions[i];
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
			setTimeout(function () { runClientAction(service, method, args, iter++); }, 50);
		}
	}

	$(document).ready(function () {
		document.body.className = '';

		$(window).on('hashchange', function () {
			if (!window.location.hash.startsWith('#/')) return;
			_hash = commonUtils.getHashParams();
			ajaxUtils.runEventWithApiResponse(_event, _eventReceiver);
		});

		$(document).ajaxSend(beforeRequest);
		$(document).ajaxStop(requestCompleted);

		var __load = document.getElementById('__load');
		if (__load) {
		    if (window.location.pathname == '/') {
		        var defAction = __load.getAttribute('data-default');
		        if (defAction) _baseUrl = '/api' + defAction;
		    }
			ajaxUtils.runEventWithApiResponse();
		}
	});

	return instance;
}();

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