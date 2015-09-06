using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Text;
using Nephrite.Layout;
using Nephrite.Html;
using Nephrite.Html.Controls;
using Nephrite.Meta;

namespace Nephrite.Razor
{
	public static class RazorLayoutFormExtensions
	{
		public static RazorHtmlHelper.FormTable FormTableBegin(this RazorHtmlHelper l)
		{
			return l.FormTableBegin(null);
		}
		public static RazorHtmlHelper.FormTable FormTableBegin(this RazorHtmlHelper l, string width)
		{
			return l.FormTableBegin(new { style = "width:" + width });
		}

		public static IEncodedString FormRowBegin(this RazorHtmlHelper.FormTable l, string title)
		{
			return l.FormRowBegin(title, "", false, null, null, null, null, null);
		}
		public static IEncodedString FormRowBegin(this RazorHtmlHelper.FormTable l, string title, object attributes)
		{
			return l.FormRowBegin(title, "", false, attributes, null, null, null, null);
		}
		public static IEncodedString FormRowBegin(this RazorHtmlHelper.FormTable l, string title, bool required)
		{
			return l.FormRowBegin(title, "", required, null, null, null, null, null);
		}
		public static IEncodedString FormRowBegin(this RazorHtmlHelper.FormTable l, string title, bool required, object attributes)
		{
			return l.FormRowBegin(title, "", required, attributes, null, null, null, null);
		}
		public static IEncodedString FormRowBegin(this RazorHtmlHelper.FormTable l, string title, string comment, bool required)
		{
			return l.FormRowBegin(title, comment, required, null, null, null, null, null);
		}

		public static IEncodedString FormRow(this RazorHtmlHelper.FormTable l, string title, object content)
		{
			return new RawString(String.Format("{0}{1}{2}", 
				l.FormRowBegin(title), 
				content == null || (content is string && (string)content == "") ? "&nbsp;" : content.ToString(), 
				l.FormRowEnd()));
		}

		public static IEncodedString FormRow(this RazorHtmlHelper.FormTable l, MetaProperty prop, object content)
		{
			return new RawString(String.Format("{0}{1}{2}",
				l.FormRowBegin(prop.Caption),
				content == null || (content is string && (string)content == "") ? "&nbsp;" : content.ToString(),
				l.FormRowEnd()));
		}

		public static IEncodedString GroupTitleBegin(this RazorHtmlHelper l)
		{
			return l.GroupTitleBegin("");
		}
		public static IEncodedString GroupTitle(this RazorHtmlHelper l, string title)
		{
			return new RawString(String.Format("{0}{1}{2}", l.GroupTitleBegin(""), title, l.GroupTitleEnd()));
		}

		public static RazorHtmlHelper.ButtonsBar ButtonsBarBegin(this RazorHtmlHelper l)
		{
			return l.ButtonsBarBegin(null);
		}
		public static RazorHtmlHelper.ButtonsBar ButtonsBarBegin(this RazorHtmlHelper l, string width)
		{
			return l.ButtonsBarBegin(new { style = "width:" + width });
		}
	}

	public static class RazorLayoutListExtensions
	{
		public static RazorHtmlHelper.ListTable ListTableBegin(this RazorHtmlHelper l)
		{
			return l.ListTableBegin(null);
		}
		public static IEncodedString ListHeaderBegin(this RazorHtmlHelper.ListTable l)
		{
			return l.ListHeaderBegin(null);
		}
		public static IEncodedString ListHeaderBegin(this RazorHtmlHelper.ListTable l, string cssClass)
		{
			return l.ListHeaderBegin(new { Class = cssClass });
		}
		public static IEncodedString TH(this RazorHtmlHelper.ListTable l, string title)
		{
			return new RawString(String.Format("{0}{1}{2}", l.THBegin(null), title, l.THEnd()));
		}
		public static IEncodedString TH(this RazorHtmlHelper.ListTable l, string title, object attributes)
		{
			return new RawString(String.Format("{0}{1}{2}", l.THBegin(attributes), title, l.THEnd()));
		}
		public static IEncodedString THBegin(this RazorHtmlHelper.ListTable l)
		{
			return l.THBegin(null);
		}
		public static IEncodedString ListRowBegin(this RazorHtmlHelper.ListTable l, string cssClass)
		{
			return l.ListRowBegin(cssClass, null);
		}
		public static IEncodedString TDBegin(this RazorHtmlHelper.ListTable l)
		{
			return l.TDBegin(null);
		}
		public static IEncodedString TD(this RazorHtmlHelper.ListTable l, object content)
		{
			return new RawString(String.Format("{0}{1}{2}", l.TDBegin(null), content == null ? "&nbsp;" : content.ToString(), l.TDEnd()));
		}
		public static IEncodedString TD(this RazorHtmlHelper.ListTable l, object content, object attributes)
		{
			return new RawString(String.Format("{0}{1}{2}", l.TDBegin(attributes), content == null ? "&nbsp;" : content.ToString(), l.TDEnd()));
		}
	}
}
