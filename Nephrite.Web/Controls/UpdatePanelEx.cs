using System;
using System.Web.UI;

namespace Nephrite.Web.Controls
{
    public class UpdatePanelEx : System.Web.UI.UpdatePanel
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            UpdateMode = UpdatePanelUpdateMode.Conditional;
            Page.ClientScript.RegisterClientScriptInclude("popup.js", Settings.JSPath + "popup.js");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack)
                ScriptManager.RegisterStartupScript(this, GetType(), "HideModalPopup", "hideModalPopup();", true);
        }

        void AddDisables(ControlCollection cc)
        {
            const string script = "showModalPopup();";
            ScriptManager.RegisterOnSubmitStatement(this, GetType(), "Disable", script);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            AddDisables(ContentTemplateContainer.Controls);
        }
    }
}
