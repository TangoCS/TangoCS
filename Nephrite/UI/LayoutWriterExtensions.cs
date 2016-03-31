using System;
using Nephrite.Html;
using Nephrite.Meta;
using Nephrite.Localization;

namespace Nephrite.UI
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
			l.ColumnHeader(attributes, prop.CaptionShort(l.TextResource));
		}

		public static void ColumnHeader(this LayoutWriter l, IMetaProperty prop)
		{
			l.ColumnHeader(prop.CaptionShort(l.TextResource));
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
	}
}
