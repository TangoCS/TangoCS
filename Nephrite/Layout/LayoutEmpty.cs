using System;
using System.Collections.Generic;
using System.Linq;
using Nephrite.Html;
using Nephrite.Http;

namespace Nephrite.Layout
{
	public class ToolbarEmpty : ILayoutToolbar
	{
		public ToolbarPosition Position { get; set; }
		public ToolbarMode Mode { get; set; }

		public string ToolbarBegin(string cssClass)
		{
			return "";
		}
		public string ToolbarBegin(ToolbarPosition? position, ToolbarMode? mode, ToolbarItemsAlign? itemsAlign, ILink titleLink)
		{
			return "";
		}

		public string ToolbarEnd()
		{
			return "";
		}

		public string ToolbarSeparator()
		{
			return "";
		}

		public string ToolbarWhiteSpace()
		{
			return "";
		}

		public string ToolbarLink(string title, string url, string image, string onclick, bool targetBlank)
		{
			return "";
		}

		public string ToolbarImageLink(string title, string url, string image, string onclick, bool targetBlank)
		{
			return "";
		}

		public string ToolbarItem(string content)
		{
			return "";
		}


		//public void ToolbarItem(HtmlTextWriter writer, IBarItem content)
		//{
			
		//}


	}

	public class PagingEmpty : ILayoutPaging
	{
		public string RenderPager(Url baseUrl, int pageIndex, int pageCount, int recordsCount)
		{
			return "";
		}

		public string RenderPager(string gotoPageJSFunction, int pageIndex, int pageCount, int recordsCount)
		{
			return "";
		}

		public string RenderPager(string gotoPageJSFunction, int pageIndex, int pageCount)
		{
			return "";
		}
	}

	public class ListRowDragEmpty : ILayoutListRowDrag
	{
		public string TDDragHandle(string tableid, string content)
		{
			return "<td></td>";
		}
	}
}