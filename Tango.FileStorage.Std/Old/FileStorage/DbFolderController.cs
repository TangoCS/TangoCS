using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tango;

namespace Abc.Controllers
{
	//public partial class DbFolderController
	//{
	//	public ActionResult View(Guid id)
	//	{
	//		return View("view", FileStorageManager.GetFolder(id));
	//	}
	//	public ActionResult Edit(Guid id)
	//	{
	//		var folder = FileStorageManager.GetFolder(id);
	//		/*bool canEdit = Tango.Web.SPM.ActionSPMContext.Current.Check(folder.SPMActionItemGUID, 4, folder, true);
	//		if (!canEdit)
	//		{
	//			WebFormRenderer.RenderMessage("Нет доступа");
	//			return;
	//		}*/
	//		return View("edit", folder);
	//	}
	//	public ActionResult Delete(Guid id)
	//	{
	//		var folder = FileStorageManager.GetFolder(id);
	//		/*bool canEdit = Tango.Web.SPM.ActionSPMContext.Current.Check(folder.SPMActionItemGUID, 4, folder, true);
	//		if (!canEdit)
	//		{
	//			WebFormRenderer.RenderMessage("Нет доступа");
	//			return;
	//		}*/
	//		return View("delete", folder);
	//	}
	//	public ActionResult Create(string parentFolderID)
	//	{
	//		if (parentFolderID != "")
	//		{
	//			var parentFolder = FileStorageManager.GetFolder(parentFolderID.ToGuid());
	//			/*bool canEdit = Tango.Web.SPM.ActionSPMContext.Current.Check(parentFolder.SPMActionItemGUID, 4, parentFolder, true);
	//			if (!canEdit)
	//			{
	//				WebFormRenderer.RenderMessage("Нет доступа");
	//				return;
	//			}*/
	//		}
	//		return View("edit", FileStorageManager.CreateFolder(Guid.NewGuid(), parentFolderID.ToGuid()));
	//	}
	//	public ActionResult PackAndDownload(Guid id)
	//	{
	//		var folder = FileStorageManager.GetFolder(id);
	//		/*bool canDownload = Tango.Web.SPM.ActionSPMContext.Current.Check(folder.SPMActionItemGUID, 2, folder, true);
	//		if (!canDownload)
	//		{
	//			WebFormRenderer.RenderMessage("Нет доступа");
	//			return;
	//		}*/
	//		var fp = new FolderPacker();
	//		fp.PackFolder(id);
	//		return null;
	//	}
	//	public ActionResult Upload(Guid id)
	//	{
	//		var folder = FileStorageManager.GetFolder(id);
	//		/*bool canUpload = Tango.Web.SPM.ActionSPMContext.Current.Check(folder.SPMActionItemGUID, 3, folder, true);
	//		if (!canUpload)
	//		{
	//			WebFormRenderer.RenderMessage("Нет доступа");
	//			return;
	//		}*/
	//		return View("upload", folder);
	//	}
	//}
}