using System;
using System.Collections.Generic;
using System.Linq;
using Tango.Html;
using Tango.Meta;
using Tango.Localization;

namespace Tango.UI.Controls
{
	public static class MultilingualFieldExtensions
	{
		public static void MultilingualTextBox<T, TValue>(this HtmlWriter w, string name, ILanguage lang, List<T> objects, Func<T, TValue> valueFunc = null, Action<InputTagAttributes> attributes = null)
			where T : IMultilingual
		{
			w.Table(a => a.Style("width:100%"), () => {
				foreach (var l in lang.List)
				{
					string value = null;
					if (valueFunc != null && objects != null && objects.Count > 0)
						value = objects.Where(o => o.LanguageCode == l.Code).Select(valueFunc).FirstOrDefault()?.ToString();
					w.Tr(() => {
						w.Td(a => a.Style("width:10px; vertical-align:middle"), () => w.Write(l.Code.ToUpper()));
						w.Td(() => w.TextBox(name + "_" + l.Code.ToLower(), value, attributes));
					});
				}
			});
		}

		public static void FormFieldMultilingualTextBox<T, TValue>(this LayoutWriter w, MetaAttribute<T, TValue> prop, ILanguage lang, List<T> objects, Grid grid = Grid.OneWhole)
			where T : IMultilingual
		{
			w.FormField(prop.Name, w.Resources.Caption(prop), 
				() => w.MultilingualTextBox(prop.Name, lang, objects, prop.GetValue, a => a.Style("width:100%")),
				grid, prop.IsRequired, w.Resources.Description(prop)
			);
		}
	}
}
