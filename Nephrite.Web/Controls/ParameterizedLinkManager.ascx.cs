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
    public partial class ParameterizedLinkManager : System.Web.UI.UserControl
    {
        public event EventHandler Click;

        public string Value
        {
            get
            {
                return hArg.Value;
            }
			set
			{
				hArg.Value = value;
			}
        }

        protected void OnClick(object sender, EventArgs e)
        {
            if (Click != null) Click(sender, e);
        }

        public string RenderLink(string text, object arg)
        {
            return String.Format("<a href='#' onclick=\"{0}_click('{1}'); return false;\">{2}</a>", ClientID, arg, text);
        }

        public string RenderImage(string image, string text, object arg)
        {
            return String.Format("<a href='#' onclick=\"{0}_click('{1}'); return false;\"><img src='" + Settings.ImagesPath + "{2}' alt='{3}' class='middle'/></a>", ClientID, arg, image, text);
        }

		public string RenderImageConfirm(string image, string text, string confirmString, object arg)
		{
            return String.Format("<a href='#' onclick=\"if (confirm('{4}')) {{ {0}_click('{1}'); return false; }}\"><img src='" + Settings.ImagesPath + "{2}' alt='{3}' class='middle'/></a>", ClientID, arg, image, text, confirmString);
		}
    }
}