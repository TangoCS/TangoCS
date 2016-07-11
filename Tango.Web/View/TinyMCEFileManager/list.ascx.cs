using System;
using System.Collections.Generic;
using Nephrite.FileStorage;

namespace Nephrite.Web.View
{
	public partial class TinyMCEFileManager_list : ViewControl
	{
		public IEnumerable<IStorageFolder> _folders = null;
		public IEnumerable<IStorageFile> _files = null;

		[Inject]
		public DbFolders Storage { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			string type = Query.GetString("type");
			Guid parentid = Query.GetGuid("parent");

			//if (parentid == Guid.Empty)
			//{
			//	_data = FileStorageManager.DbItems.Where(o => o.ParentID == null && o.Type == DbItemType.Folder);
			//}
			//else
			//	_data = FileStorageManager.DbItems.Where(o => o.ParentID == parentid).OrderBy(o => o.Type);

			IStorageFolder curFolder = null;
			if (parentid != Guid.Empty)
			{
				curFolder = Storage.GetFolder(parentid);
				_files = curFolder.GetAllFiles();
			}
			_folders = Storage.GetFolders(curFolder);


			if (curFolder != null)
			{
				Cramb.Add("Файлы", Query.RemoveParameter("parent"));
				SetTitle(curFolder.Name);
			}
			else
			{
				SetTitle("Файлы");
			}

			/*Action<N_Folder> addCramb = null;
			addCramb = f =>
			{
				if (f == null) return;
				addCramb(f.Parent);
				Cramb.Add(f.Title, Query.RemoveParameter("parent") + "&parent=" + f.FolderID.ToString());
			};
			addCramb(p);*/

			if (parentid != Guid.Empty)
			{
				//toolbar.AddItem("back.gif", "На уровень выше", UrlHelper.Current().SetParameter("parent", p.ParentFolderID.ToString()).RemoveParameter("op"));
				toolbar.AddItemSeparator();
				toolbar.AddItem("upload.gif", "Загрузить файлы", UrlHelper.Current().SetParameter("op", "upload"));
			}

			//toolbar.AddItemFilter(filter);
			//toolbar.AddItemSeparator();
			//toolbar.AddItem<N_FolderController>("add.png", "Создать папку", c => c.CreateNew(parentid, Query.CreateReturnUrl()));
			//if (parentid > 0)
			//	toolbar.AddItem<N_FolderController>("upload.gif", "Загрузить файлы", c => c.Upload(Query.GetString("parent"), Query.CreateReturnUrl()));
		}
	}
}