using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Nephrite.Web.Controls
{
	public partial class ToolbarPopupMenuLarge : PopupMenu
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			
		}
		protected override void Render(HtmlTextWriter writer)
		{
			bool v = false;
			if (Items != null)
			{
				foreach (PopupMenuItem item in Items)
				{
					if (item is PopupMenuMethod)
					{
						PopupMenuMethod pmm = item as PopupMenuMethod;
						if ((!String.IsNullOrEmpty(pmm.URL) && pmm.URL != "#") || !String.IsNullOrEmpty(pmm.OnClick) || pmm.ShowDisabled)
						{
							v = true;
						}
					}
				}
			}

			if (v) base.Render(writer);
		}
	}
}