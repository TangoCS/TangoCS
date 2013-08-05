using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web.Controls;
using Nephrite.Web.FileStorage;

namespace Nephrite.Web.View
{
	public partial class TinyMCEFileManager_list : ViewControl
	{
		public IQueryable<IDbItem> _data = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			string type = Query.GetString("type");
			Guid parentid = Query.GetGuid("parent");

			if (parentid == Guid.Empty)
			{
				_data = FileStorageManager.DbItems.Where(o => o.ParentID == null && o.Type == DbItemType.Folder);
			}
			else
				_data = FileStorageManager.DbItems.Where(o => o.ParentID == parentid).OrderBy(o => o.Type);
			

			/*N_Folder p = AppWeb.DataContext.N_Folders.SingleOrDefault(o => o.FolderID == parentid);
			if (p != null)
			{
				//Cramb.Add("Файлы", Query.RemoveParameter("parent"));
				//SetTitle(p.Title);
			}
			else
			{
				//SetTitle("Файлы");
			}*/

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
				var p = FileStorageManager.GetFolder(parentid);
				toolbar.AddItem("back.gif", "На уровень выше", Url.Current.SetParameter("parent", p.ParentFolderID.ToString()).RemoveParameter("op"));
				toolbar.AddItemSeparator();
				toolbar.AddItem("upload.gif", "Загрузить файлы", Url.Current.SetParameter("op", "upload"));
			}

			//toolbar.AddItemFilter(filter);
			//toolbar.AddItemSeparator();
			//toolbar.AddItem<N_FolderController>("add.png", "Создать папку", c => c.CreateNew(parentid, Query.CreateReturnUrl()));
			//if (parentid > 0)
			//	toolbar.AddItem<N_FolderController>("upload.gif", "Загрузить файлы", c => c.Upload(Query.GetString("parent"), Query.CreateReturnUrl()));

			filter.AddFieldString<IDbItem>("Наименование", o => o.Title);
			filter.AddFieldDate<IDbItem>("Дата последнего изменения", o => o.LastModifiedDate, false);
			filter.AddFieldString<IDbItem>("Последний редактировавший пользователь", o => o.LastModifiedUserName);
			filter.AddFieldBoolean<IDbItem>("Удален", o => o.IsDeleted);
		}
	}
}