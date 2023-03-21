using System;
using System.Linq;

namespace Tango.Html
{
	public static class HtmlWriterIconHelper
	{
		public static void Icon(this HtmlWriter w, string name, string tip = null, string color = null)
		{
			w.Icon(name, a => {
				if (tip != null)
					a.Title(tip);
				if (color != null)
					a.Style("color:" + color);
			}, null);
		}

		public static void Icon(this HtmlWriter w, string name, Action<TagAttributes> attrs, Action content = null)
		{
			Action<TagAttributes> ta = a => {
				if (name == null)
					a.Class("icon");
				else
				{
					name = name.ToLower();
					a.Class("icon icon-" + name);
				}
				a.Set(attrs);
			};
			w.I(ta, () => {
				if (name != null)
					w.SvgIcon(name);
				content?.Invoke();
			});
		}

		public static string Icon(this bool src)
		{
			if (src)
				return $"<i class='icon icon-bool-true'>{SvgIcon("bool-true")}</i>";
			else
				return $"<i class='icon icon-bool-false'>{SvgIcon("bool-false")}</i>";
		}

		public static string Icon(this bool? src)
		{
			return Icon(src ?? false);
		}

		public static string Icon(this int src)
		{
			return Icon(src == 1);
		}

		public static string Icon(this int? src)
		{
			return Icon(src.HasValue && src.Value == 1);
		}


		public static void IconCheckBox(this HtmlWriter w, Action<TagAttributes> attrs = null)
		{
			w.IconCheckBox(false, attrs);
		}
		
		public static void IconCheckBox(this HtmlWriter w, bool value, Action<TagAttributes> attrs = null)
		{
			var state = new [] {"checkbox-unchecked", "checkbox-checked"};
			if (value)
			{
				state = state.Reverse().ToArray();
			}
			w.Icon(state[0], attrs, () => w.SvgIcon(state[1]));
		}

		public static void IconThreeStateCheckBox(this HtmlWriter w, Action<TagAttributes> attrs = null)
		{
			w.Icon("checkbox-unchecked", attrs, () => {
				w.SvgIcon("checkbox-checked");
				w.SvgIcon("minus-box");
			});
		}


		public static void IconExpander(this HtmlWriter w, Action<TagAttributes> attrs = null, bool collapsed = true)
		{
			w.Icon(collapsed ? "collapsed" : "expanded", attrs, () => w.SvgIcon(collapsed ? "expanded" : "collapsed"));
		}


		static string SvgIcon(string name) => $"<svg class='svgicon-{name.ToLower()}'><use xlink:href=\"{HtmlWriter.SVGPATH}#icon-{name.ToLower()}\"></use></svg>";

		static void SvgIcon(this HtmlWriter w, string name)
		{
			w.Write(SvgIcon(name));
		}

		public static void IconFlag<T>(this TagAttributes<T> a, string name, bool issquare = false)
			where T : TagAttributes<T>
		{
			if (!issquare)
				a.Class("flag-icon flag-icon-" + name?.ToLower());
			else
				a.Class("flag-icon flag-icon-" + name?.ToLower() + " flag-icon-squared");
		}

		public static void BoolMark(this HtmlWriter w, bool mark, bool renderFalse = true, string tip = null, string color = null)
		{
			if (mark)
				w.Icon("boolmark-true", a => {
					if (tip != null)
						a.Title(tip);
					if (color != null)
						a.Style("color:" + color);
				}, null);
			else if (renderFalse)
				w.Icon("boolmark-false", a => {
					if (tip != null)
						a.Title(tip);
					if (color != null)
						a.Style("color:" + color);
				}, null);
		}
	}
}