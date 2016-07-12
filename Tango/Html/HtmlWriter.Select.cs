using System;
using System.Collections.Generic;
using System.Linq;

namespace Tango.Html
{
	public static class HtmlWriterSelectExtensions
	{
		public static void DropDownList(this IHtmlWriter w, string name, string value, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			Action<SelectTagAttributes> a = ta => {
				ta.Name(name).ID(name);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("select", a, () => {
				if (items == null) return;
				foreach (var item in items)
				{
					w.WriteTag<OptionTagAttributes>("option", 
						oa => oa.Value(item.Value).Selected(item.Selected || item.Value == value), 
						() => w.Write(item.Text));
				}
			});
		}

		public static void ListBox(this IHtmlWriter w, string name, int size, IEnumerable<string> values, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			Action<SelectTagAttributes> a = ta => {
				ta.Name(name).ID(name).Size(size);
				if (attributes != null) attributes(ta);
			};
			w.WriteTag("select", a, () => {
				if (items == null) return;
				foreach (var item in items)
				{
					w.WriteTag<OptionTagAttributes>("option",
						oa => oa.Value(item.Value).Selected(item.Selected || (values != null && values.Contains(item.Value))),
						() => w.Write(item.Text));
				}
			});
		}
	}

	public class SelectListItem
	{
		public string Text { get; set; }
		public string Value { get; set; }
		public bool Selected { get; set; }

		public SelectListItem()
		{
		}

		public SelectListItem(object text, object value, bool selected)
		{
			Text = text?.ToString();
			Value = value?.ToString();
			Selected = selected;
		}

		public SelectListItem(object text, object value)
		{
			Text = text?.ToString();
			Value = value?.ToString();
		}

		public SelectListItem(object text)
		{
			Text = text?.ToString();
			Value = text?.ToString();
		}
	}

	public static class SelectListItemExtensions
	{
		public static List<SelectListItem> AddEmptyItem(this List<SelectListItem> list)
		{
			list.Insert(0, new SelectListItem());
			return list;
		}

		public static List<SelectListItem> AddEmptyItem(this IEnumerable<SelectListItem> list)
		{
			return list.ToList().AddEmptyItem();
		}
	}
}