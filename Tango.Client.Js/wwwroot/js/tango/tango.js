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
		getScrollParent: function (element, includeHidden) {
			var style = getComputedStyle(element);
			var excludeStaticParent = style.position === "absolute";
			var overflowRegex = includeHidden ? /(auto|scroll|hidden)/ : /(auto|scroll)/;

			if (style.position === "fixed") return document.body;
			for (var parent = element; (parent = parent.parentElement) ;) {
				style = getComputedStyle(parent);
				if (excludeStaticParent && style.position === "static") {
					continue;
				}
				if (overflowRegex.test(style.overflow + style.overflowY + style.overflowX)) return parent;
			}

			return document.body;
		},
		scrollToView: function (element) {
			var r = element.getBoundingClientRect();
			if (r.bottom > window.innerHeight) {
				var scrl = instance.getScrollParent(element);
				if (scrl)
					scrl.scrollTop += r.bottom - window.innerHeight + 16;
				else
					window.scrollBy(0, r.bottom - window.innerHeight + 16);
			}
		}
	}

	return instance;
}($);

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

var domActions = function () {
	var instance = {
		setValue: function (args) {
			var e = document.getElementById(args.id);
			if (e) {
				if (e instanceof HTMLInputElement || e instanceof HTMLSelectElement)
					e.value = args.value;
				else
					e.innerHTML = args.value;
			}
		},
		setAttribute: function (args) {
			var e = document.getElementById(args.id);
			e.setAttribute(args.attrName, args.attrValue);
		},
		removeAttribute: function (args) {
			var e = document.getElementById(args.id);
			e.removeAttribute(args.attrName);
		},
		setVisible: function (args) {
			var e = document.getElementById(args.id);
			if (args.visible) e.classList.remove('hide'); else e.classList.add('hide');
		},
		setClass: function (args) {
			var e = document.getElementById(args.id);
			e.classList.add(args.clsName);
		},
		removeClass: function (args) {
			var e = document.getElementById(args.id);
			e.classList.remove(args.clsName);
		},
		toggleClass: function (args) {
			event.stopPropagation();
			if (args.id) {
				var el = document.getElementById(args.id);
				el.classList.toggle(args.clsName);
			}
			else {
				var root = args.root ? args.root : document;
				var grels = root.querySelectorAll(args.groupSelector);
				var els = root.querySelectorAll(args.itemsSelector);

				var b = args.sender.classList.contains(args.senderClsName);

				for (var i = 0; i < grels.length; i++) {
					grels[i].classList.remove(args.clsName);
					grels[i].classList.remove(args.senderClsName);
				}

				if (!b) {
					for (var i = 0; i < els.length; i++) {
						els[i].classList.add(args.clsName);
					}
					args.sender.classList.add(args.senderClsName);
				}
			}
		},
		hideShow: function (id) {
			instance.toggleClass({ id: id, clsName: 'hide' });
		}
	}

	return instance;
}();

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
				processElementDataOnFormSubmit(el, function (key, value) { fd.append(key, value); });
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
			var data = {}, target = {};
			processElementDataOnEvent(el, data, target);
			if (target.e)
				state.loc.event = target.e;
			else
				state.loc.event = state.loc.DEF_EVENT_NAME;
			if (target.r) state.loc.receiver = target.r;
			if (el instanceof HTMLInputElement || el instanceof HTMLSelectElement || el instanceof HTMLTextAreaElement)
				data[el.name] = el.value;
			instance.setHashParms(data);
		},
		setHashParms: function (args) {
			var hashParms = cu.getHashParams();
			if (args) {
				for (key in args) {
					hashParms[key] = args[key];
				}
			}
			var hashUrl = '/';
			for (key in hashParms) {
				if (hashParms[key] && hashParms[key] != '' && hashParms[key] != 'null')
					hashUrl += key + '=' + hashParms[key] + '&';
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
			if (!target) target = {};
			processElementDataOnEvent(el, data, target);
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
				data: isForm ? args : JSON.stringify(args)
			}).fail(instance.error).then(onRequestResult);
		},
		postEventWithApiResponse: function (target, args) {
			return instance.postEvent(target, args).then(instance.loadScripts).then(processApiResponse);
		},
		postEventFromElementWithApiResponse: function (el, target) {
			var form = $(el).closest('form')[0];
			var data = {};
			if (form) {
				data = $(form).serializeObject();
				var els = form.elements;
				for (var i = 0, fel; fel = els[i++];) {
					processElementDataOnFormSubmit(fel, function (key, value) { data[key] = value; });
				}
			}
			if (!target) target = {};
			processElementDataOnEvent(el, data, target);
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
			var hashParms = cu.getHashParams();

			var url = target.url ?
				target.url :
				('/api' + (window.location.pathname == '/' ? state.loc.defAction : window.location.pathname));
			if (window.location.search == '')
				url += '?';
			else
				url += window.location.search;

			if (!url.endsWith('?')) url += '&';
			if (target.e) url += 'e=' + target.e;
			var p = document.head.getAttribute('data-p');
			if (p) url += '&p=' + p;

			for (var key in hashParms) {
				if (key != 'e' && key != 'r' && key != 'p' && hashParms[key] && !(args && key in args))
					url += '&' + key + '=' + encodeURIComponent(hashParms[key]);
				else if (key == 'r')
					target.r = hashParms[key];
			}
			for (var key in args) {
				url += '&' + key + '=' + encodeURIComponent(args[key]);
			}
			if (target.r) url += '&r=' + target.r;

			return url;
		},
		stopRequest: function () {
			requestCompleted();
		}
	};

	var state = {
		com: {
			requestId: null,
			message: null,
			apiResult: null,
			requestedJs: []
		},
		loc: {
			DEF_EVENT_NAME: 'onload',
			defAction: null,
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
		if (state.com.message) state.com.message.css('display', 'none')
	}

	function onRequestResult(data, status, xhr) {
		if (xhr.getResponseHeader('X-Request-Guid') == state.com.requestId) {
			var disposition = xhr.getResponseHeader('Content-Disposition');
			// check file download response
			if (disposition && disposition.indexOf('attachment') !== -1) {
				var filename = "";
				var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
				var matches = filenameRegex.exec(disposition);
				if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
				filename = decodeURIComponent(filename);
				var type = xhr.getResponseHeader('Content-Type');

				var blob = typeof File === 'function'
					? new File([data], filename, { type: type })
					: new Blob([data], { type: type });

				if (typeof window.navigator.msSaveBlob !== 'undefined') {
					// IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
					window.navigator.msSaveBlob(blob, filename);
				} else {
					var URL = window.URL || window.webkitURL;
					var downloadUrl = URL.createObjectURL(blob);

					if (filename) {
						// use HTML5 a[download] attribute to specify filename
						var a = document.createElement("a");
						// safari doesn't support this yet
						if (typeof a.download === 'undefined') {
							window.location = downloadUrl;
						} else {
							a.href = downloadUrl;
							a.download = filename;
							document.body.appendChild(a);
							a.click();
						}
					} else {
						window.location = downloadUrl;
					}

					setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100); // cleanup
				}
				return $.Deferred().reject();
			}
			else
				return $.Deferred().resolve(data);
		}
		else
			return $.Deferred().reject();
	}

	function processElementDataOnEvent(el, data, target) {
		for (var attr, i = 0, attrs = el.attributes, n = attrs ? attrs.length : 0; i < n; i++) {
			attr = attrs[i];
			if (attr.name.startsWith('data-p-')) {
				data[attr.name.replace('data-p-', '')] = attr.value;
			} else if (attr.name == 'data-e') {
				target.e = attr.value;
			} else if (attr.name == 'data-r') {
				target.r = attr.value;
			} else if (attr.name.startsWith('data-format')) {
				data['__format_' + el.name] = attr.value;
			} else if (attr.name.startsWith('data-c-')) {
				data[attr.name.replace('data-c-', 'c-')] = attr.value;
			} else if (attr.name.startsWith('data-ref-')) {
				var refEl = document.getElementById(attr.name.replace('data-ref-', ''));
				if (refEl) data[refEl.name] = refEl.value;
			} else if (attr.name == 'href') {
				target.url = '/api' + attr.value;
			}
		}
	}

	function processElementDataOnFormSubmit(el, setvalfunc) {
		for (var attr, i = 0, attrs = el.attributes, n = attrs ? attrs.length : 0; i < n; i++) {
			attr = attrs[i];
			if (attr.name.startsWith('data-format')) {
				setvalfunc('__format_' + el.name, attr.value);
			}
		}
	}

	function processApiResponse(apiResult) {
		console.log('processApiResponse');
		if (!apiResult) return;

		if (apiResult.url) {
			window.location = apiResult.url;
			return;
		}

		state.com.apiResult = apiResult;

		if (document.readyState == 'complete' || document.readyState == 'interactive')
			renderApiResult();

	}

	function renderApiResult() {
		var apiResult = state.com.apiResult;
		state.com.apiResult = null;

		if (apiResult.error) {
			document.documentElement.innerHTML = 'Server error<br/>' + apiResult.error;
			return;
		}

		if (apiResult.widgets) {
			for (var w in apiResult.widgets) {
				var obj = apiResult.widgets[w];
				if (obj && typeof (obj) == "object") {
					var el = obj.name == 'body' ? document.body : document.getElementById(obj.name);
					if (el && (obj.action == 'replace' || obj.action == 'adjacent'))
						el.outerHTML = obj.content;
					else if (el && obj.action == 'add')
						el.innerHTML = obj.content;
					else if (el && obj.action == 'remove')
						el.remove();
					else if (obj.action == 'adjacent') {
						var el2 = (obj.parent && obj.parent != 'body' && obj.parent != '') ? document.getElementById(obj.parent) : document.body;
						el2.insertAdjacentHTML(obj.position, obj.content);
						if (obj.position.toLowerCase() == 'afterend') {
							cu.scrollToView(el2.nextSibling);
						}
					}
				} 
				else if (el) {
					el.innerHTML = obj;
				}
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

	document.addEventListener('DOMContentLoaded', function () {
		state.com.message = $("#topmessagecontainer");
		setTimeout(function () {
			if (state.com.requestId) state.com.message.css('display', 'block');
		}, 100);
		document.body.className = '';

		$(window).on('hashchange', function () {
			if (!window.location.hash.startsWith('#/')) return;
			if (!state.loc.event) state.loc.event = state.loc.DEF_EVENT_NAME;
			if (state.loc.event != '#') instance.runEventWithApiResponse({ e: state.loc.event, r: state.loc.receiver });
			state.loc.event = null;
			state.loc.receiver = null;
		});

		if (state.com.apiResult)
			renderApiResult();
	});

	$(document).ajaxSend(beforeRequest);
	$(document).ajaxStop(requestCompleted);

	var __load = document.head.attributes['data-load'];
	if (__load) {
		if (window.location.pathname == '/') {
			state.loc.defAction = __load.value;
		}
		instance.runEventWithApiResponse({ e: 'onload' }, { firstload: true });
	}

	return instance;
}($, commonUtils);
