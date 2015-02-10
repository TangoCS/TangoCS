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
using System.ComponentModel;

using Nephrite.Multilanguage;
using Nephrite.Web.Layout;


namespace Nephrite.Web.Controls
{
	[ParseChildren(true, "ContentTemplate")]
	[PersistChildren(false)]
	public partial class ModalDialog : System.Web.UI.UserControl, INamingContainer
	{
		protected string width = "450px";
		protected string top = "150px";
		protected string close = Properties.Resources.Close;

		public int PageCount { get; set; }
		public string Title { get; set; }
		public string Width { get { return width; } set { width = value; } }
		public string DefaultFocus { get; set; }

		public ILayoutModal Layout { get; set; }

		protected string clientAreaWidth 
		{ 
			get 
			{
				return (Width.Replace("px", "").ToInt32(0) - 16).ToString() + "px";
			}
		}
		public string Top { get { return top; } set { top = value; } }
		public string OnClientHide { get; set; }
		public bool HideOnOK { get; set; }
		public bool MessageBoxMode { get; set; }
		public string TargetModalDialogID
		{
			get
			{
				if (TargetModalDialog != null)
					return TargetModalDialog.ClientID;
				return String.Empty;
			}
			set
			{
				if (!String.IsNullOrEmpty(value))
				{
					TargetModalDialog = NamingContainer.FindControl(value) as ModalDialog;
					if (TargetModalDialog == null)
						throw new Exception("Контрол с идентификатором " + value + " не найден в контейнере " + NamingContainer.ClientID);
				}
				else
					TargetModalDialog = null;
			}
		}

		ModalDialogManager mgr = null;

		protected void Page_Init(object sender, EventArgs e)
		{
			Layout = AppLayout.Current.Modal;
			mgr = Page.Items["ModalDialogManager"] as ModalDialogManager;
			if (mgr == null)
				return;
			mgr.ModalWindows.Add(this);

			btnOK.Text = Properties.Resources.OK;
            btnOK.Click += new EventHandler(btnOK_Click);
			//btnOK.Visible = !MessageBoxMode;
            btnOK.Attributes.Add("disabled", "disabled");

			DefaultFocus = btnOK.ClientID;

			btnCancel.Text = TextResource.Get("Common.Buttons.Cancel", Properties.Resources.Cancel);
			btnCancel.OnClientClick = String.Format("javascript:hide{0}();return false", ClientID);
            btnCancel.Attributes.Add("disabled", "disabled");

			hfPageIndex.Value = "1";
			hfVisible.Value = "0";
			hfFirstPopulate.Value = "0";
			hfArgument.Value = String.Empty;
			hfMode.Value = String.Empty;

			if (content != null)
			{
				contentInstance = new Control();
				content.InstantiateIn(contentInstance);
				phContent.Controls.Add(contentInstance);
			}
			
			if (nonajaxcontent != null)
			{
				nonajaxcontentInstance = new Control();
				nonajaxcontent.InstantiateIn(nonajaxcontentInstance);
				phnonajaxContent.Controls.Add(nonajaxcontentInstance);
			}

			SetBottomLeft();
			Page.ClientScript.RegisterClientScriptInclude("popup", Settings.JSPath + "popup.js?v=1");
			Page.ClientScript.RegisterStartupScript(typeof(Page), "initModalPopup", "<script>initModalPopup()</script>");

			Page.ClientScript.RegisterClientScriptBlock(GetType(), "ProcessEnter", @"
	document.onkeypress = processKeyMD;
	var defaultButtonId = '';
	function processKeyMD(e)
	{
		if (null == e)
			e = window.event ;
		if (e.keyCode == 13 && defaultButtonId != '')  {
			document.getElementById(defaultButtonId).click();
		}
	}", true);
		}

		public void SetBottomLeft()
		{
			if (bottomLeftContent != null)
			{
				bottomLeftContentInstance = new Control();
				bottomLeftContent.InstantiateIn(bottomLeftContentInstance);
				phBottomLeft.Controls.Add(bottomLeftContentInstance);
				foreach (Control c in bottomLeftContentInstance.Controls)
				{
					if (c is CheckBox || c is Button || c is DropDownList)
						up.Triggers.Add(new AsyncPostBackTrigger { ControlID = c.ID });
				}
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			
			if (IsVisible)
			{
				Page.Form.DefaultButton = btnOK.UniqueID;
				OnPopulate(EventArgs.Empty);
			}
			if (hfOKClick.Value == "1")
			{
				OnOKClick(new EventArgs());
				if (TargetModalDialog != null)
					TargetModalDialog.OnPopulate(EventArgs.Empty);
				hfOKClick.Value = "0";
			}

            //panel.Attributes.Add("onkeypress", "javascript: alert('');" + Title + " return keypress_" + ClientID + "(event)");
		}

		void btnOK_Click(object sender, EventArgs e)
		{
			OnOKClick(new EventArgs());
		}

		public event EventHandler OKClick;
		protected internal virtual void OnOKClick(EventArgs e)
		{
			if (OKClick != null)
			{
				OKClick(this, e);
			}
		}

		public event EventHandler Populate;
		protected internal virtual void OnPopulate(EventArgs e)
		{
            if (Populate != null)
				Populate(this, e);
			if (hfFirstPopulate.Value != "0")
			{
				hfFirstPopulate.Value = "0";
			}
			ScriptManager.RegisterStartupScript(up, up.GetType(), "showwnd" + ID, "loaded" + ClientID + "();", true);
            if (MessageBoxMode)
                ScriptManager.RegisterStartupScript(up, up.GetType(), "hideok" + ID, "document.getElementById('" + btnOK.ClientID + "').style.display = 'none';", true);
		}

		void AddDisables(ControlCollection cc)
		{
			ScriptManager.RegisterOnSubmitStatement(this, GetType(), "Disable", "disable" + ClientID + "();");
		}

		public string postBackFunc()
		{
			return Page.ClientScript.GetPostBackEventReference(lbRun, "");
		}

        public string SubmitFunc()
        {
            return Page.ClientScript.GetPostBackEventReference(btnOK, "");
        }

		Control contentInstance;
		ITemplate content = null;

		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		public virtual ITemplate ContentTemplate
		{
			get { return content; }
			set { content = value; }
		}


		Control nonajaxcontentInstance;
		ITemplate nonajaxcontent = null;

		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		public virtual ITemplate NonAJAXContentTemplate
		{
			get { return nonajaxcontent; }
			set { nonajaxcontent = value; }
		}

		Control bottomLeftContentInstance;
		ITemplate bottomLeftContent = null;

		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		public virtual ITemplate BottomLeftContentTemplate
		{
			get { return bottomLeftContent; }
			set 
			{
				bottomLeftContent = value;
			}
		}

        void EnableDisableControls(ControlCollection cc, bool enable)
        {
            foreach (Control c in cc)
            {
				if (c is TextBox)
				{
					((TextBox)c).Enabled = enable;
					if (!enable)
						((TextBox)c).BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
				}
                if (c is CheckBox)
                    ((CheckBox)c).Enabled = enable;
                if (c is FileUpload)
                    ((FileUpload)c).Visible = enable;
                if (c is FileUploadSimple)
                    ((FileUploadSimple)c).Enabled = enable;
                if (c is DropDownList)
                    ((DropDownList)c).Enabled = enable;
                if (c is Button)
                    ((Button)c).Enabled = enable;
                if (c is SelectObjectHierarchic)
                    ((SelectObjectHierarchic)c).Enabled = enable;
                if (c.Controls.Count > 0)
                    EnableDisableControls(c.Controls, enable);
            }
        }

        public string OKClientClick { get; set; }
		internal bool DisableHfOkClick = false;
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (!Visible)
				return;
			if (contentInstance == null)
				return;
            EnableDisableControls(contentInstance.Controls, !MessageBoxMode);
			btnOK.Visible = !MessageBoxMode;
            if (String.IsNullOrEmpty(OKClientClick))
            {
                if (TargetModalDialog == null)
                    btnOK.OnClientClick = "document.getElementById('" + hfVisible.ClientID + "').value='0'; " + (HideOnOK ? "hide" : "disable") + ClientID + "(); " + Page.ClientScript.GetPostBackEventReference(new PostBackOptions(btnOK) { PerformValidation = false }) + "; return false;";
                else
                {
                    btnOK.OnClientClick = "document.getElementById('" + hfOKClick.ClientID + "').value='1'; document.getElementById('" + hfVisible.ClientID + "').value='0'; hide" + ClientID + "();" +
                        TargetModalDialog.postBackFunc() + "; document.getElementById('" + hfOKClick.ClientID + "').value='0'; return false;";
                }
            }
            else
            {
				btnOK.OnClientClick = "document.getElementById('" + hfOKClick.ClientID + "').value='" + (DisableHfOkClick ? "0" : "1") + "'; document.getElementById('" + hfVisible.ClientID + "').value='0'; if (" + OKClientClick + "){" + (HideOnOK ? "hide" : "disable") + ClientID + "();} else {document.getElementById('" + hfVisible.ClientID + "').value='1';} return false;";
            }

			if (contentInstance != null) AddDisables(contentInstance.Controls);
			hfOKClick.Value = "0";
			
			if (ReadOnly)
			{
				foreach (Control ctrl in contentInstance.Controls)
				{
					if (ctrl is ImageButton) { ((ImageButton)ctrl).Enabled = false; continue; }
					if (ctrl is Button) { ((Button)ctrl).Enabled = false; continue; }
					if (ctrl is DropDownList) { ((DropDownList)ctrl).Enabled = false; continue; }
					if (ctrl is CheckBox) { ((CheckBox)ctrl).Enabled = false; continue; }
					if (ctrl is TextBox) { ((TextBox)ctrl).Enabled = false; continue; }
				}
				btnOK.Visible = false;
				btnCancel.Text = "Закрыть";
			}
		}

		public override void RenderControl(HtmlTextWriter writer)
		{
			if (!SkipRender)
			{
				base.RenderControl(writer);
			}
		}

        public string SkipDisableFunction
        {
            get { return "skipDisable" + ClientID + "()"; }
        }
		public ModalDialog TargetModalDialog { get; set; }

		public bool ReadOnly { get; set; }

		internal bool SkipRender;

		#region Properties
		
		public int PageIndex
		{
			get { int i = hfPageIndex.Value.ToInt32(1); if (i == 0) i = 1; return i; }
			set { hfPageIndex.Value = value.ToString(); }
		}

		public bool IsVisible
		{
			get { return hfVisible.Value == "1"; }
			set { if (value) hfVisible.Value = "1"; }
		}

		public string RenderRun()
		{
			return "show" + ClientID + "();return false;";
		}

		public string RenderRun(object arg)
		{
			return "show" + ClientID + "Arg('" + arg.ToString() + "');return false;";
		}

		public string RenderRun(object arg, object mode)
		{
			return "show" + ClientID + "Arg('" + arg.ToString() + "', '" + mode.ToString() + "');return false;";
		}

		public bool IsFirstPopulate
		{
			get { return hfFirstPopulate.Value == "1"; }
		}

		public string Argument
		{
			get { return hfArgument.Value; }
		}

		public string Mode
		{
			get { return hfMode.Value; }
		}

        public string SetPageIndexFunction
        {
            get { return "setPageIndex" + ClientID; }
        }
		#endregion

		public void Reopen()
		{
            if (TargetModalDialog == null)
                Page.ClientScript.RegisterStartupScript(typeof(Page), "reopen" + ClientID, "<script>reopen" + ClientID + "();</script>");
            else
                ScriptManager.RegisterStartupScript(TargetModalDialog.UpdatePanel, TargetModalDialog.UpdatePanel.GetType(), "reopen" + ClientID, "reopen" + ClientID + "();", true);
		}

        public System.Web.UI.UpdatePanel UpdatePanel
        {
            get { return up; }
        }

		public PlaceHolder ContentPlaceHolder
		{
			get { return phContent; }
		}

		public string GetOKClickScript()
		{
			return Page.ClientScript.GetPostBackEventReference(new PostBackOptions(btnOK) { PerformValidation = false }) + ";";
		}
	}
}