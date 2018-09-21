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
			w.Div(a => a.Class("radiobuttonlist").ID(name + "_placeholder").Set(attributes), () => {
				int i = 0;
				if (items != null)
					foreach (var item in items)
					{
						w.Label(a => a.For(name + i.ToString()), () => {
							w.RadioButton(name, name + i.ToString(), item.Value, item.Selected || item.Value == value, itemAttributes?.Invoke(item));
							w.Write(item.Text);
						});
						i++;
					}
			});
		}

		public static void CheckBoxList(this IHtmlWriter w, string name, string[] value, IEnumerable<SelectListItem> items, Action<TagAttributes> attributes = null, Func<SelectListItem, Action<InputTagAttributes>> itemAttributes = null)
		{
			w.Div(a => a.Class("checkboxlist").ID(name + "_placeholder").Set(attributes), () => {
				int i = 0;
				if (items != null)
					foreach (var item in items)
					{
						w.Label(a => a.For(name + i.ToString()), () => {
							w.CheckBox(new InputName { Name = name + "[]", ID = name + i.ToString() }, item.Value, item.Selected || (value != null && value.Contains(item.Value)), itemAttributes?.Invoke(item));
							w.Write(item.Text);
						});
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
}