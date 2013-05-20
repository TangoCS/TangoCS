using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Web.Controls
{
    [ParseChildren(true)]
    [PersistChildren(false)]
    public class MessageBox : Control, INamingContainer
    {
        Control contentInstance;
        ITemplate content = null;
        
        string width = "450px";

        [TemplateInstance(TemplateInstance.Single)]
        [Browsable(false)]
        public virtual ITemplate ContentTemplate
        {
            get { return content; }
            set { content = value; }
        }

        public string Title { get; set; }
        public string Width { get { return width; } set { width = value; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            if (content != null)
            {
                contentInstance = new Control();
                content.InstantiateIn(contentInstance);
                Controls.Add(contentInstance);
            }

			Page.ClientScript.RegisterClientScriptInclude("popup.js", Settings.JSPath + "popup.js");
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.Write(@"<script type=""text/javascript"">
function hide" + ID + @"()
{
    var " + ID + @"node = document.getElementById('" + ID + @"1');
    document.body.style.background = 'none';
	document.body.style.backgroundAttachment = 'scroll';
	whichDog=document.getElementById('" + ID + @"');
	document.getElementById('" + ID + @"1').style.visibility='hidden';
    " + ID + @"node.parentNode.removeChild(" + ID + @"node);
}

function show" + ID + @"()
{
    var " + ID + @"node = document.getElementById('" + ID + @"1');
    " + ID + @"node.parentNode.removeChild(" + ID + @"node);
    document.body.getElementsByTagName('form')[0].appendChild(" + ID + @"node);

    document.body.style.background = 'url(" + Settings.ImagesPath + @"modalback.png) no-repeat';
	document.body.style.backgroundAttachment = 'fixed';
	whichDog=document.getElementById('" + ID + @"');  
	document.getElementById('" + ID + @"1').style.visibility='visible';
    document.getElementById('" + ID + @"').style.left = (document.body.offsetWidth - parseInt(whichDog.style.width)) / 2 + 'px';
}
</script>");
			writer.Write(@"<div id=""{0}1"" style=""background-image:url(" + Settings.ImagesPath + @"modalback.png);
        background-repeat:repeat;
        top:0px;
        left:0px;
        width:100%;
        height:100%;
        position:fixed;
        //position:absolute;
        top:expression(eval(document.body.scrollTop) + 'px');visibility:hidden;"">
<div id=""{0}"" style=""top:150px;width:{2};position: absolute;background-color: #FFF;border: solid 3px Silver;"">
<table border=""0"" width=""100%"" cellspacing=""0"" cellpadding=""5"">
	<tr style=""background-color:Silver"">
		<td id=""titleBar"" width=""100%"" style=""cursor:move"" titleBar=""titleBar"">{1}</td>
		<td><a onclick=""hide{0}();return false"">
			<img src=""" + Settings.ImagesPath + @"stop.gif"" alt=""{3}"" /></a></td>  
	</tr>
	<tr>
		<td style=""padding:4px"" colspan=""2"">", ID, Title, width, Properties.Resources.Close);
            RenderChildren(writer);

            writer.Write(@"</td>
	</tr>
	<tr>
		<td align=""center""><input type=""button"" value=""{1}"" onclick=""hide{0}();return false;"" /></td>
	</tr>
</table>
</div>
</div>", ID, Properties.Resources.OK);
        }

        public void Show()
        {
            Page.ClientScript.RegisterStartupScript(GetType(), ID + "Show", "show" + ID + "();", true);
            // Если мы лежим внутри UpdatePanel, то зарегаем ещё один стартап
            Control c = this.Parent;
            while (c != null)
            {
                if (c is UpdatePanel)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), ID + "Show1", "show" + ID + "();", true);
                }
                c = c.Parent;
            }
        }
    }

    public class MessageBoxText : LiteralControl, ITemplate
    {
        public MessageBoxText() : base()
        {

        }

        public MessageBoxText(string text) : base(text)
        {

        }

        #region ITemplate Members

        public void InstantiateIn(Control container)
        {
            container.Controls.Add(this);
        }

        #endregion
    }

}
