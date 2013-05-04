using System;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Nephrite.Web.App;

namespace Nephrite.Web
{
    public class Workspace : WebPart
    {
        Exception error = null;
        
        public bool SkipCreateMdm { get; set; }
        public bool DisableScriptManager { get; set; }
        public string DefaultMode { get; set; }
        public string DefaultAction { get; set; }
		public bool RenderTitle { get; set; }
		public bool DisablePaging { get; set; }
		public string LayoutClass { get; set; }
		
        public Workspace() : base()
        {
            DefaultMode = "Default";
            DefaultAction = "View";
            ChromeType = PartChromeType.None;
            Title = Resources.Common.Welcome;
			RenderTitle = true;
			LayoutClass = "WSSLayout";
        }

		public PlaceHolder PlaceHolder { get; private set; }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
			PlaceHolder = new PlaceHolder();
			Controls.Add(PlaceHolder);
			try
			{
				HttpContext.Current.Items["WorkspaceWebPart"] = this;
				TraceContext tc = HttpContext.Current.Trace;
				tc.Write("Workspace class - begin");

                ChromeType = PartChromeType.None;
				Title = Resources.Common.Welcome;
				
                //Page.Title = (String.IsNullOrEmpty(Settings.SystemTitle) ? "" : (Settings.SystemTitle + " :: ")) + Title;

				if (Query.GetString("notitle").Length == 0 && RenderTitle)
					Controls.Add(Page.LoadControl(Settings.ControlsPath + "/Common/title.ascx"));

				string mode = Query.GetString("mode");
				string action = Query.GetString("action");

				if (String.IsNullOrEmpty(mode) || String.IsNullOrEmpty(action))
				{
					mode = DefaultMode;
					action = DefaultAction;
				}

				BaseController.Run(this, mode, action, DisableScriptManager, SkipCreateMdm);
			}
			catch (ThreadAbortException) 
			{
				
			}
			catch (Exception e)
			{
				error = e;
				int errorID = ErrorLogger.Log(e);
			}
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (error != null)
            {
                if (Controls.Count > 1 && Controls[1].ID == "ChangeEmployee")
                    Controls[1].RenderControl(writer);

                error.Render(writer);
            }
            else
            {
                try
                {
                    base.RenderContents(writer);
                }
                catch (Exception e)
                {
					ErrorLogger.Log(e);
                    e.Render(writer);
                }
            }
        }
    }
}
