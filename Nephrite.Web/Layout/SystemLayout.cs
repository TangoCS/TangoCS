using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Web.Layout
{
	public class AppLayout : ILayoutList, ILayoutForm,
		ILayoutListRowDrag, ILayoutMessage, ILayoutPaging, ILayoutSimpleTags, ILayoutLabels
	{
		public List<ILayoutInit> Init { get; set; }
		public ILayoutList List { get; set; }
		public ILayoutForm Form { get; set; }
		public ILayoutListRowDrag ListRowDrag { get; set; }
		public ILayoutToolbar Toolbar { get; set; }
		public ILayoutToolbar ButtonBar { get; set; }
		public ILayoutMessage Message { get; set; }
		public ILayoutPaging Paging { get; set; }
		public ILayoutModal Modal { get; set; }
		public ILayoutPopupMenu ToolbarDropdownCompact { get; set; }
		public ILayoutPopupMenu ToolbarDropdownLarge { get; set; }
		public ILayoutPopupMenu ButtonDropdown { get; set; }
		public ILayoutPopupMenu SplitButtonDropdown { get; set; }
		public ILayoutSimpleTags SimpleTags { get; set; }
		public ILayoutLabels Labels { get; set; }
		public ILayoutAutoMargin AutoMargin { get; set; }
		public ILayoutBarItem Button { get; set; }
		public ILayoutBarItem ToolbarButton { get; set; }

		static AppLayout _layout = null;

		public static AppLayout Current
		{
			get
			{
				if (_layout == null)
				{
					AppLayout sl = new AppLayout();
					sl.List = new ListWSS2007();
					sl.ListRowDrag = new ListRowDragWSS2007();
					sl.Message = new MessageWSS2007();
					sl.Paging = new PagingWSS2007();
					sl.Form = new FormWSS2007();
					sl.Toolbar = new ToolbarWSS2007();
					sl.ButtonBar = new ButtonsBarWSS2007();
					sl.Modal = new WinModal();
					sl.ToolbarDropdownCompact = new PopupMenuWSS2007Compact();
					sl.ToolbarDropdownLarge = new PopupMenuWSS2007Large();
					sl.ButtonDropdown = new PopupMenuWSS2007Compact();
					sl.SimpleTags = new SimpleTags();
					sl.Labels = new LabelsBootstrap();
					sl.AutoMargin = new AutoMarginWSS2007();
					sl.Button = new ButtonWSS2007();
					sl.ToolbarButton = new ToolbarButtonWSS2007();

					_layout = sl;
				}
				return _layout;
			}
		}

		public string ListTableBegin(object attributes)
		{
			return List.ListTableBegin(attributes);
		}

		public string ListHeaderBegin(object attributes)
		{
			return List.ListHeaderBegin(attributes);
		}

		public string THBegin(object attributes)
		{
			return List.THBegin(attributes);
		}

		public string THEnd()
		{
			return List.THEnd();
		}

		public string ListHeaderEnd()
		{
			return List.ListHeaderEnd();
		}

		public string ListRowBegin(string cssClass, object attributes)
		{
			return List.ListRowBegin(cssClass, attributes);
		}

		public string TDBegin(object attributes)
		{
			return List.TDBegin(attributes);
		}

		public string TDEnd()
		{
			return List.TDEnd();
		}

		public string ListRowEnd()
		{
			return List.ListRowEnd();
		}

		public string ListTableEnd()
		{
			return List.ListTableEnd();
		}

		public string FormTableBegin(object attributes)
		{
			return Form.FormTableBegin(attributes);
		}

		public string FormRowBegin(string title, string comment, bool required,
			object rowAttributes, object labelAttributes, object requiredAttributes, object commentAttributes, object bodyAttributes)
		{
			return Form.FormRowBegin(title, comment, required, rowAttributes, labelAttributes, requiredAttributes, commentAttributes, bodyAttributes);
		}

		public string FormRowEnd()
		{
			return Form.FormRowEnd();
		}

		public string FormTableEnd()
		{
			return Form.FormTableEnd();
		}


		public string GroupTitleBegin(string id)
		{
			return Form.GroupTitleBegin(id);
		}

		public string GroupTitleEnd()
		{
			return Form.GroupTitleEnd();
		}

		public string ButtonsBarBegin(object attributes)
		{
			return Form.ButtonsBarBegin(attributes);
		}

		public string ButtonsBarEnd()
		{
			return Form.ButtonsBarEnd();
		}

		public string ButtonsBarWhiteSpace()
		{
			return Form.ButtonsBarWhiteSpace();
		}

		public string ButtonsBarItemBegin()
		{
			return Form.ButtonsBarItemBegin();
		}

		public string ButtonsBarItemEnd()
		{
			return Form.ButtonsBarItemEnd();
		}

		public string TDDragHandle(string tableid, string content)
		{
			return ListRowDrag.TDDragHandle(tableid, content);
		}

		public string ExclamationMessage(string str, object attributes)
		{
			return Message.ExclamationMessage(str, attributes);
		}
		public string InformationMessage(string str, object attributes)
		{
			return Message.InformationMessage(str, attributes);
		}
		public string ErrorMessage(string str, object attributes)
		{
			return Message.ErrorMessage(str, attributes);
		}
		public string CustomMessage(string str, string image, object attributes)
		{
			return Message.CustomMessage(str, image, attributes);
		}

		public string RenderPager(Url baseUrl, int pageIndex, int pageCount, int recordsCount)
		{
			return Paging.RenderPager(baseUrl, pageIndex, pageCount, recordsCount);
		}

		public string RenderPager(string gotoPageJSFunction, int pageIndex, int pageCount, int recordsCount)
		{
			return Paging.RenderPager(gotoPageJSFunction, pageIndex, pageCount, recordsCount);
		}

		public StringBuilder Link(ILink link)
		{
			return SimpleTags.Link(link);
		}

		public StringBuilder ImageLink(ILink link)
		{
			return SimpleTags.ImageLink(link);
		}

		public StringBuilder Image(string src, string alt, object attributes)
		{
			return SimpleTags.Image(src, alt, attributes);
		}

		public string Label(string text)
		{
			return Labels.Label(text);
		}

		public string LabelSuccess(string text)
		{
			return Labels.LabelSuccess(text);
		}

		public string LabelWarning(string text)
		{
			return Labels.LabelWarning(text);
		}

		public string LabelImportant(string text)
		{
			return Labels.LabelImportant(text);
		}

		public string LabelInfo(string text)
		{
			return Labels.LabelInfo(text);
		}

		public string LabelInverse(string text)
		{
			return Labels.LabelInverse(text);
		}
	}
}