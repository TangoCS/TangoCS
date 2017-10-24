using System;
using System.Collections.Generic;
using System.Linq;

namespace Tango.Html
{
	public static class HtmlWriterSelectExtensions
	{
		public static void DropDownList(this IHtmlWriter w, InputName name, string value, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			w.WriteTag<SelectTagAttributes>("select", a => a.Name(name.Name).ID(name.ID).Set(attributes), () => {
				if (items == null) return;
				foreach (var item in items)
				{
					w.WriteTag<OptionTagAttributes>("option", 
						oa => oa.Value(item.Value).Selected(item.Selected || item.Value == value), 
						() => w.Write(item.Text));
				}
			});
		}

		public static void RadioButtonList(this IHtmlWriter w, string name, string value, IEnumerable<SelectListItem> items, Action<TagAttributes> attributes = null, Func<SelectListItem, Action<InputTagAttributes>> itemAttributes = null)
		{
			if (items == null) return;
			w.Div(a => a.Class("radiobuttonlist").Set(attributes), () => {
				int i = 0;
				foreach (var item in items)
				{
					w.RadioButton(name, name + i.ToString(), item.Value, item.Selected || item.Value == value, itemAttributes?.Invoke(item));
					w.Label(name + i.ToString(), item.Text);
					i++;
				}
			});
		}

		public static void CheckBoxList(this IHtmlWriter w, string name, string[] value, IEnumerable<SelectListItem> items, Action<TagAttributes> attributes = null)
		{
			if (items == null) return;
			w.Div(a => a.Class("checkboxlist").Set(attributes), () => {
				int i = 0;
				foreach (var item in items)
				{
					w.CheckBox(new InputName { Name = name + "[]", ID = name + i.ToString() }, item.Value, item.Selected || (value != null && value.Contains(item.Value)));
					w.Label(name + i.ToString(), item.Text);
					i++;
				}
			});
		}

		public static void ListBox(this IHtmlWriter w, InputName name, int size, IEnumerable<string> values, IEnumerable<SelectListItem> items, Action<SelectTagAttributes> attributes = null)
		{
			w.WriteTag<SelectTagAttributes>("select", a => a.Name(name.Name).ID(name.ID).Size(size).Set(attributes), () => {
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