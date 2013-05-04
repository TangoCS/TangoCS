using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Nephrite.Meta;

namespace Nephrite.Web.Controls
{
	public class ToolbarPopupMenuCompact : PopupMenu
	{
		public ILayoutPopupMenu Layout { get; set; }

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
			if (Layout == null) Layout = AppWeb.Layout.ToolbarDropdownCompact;

			if (Items != null && Items.Count > 0)
			{
				writer.Write(Layout.PopupMenuBegin());
				writer.Write(Title);
				writer.Write(Layout.PopupMenuEnd());

				writer.Write(Layout.PopupMenuBodyBegin());
				foreach (IPopupMenuItem item in Items)
				{
					if (item is PopupMenuLink)
					{
						PopupMenuLink pmm = item as PopupMenuLink;
						if ((!String.IsNullOrEmpty(pmm.Href) && pmm.Href != "#") || !String.IsNullOrEmpty(pmm.OnClick))
						{
							writer.Write(Layout.PopupMenuLink(pmm));
						}
					}
					if (item is PopupMenuSeparator)
						writer.Write(Layout.PopupMenuSeparator());
				}
				writer.Write(Layout.PopupMenuBodyEnd());
			}
		}
	}
}