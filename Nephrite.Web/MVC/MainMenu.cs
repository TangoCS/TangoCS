using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI;
using System.Drawing;
using Nephrite.Web;

using Nephrite.Web.App;
using System.Threading;

namespace Nephrite.Web
{
    public class MainMenu : WebPart
    {
        Exception error = null;
        public MainMenu() : base()
        {
            AscxFileName = "MainMenu.ascx";
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

			


			ChromeType = PartChromeType.None;

            try
            {
                Controls.Add(Page.LoadControl("~/_controltemplates/Tessera/" + AscxFileName));
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception e)
            {
                error = e;
                ErrorLogger.Log(e);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (error != null)
            {
                error.Render(writer);
            }
            else
            {
                try
                {
                    base.RenderContents(writer);
                }
				catch (ThreadAbortException)
				{

				}
				catch (Exception e)
                {
                    ErrorLogger.Log(e);
                    e.Render(writer);
                }
            }
        }

        public string AscxFileName { get; set; }
    }
}
