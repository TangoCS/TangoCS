using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Nephrite.TextResources;
using Nephrite.Web.Layout;
using System.IO;

namespace Nephrite.Web.Controls
{
	public class BackButton : Control//, IBarItem
    {
		public ILayoutBarItem Layout { get; set; }
		public string AccessKey { get; set; }
		public string Image { get; set; }

		public BackButton()
		{
			Layout = AppLayout.Current.Button;
		}

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Url = Query.GetReturnUrl();
        }

        protected override void Render(HtmlTextWriter writer)
        {
			if (Text.IsEmpty()) Text = TextResource.Get("Common.Buttons.Back", "Назад");
			writer.Write(String.Format(@"<input class=""{2}"" type=""button"" value=""{0}"" onClick=""{1}"" style=""{3}"" />", Text, OnClientClick, Layout.CssClass, Layout.Style(Image)));
        }

        public string Url { get; set; }
		public string Text { get; set; }
		//public ButtonNextOption? Next { get; set; }
		
		public string OnClientClick
		{
			get 
			{				
				if (String.IsNullOrEmpty(Url))
					return "history.back();return false;";
				else
					return "document.location='" + Url + "';return false;"; 
			}
		}

		public event EventHandler Click;

	}
}
