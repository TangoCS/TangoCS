using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Nephrite.Web.Layout
{
	public interface ILayoutInit
	{
		List<string> CSSLinks { get; }
		void RegisterJS();
	}

	public interface ILayoutMain
	{
		string HeaderBegin();
		string HeaderEnd();

		string MainBegin();
		string MainEnd();

		string ContainerBegin();
		string ContainerEnd();

		string ContainerFluidBegin();
		string ContainerFluidEnd();
		
		string SidebarBegin();
		string SidebarEnd();

		string ContentBegin();
		string ContentEnd();

		string ContentHeaderBegin();
		string ContentHeaderEnd();
		string ContentBodyBegin();
		string ContentBodyEnd();

		string FooterBegin();
		string FooterEnd();

		string GridRowBegin();
		string GridRowEnd();

		string GridSpanBegin(int value);
		string GridSpanEnd();
	}

	public interface ILayoutForm
	{
		string FormTableBegin(object attributes);
		string FormRowBegin(string title, string comment, bool required,
			object rowAttributes, object labelAttributes, object requiredAttributes, object commentAttributes, object bodyAttributes);
		string FormRowEnd();
		string FormTableEnd();
	
		string GroupTitleBegin(string id);
		string GroupTitleEnd();

		string ButtonsBarBegin(object attributes);
		string ButtonsBarEnd();
		string ButtonsBarWhiteSpace();
		string ButtonsBarItemBegin();
		string ButtonsBarItemEnd();
	}

	public static class LayoutFormExtensions
	{
		public static string FormRowBegin(this ILayoutForm l, string title)
		{
			return l.FormRowBegin(title, "", false, null, null, null, null, null);
		}
		public static string FormRowBegin(this ILayoutForm l, string title, object attributes)
		{
			return l.FormRowBegin(title, "", false, attributes, null, null, null, null);
		}
		public static string FormRowBegin(this ILayoutForm l, string title, bool required)
		{
			return l.FormRowBegin(title, "", required, null, null, null, null, null);
		}
		public static string FormRowBegin(this ILayoutForm l, string title, bool required, object attributes)
		{
			return l.FormRowBegin(title, "", required, attributes, null, null, null, null);
		}
		public static string FormRowBegin(this ILayoutForm l, string title, string comment, bool required)
		{
			return l.FormRowBegin(title, comment, required, null, null, null, null, null);
		}

		public static string FormRow(this ILayoutForm l, string title, object content)
		{
			return String.Format("{0}{1}{2}", l.FormRowBegin(title), content == null || (content is string && (string)content == "") ? "&nbsp;" : content.ToString(), l.FormRowEnd());
		}

		public static string GroupTitleBegin(this ILayoutForm l)
		{
			return l.GroupTitleBegin("");
		}
		public static string GroupTitle(this ILayoutForm l, string title)
		{
			return String.Format("{0}{1}{2}", l.GroupTitleBegin(""), title, l.GroupTitleEnd());
		}

		public static string FormTableBegin(this ILayoutForm l)
		{
			return l.FormTableBegin(null);
		}
		public static string FormTableBegin(this ILayoutForm l, string width)
		{
			return l.FormTableBegin(new { style = "width:" + width });
		}

		public static string ButtonsBarBegin(this ILayoutForm l)
		{
			return l.ButtonsBarBegin(null);
		}
		public static string ButtonsBarBegin(this ILayoutForm l, string width)
		{
			return l.ButtonsBarBegin(new { style = "width:" + width });
		}
	}

	public interface ILayoutList
	{
		string ListTableBegin(object attributes);
		string ListHeaderBegin(object attributes);
		string THBegin(object attributes);
		string THEnd();
		string ListHeaderEnd();
		string ListRowBegin(string cssClass, object attributes);
		string TDBegin(object attributes);
		string TDEnd();
		string ListRowEnd();
		string ListTableEnd();
	}

	public static class LayoutListExtensions
	{
		public static string ListTableBegin(this ILayoutList l)
		{
			return l.ListTableBegin(null);
		}
		public static string ListHeaderBegin(this ILayoutList l)
		{
			return l.ListHeaderBegin(null);
		}
		public static string ListHeaderBegin(this ILayoutList l, string cssClass)
		{
			return l.ListHeaderBegin(new { Class = cssClass });
		}
		public static string TH(this ILayoutList l, string title)
		{
			return String.Format("{0}{1}{2}", l.THBegin(null), title, l.THEnd());
		}
		public static string TH(this ILayoutList l, string title, object attributes)
		{
			return String.Format("{0}{1}{2}", l.THBegin(attributes), title, l.THEnd());
		}
		public static string THBegin(this ILayoutList l)
		{
			return l.THBegin(null);
		}
		public static string ListRowBegin(this ILayoutList l, string cssClass)
		{
			return l.ListRowBegin(cssClass, null);
		}
		public static string TDBegin(this ILayoutList l)
		{
			return l.TDBegin(null);
		}
		public static string TD(this ILayoutList l, object content)
		{
			return String.Format("{0}{1}{2}", l.TDBegin(null), content == null ? "&nbsp;" : content.ToString(), l.TDEnd());
		}
		public static string TD(this ILayoutList l, object content, object attributes)
		{
			return String.Format("{0}{1}{2}", l.TDBegin(attributes), content == null ? "&nbsp;" : content.ToString(), l.TDEnd());
		}
	}

	public interface ILayoutListRowDrag
	{
		string TDDragHandle(string tableid, string content);
	}

	public static class LayoutListRowDragExtensions
	{
		public static string TDDragHandle(this ILayoutListRowDrag l, string tableid)
		{
			return l.TDDragHandle(tableid, "");
		}
	}

	public enum ToolbarPosition
	{
		FixedTop,
		FixedBottom,
		StaticTop,
		StaticBottom,
		Float
	}
	public enum ToolbarMode
	{
		HeaderBar,
		FormsToolbar
	}
	public enum ToolbarItemsAlign
	{
		Left,
		Right
	}

	public interface ILayoutToolbar
	{
		[Obsolete("Use ToolbarPosition and ToolbarMode properties")]
		string ToolbarBegin(string cssClass);
		string ToolbarBegin(ToolbarPosition? position, ToolbarMode? mode, ToolbarItemsAlign? itemsAlign, ILink titleLink);
		string ToolbarEnd();
		string ToolbarSeparator();
		string ToolbarWhiteSpace();
		string ToolbarLink(string title, string url, string image, string onclick, bool targetBlank);
		string ToolbarImageLink(string title, string url, string image, string onclick, bool targetBlank);
		string ToolbarItem(string content);
		void ToolbarItem(HtmlTextWriter writer, IBarItem content);
	}

	public static class LayoutToolbarExtensions
	{
		public static string ToolbarBegin(this ILayoutToolbar l, ToolbarPosition? position, ToolbarMode? mode, ToolbarItemsAlign? itemsAlign)
		{
			return l.ToolbarBegin(position, mode, itemsAlign, null);
		}
	}

	public interface ILayoutPaging
	{
		string RenderPager(Url baseUrl, int pageIndex, int pageCount, int recordsCount);
		string RenderPager(string gotoPageJSFunction, int pageIndex, int pageCount, int recordsCount);
	}

	public interface ILayoutMessage
	{
		string ExclamationMessage(string str, object attributes);
		string InformationMessage(string str, object attributes);
		string ErrorMessage(string str, object attributes);
		string CustomMessage(string str, string image, object attributes);
	}

	public static class LayoutMessageExtensions
	{
		public static string ExclamationMessage(this ILayoutMessage l, string str)
		{
			return l.ExclamationMessage(str, null);
		}
		public static string InformationMessage(this ILayoutMessage l, string str)
		{
			return l.InformationMessage(str, null);
		}
		public static string ErrorMessage(this ILayoutMessage l, string str)
		{
			return l.ErrorMessage(str, null);
		}
		public static string CustomMessage(this ILayoutMessage l, string str, string image)
		{
			return l.CustomMessage(str, null);
		}
	}

	public interface ILayoutModal
	{
		string ModalBegin(string clientID, string width, string top);
		string ModalEnd();
		string ModalHeaderBegin();
		string ModalHeaderEnd();
		string ModalBodyBegin();
		string ModalBodyEnd();
		string ModalFooterBegin();
		string ModalFooterEnd();
		string ModalFooterLeftBegin();
		string ModalFooterLeftEnd();
		string ModalFooterRightBegin();
		string ModalFooterRightEnd();
	}

	public interface ILayoutPopupMenu
	{
		string PopupMenuBegin();
		string PopupMenuBodyBegin();
		string PopupMenuBodyEnd();

		string PopupMenuLink(ILink link);
		string PopupMenuSeparator();

		string PopupMenuEnd();

	}

	public interface ILayoutSimpleTags
	{
		StringBuilder Link(ILink link);
		StringBuilder ImageLink(ILink link);
		StringBuilder Image(string src, string alt, object attributes);
	}

	public static class LayoutSimpleTagsExtensions
	{
		public static StringBuilder Image(this ILayoutSimpleTags l, string src, string alt)
		{
			return l.Image(src, alt, null);
		}
	}

	public interface ILink
	{
		string Title { get; }
		string Href { get; }
		string Description { get; }
		string Image { get; }
		string OnClick { get; }
		bool TargetBlank { get; }
		//bool ShowDisabled { get; }
		string AccessKey { get; }
		object Attributes { get; }
	}

	public enum ButtonNextOption
	{
		Separator,
		WhiteSpace
	}

	public interface IBarItem
	{
		string Text { get; }
		string Image { get; }
		string OnClientClick { get; }
		string AccessKey { get; }
		ButtonNextOption? Next { get; }
		ILayoutBarItem Layout { get; set; }

		event EventHandler Click;
		void RenderControl(HtmlTextWriter writer);
	}

	public interface ILayoutBarItem
	{
		string CssClass { get; }
		string Style(string image);
	}

	public interface ILayoutLabels
	{
		string Label(string text);
		string LabelSuccess(string text);
		string LabelWarning(string text);
		string LabelImportant(string text);
		string LabelInfo(string text);
		string LabelInverse(string text);
	}

	public static class LayoutLabelsExtensions
	{
		public static string Label(this ILayoutLabels l, string str)
		{
			return l.Label(str);
		}
		public static string LabelImportant(this ILayoutLabels l, string str)
		{
			return l.LabelImportant(str);
		}
		public static string LabelInfo(this ILayoutLabels l, string str)
		{
			return l.LabelInfo(str);
		}
		public static string LabelInverse(this ILayoutLabels l, string str)
		{
			return l.LabelInverse(str);
		}
		public static string LabelSuccess(this ILayoutLabels l, string str)
		{
			return l.LabelSuccess(str);
		}
		public static string LabelWarning(this ILayoutLabels l, string str)
		{
			return l.LabelWarning(str);
		}
	}

	public interface ILayoutAutoMargin
	{
		string MarginBegin();
		string MarginEnd();
	}
}