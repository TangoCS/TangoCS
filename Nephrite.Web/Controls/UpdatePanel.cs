using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Nephrite.Web.Controls
{
    public class UpdatePanel : System.Web.UI.UpdatePanel
    {
        public UpdatePanel()
            : base()
        {
            BackColor = Color.Gray;
            BorderColor = Color.Black;
            BackgroundImage = "loading.gif";
            UpdateMode = System.Web.UI.UpdatePanelUpdateMode.Conditional;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), ClientID + "Show", ClientID + "_show();", true);
            
            if (!Page.IsCallback)
            {
                string script = "function " + ClientID + "_hide() {";
                script += " document.getElementById('" + ClientID + "_inner').style.visibility = 'hidden';";
                script += " document.getElementById('" + ClientID + "_outer').style.backgroundColor = '#" + BackColor.ToArgb().ToString("X").Substring(2) + "';";
                script += " document.getElementById('" + ClientID + "_outer').style.borderWidth = '1px';";
				script += " document.getElementById('" + ClientID + "_outer').style.backgroundImage = 'url(" + Settings.ImagesPath + BackgroundImage + ")';";
                script += "} ";
                script += "function " + ClientID + "_show() {";
                script += " document.getElementById('" + ClientID + "_inner').style.visibility = 'visible';";
                script += " document.getElementById('" + ClientID + "_outer').style.backgroundColor = '';";
                script += " document.getElementById('" + ClientID + "_outer').style.borderWidth = '0';";
                script += " document.getElementById('" + ClientID + "_outer').style.backgroundImage = 'none';";
                script += "}";
                
                Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_script", script, true);
            }
        }

        void AddDisables(ControlCollection cc)
        {
            string hide = ClientID + "_hide();";
            foreach (Control ctrl in cc)
            {
                if (ctrl is ImageButton) { ((ImageButton)ctrl).OnClientClick = hide + ((ImageButton)ctrl).OnClientClick; continue; }
                if (ctrl is Button) { ((Button)ctrl).OnClientClick = hide + ((Button)ctrl).OnClientClick; continue; }
                if (ctrl is DropDownList && ((DropDownList)ctrl).AutoPostBack) { ((DropDownList)ctrl).Attributes.Add("onchange", hide); continue; }
                if (ctrl is CheckBox && ((CheckBox)ctrl).AutoPostBack) { ((CheckBox)ctrl).Attributes.Add("onclick", hide); continue; }
                AddDisables(ctrl.Controls);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            AddDisables(Controls);
        }

        public override void RenderControl(System.Web.UI.HtmlTextWriter writer)
        {
            if (!Page.IsPostBack)
                writer.Write("<div id='" + ClientID +
                    "_outer' style='margin:0; padding:0; border-width:0; border-style:solid; border-color:#" +
                    BorderColor.ToArgb().ToString("X").Substring(2) + "; " + (String.IsNullOrEmpty(Width) ? "width:" : Width) +
                    "; background-position:center; background-repeat:no-repeat'><div id='" + ClientID + "_inner'>");

            base.RenderControl(writer);
            
            if (!Page.IsPostBack)
                writer.Write("</div></div>");
        }

        /// <summary>
        /// Ширина панели
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Цвет фона во время затемнения
        /// </summary>
        public Color BackColor { get; set; }

        /// <summary>
        /// Цвет границы во время затемнения
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// Имя файла фоновой картинки относительно пути /_layouts/images/n/
        /// </summary>
        public string BackgroundImage { get; set; }
    }
}
