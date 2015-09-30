using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Html
{
	public static class HtmlWriterSelectExtensions
	{
		public static void DropDownList(this IHtmlWriter w, string name, string value, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("select");
			SelectTagAttributes ta = new SelectTagAttributes(tb);
			ta.Name(name);
			if (attributes != null) attributes(ta);
			tb.Render(w, TagRenderMode.StartTag);

			foreach (var item in items)
			{
				TagBuilder tb_o = new TagBuilder("option");
				OptionTagAttributes oa = new OptionTagAttributes(tb_o);
				oa.Title(item.Text).Value(item.Value).Selected(item.Selected || item.Value == value);
				tb_o.Render(w, TagRenderMode.SelfClosing);
			}
			
			tb.Render(w, TagRenderMode.EndTag);
		}

		public static void ListBox(this IHtmlWriter w, string name, IEnumerable<string> values, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("select");
			SelectTagAttributes ta = new SelectTagAttributes(tb);
			ta.Name(name);
			if (attributes != null) attributes(ta);
			tb.Render(w, TagRenderMode.StartTag);

			foreach (var item in items)
			{
				TagBuilder tb_o = new TagBuilder("option");
				OptionTagAttributes oa = new OptionTagAttributes(tb_o);
				oa.Title(item.Text).Value(item.Value).Selected(item.Selected || (values != null && values.Contains(item.Value)));
				tb_o.Render(w, TagRenderMode.SelfClosing);
			}

			tb.Render(w, TagRenderMode.EndTag);
		}
	}

	public class SelectListItem
	{
		public string Text { get; set; }
		public string Value { get; set; }
		public bool Selected { get; set; }
	}
}