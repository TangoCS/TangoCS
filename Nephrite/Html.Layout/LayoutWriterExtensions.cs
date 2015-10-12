using System;
using System.Collections;
using System.Collections.Generic;
using Nephrite.Meta;

namespace Nephrite.Html.Layout
{
	public static class LayoutFormWriterExtensions
	{
		public static void ButtonsBar(this LayoutWriter l, Action<LayoutWriter.ButtonsBarWriter> a)
		{
			l.ButtonsBar(null, a);
		}
	}

	public static class LayoutListWriterExtensions
	{
		public static void ListTable(this LayoutWriter l, Action<LayoutWriter.ListTableWriter> a)
		{
			l.ListTable(null, a);
		}
		public static void ListHeader(this LayoutWriter.ListTableWriter l, Action content)
		{
			l.ListHeader(null, content);
		}
		public static void ColumnHeader(this LayoutWriter.ListTableWriter l, Action content)
		{
			l.ColumnHeader(null, content);
		}
		public static void ColumnHeader(this LayoutWriter.ListTableWriter l, Action<ThTagAttributes> attributes, string title)
		{
			l.ColumnHeader(attributes, () => l.Writer.Write(title));
		}
		public static void ColumnHeader(this LayoutWriter.ListTableWriter l, string title)
		{
			l.ColumnHeader(null, () => l.Writer.Write(title));
		}

		public static void ColumnHeader(this LayoutWriter.ListTableWriter l, Action<ThTagAttributes> attributes, MetaProperty prop)
		{
			l.ColumnHeader(attributes, prop.CaptionShort);
		}

		public static void ColumnHeader(this LayoutWriter.ListTableWriter l, MetaProperty prop)
		{
			l.ColumnHeader(prop.CaptionShort);
		}

		public static void Cell(this LayoutWriter.ListTableWriter l, object content)
		{
			l.Cell(null, () => l.Writer.Write(content == null ? "&nbsp;" : content.ToString()));
		}

		public static void Cell(this LayoutWriter.ListTableWriter l, Action content)
		{
			l.Cell(null, content);
		}

		public static void Cell(this LayoutWriter.ListTableWriter l, Action<TdTagAttributes> attributes, object content)
		{
			l.Cell(attributes, () => l.Writer.Write(content == null ? "&nbsp;" : content.ToString()));
		}
	}

	public static class FormTableExtensions
	{
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

		public static void FormRow(this LayoutWriter w, string title, string comment, bool required, Action content)
		{
			w.FormRow(null, title, comment, required, content);
		}
		public static void FormRow(this LayoutWriter w, string title, string comment, bool required, object content)
		{
			w.FormRow(null, title, comment, required, content);
		}

		public static void FormRow(this LayoutWriter w, string title, bool required, Action content)
		{
			w.FormRow(title, "", required, content);
		}
		public static void FormRow(this LayoutWriter w, string title, Action content)
		{
			w.FormRow(title, "", false, content);
		}

		public static void FormRow(this LayoutWriter w, string title, object content)
		{
			w.FormRow(title, "", false, content);
		}

		public static void FormRow(this LayoutWriter w, MetaProperty prop, object content)
		{
			w.FormRow(prop.Caption, content);
		}

		public static void FormRow(this LayoutWriter w, MetaAttribute attr, string value)
		{
			w.FormRow(attr.Caption, value);
		}

		public static void FormRow<T>(this LayoutWriter w, MetaAttribute attr, T model)
		{
			w.FormRow(attr.Caption, attr.GetStringValue(model));
		}
	}
}
