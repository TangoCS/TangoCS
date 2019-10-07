using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tango;
using Tango.Data;
using Tango.FileStorage;
using Abc.Model;

namespace Abc.Controllers
{
	public partial class N_FolderController
	{
		//public ActionResult PackAndDownload(int id)
		//{
		//	DbFolders storage = new DbFolders(App.DataContext.Session.Connection);
		//	var folder = storage.GetFolder(id);

		//	var fp = new FolderPacker();
		//	fp.PackFolder(storage, folder);
		//	return null;
		//}
		//public ActionResult Edit(int id)
		//{
		//	var obj = App.DataContext.N_Folder.Filtered().FirstOrDefault(o => o.FolderID == id);
		//	if (!obj.ParentID.HasValue)
		//		return View("rootedit", obj);
		//	else
		//		return View("edit", obj);
		//}
		//public ActionResult CreateNew(int parentid)
		//{
		//	N_Folder obj = new N_Folder();
		//	obj.FileLibrary = new N_FileLibrary();
		//	App.DataContext.N_FileLibrary.InsertOnSubmit(obj.FileLibrary);
		//	App.DataContext.N_Folder.InsertOnSubmit(obj);
		//	obj.Guid = Guid.NewGuid();

		//	if (parentid == 0)
		//		return View("rootedit", obj);
		//	else
		//	{
		//		obj.ParentID = parentid;
		//		return View("edit", obj);
		//	}
		//}
		//public ActionResult ViewList()
		//{
		//	int parentid = ActionContext.GetArg("parentid", 0);
		//	if (parentid == 0)
		//		return View("list", App.DataContext.N_Folder.Filtered().Where(o => !o.ParentID.HasValue));
		//	else
		//		return View("list", App.DataContext.N_Folder.Filtered().Where(o => o.ParentID == parentid));
		//}
	}
}

//namespace Abc.Model
//{
//	public class N_FolderLogic : ILogic
//	{
//		public void Delete(N_Folder obj)
//		{
//			App.DataContext.N_File.DeleteAllOnSubmit(obj.Files);
//			App.DataContext.N_Folder.DeleteAllOnSubmit(App.DataContext.N_Folder.Where(o => o.Path.StartsWith(obj.Path)));
//			App.DataContext.SubmitChanges();
//		}
//	}
//}