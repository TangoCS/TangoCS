using System;
using Tango.Html;
using Tango.Meta;
using Tango.Localization;

namespace Tango.UI
{
	public static class LayoutWriterTablesExtensions
	{
		//public static void ListTable(this LayoutWriter l, Action a)
		//{
		//	l.ListTable(null, a);
		//}

		public static void Th(this LayoutWriter l, Action content)
		{
			l.Th(null, content);
		}
		public static void Th(this LayoutWriter l, Action<ThTagAttributes> attributes, string title)
		{
			l.Th(attributes, () => l.Write(title));
		}

		public static void Th(this LayoutWriter l, string title)
		{
			l.Th(null, () => l.Write(title));
		}

		public static void Th(this LayoutWriter l, Action<ThTagAttributes> attributes, IMetaProperty prop)
		{
			l.Th(attributes, l.Resources.CaptionShort(prop));
		}

		public static void Th(this LayoutWriter l, IMetaProperty prop)
		{
			l.Th(l.Resources.CaptionShort(prop));
		}

		//public static void Td(this LayoutWriter l, Action content)
		//{
		//	l.Td(null, content);
		//}

		//public static void Td(this LayoutWriter l, int content) => l.Td(content, null);
		//public static void Td(this LayoutWriter l, decimal content) => l.Td(content, null);
		//public static void Td(this LayoutWriter l, int? content) => l.Td(content, null);
		//public static void Td(this LayoutWriter l, decimal? content) => l.Td(content, null);

		//public static void Td(this LayoutWriter l, Action<TdTagAttributes> attributes, int content) => l.Td(attributes, content.ToString());
		//public static void Td(this LayoutWriter l, Action<TdTagAttributes> attributes, decimal content) => l.Td(attributes, content.ToString());

		public static void Td<T>(this LayoutWriter l, Action<TdTagAttributes> attributes, T? content)
			where T : struct
		{
			l.Td(attributes, () => l.Write(content?.ToString() ?? "&nbsp;"));
		}

		//public static void GroupTitleCell(this LayoutWriter w, int colSpan, string value)
		//{
		//	w.Td(a => a.Class("ms-gb").ColSpan(colSpan), () => w.Write(value));
		//}

		public static void FormTable(this LayoutWriter w, Action content)
		{
			w.FormTable(null, content);
		}

		public static void GroupTitle(this LayoutWriter w, Action<TagAttributes> attributes, string content)
		{
			w.GroupTitle(() => w.Write(content));
		}

		public static void GroupTitle(this LayoutWriter w, Action content)
		{
			w.GroupTitle(null, content);
		}

		public static void GroupTitle(this LayoutWriter w, string content)
		{
			w.GroupTitle(null, content);
		}

		public static void LabelDefault(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-default"), text);
		}

		public static void LabelSuccess(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-success"), text);
		}

		public static void LabelWarning(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-warning"), text);
		}

		public static void LabelImportant(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-important"), text);
		}

		public static void LabelInfo(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-info"), text);
		}

		public static void LabelInverse(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label label-inverse"), text);
		}

        public static void LabelDanger(this LayoutWriter w, string text)
        {
            w.Span(a => a.Class("label label-danger"), text);
        }
    }
}
