using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using Nephrite.Meta;

namespace Nephrite.Web.Controls
{
	public class PopupMenu : UserControl
	{
		public string Title { get; set; }
		public List<IPopupMenuItem> Items { get; set; }

		public PopupMenuLink AddItem(string title, string url, string image, string description)
        {
            return AddItem(title, url, image, description, false);
        }

		public PopupMenuLink AddItem(string title, string url, string image, string description, bool targetBlank)
        {
            if (url == "#")
                return new PopupMenuLink();
            if (Items == null) Items = new List<IPopupMenuItem>();
			var pmm = new PopupMenuLink { Description = description, Title = title, Href = url, Image = image, TargetBlank = targetBlank };
            Items.Add(pmm);
			return pmm;
        }
		public PopupMenuLink AddItem(string title, string url, string image)
		{
			return AddItem(title, url, image, "");
		}
		public PopupMenuLink AddItem(string title, string url)
		{
			return AddItem(title, url, "", "");
		}

		public PopupMenuLink AddItemJS(string title, string onClick, string image, string description)
		{
			if (Items == null) Items = new List<IPopupMenuItem>();
			var pmm = new PopupMenuLink { Description = description, Title = title, OnClick = onClick, Image = image };
			Items.Add(pmm);
			return pmm;
		}
		public PopupMenuLink AddItemJS(string title, string onClick, string image)
		{
			return AddItemJS(title, onClick, image, "");
		}
		public PopupMenuLink AddItemJS(string title, string onClick)
		{
			return AddItemJS(title, onClick, "", "");
		}

		public void AddSeparator()
		{
			if (Items == null) Items = new List<IPopupMenuItem>();
			Items.Add(new PopupMenuSeparator());
		}
	}

	public interface IPopupMenuItem
	{
	}

	public class PopupMenuSeparator : IPopupMenuItem
	{
	}

	public class PopupMenuLink : SimpleLink, IPopupMenuItem
	{
		public bool ShowDisabled { get; set; }
	}
}
