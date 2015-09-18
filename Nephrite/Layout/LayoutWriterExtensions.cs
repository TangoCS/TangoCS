using System;
using System.Collections;
using System.Collections.Generic;
using Nephrite.Meta;

namespace Nephrite.Layout
{
	public static class LayoutFormWriterExtensions
	{
		public static void FormTable(this LayoutWriter l, Action<LayoutWriter.FormTableWriter> a)
		{
			l.FormTable(null, a);
		}
		public static void FormTable(this LayoutWriter l, string width, Action<LayoutWriter.FormTableWriter> a)
		{
			l.FormTable(new { style = "width:" + width }, a);
		}

		public static void FormRowBegin(this LayoutWriter.FormTableWriter l, string title)
		{
			l.FormRowBegin(title, "", false, null, null, null, null, null);
		}
		public static void FormRowBegin(this LayoutWriter.FormTableWriter l, string title, object attributes)
		{
			l.FormRowBegin(title, "", false, attributes, null, null, null, null);
		}
		public static void FormRowBegin(this LayoutWriter.FormTableWriter l, string title, bool required)
		{
			l.FormRowBegin(title, "", required, null, null, null, null, null);
		}
		public static void FormRowBegin(this LayoutWriter.FormTableWriter l, string title, bool required, object attributes)
		{
			l.FormRowBegin(title, "", required, attributes, null, null, null, null);
		}
		public static void FormRowBegin(this LayoutWriter.FormTableWriter l, string title, string comment, bool required)
		{
			l.FormRowBegin(title, comment, required, null, null, null, null, null);
		}

		public static void FormRow(this LayoutWriter.FormTableWriter l, string title, object content)
		{
			l.FormRowBegin(title);
			l.Writer.Write(content == null || (content is string && (string)content == "") ? "&nbsp;" : content.ToString());
			l.FormRowEnd();
		}

		public static void FormRow(this LayoutWriter.FormTableWriter l, MetaProperty prop, object content)
		{
			l.FormRowBegin(prop.Caption);
			l.Writer.Write(content == null || (content is string && (string)content == "") ? "&nbsp;" : content.ToString());
			l.FormRowEnd();
		}

		public static void GroupTitleBegin(this LayoutWriter l)
		{
			l.GroupTitleBegin("");
		}
		public static void GroupTitle(this LayoutWriter l, string title)
		{
			l.GroupTitleBegin("");
			l.Write(title);
			l.GroupTitleEnd();
		}

		public static void ButtonsBar(this LayoutWriter l, Action<LayoutWriter.ButtonsBarWriter> a)
		{
			l.ButtonsBar(null, a);
		}
		public static void ButtonsBar(this LayoutWriter l, string width, Action<LayoutWriter.ButtonsBarWriter> a)
		{
			l.ButtonsBar(new { style = "width:" + width }, a);
		}
	}

	public static class LayoutListWriterExtensions
	{
		public static void ListTable(this LayoutWriter l, Action<LayoutWriter.ListTableWriter> a)
		{
			l.ListTable(null, a);
		}
		public static void ListHeader(this LayoutWriter.ListTableWriter l, Action columns)
		{
			l.ListHeader(null, columns);
		}
		public static void ListHeader(this LayoutWriter.ListTableWriter l, string cssClass, Action columns)
		{
			l.ListHeader(new { Class = cssClass }, columns);
		}
		public static void TH(this LayoutWriter.ListTableWriter l, string title)
		{
			l.THBegin(null);
			l.Writer.Write(title);
			l.THEnd();
		}
		public static void TH(this LayoutWriter.ListTableWriter l, string title, object attributes)
		{
			l.THBegin(attributes);
			l.Writer.Write(title);
			l.THEnd();
		}
		public static void ListRow(this LayoutWriter.ListTableWriter l, string cssClass, Action content)
		{
			l.ListRowBegin(cssClass, null);
			content();
			l.ListRowEnd();
        }
		public static void TD(this LayoutWriter.ListTableWriter l, object content)
		{
			l.TDBegin(null);
			l.Writer.Write(content == null ? "&nbsp;" : content.ToString());
			l.TDEnd();
		}
		public static void TD(this LayoutWriter.ListTableWriter l, Action content)
		{
			l.TDBegin(null);
			content();
            l.TDEnd();
		}
		public static void TD(this LayoutWriter.ListTableWriter l, object attributes, Action content)
		{
			l.TDBegin(attributes);
			content();
			l.TDEnd();
		}
		public static void TD(this LayoutWriter.ListTableWriter l, object attributes, object content)
		{
			l.TDBegin(attributes);
			l.Writer.Write(content == null ? "&nbsp;" : content.ToString());
			l.TDEnd();
		}
	}
}
