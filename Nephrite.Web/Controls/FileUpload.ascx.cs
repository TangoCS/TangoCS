using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Nephrite.Web.Controls
{
	public partial class FileUpload : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			fuFile.Attributes.Add("onchange", "fileSelected" + ClientID + "()");
			lMess.Visible = false;

			if (!IsPostBack)
			{
				cbDel.Visible = false;
				lLink.Visible = false;
				Page.ClientScript.RegisterClientScriptInclude("popup.js", Settings.JSPath + "popup.js");
			}
		}

		public string GetFile()
		{
			if (!String.IsNullOrEmpty(savedID.Value))
			{
				return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "temp\\" + savedID.Value)[0];
			}
			else
				return String.Empty;
		}

		public void Clear()
		{
			lLink.Text = "";
			lLink.Visible = false;
			cbDel.Visible = false;
			savedID.Value = "";
		}

		public bool DeleteFlag()
		{
			return cbDel.Visible && cbDel.Checked && !fuFile.HasFile;
		}

		public void SetFile(int fileID, string fileName)
		{
			lLink.Text = String.Format("<a href='/file.ashx?oid={0}'>{1}</a>", fileID.ToString(), fileName);
			lLink.Visible = true;
			cbDel.Visible = true;
		}

		protected void lbLoad_Click(object sender, EventArgs e)
		{
			if (fuFile.HasFile)
			{
				

				if (fuFile.PostedFile.ContentLength / 1024 / 1024 > Settings.MaxFileSize)
				{
					lMess.Text = "Превышен максимальный размер файла (" + Settings.MaxFileSize.ToString() + " МБ)<br/>";
					lMess.Visible = true;
					return;
				}
				

				if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "temp"))
					Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "temp");

				Guid g = Guid.NewGuid();
				Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "temp\\" + g.ToString());
				fuFile.PostedFile.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "temp\\" + g.ToString() + "\\" + fuFile.FileName);

				if (!String.IsNullOrEmpty(savedID.Value))
					Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "temp\\" + savedID.Value, true);

				savedID.Value = g.ToString();
				lLink.Text = fuFile.FileName;
				lLink.Visible = true;
			}

		}

		protected void lbDelNewFile_Click(object sender, EventArgs e)
		{
			CleanTemp();
		}

		public void CleanTemp()
		{
			if (!String.IsNullOrEmpty(savedID.Value))
			{
				FileInfo fi = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "temp\\" + savedID.Value);

				//Directory.Delete пытается удалить всё вместе с каталогом temp
				string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "temp\\" + savedID.Value);
				foreach (string file in files)
					File.Delete(file);

				Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "temp\\" + savedID.Value);
				lLink.Text = "";
				lLink.Visible = false;
				savedID.Value = "";
			}
		}


	}
}