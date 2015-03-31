using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Nephrite.Web.FileStorage;
using Nephrite.Http;


namespace Nephrite.Web.View
{
	public partial class TinyMCEFileManager_upload : ViewControl
	{
		IDbFolder _root = null;
		
		protected void Page_Load(object sender, EventArgs e)
		{
			RenderMargin = true;
			var p = FileStorageManager.GetFolder(UrlHelper.Current().GetGuid("parent"));
			if (p != null)
			{
				Cramb.Add("Папки", UrlHelper.Current().RemoveParameter("parent"));
				SetTitle(p.Title + " - загрузка файлов");
			}
			else
			{
				SetTitle("Загрузка файлов");
			}

			Action<IDbFolder> addCramb = null;
			addCramb = f =>
			{
				if (f == null) return;
				_root = f;
				addCramb(f.GetParentFolder());
				Cramb.Add(f.Title, UrlHelper.Current().SetParameter("parent", f.ID.ToString()));
			};
			addCramb(p);

			//lInfo.Text = "Допустимые расширения файлов: " + _root.N_FileLibrary.N_FileLibraryType.Extensions + "<br/>";
			//lInfo.Text += "Максимальный размер загружаемых файлов: " + _root.N_FileLibrary.MaxFileSize.ToString() + " кб.";
		}

		protected void bOK_Click(object sender, EventArgs e)
		{
			if (!Check(fuFile1)) return;
			if (!Check(fuFile2)) return;
			if (!Check(fuFile3)) return;
			if (!Check(fuFile4)) return;
			if (!Check(fuFile5)) return;
			if (!Check(fuFile6)) return;


			if (!Upload(fuFile1)) return;
			if (!Upload(fuFile2)) return;
			if (!Upload(fuFile3)) return;
			if (!Upload(fuFile4)) return;
			if (!Upload(fuFile5)) return;
			if (!Upload(fuFile6)) return;
			Nephrite.Web.A.Model.SubmitChanges();
			
			Response.Redirect(Settings.BaseControlsPath + "TinyMCEFileManager.aspx" + Query.RemoveParameter("op"));
		}

		bool Check(System.Web.UI.WebControls.FileUpload fu)
		{
			if (!fu.HasFile)
				return true;

			/*if (fu.PostedFile.ContentLength > _root.N_FileLibrary.MaxFileSize * 1024)
			{
				lMsg.Text = "Размер файла " + fu.PostedFile.FileName + " превышает максимальный разрешенный.";
				return false;
			}

			string s = _root.N_FileLibrary.N_FileLibraryType.Extensions.Trim();
			if (s != "*.*")
			{
				bool b = false;
				string[] exts = s.Split(new char[] { ',' });
				string fileExt = System.IO.Path.GetExtension(fu.PostedFile.FileName).ToLower();
				foreach (string ext in exts)
				{
					b = (ext.Substring(1) == fileExt);
					if (b) break;
				}
				if (!b) lMsg.Text = "Расширение файла " + fileExt + " запрещено для загрузки в данную папку.";
				return b;
			}
			*/
			return true;
		}

		bool Upload(System.Web.UI.WebControls.FileUpload fu)
		{
			if (!fu.HasFile)
				return true;

			var folder = FileStorageManager.GetFolder(UrlHelper.Current().GetGuid("parent"));
			string fullPath = folder != null ? (folder.Path.IsEmpty() ? "" : folder.Path + "/") + folder.Title : "";
			var file = FileStorageManager.CreateFile(Path.GetFileName(fu.FileName), fullPath);
			file.Write(fu.FileBytes);
			if (!file.CheckValid())
			{
				lMsg.Text += file.GetValidationMessages().Select(o => o.Message).Join("; ");
				return false;
			}
			return true;
		}
	}
}