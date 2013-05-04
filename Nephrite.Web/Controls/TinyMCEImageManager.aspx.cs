using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;

namespace Nephrite.Web.Controls
{
	public partial class TinyMCEImageManager : System.Web.UI.Page
	{
		protected string[] _dirs = null;
        protected string[] _files = null;
        protected IImageStorage storage;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["ImageStorageClass"]))
                storage = new ImageStorageFileSystem();
            else
                storage = (IImageStorage)Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["ImageStorageClass"]));

            storage.SetCurrentDirectory(HttpUtility.UrlDecode(Query.GetString("dir")));
			
			StringBuilder sb = new StringBuilder(4096);

			if (storage.CurrentDirectory != String.Empty)
			{
				// not at the root
				string currentDirParent = null;
                int lastIndex = storage.CurrentDirectory.LastIndexOf('/');
				if (lastIndex != -1)
				{
					currentDirParent = storage.CurrentDirectory.Substring(0, lastIndex);
				}
				else
				{
                    currentDirParent = "";
				}
				litFolderNav.Text = "<a href=" + Request.Path + "?dir=" + Server.UrlEncode(currentDirParent) + "><img border=0 src='" + Settings.ImagesPath + "back.gif'></a>Назад...";
			}

			DoUpload();

			litFolderDisplay.Text = "<img border=0 src=" + Settings.ImagesPath + "openfold.gif> " + storage.CurrentDirectory;

            _dirs = storage.GetDirectories();
			_files = storage.GetFiles();
		}
		private void DoUpload()
		{
			if (uploadedFile.HasFile)
			{

				HttpPostedFile postedFile = uploadedFile.PostedFile;
				string filename = System.IO.Path.GetFileName(postedFile.FileName);
                storage.Store(postedFile.InputStream, filename);
			}
		}

		protected void bCreateFolder_Click(object sender, System.EventArgs e)
		{
            storage.CreateDirectory(tbFolderName.Text);
            Response.Redirect(Request.Url.ToString());	
			//tbFolderName.Text = "";
			//_dirs = storage.GetDirectories();
			//_files = storage.GetFiles();
		}

		protected void lbDel_Click(object sender, System.EventArgs e)
		{
			string[] arg = plm.Value.Split(new char[] {','});
			if (arg.Length == 2)
			{
				if (arg[0] == "folder" && !String.IsNullOrEmpty(arg[1].Trim()))
				{
                    storage.DeleteDirectory(arg[1]);
				}
				else if (arg[0] == "file" && !String.IsNullOrEmpty(arg[1].Trim()))
				{
                    storage.DeleteFile(arg[1]);
				}
			}
            _dirs = storage.GetDirectories();
            _files = storage.GetFiles();
		}
	}

}
