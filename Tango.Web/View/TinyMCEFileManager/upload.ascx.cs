using System;
using System.Web.UI.WebControls;
using Nephrite.FileStorage;


namespace Nephrite.Web.View
{

	public partial class TinyMCEFileManager_upload : ViewControl
	{
		[Inject]
		DbFolders Storage { get; set; }

		//IDbFolder _root = null;
		
		protected void Page_Load(object sender, EventArgs e)
		{
			RenderMargin = true;
			var p = Storage.GetFolder(UrlHelper.Current().GetGuid("parent"));
			if (p != null)
			{
				SetTitle(p.Name + " - загрузка файлов");
			}
			else
			{
				SetTitle("Загрузка файлов");
			}

			//Action<IDbFolder> addCramb = null;
			//addCramb = f =>
			//{
			//	if (f == null) return;
			//	_root = f;
			//	addCramb(f.GetParentFolder());
			//	Cramb.Add(f.Title, UrlHelper.Current().SetParameter("parent", f.ID.ToString()));
			//};
			//addCramb(p);

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
			A.Model.SubmitChanges();
			
			Response.Redirect(Settings.BaseControlsPath + "TinyMCEFileManager.aspx" + Query.RemoveParameter("op"));
		}

		bool Check(FileUpload fu)
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

		bool Upload(FileUpload fu)
		{
			if (!fu.HasFile)
				return true;


			var folder = Storage.GetFolder(UrlHelper.Current().GetGuid("parent"));
			//string fullPath = folder != null ? (folder.Path.IsEmpty() ? "" : folder.Path + "/") + folder.Title : "";
			var file = folder.CreateFile(fu.FileName); //FileStorageManager.CreateFile(Path.GetFileName(fu.FileName), fullPath);
			file.WriteAllBytes(fu.FileBytes);
			return true;
		}
	}
}