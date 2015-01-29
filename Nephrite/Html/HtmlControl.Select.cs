using System;
using System.Collections.Generic;
using System.Linq;

namespace Nephrite.Html
{
	public partial class HtmlControl
	{
		public void DropDownList(string name, string value, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("select");
			SelectTagAttributes ta = new SelectTagAttributes(tb) { Name = name };
			if (attributes != null) attributes(ta);
			Write(tb.Render(TagRenderMode.StartTag));

			foreach (var item in items)
			{
				TagBuilder tb_o = new TagBuilder("option");
				OptionTagAttributes oa = new OptionTagAttributes(tb_o) { Title = item.Text, Value = item.Value, 
					Selected = item.Selected || item.Value == value };
				Write(tb_o.Render(TagRenderMode.SelfClosing));
			}
			
			Write(tb.Render(TagRenderMode.EndTag));
		}

		public void ListBox(string name, IEnumerable<string> values, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			TagBuilder tb = new TagBuilder("select");
			SelectTagAttributes ta = new SelectTagAttributes(tb) { Name = name };
			if (attributes != null) attributes(ta);
			Write(tb.Render(TagRenderMode.StartTag));

			foreach (var item in items)
			{
				TagBuilder tb_o = new TagBuilder("option");
				OptionTagAttributes oa = new OptionTagAttributes(tb_o)
				{
					Title = item.Text,
					Value = item.Value,
					Selected = item.Selected || (values != null && values.Contains(item.Value))
				};
				Write(tb_o.Render(TagRenderMode.SelfClosing));
			}

			Write(tb.Render(TagRenderMode.EndTag));
		}


	}

	public class SelectListItem
	{
		public string Text { get; set; }
		public string Value { get; set; }
		public bool Selected { get; set; }
	}
}