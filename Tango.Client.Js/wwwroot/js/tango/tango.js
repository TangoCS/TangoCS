var commonUtils = function ($) {
	var instance = {
		getParams: function (query, raw) {
			var p = {};
			var e,
				a = /\+/g, // Regex for replacing addition symbol with a space
				r = /([^&;=]+)=?([^&;]*)/g,
				d = function (s) {
					const parm = s.replace(a, " ");
					return raw ? parm : decodeURIComponent(parm);
				},
				q = query;

			while (e = r.exec(q))
				p[d(e[1])] = d(e[2]);

			return p;
		},
		defaultFor: function (arg, val) { return typeof arg !== 'undefined' ? arg : val; },
		setFocus: function (el) {
			el.focus();
			if (el.value) {
				var strLength = el.value.length * 2;
				el.setSelectionRange(strLength, strLength);
			}
		},
		createGuid: function () {
			return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, function (c) {
				return (c ^ (window.crypto || window.msCrypto).getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
			});
		},
		getScrollParent: function (element, includeHidden) {
			var style = getComputedStyle(element);
			var excludeStaticParent = style.position === "absolute";
			var overflowRegex = includeHidden ? /(auto|scroll|hidden)/ : /(auto|scroll)/;

			if (style.position === "fixed") return document.body;
			for (var parent = element; (parent = parent.parentElement);) {
				style = getComputedStyle(parent);
				if (excludeStaticParent && style.position === "static") {
					continue;
				}
				if (overflowRegex.test(style.overflow + style.overflowY + style.overflowX)) return parent;
			}

			return document.body;
		},
		scrollToView: function (element) {
			if (!element || !element.getBoundingClientRect) return;
			var r = element.getBoundingClientRect();
			if (r.bottom > window.innerHeight) {
				var scrl = instance.getScrollParent(element);
				if (scrl)
					scrl.scrollTop += r.bottom - window.innerHeight + 16;
				else
					window.scrollBy(0, r.bottom - window.innerHeight + 16);
			}
		},
		checkids: function () {
			var elms = document.getElementsByTagName("*"), i, len, ids = {}, id;
			for (i = 0, len = elms.length; i < len; i += 1) {
				id = elms[i].id || null;
				if (id) {
					ids[id] = ids.hasOwnProperty(id) ? ids[id] += 1 : 0;
				}
			}
			for (id in ids) {
				if (ids.hasOwnProperty(id)) {
					if (ids[id]) {
						console.warn("Multiple IDs #" + id);
					}
				}
			}
		},
		getThisOrParent: function (caller, predicate) {
			var el = caller;
			while (el) {
				if (predicate(el)) return el;
				el = el.parentNode;
				if (el instanceof HTMLBodyElement) return;
				if (el instanceof HTMLDocument) return;
			}
		},
		getParent: function (caller, predicate) {
			if (!caller.parentNode) return;
			return instance.getThisOrParent(caller.parentNode, predicate);
		},
		getRow: function (caller) {
			return instance.getThisOrParent(caller, function (el) { return el instanceof HTMLTableRowElement; });
		},
		getCell: function (caller) {
			return instance.getThisOrParent(caller, function (el) { return el instanceof HTMLTableCellElement; });
		},
		processFile: function (contenttype, disposition, data) {
			var filename = "";
			var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
			var matches = filenameRegex.exec(disposition);
			if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
			filename = decodeURIComponent(filename);

			var blob = typeof File === 'function'
				? new File([data], filename, { type: contenttype })
				: new Blob([data], { type: contenttype });

			if (typeof window.navigator.msSaveOrOpenBlob !== 'undefined') {
				// IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
				window.navigator.msSaveOrOpenBlob(blob, filename);
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
		},
		copyToClipboard: function (sourceId) {
			var source = document.querySelector('#' + sourceId);
			if(source) {
				var dummy = document.createElement("textarea");
				document.body.appendChild(dummy);
				dummy.value = source.value;
				dummy.select();
				document.execCommand("copy");
				document.body.removeChild(dummy);
			}
		},
		copyTextToClipboard: function (str) {
			var dummy = document.createElement("textarea");
			document.body.appendChild(dummy);
			dummy.value = str;
			dummy.select();
			document.execCommand("copy");
			document.body.removeChild(dummy);			
		},
        clipboardToElementId: function(id) {
            if (window.clipboardData && window.clipboardData.getData) { //IE
                document.querySelector(id).innerText = window.clipboardData.getData("Text");
            }
            else {
                /*
                document.addEventListener('paste', function (evt) {
                    var qwe = evt.originalEvent.clipboardData.getData('Text');
                }, false);

                var event = new Event("paste", { bubbles: true, cancelable: true });
                document.documentElement.dispatchEvent(event);
                */
                /*
                 navigator.clipboard.readText()
                .then(
                    clipText => document.querySelector(id).innerText = clipText )
                .catch(err => {
                    console.log('Failed to read clipboard contents: ', err);
                });
                 */
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

NodeList.prototype.forEach = Array.prototype.forEach;

if (!Element.prototype.matches) {
	Element.prototype.matches = Element.prototype.msMatchesSelector || Element.prototype.webkitMatchesSelector;
}

if (!Element.prototype.closest) {
	Element.prototype.closest = function (s) {
		var el = this;
		do {
			if (Element.prototype.matches.call(el, s)) return el;
			el = el.parentElement || el.parentNode;
		} while (el !== null && el.nodeType === 1);
		return null;
	};
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
			if (e instanceof HTMLSelectElement && args.attrName == 'readonly') {
				for (i = 0; i < e.options.length; i++) {
					if (e.value != e.options[i].value)
						e.options[i].setAttribute('disabled', 'disabled');
				}
				e.classList.add("readonly");
			}
			else
				e.setAttribute(args.attrName, args.attrValue);
		},
		removeAttribute: function (args) {
			var e = document.getElementById(args.id);
			if (e instanceof HTMLSelectElement && args.attrName == 'readonly') {
				for (i = 0; i < e.options.length; i++) {
					e.options[i].removeAttribute('disabled');
				}
				e.classList.remove("readonly");
			}
			else
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
				var els = root.querySelectorAll(args.itemsSelector);
				var b = args.sender.classList.contains(args.senderClsName);

				if (args.groupSelector) {
					var grels = root.querySelectorAll(args.groupSelector);
					for (var i = 0; i < grels.length; i++) {
						grels[i].classList.remove(args.clsName);
						grels[i].classList.remove(args.senderClsName);
					}
				}

				if (!b) {
					for (var i = 0; i < els.length; i++) {
						els[i].classList.add(args.clsName);
					}
					args.sender.classList.add(args.senderClsName);
				}
				else if (!args.groupSelector) {
					for (var i = 0; i < els.length; i++) {
						els[i].classList.remove(args.clsName);
					}
					args.sender.classList.remove(args.senderClsName);
				}
			}
		},
		hideShow: function (id) {
			instance.toggleClass({ id: id, clsName: 'hide' });
		},
		setCookie: function (args) {
			$.cookie(args.id, args.value, { path: '/' });
		},
		setClientArg: function (args) {
			ajaxUtils.state.loc.clientArgs[args.id] = args.value;
		}
	}

	return instance;
}();

var ajaxUtils = function ($, cu) {
	const DEF_EVENT_NAME = 'onload';
	const META_HOME = '_home';
	const META_CURRENT = '_current';
	const META_PERSISTENT_ARGS = '_parms';
	const FORMAT_PREFIX = '__format_';

	var timer = null;
	var intervals = {};

	var state = {
		com: {
			requestId: null,
			message: null,
			apiResult: null,
			requestedJs: []
		},
		loc: {
			url: null,
			parms: {},
			onBack: null,
			clientArgs: {},
			onBackArgs: {}
		},
		ctrl: {}
	};

	var instance = {
		initForm: function (args) {
			var form = $('#' + args.id);
			form.on('click', 'input[type="submit"], button[type="submit"]', function (event) {
				if (event.target.getAttribute('data-cancelevent') == 'true') return;
				/* horrible hack to detect form submissions via ajax */
				event.preventDefault();
				$(event.target.form).trigger('submit', event.target);
			});
			form.on('keydown', 'input[type="text"]', function (event) {
				/* horrible hack to detect form submissions via ajax */
				if (event.target.readOnly)
					event.preventDefault();
			});
			form.on('submit', { el: form[0] }, function (e, submitter) {
				if (submitter.classList.contains('ajax-loading'))
					return false;

				const confirmMsg = submitter.getAttribute('data-confirm');
				if (confirmMsg) {
					if (!window.confirm(confirmMsg)) {
						return false;
					}
				}

				return instance.formSubmit(submitter, e.data.el);
			});
			if (!args.submitOnEnter) {
				form.on("keypress", ":input:not(textarea):not([type=submit])", function (e) {
					return e.keyCode != 13;
				});
			}
		},
		formSubmit: function (sender, form, dict) {
			if (form.hasAttribute('data-res') && instance.processResult(form) == false) return false;
			var fd = new FormData(form);
			fd.append('submit', sender.value);
			var els = form.elements;
			for (var i = 0, el; el = els[i++];) {
				processElementDataOnFormSubmit(el, function (key, value) { fd.append(key, value); });
			}
			var target = { data: fd, method: 'POST' };
			processElementDataOnEvent(sender, target, function (key, value) { fd.append(key, value); });
			if (!target.e) target.e = 'onsubmit';

			//target.e = sender.hasAttribute('data-e') ? sender.getAttribute('data-e') : 'onsubmit';
			//if (sender.hasAttribute('data-r')) target.r = sender.getAttribute('data-r');
			//target.url = instance.findServiceAction(form);
			//const container = cu.getThisOrParent(sender, function (n) { return n.hasAttribute && n.hasAttribute('data-c-prefix'); });
			//if (container) {
			//	target.containerPrefix = container.getAttribute('data-c-prefix');
			//	target.containerType = container.getAttribute('data-c-type');
			//}
			//if (sender.hasAttribute('data-responsetype')) {
			//	target.responsetype = sender.getAttribute('data-responsetype');
			//}
			runOnAjaxSend(sender, target);
			const r = instance.postEventWithApiResponse(target);
			if (form.hasAttribute('data-res-postponed'))
				r.then(function (apiResult) {
					if (apiResult.success != false)
						instance.processResult(form);
				});

			return false;
		},
		error: function (xhr, status, e) {
			var text = '';
			var title = localization.resources.title.systemError;
			var showinframe = false;
			var severity = 'err';

			if (e && e.message) {
				title = localization.resources.title.javascriptError;
				text = e.message + '<br>' + e.stack;
			}
			else if (xhr.status == '401') {
				const location = xhr.getResponseHeader('location');
				if (location)
					window.location = location;
				else {
					title = localization.resources.title.noAccess;
					text = localization.resources.text.notLoggedSystem;
					severity = 'warn';
				}
			}
			else if (xhr.status == '403') {
				title = localization.resources.title.noAccess;
				text = localization.resources.text.insufficientPermissionsOperation;
				severity = 'warn';
			}
			else if (xhr.status == '404') {
				title = localization.resources.title.pageMissing;
				text = localization.resources.text.linkGoMainPage;
				severity = 'warn';
			}
			else if (e && this.url && xhr.status != '500') {
				title = localization.resources.title.ajaxError;
				text = this.url + '<br>' + xhr.status + ' ' + e;
			}
			else {
				text = xhr.responseText;
				showinframe = true;
			}

			requestCompleted();
			showError(title, text, severity, showinframe);
		},
		delay: function (caller, func, timeout) {
			if(!timeout) {
				timeout = 400;
			}
			if (timer) {
				window.clearTimeout(timer);
				timer = null;
			}
			timer = window.setTimeout(function () { func(caller); }, timeout);
		},
		bindevent: function (args) {
			$('#' + args.id).on(args.clientEvent, { serverEvent: args.serverEvent, receiver: args.serverEventReceiver, method: args.method }, function (e) {
				if (e.data.method && e.data.method == 'get')
					instance.runEventFromElementWithApiResponse(this, { e: e.data.serverEvent, r: e.data.receiver });
				else
					instance.postEventFromElementWithApiResponse(this, { e: e.data.serverEvent, r: e.data.receiver });
			});
		},
		repeatedPostEvent: function (args) {
			if (intervals[args.id]) return;

			var n = window.setInterval(function () {
				var el = document.getElementById(args.id);
				if (el)
					instance.postEventFromElementWithApiResponse(el);
				else if (intervals[args.id]) {
					window.clearInterval(intervals[args.id]);
					delete intervals[args.id];
				}
			}, args.interval);

			intervals[args.id] = n;
		},
		runEvent: function (target) {
			return $.ajax({
				url: instance.prepareUrl(target),
				type: 'GET',
				responseType: target.responsetype ? target.responsetype : ""
			}).fail(instance.error).then(onRequestResult);
		},
		runEventWithApiResponse: function (target) {
			return instance.runEvent(target).then(instance.loadScripts).then(processApiResponse);
		},
		runEventFromElementWithApiResponse: function (el, target) {
			if (el.hasAttribute('data-res') && instance.processResult(el) == false) return;
			if (!target) target = {};
			if (!target.data) target.data = {};
			if (!target.query) target.query = {};
			target.method = 'GET';
			processElementDataOnEvent(el, target, function (key, value) { target.query[key] = value; });
			if (el instanceof HTMLInputElement || el instanceof HTMLSelectElement || el instanceof HTMLTextAreaElement)
				target.query[el.name] = el.value;
			runOnAjaxSend(el, target);
			return instance.runEventWithApiResponse(target);
		},
		postEvent: function (target) {
			const isForm = target.data instanceof FormData;
			if (!target.responsetype) {
				return $.ajax({
					url: instance.prepareUrl(target),
					type: 'POST',
					processData: !isForm,
					contentType: isForm ? false : "application/json; charset=utf-8",
					data: isForm ? target.data : JSON.stringify(target.data)
				}).fail(instance.error).then(onRequestResult);
			}
			else {
				const r = $.Deferred();
				const xhr = new XMLHttpRequest();
				xhr.open('POST', instance.prepareUrl(target));
				xhr.responseType = 'arraybuffer';
				xhr.onload = function () {
					if (this.status >= 200 && this.status < 300) {
						r.resolve(xhr.response, this.status, xhr);//.then(onRequestResult);
					} else {
						r.reject(xhr, status, null);//.then(instance.error);
					}
				};
				xhr.onerror = function () {
					r.reject(xhr, status, null);//.then(instance.error);
				};
				beforeRequest(this, xhr, null);
				xhr.contentType = isForm ? false : "application/json; charset=utf-8";
				xhr.processData = !isForm;
				xhr.send(isForm ? target.data : JSON.stringify(target.data));
				return r.fail(instance.error).then(onRequestResult)/*.promise()*/;
			}
		},
		postEventWithApiResponse: function (target) {
			return instance.postEvent(target).then(instance.loadScripts).then(processApiResponse);
		},
		postEventFromElementWithApiResponse: function (el, target) {
			if (el.hasAttribute('data-res') && instance.processResult(el) == false) return;
			const form = $(el).closest('form')[0];
			if (!target) target = {};
			if (!target.data) target.data = {};
			if (!target.query) target.query = {};
			if (form) {
				target.data = $(form).serializeObject();
				var els = form.elements;
				for (var i = 0, fel; fel = els[i++];) {
					processElementDataOnFormSubmit(fel, function (key, value) { target.data[key] = value; });
				}
			}

			target.method = 'POST';
			processElementDataOnEvent(el, target, function (key, value) { target.data[key] = value; });
			if (!form && (el instanceof HTMLInputElement || el instanceof HTMLSelectElement || el instanceof HTMLTextAreaElement))
				target.data[el.name] = el.value;

			runOnAjaxSend(el, target);
			const r = instance.postEventWithApiResponse(target);
			if (el.hasAttribute('data-res-postponed'))
				return r.then(function (apiResult) {
					if (apiResult.success != false)
						instance.processResult(el);
				});
			else
				return r;
		},
		runHrefWithApiResponse: function (a, target) {
			if (!target) target = {};
			target.changeloc = true;
			instance.runEventFromElementWithApiResponse(a, target);
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
		prepareTarget: function (target) {
			var parms = {};
			const isForm = target.data instanceof FormData;

			if (!target.currenturl)
				target.currenturl = target.url;

			var sep = target.url.indexOf('?');
			var targetpath = sep >= 0 ? target.url.substring(0, sep) : target.url;
			const targetquery = sep >= 0 ? target.url.substring(sep + 1) : '';
			const targetqueryparms = cu.getParams(targetquery, true);
			for (var key in target.query) {
				targetqueryparms[key] = encodeURIComponent(target.query[key]);
			}
			for (var key in target.data) {
				if (targetqueryparms[key]) {
					targetqueryparms[key] = encodeURIComponent(target.data[key]);
					delete target.data[key];
				}
			}
			target.url = targetpath + '?';
			for (var key in targetqueryparms) {
				if (targetqueryparms[key] && targetqueryparms[key] != '')
					target.url += key + '=' + targetqueryparms[key] + '&';
			}
			target.url = target.url.slice(0, -1);

			if (target.changeloc) {
				state.loc.url = target.url;
			}

			if (target.method == 'GET') {
				for (var key in target.data) {
					parms[key] = target.data[key];
				}
			}

			var page = document.head.getAttribute('data-page');
			if (page) parms.p = page;
			if (target.r) parms.r = target.r;
			if (target.sender) parms.sender = target.sender;
			parms.e = target.e ? target.e : DEF_EVENT_NAME;

			state.loc.parms = parms;

			var curpath = target.currenturl;
			sep = curpath.indexOf('?');
			curpath = sep >= 0 ? curpath.substring(0, sep) : curpath;

			if (curpath == '/' || targetpath == '/') {
				const home = document.getElementById(META_HOME);
				const alias = home.getAttribute('data-alias');
				if (curpath == '/') curpath = alias || '/';
				if (targetpath == '/') targetpath = alias || '/';
			}
			var base = document.getElementsByTagName('base')[0];
			base = base ? base.getAttribute('href') : '';
			if (base && !targetpath.startsWith(base))
				targetpath = base + targetpath;
			if (base && !curpath.startsWith(base))
				curpath = base + curpath;

			if (targetpath != curpath) {
				parms['c-new'] = 1;
				if (target.changeloc)
					state.ctrl = {};
			}
			else if (!parms['c-prefix'] && target.containerPrefix) {
				parms['c-prefix'] = target.containerPrefix;
				parms['c-type'] = target.containerType;
			}

			if (target.responsetype)
				parms['responsetype'] = target.responsetype;

			if (target.changeloc) {
				if (target.onBack) state.loc.onBack = target.onBack;
				window.history.pushState(state.loc, "", target.url);
			}

			for (var key in state.loc.clientArgs) {
				parms[key] = state.loc.clientArgs[key];
			}

			for (var key in parms) {
				if (!parms[key]) delete parms[key];
			}

			target.parms = parms;
		},
		prepareUrl: function (target) {
			instance.prepareTarget(target);
			return getApiUrl(target.url, target.parms, target.isfirstload);
		},
		stopRequest: function () {
			requestCompleted();
		},
		processResult: function (el) {
			const result = el.getAttribute('data-res') || el.getAttribute('data-res-postponed');
			const handler = commonUtils.getThisOrParent(el, function (parent) { return parent.hasAttribute('data-res-handler'); });

			if (!handler) return;

			const callOnResult = function (ctrl) {
				const t = ctrl.getAttribute('data-ctrl');
				const ctrlid = ctrl.hasAttribute('data-ctrl-id') ? ctrl.getAttribute('data-ctrl-id') : ctrl.id;
				if (window[t] && window[t]['onResult']) {
					return window[t]['onResult'](result, state.ctrl[ctrlid]);
				}
			};

			const children = handler.querySelectorAll('[data-ctrl]');

			for (var i = 0; i < children.length; i++) {
				if (callOnResult(children[i]) == false) return false;
			}

			if (callOnResult(handler) == false) return false;
		},
		findServiceAction: function (el) {
			var root = el;
			if (root != document.head) {
				root = cu.getParent(el, function (n) { return n.hasAttribute && n.hasAttribute('data-href'); });
				if (!root) root = document.getElementById(META_CURRENT);
			}
			const home = document.getElementById(META_HOME);
			return root.getAttribute('data-href') || home.getAttribute('data-href') || '/';
		},
		findControl: function (el) {
			const ctrl = cu.getThisOrParent(el, function (n) { return n.hasAttribute && n.hasAttribute('data-ctrl'); });
			if (!ctrl) return null;
			const id = ctrl.hasAttribute('data-ctrl-id') ? ctrl.getAttribute('data-ctrl-id') : ctrl.id;
			return { root: ctrl, id: id, state: state.ctrl[id] };
		},
		setValue: function (args) {
			var e = document.getElementById(args.id);
			if (e) {
				if (e instanceof HTMLInputElement || e instanceof HTMLSelectElement)
					e.value = args.value;
				else
					e.innerHTML = args.value;
			}
			const st = getElementStateAttrs(e);
			if (st) {
				if (st.type == 'array') {
					const values = args.value.split(',').filter(String);
					const arr = state.ctrl[st.owner][st.name];
					arr.splice(0, arr.length);
					for (var i = 0; i < values.length; i++) {
						arr.push(values[i]);
					}
				}
			}
		},
		changeUrl: function (args) {
			const deleteRegex = new RegExp(args.remove.join('=|') + '=');
			const params = location.search.slice(1).split('&');
			var search = [];

			for (var i = 0; i < params.length; i++)
				if (deleteRegex.test(params[i]) === false)
					search.push(params[i]);

			for (var key in args.add) {
				search.push(key + '=' + args.add[key]);
			}

			var url = location.pathname + (search.length ? '?' + search.join('&') : '') + location.hash;

			window.history.replaceState({}, document.title, url);
			state.loc.url = url;

			const current = document.getElementById(META_CURRENT);
			current.setAttribute('data-href', url);
		},
		state: state
	};

	function getApiUrl(path, parms, isfirstload) {
		const k = path.indexOf('?');

		var url;
		if (k > 0) {
			url = path.substring(0, k);
			const urlParms = cu.getParams(path.substring(k + 1));
			for (var key in urlParms) {
				if (!parms[key]) {
					parms[key] = urlParms[key];
				}
			}
		}
		else
			url = path;

		url += '?';

		for (var key in parms) {
			url += key + '=' + encodeURIComponent(parms[key]) + '&';
		}

		return isfirstload ? url += 'firstload=true' : url.slice(0, -1);
	}

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
		xhr.setRequestHeader('x-request-guid', state.com.requestId);
		xhr.setRequestHeader('x-csrf-token', document.head.getAttribute('data-x-csrf-token'));
		setTimeout(function () {
			if (state.com.requestId && state.com.message) document.body.style.cursor = 'wait';
		}, 100);
		setTimeout(function () {
			if (state.com.requestId && state.com.message) state.com.message.css('display', 'block');
		}, 1000);
	}

	function requestCompleted() {
		state.com.requestId = null;
		if (document.body) document.body.style.cursor = '';
		if (state.com.message) state.com.message.css('display', 'none');

		const nodes = document.querySelectorAll('.ajax-loading');

		for (var i = 0; i < nodes.length; i++) {
			nodes[i].classList.remove('ajax-loading');
		}
	}

	function onRequestResult(data, status, xhr) {
		if (xhr.getResponseHeader('X-Request-Guid') == state.com.requestId) {
			requestCompleted();
			const disposition = xhr.getResponseHeader('Content-Disposition');

			// check file download response
			if (disposition && disposition.indexOf('attachment') !== -1) {
				const contenttype = xhr.getResponseHeader('Content-Type');
				cu.processFile(contenttype, disposition, data);
				return $.Deferred().reject();
			}
			else
				return $.Deferred().resolve(data);
		}
		else
			return $.Deferred().reject();
	}

	function processElementDataOnEvent(el, target, setvalfunc) {
		for (var attr, i = 0, attrs = el.attributes, n = attrs ? attrs.length : 0; i < n; i++) {
			attr = attrs[i];
			var val = attr.value == '' ? null : attr.value;
			if (attr.name.startsWith('data-p-')) {
				if (target.method == 'POST') {
					if (target.data instanceof FormData) {
						target.data.append(attr.name.replace('data-p-', ''), val || '');

					}
					else
						target.data[attr.name.replace('data-p-', '')] = val || '';
				}
				else
					target.query[attr.name.replace('data-p-', '')] = val || '';
			} else if (attr.name == 'href') {
				target.url = val;
			} else if (attr.name == 'data-href') {
				target.url = val;
			} else if (attr.name == 'data-e') {
				target.e = val;
			} else if (attr.name == 'data-r') {
				target.r = val;
			} else if (attr.name.startsWith('data-format')) {
				target.data[FORMAT_PREFIX + el.name] = val;
			} else if (attr.name.startsWith('data-c-')) {
				target.data[attr.name.replace('data-c-', 'c-')] = val || '';
			} else if (attr.name.startsWith('data-ref')) {
				processElementValue(document.getElementById(val), setvalfunc);
			} else if (attr.name == 'data-responsetype') {
				target.responsetype = val;
			}
		}

		// TODO: доработать для определения модальных контейнеров + обработка открытия модального окна из модального окна.
		if (el.hasAttribute('data-c-new') && el.hasAttribute('data-c-type')) {
			if(el.getAttribute('data-c-type') === 'dialogform') {
				for (var key in target.data) {
					if (!key.startsWith('c-'))
						state.loc.onBackArgs[key] = target.data[key];
				}
			}
		}

		target.currenturl = instance.findServiceAction(el);
		if (!target.url) {
			target.url = target.currenturl;
		}

		if (el.id) target.sender = el.id;

		const startEl = el.hasAttribute('data-c-external') ? document.getElementById(el.getAttribute('data-c-external')) : el;
		const container = cu.getThisOrParent(startEl, function (n) { return n.hasAttribute && n.hasAttribute('data-c-prefix'); });

		if (container) {
			target.containerPrefix = container.getAttribute('data-c-prefix');
			target.containerType = container.getAttribute('data-c-type');
			if (container.getAttribute('aria-modal') == 'true') {
				target.changeloc = false;
				if (el.hasAttribute('data-res')) {
					for (var key in state.loc.onBackArgs) {
						if (target.data[key] == null) {
							target.data[key] = state.loc.onBackArgs[key];
						}
					}
				}
			}
		}

		if (el.hasAttribute('data-res')) {
			state.loc.onBackArgs = {};
		}
	}

	function processElementValue(el, setvalfunc) {
		if (!el) return;

		var parmname = null;
		if (el.name)
			parmname = el.name;
		else if (el.hasAttribute('data-name'))
			parmname = el.getAttribute('data-name');

		if (parmname) {
			var val = null;

			if (el.name !== undefined && el.type == 'checkbox' && !el.hasAttribute('disabled')) {
				val = el.checked;
			}
			else if (el.name !== undefined && el.type == 'select-multiple' && !el.hasAttribute('disabled')) {
				parmname = parmname.replace('[]', '');
				var result = [];
				for (var i = 0, iLen = el.options.length; i < iLen; i++) {
					const opt = el.options[i];
					if (opt.selected) {
						result.push(opt.value || opt.text);
					}
				}
				val = result.join(',');
			}
			else if (el.name !== undefined && el.value !== undefined && !el.hasAttribute('disabled')) {
				val = el.value;
			}
			else if (el.isContentEditable) {
				val = el.innerText;
			}

			if (val) {
				setvalfunc(parmname, val);
			}
		}

		for (var i = 0; i < el.children.length; i++) {
			processElementValue(el.children[i], setvalfunc);
		}
	}

	function processElementDataOnFormSubmit(el, setvalfunc) {
		for (var attr, i = 0, attrs = el.attributes, n = attrs ? attrs.length : 0; i < n; i++) {
			attr = attrs[i];
			if (attr.name.startsWith('data-format')) {
				setvalfunc(FORMAT_PREFIX + el.name, attr.value);
			}
		}
	}

	function processApiResponse(apiResult) {
		console.log('processApiResponse');

		if (!apiResult) return;
		if (apiResult instanceof ArrayBuffer)
			apiResult = JSON.parse(textDecode(apiResult));

		if (apiResult.url) {
			window.location = apiResult.url;
			return;
		}

		state.com.apiResult = apiResult;

		if (document.readyState == 'complete' || document.readyState == 'interactive')
			renderApiResult();

		return $.Deferred().resolve(apiResult);
	}

	function getElementStateAttrs(el) {
		const type = el.getAttribute('data-hasclientstate');
		var owner = el.getAttribute('data-clientstate-owner');
		const name = el.getAttribute('data-clientstate-name') || el.name;
		if (!owner) {
			const parentctrl = instance.findControl(el);
			if (!parentctrl) return null;
			owner = parentctrl.id;
		}
		return { type: type, owner: owner, name: name };
	}

	function renderApiResult() {
		var apiResult = state.com.apiResult;
		state.com.apiResult = null;

		if (apiResult.error) {
			showError(localization.resources.title.systemError, apiResult.error, 'err');
			return;
		}

		const nodes = [];
		const shadow = (new DOMParser()).parseFromString("<!DOCTYPE html>", "text/html");

		const replaceFunc = function (el, obj) {
			if (obj.name == el.id)
				obj.content.firstChild.id = el.id;

			if (obj.content.firstChild.id == '' && obj.content.querySelector('#' + el.id) == null)
				obj.content.firstChild.id = el.id;

			el.parentNode.replaceChild(obj.content.firstChild, el);
		};
		const addFunc = function (el, obj) {
			if (obj.content.childNodes.length == 1 && el.id == obj.content.childNodes[0].id) {
				replaceFunc(el, obj);
				return;
			}
			while (el.firstChild) {
				el.removeChild(el.firstChild);
			}
			while (obj.content.childNodes.length > 0)
				el.appendChild(obj.content.childNodes[0]);
		};
		const adjacentFunc = function (el, obj) {
			while (obj.content.childNodes.length > 0) {
				const i = obj.position == 'beforeEnd' || obj.position == 'beforeBegin' ? 0 : obj.content.childNodes.length - 1;
				el.insertAdjacentElement(obj.position, obj.content.childNodes[i]);
			}
			if (obj.position == 'afterEnd') {
				cu.scrollToView(el.nextSibling);
			}
		};

		var rtagName = /<([\w:]+)/,
			// We have to close these tags to support XHTML (#13200)
			wrapMap = {
				option: [1, "<select multiple='multiple'>", "</select>"],
				thead: [1, "<table>", "</table>"],
				col: [2, "<table><colgroup>", "</colgroup></table>"],
				tr: [2, "<table><tbody>", "</tbody></table>"],
				td: [3, "<table><tbody><tr>", "</tr></tbody></table>"],
				_default: [0, "", ""]
			};

		function parseHTML(htmlString) {
			var tag, wrap, j,
				fragment = document.createElement('div');

			// Deserialize a standard representation
			tag = (rtagName.exec(htmlString) || ["", ""])[1].toLowerCase();
			wrap = wrapMap[tag] || wrapMap._default;
			fragment.innerHTML = wrap[1] + htmlString + wrap[2];

			// Descend through wrappers to the right content
			j = wrap[0];
			while (j--) {
				fragment = fragment.lastChild;
			}

			return fragment;
		}

		if (apiResult.widgets) {
			for (var w in apiResult.widgets) {
				var obj = apiResult.widgets[w];
				if (obj && typeof (obj) == "object") {
					var el = obj.name == 'body' ? document.body : document.getElementById(obj.name);

					if (obj.action == 'remove') {
						if (el) el.parentElement.removeChild(el);
						continue;
					}

					const shadowel = shadow.getElementById(obj.name);
					if (shadowel) {
						el = shadowel;
						obj.nested = true;
					}

					var parentel = null;
					if (obj.action == 'adjacent') {
						if (obj.parent && obj.parent != 'body' && obj.parent != '') {
							parentel = shadow.getElementById(obj.parent);
							if (parentel) {
								obj.nested = true;
								el = shadow.getElementById(obj.name);
							}
							else
								parentel = document.getElementById(obj.parent);

						}
						else
							parentel = document.body;
					}

					if (obj.action == 'replace' || (el && obj.action == 'adjacent') || obj.action == 'add') {
						if (!el) continue;
						obj.content = parseHTML(obj.content);
						obj.el = el;
						obj.func = obj.action == 'add' ? addFunc : replaceFunc;
						nodes.push(obj);
					}
					else if (obj.action == 'adjacent') {
						if (!parentel) continue;
						obj.content = parseHTML(obj.content);
						obj.el = parentel;
						obj.func = adjacentFunc;
						nodes.push(obj);
					}

					if (obj.nested)
						obj.func(el || parentel, obj);
					else {
						const parent = obj.el.parentNode.nodeName == 'HEAD' ? shadow.head : shadow.body;
						parent.appendChild(obj.content);
					}
				}
			}

			const ctrls = shadow.querySelectorAll('[data-ctrl]');
			const bindels = shadow.querySelectorAll('[data-hasclientstate]');

			for (var j = 0; j < bindels.length; j++) {
				const node = bindels[j];
				const st = getElementStateAttrs(node);
				if (!st) continue;

				if (!state.ctrl[st.owner]) state.ctrl[st.owner] = {};
				const nodectrl = state.ctrl[st.owner];
				if (st.type == 'array') {
					const ctrlvar = nodectrl[st.name] ? new ObservableArray(nodectrl[st.name]) : new ObservableArray();
					const values = node.value.split(',').filter(String);
					for (var i = 0; i < values.length; i++) {
						if (ctrlvar.indexOf(values[i]) == -1)
							ctrlvar.push(values[i]);
					}
					ctrlvar.on('pop push shift unshift splice reverse sort', function () {
						node.value = this.join(',');
					});
					nodectrl[st.name] = ctrlvar;
					node.value = ctrlvar.join(',');
				} else if (st.type == 'value') {
					Object.defineProperty(nodectrl, st.name, {
						enumerable: true,
						configurable: true,
						get: function () { return node.value; },
						set: function (val) { node.value = val; }
					});
				}
			}

			for (var i = 0; i < ctrls.length; i++) {
				var root = ctrls[i];
				const t = root.getAttribute('data-ctrl');
				const ctrl = instance.findControl(root);
				if (root.id != ctrl.id) root = shadow.getElementById(ctrl.id);

				if (!ctrl.state) {
					ctrl.state = {};
					state.ctrl[ctrl.id] = ctrl.state;
				}

				if (!ctrl.state.type) {
					ctrl.state.type = t
					ctrl.state.root = ctrl.id;

					if (window[t] && window[t]['init']) {
						window[t]['init'](root, ctrl.state);
						console.log('widget: ' + ctrl.id + ' init ' + t);
					}
				}

				if (window[t] && window[t]['widgetWillMount']) {
					window[t]['widgetWillMount'](shadow, ctrl.state);
					console.log('widget: ' + ctrl.id + ' widgetWillMount ' + t);
				}
			}

			nodes.forEach(function (n) {
				if (n.nested) return;
				n.func(n.el, n);

				const ctrl = instance.findControl(n.el);
				if (ctrl) {
					const t = ctrl.root.getAttribute('data-ctrl');
					if (window[t] && window[t]['widgetContentChanged']) {
						window[t]['widgetContentChanged'](ctrl.state);
						console.log('widget: ' + ctrl.id + ' widgetContentChanged ' + t);
					}
				}
			});

			for (var i = 0; i < ctrls.length; i++) {
				const root = ctrls[i];
				const ctrl = instance.findControl(root);
				const t = root.getAttribute('data-ctrl');

				if (window[t] && window[t]['widgetDidMount']) {
					if (apiResult.state) {
						for (var key in apiResult.state[t]) {
							ctrl.state[key] = apiResult.state[t][key];
						}
					}

					window[t]['widgetDidMount'](ctrl.state);
					console.log('widget: ' + ctrl.id + ' widgetDidMount ' + t);
				}
			}
		}

		if (apiResult.redirect) {
			state.loc.url = apiResult.redirect.url;
			state.loc.parms = apiResult.redirect.parms;
			window.history.pushState(state.loc, "", apiResult.redirect.url);
		}

		const current = document.getElementById(META_CURRENT);
		current.setAttribute('data-href', state.loc.url);

		if (apiResult.clientactions) {
			var ca;
			for (var i = 0; i < apiResult.clientactions.length; i++) {
				ca = apiResult.clientactions[i];
				runClientAction(ca.service, ca.callChain, 0);
			}
		}

		if (apiResult.hardredirect) {
			window.location = apiResult.hardredirect.url;
		}

		if (window.homePage) homePage.countNavBodyHeight();
		console.log("renderApiResult complete");
	}

	function runOnAjaxSend(el, target) {
		var node = el;
		node.classList.add('ajax-loading');
		do {
			node = cu.getParent(node, function (n) { return n.hasAttribute('data-ctrl'); });
			if (!node) break;
			const t = node.getAttribute('data-ctrl');
			if (window[t] && window[t]['onAjaxSend']) {
				window[t]['onAjaxSend'](el, target, state.ctrl[node.id]);
				console.log('widget: ' + node.id + ' onAjaxSend ' + t);
			}
		} while (true);
	}

	function runClientAction(service, callChain, iter) {
		if (iter > 10) return;
		var caller = window[service];
		if (caller) {
			for (var j = 0; j < callChain.length; j++) {
				step = callChain[j];
				if (step.method == 'apply')
					caller = caller[step.method](caller, Array.isArray(step.args) ? step.args : [step.args]);
				else
					caller = caller[step.method](step.args);
			}
		}
		else {
			console.log("wait for " + service);
			setTimeout(function () { runClientAction(service, callChain, iter++); }, 50);
		}
	}

	function showError(title, text, severity, showinframe) {
		if (window.dialog) {
			const placeholder = 'container_err';
			const errd = document.getElementById(placeholder);
			const errt = document.getElementById(placeholder + '_title');
			const errb = document.getElementById(placeholder + '_body');
			if (!state.ctrl[placeholder]) {
				state.ctrl[placeholder] = {};
				state.ctrl[placeholder].root = placeholder;
			}
			errt.innerHTML = title;
			errd.classList.remove('err');
			errd.classList.remove('warn');
			errd.classList.remove('info');
			errd.classList.add(severity);

			var frame = document.getElementById(placeholder + '_frame');

			if (showinframe) {
				if (!frame) {
					frame = document.createElement('iframe');
					frame.id = placeholder + '_frame';
					errt.insertAdjacentElement('afterEnd', frame);
				}
				frame.classList.remove('hide');
				errb.classList.add('hide');

				frame.contentWindow.contents = text;
				frame.src = 'javascript:window["contents"]';
			}
			else {
				if (frame) frame.classList.add('hide');
				errb.classList.remove('hide');
				errb.innerHTML = '<pre>' + text + '</pre>';
			}
			dialog.widgetWillMount(document, state.ctrl[placeholder]);
		}
		else
			document.body.innerHTML = title + '<br/><pre>' + text + '</pre>';
	}

	function textDecode(str) {
		if (window.TextDecoder) {
			return new TextDecoder('utf-8').decode(str);
		}
		const bytes = new Uint8Array(str);
		let pos = 0;
		const len = bytes.length;
		const out = [];

		while (pos < len) {
			const byte1 = bytes[pos++];
			if (byte1 === 0) {
				break;  // NULL
			}

			if ((byte1 & 0x80) === 0) {  // 1-byte
				out.push(byte1);
			} else if ((byte1 & 0xe0) === 0xc0) {  // 2-byte
				const byte2 = bytes[pos++] & 0x3f;
				out.push(((byte1 & 0x1f) << 6) | byte2);
			} else if ((byte1 & 0xf0) === 0xe0) {
				const byte2 = bytes[pos++] & 0x3f;
				const byte3 = bytes[pos++] & 0x3f;
				out.push(((byte1 & 0x1f) << 12) | (byte2 << 6) | byte3);
			} else if ((byte1 & 0xf8) === 0xf0) {
				const byte2 = bytes[pos++] & 0x3f;
				const byte3 = bytes[pos++] & 0x3f;
				const byte4 = bytes[pos++] & 0x3f;

				// this can be > 0xffff, so possibly generate surrogates
				let codepoint = ((byte1 & 0x07) << 0x12) | (byte2 << 0x0c) | (byte3 << 0x06) | byte4;
				if (codepoint > 0xffff) {
					// codepoint &= ~0x10000;
					codepoint -= 0x10000;
					out.push((codepoint >>> 10) & 0x3ff | 0xd800)
					codepoint = 0xdc00 | codepoint & 0x3ff;
				}
				out.push(codepoint);
			} else {
				// FIXME: we're ignoring this
			}
		}

		return String.fromCharCode.apply(null, out);
	}

	document.addEventListener('DOMContentLoaded', function () {
		state.com.message = $("#topmessagecontainer");
		setTimeout(function () {
			if (state.com.requestId) state.com.message.css('display', 'block');
		}, 100);
		document.body.className = '';

		window.addEventListener('popstate', function (event) {
			const s = window.history.state;
			if (!s) return;

			if (!s.parms || Object.keys(s.parms).length == 0) {
				s.parms = [];
				s.parms['p'] = state.loc.parms['p'];
				s.url = window.location.pathname + window.location.search;
			}

			if (state.loc.parms['c-new'] == 1) {
				s.parms['c-new'] = 1;
				s.parms['e'] = DEF_EVENT_NAME;
				if (s.parms['r']) delete s.parms['r'];
				if (s.parms['c-prefix']) delete s.parms['c-prefix'];
			}

			var parser = document.createElement('a');
			parser.href = state.loc.url;

			if (window.location.pathname != parser.pathname)
				state.ctrl = {};

			state.loc = s;

			if (s.onBack && !s.parms['c-new'])
				runClientAction(s.onBack.service, s.onBack.callChain, 0);
			else
				$.get(getApiUrl(s.url, s.parms))
					.fail(instance.error)
					.then(onRequestResult).then(instance.loadScripts).then(processApiResponse);
		});


		if (state.com.apiResult)
			renderApiResult();
	});

	$(document).ajaxSend(beforeRequest);
	$(document).ajaxStop(requestCompleted);

	const current = document.getElementById(META_CURRENT);
	state.loc.url = document.location.pathname + document.location.search;
	current.setAttribute('data-href', state.loc.url);
	instance.runEventFromElementWithApiResponse(current, { url: state.loc.url, isfirstload: true });

	history.replaceState(state.loc, document.title, state.loc.url);

	return instance;
}($, commonUtils);

var ObservableArray = (function () {
	function ObservableArray(collection) {
		// calling with `new` is optional
		if (!(this instanceof ObservableArray)) {
			return new ObservableArray(collection);
		}

		// add items from passed `collection` to `this`
		collection = collection || [];
		for (var i = 0; i < collection.length; i++) {
			this[i] = collection[i];
		}

		// set length so it acts like an array: http://stackoverflow.com/a/6599447/552067
		this.length = collection.length;

		// keep list of observing functions
		this.subscribers = {};
	}

	ObservableArray.prototype = {
		// Subscribe a function to each event in a space-separated
		// list of events. If an event doesn't exist, create it.
		on: function (eventsStr, fn, callNow) {
			eventsStr.split(' ').forEach(function (event) {
				if (!this.subscribers[event]) this.subscribers[event] = [];
				this.subscribers[event].push(fn);
			}, this);
			if (callNow) fn.call(this);
			return this;
		},

		// Pass a space-separated list of events and a function to unsubscribe a specific
		// function from those events, pass just the events to unsubscribe all functions
		// from those events, or don't pass any arguments to cancel all subscriptions.
		off: function (eventsStr, fn) {
			if (eventsStr) {
				eventsStr.split(' ').forEach(function (event) {
					if (fn) {
						var fnIndex = this.subscribers[event].indexOf(fn);
						if (fnIndex >= 0) this.subscribers[event].splice(fnIndex, 1);
					} else {
						this.subscribers[event] = [];
					}
				}, this);
			} else {
				for (var event in this.subscribers) {
					this.subscribers[event] = [];
				}
			}
			return this;
		},

		// Notify all the subscribers of an event
		trigger: function (event) {
			var args = arguments;
			var t = this;
			function caller(fn) {
				fn.apply(t, args);
			}
			(this.subscribers[event] || []).forEach(caller);
			if (event !== 'any') (this.subscribers.any || []).forEach(caller);
			return this;
		}
	};

	var arrProto = Array.prototype;

	'pop push shift unshift splice reverse sort'
		.split(' ').forEach(function (methodName) {
			var method = arrProto[methodName];
			ObservableArray.prototype[methodName] = function () {
				var returnValue = method.apply(this, arguments);
				var args = [methodName].concat(arrProto.slice.call(arguments));
				this.trigger.apply(this, args);
				return returnValue;
			};
		});

	// add the above native array methods to ObservableArray.prototype
	'slice concat join some every forEach map filter reduce reduceRight indexOf lastIndexOf toString toLocaleString'
		.split(' ').forEach(function (methodName) {
			ObservableArray.prototype[methodName] = arrProto[methodName];
		});

	return ObservableArray;
})();
