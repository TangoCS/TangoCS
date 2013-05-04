using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Diagnostics;

namespace Nephrite.Web.Controls
{
	public partial class FileUploadSimple : System.Web.UI.UserControl
	{
        public bool AllowDelete { get; set; }
        
        public FileUploadSimple()
        {
            AllowDelete = true;
            Enabled = true;
        }

        protected void Page_Init(object sender, EventArgs e)
		{
			lMess.Visible = false;
			if (!IsPostBack)
			{
				cbDel.Visible = false;
				lLink.Visible = false;
			}

			string s = Guid.NewGuid().ToString();
			cbDel.GroupName = s;
			rbChange.GroupName = s;
			rbNoChanges.GroupName = s;
			fuFile.Attributes.Add("onchange", "document.getElementById('" + rbChange.ClientID + "').click();");
		}

		public void Clear()
		{
			lLink.Text = "";
			lLink.Visible = false;
			cbDel.Visible = false;
		}

		public bool DeleteFlag()
		{
			return cbDel.Visible && cbDel.Checked && !fuFile.HasFile;
		}

		public void SetFile(int fileID, string fileName)
		{
			lLink.Text = String.Format("<a href='/file.ashx?oid={0}'>{1}</a>", fileID.ToString(), fileName);
			lLink.Visible = true;
            cbDel.Visible = AllowDelete;
		}

		public void SetFile(Guid fileID, string fileName)
		{
			lLink.Text = String.Format("<a href='/file.ashx?guid={0}'>{1}</a>", fileID.ToString(), fileName);
			lLink.Visible = true;
			cbDel.Visible = AllowDelete;
		}

		public bool CheckFileSize()
		{
            return fuFile.PostedFile.ContentLength / 1024 / 1024 > Settings.MaxFileSize;
		}

		public System.Web.UI.WebControls.FileUpload GetFile()
		{
			return fuFile;
		}

        public bool Enabled { get; set; }
	}
}