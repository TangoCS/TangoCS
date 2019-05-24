/**
 * jQuery Button Switch Plugin
 * Version 1.0.0
 * 
 * Minimal Usage: $('#switch').btnSwitch();
 * Settings:
 * Theme: Select a theme (Button, Light, Swipe, iOS, Android)
 * OnText: What to display for the "On" Button
 * OffText: What to display for the "Off" Button
 */

(function ($) {
	$.fn.btnSwitch = function (options) {
		var settings = $.extend({
			theme: "Swipe",
			OnText: "On",
			OffText: "Off"
		}, options);

		var setClass = function (el, prefix) {
			if (el.checked) {
				el.classList.add('tgl-sw-' + prefix + '-checked');
				el.classList.add('tgl-sw-active');
			} else {
				el.classList.remove('tgl-sw-' + prefix + '-checked');
				el.classList.remove('tgl-sw-active');
			}
		};

		var setHtml = function (id, prefix) {
			const el = document.getElementById(id);
			const parent = el.parentElement;

			const div = document.createElement('div');
			div.id = id + '_bsh';

			div.appendChild(el);
			parent.appendChild(div);

			const label = document.createElement('label');
			label.classList.add('btn-switch');
			label.htmlFor = id;
			label.setAttribute('data-tg-on', settings.OnText);
			label.setAttribute('data-tg-off', settings.OffText);
			div.appendChild(label);

			const divcl = document.createElement('div');
			divcl.style.clear = 'both';
			div.appendChild(divcl);

			el.className = 'tgl-sw tgl-sw-' + prefix;

			div.addEventListener('click', function (e) {
				el.checked = !el.checked;
				setClass(el, prefix);
			});

			setClass(el, prefix);
		};

		return this.each(function () {
			if (['Light', 'Swipe', 'iOS', 'Android'].indexOf(settings.theme) == -1)
				settings.theme = 'Swipe';

			setHtml(this.id, settings.theme.toLowerCase());
		});
	};
}(jQuery));