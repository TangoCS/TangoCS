using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Nephrite.Web.Layout;
using Nephrite.Web.SettingsManager;

namespace Nephrite.Web.Controls
{
	public class ImageUploadSimple : System.Web.UI.UserControl
	{
		public object ImageAttributes { get; set; }
		public bool AllowDelete { get; set; }

		protected string _image = "";
		CheckBox cbDel = new CheckBox();
		System.Web.UI.WebControls.FileUpload fuFile = new System.Web.UI.WebControls.FileUpload();

		public ImageUploadSimple()
        {
            AllowDelete = true;
            Enabled = true;

			cbDel.Text = "удалить изображение";
        }

        protected override void  OnInit(EventArgs e)
		{
 			base.OnInit(e);

			if (!IsPostBack)
			{
				cbDel.Visible = false;
			}

			fuFile.ID = "fuFile" + ClientID;
			cbDel.ID = "cbDel" + ClientID;

			Controls.Add(cbDel);
			Controls.Add(fuFile);
			
		}

		public void Clear()
		{
			_image = "";
			cbDel.Visible = false;
		}

		public bool DeleteFlag()
		{
			return cbDel.Visible && cbDel.Checked;
		}

		public void SetImage(int imageID, string imageName)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("<img src='/data.ashx?oid={0}' alt='{1}' ", imageID.ToString(), imageName);
			sb.AppendAttributes(ImageAttributes, "");
			sb.Append(" />");
			_image = sb.ToString();
			cbDel.Visible = AllowDelete;
		}
		public void SetImage(Guid imageID, string imageName)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("<img src='/data.ashx?guid={0}' alt='{1}' ", imageID.ToString(), imageName);
			sb.AppendAttributes(ImageAttributes, "");
			sb.Append(" />");
			_image = sb.ToString();
			cbDel.Visible = AllowDelete;
		}

		public bool CheckFileSize()
		{
            return fuFile.PostedFile.ContentLength / 1024 / 1024 > Settings.MaxFileSize;
		}

		public System.Web.UI.WebControls.FileUpload GetImage()
		{
			return fuFile;
		}

		public bool Enabled { get; set; }

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(_image);
			if (cbDel.Visible) writer.Write("<div>");
			cbDel.RenderControl(writer);
			if (cbDel.Visible) writer.Write("</div>");
			if (Enabled) fuFile.RenderControl(writer);

			//Controls.Add(new LiteralControl(_image));
			//if (cbDel.Visible) Controls.Add(new LiteralControl("<div>"));
			//Controls.Add(cbDel);
			//if (cbDel.Visible) Controls.Add(new LiteralControl("</div>"));
			//if (Enabled) Controls.Add(fuFile);
		}
	}
}