using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tango;

namespace Abc.Controllers
{
	//public partial class DbFileController
	//{
	//	public ActionResult View(Guid id)
	//	{
	//		return View("view", FileStorageManager.GetFile(id));
	//	}

	//	public ActionResult Edit(Guid id)
	//	{
	//		var file = FileStorageManager.GetFile(id);
	//		//bool canEdit = ActionAccessControl.Instance.Check(file.SPMActionItemGUID.ToString(), true);
	//		//if (!canEdit)
	//		//{
	//		//	WebFormRenderer.RenderMessage("Нет доступа");
	//		//	return;
	//		//}
	//		return View("edit", file);
	//	}

	//	public ActionResult Delete(Guid id)
	//	{
	//		var file = FileStorageManager.GetFile(id);
	//		/*bool canDelete = Tango.Web.SPM.ActionSPMContext.Current.Check(file.SPMActionItemGUID, 4, file, true);
	//		if (!canDelete)
	//		{
	//			WebFormRenderer.RenderMessage("Нет доступа");
	//			return;
	//		}*/
	//		return View("delete", file);
	//	}

	//	public ActionResult Create(string parentFolderID)
	//	{
	//		string path = null;
	//		if (parentFolderID != "")
	//		{
	//			var parentFolder = FileStorageManager.GetFolder(parentFolderID.ToGuid());
	//			/*bool canUpload = Tango.Web.SPM.ActionSPMContext.Current.Check(parentFolder.SPMActionItemGUID, 3, parentFolder, true);
	//			if (!canUpload)
	//			{
	//				WebFormRenderer.RenderMessage("Нет доступа");
	//				return;
	//			}*/
	//			path = parentFolder.Title;
	//			if (!parentFolder.Path.IsEmpty())
	//				path = parentFolder.Path + "/" + path;
	//		}
	//		return View("edit", FileStorageManager.CreateFile("", path));
	//	}

	//	public ActionResult CheckOut(Guid id)
	//	{
	//		var file = FileStorageManager.GetFile(id);
	//		/*bool canEdit = Tango.Web.SPM.ActionSPMContext.Current.Check(file.SPMActionItemGUID, 4, file, true);
	//		if (!canEdit)
	//		{
	//			WebFormRenderer.RenderMessage("Нет доступа");
	//			return;
	//		}*/
	//		file.CheckOut();
	//		App.DataContext.SubmitChanges();
	//		return RedirectBack();
	//	}

	//	public ActionResult CheckIn(Guid id)
	//	{
	//		var file = FileStorageManager.GetFile(id);
	//		/*bool canEdit = Tango.Web.SPM.ActionSPMContext.Current.Check(file.SPMActionItemGUID, 4, file, true);
	//		if (!canEdit)
	//		{
	//			WebFormRenderer.RenderMessage("Нет доступа");
	//			return;
	//		}*/
	//		file.CheckIn();
	//		App.DataContext.SubmitChanges();
	//		return RedirectBack();
	//	}
	//}
}
