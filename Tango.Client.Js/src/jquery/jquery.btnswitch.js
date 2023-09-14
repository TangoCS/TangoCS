/**
 * jQuery Button Switch Plugin
 * Version 1.0.0
 * 
 * Minimal Usage: $('#switch').btnSwitch();
 * Settings:
 * Theme: Select a theme (Button, Light, Swipe, iOS, Android)
 */

(function ($) {
	$.fn.btnSwitch = function (options) {
		var settings = $.extend({}, options);

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
			var div = null;

			if (parent.id != id + '_bsh') {
				div = document.createElement('div');
				div.id = id + '_bsh';
				div.appendChild(el);
				parent.appendChild(div);
			}
			else
				div = parent;

			if (div.getElementsByTagName('label').length == 0) {
				const label = document.createElement('label');
				label.classList.add('btn-switch');
				label.htmlFor = id;
				div.appendChild(label);
			}

			el.className = 'tgl-sw tgl-sw-' + prefix;

			div.addEventListener('click', function (e) {
				e.preventDefault();
                if (el.disabled || el.hasAttribute('readonly')) return;
				el.checked = !el.checked;
				setClass(el, prefix);
				if (el.onchange)
					el.dispatchEvent(new Event("change"));
			});

			setClass(el, prefix);
		};

		return this.each(function () {
			if (['Light', 'Swipe', 'iOS', 'Android'].indexOf(settings.theme) == -1)
				settings.theme = 'Light';

			setHtml(this.id, settings.theme.toLowerCase());
		});
	};
}(jQuery));


class BtnSwitch extends Tango.Component {
	setValue(checked) {
		const el = document.getElementById(this.root);

		if (el.disabled || el.hasAttribute('readonly'))
			return;

		el.checked = checked;

		const isActive = el.classList.contains('tgl-sw-active');
		if (checked && !isActive) {
			el.classList.add('tgl-sw-' + this.props.prefix.toLowerCase() + '-checked');
			el.classList.add('tgl-sw-active');
		} else if (!checked && isActive) {
			el.classList.remove('tgl-sw-' + this.props.prefix.toLowerCase() + '-checked');
			el.classList.remove('tgl-sw-active');
		}
	}
}

Tango.registerComponent(BtnSwitch, sp => ({ cu: sp.commonUtils, prefix: 'Light' }));

