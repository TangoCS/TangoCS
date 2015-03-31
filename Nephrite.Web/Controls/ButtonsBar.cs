using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;


namespace Nephrite.Web.Controls
{
	//[ParseChildren(typeof(IBarItem))]
	//public class ButtonBar : System.Web.UI.Control //, INamingContainer
	//{
	//	public ILayoutToolbar Layout { get; set; }

	//	public ILink TitleLink { get; set; }
	//	public ToolbarPosition? Position { get; set; }
	//	public ToolbarMode? Mode { get; set; }
	//	public ToolbarItemsAlign? ItemsAlign { get; set; }

	//	public ButtonBar()
	//	{
	//		Layout = AppLayout.Current.ButtonBar;
	//		//ItemsAlign = ToolbarItemsAlign.Right;
	//	}

	//	public void AddButton(string text, EventHandler onClick, ButtonNextOption? nextOption = null)
	//	{
	//		SimpleButton b = new SimpleButton();
	//		b.Text = text;
	//		b.Click += onClick;
	//		b.Next = nextOption;
	//		Controls.Add(b);
	//	}
	//	public void AddBackButton(ButtonNextOption? nextOption = null)
	//	{
	//		BackButton b = new BackButton();
	//		b.Next = nextOption;
	//		Controls.Add(b);
	//	}

	//	protected override void Render(HtmlTextWriter writer)
	//	{
	//		writer.Write(Layout.ToolbarBegin(Position ?? ToolbarPosition.StaticTop, Mode ?? ToolbarMode.FormsToolbar, ItemsAlign, TitleLink));

	//		foreach (Control c in Controls)
	//		{
	//			IBarItem item = c as IBarItem;
	//			if (item == null) continue;

	//			writer.Write(Layout.ToolbarItem(item));
				
	//			if (item.Next == ButtonNextOption.Separator) writer.Write(Layout.ToolbarSeparator());
	//			if (item.Next == ButtonNextOption.WhiteSpace) writer.Write(Layout.ToolbarWhiteSpace());
	//		}

	//		writer.Write(Layout.ToolbarEnd());
	//	}
	//}
}