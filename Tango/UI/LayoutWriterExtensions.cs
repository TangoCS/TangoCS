using System;
using Tango.Html;
using Tango.Meta;
using Tango.Localization;

namespace Tango.UI
{
	public static class LayoutWriterTablesExtensions
	{
		public static void ListTable(this LayoutWriter l, Action a)
		{
			l.ListTable(null, a);
		}
		public static void ListHeader(this LayoutWriter l, Action content)
		{
			l.ListHeader(null, content);
		}
		public static void ColumnHeader(this LayoutWriter l, Action content)
		{
			l.ColumnHeader(null, content);
		}
		public static void ColumnHeader(this LayoutWriter l, Action<ThTagAttributes> attributes, string title)
		{
			l.ColumnHeader(attributes, () => l.Write(title));
		}

		public static void ColumnHeader(this LayoutWriter l, string title)
		{
			l.ColumnHeader(null, () => l.Write(title));
		}

		public static void ColumnHeader(this LayoutWriter l, Action<ThTagAttributes> attributes, IMetaProperty prop)
		{
			l.ColumnHeader(attributes, l.TextResource.CaptionShort(prop));
		}

		public static void ColumnHeader(this LayoutWriter l, IMetaProperty prop)
		{
			l.ColumnHeader(l.TextResource.CaptionShort(prop));
		}

		public static void Cell(this LayoutWriter l, object content)
		{
			l.Cell(null, () => l.Write(content == null ? "&nbsp;" : content.ToString()));
		}

		public static void Cell(this LayoutWriter l, Action content)
		{
			l.Cell(null, content);
		}

		public static void Cell(this LayoutWriter l, Action<TdTagAttributes> attributes, object content)
		{
			l.Cell(attributes, () => l.Write(content == null ? "&nbsp;" : content.ToString()));
		}

		public static void GroupTitleCell(this LayoutWriter w, int colSpan, string value)
		{
			w.Td(a => a.Class("ms-gb").ColSpan(colSpan), () => w.Write(value));
		}

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

		public static void ButtonsBar(this LayoutWriter l, Action a)
		{
			l.ButtonsBar(null, a);
		}

		public static void LabelDefault(this LayoutWriter w, string text)
		{
			w.Span(a => a.Class("label"), text);
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
	}
}
