using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tango;
using Abc.Model;
using Tango.FileStorage;

namespace Abc.Controllers
{
	public partial class N_FileController
	{
		//public ActionResult Edit(Guid folderid, string filekey)
		//{
		//	DbFolders storage = new DbFolders(App.DataContext.Session.Connection);
		//	var folder = storage.GetFolder(folderid);
		//	Guid g = Guid.Empty;
		//	bool isGuid = Guid.TryParse(filekey, out g);
  //          return View("edit", isGuid ? folder.GetFile(g) : folder.GetFile(filekey));
		//}

		//public ActionResult Delete(Guid folderid, string filekey)
		//{
		//	DbFolders storage = new DbFolders(App.DataContext.Session.Connection);
		//	var folder = storage.GetFolder(folderid);
		//	Guid g = Guid.Empty;
		//	bool isGuid = Guid.TryParse(filekey, out g);
		//	return View("delete", isGuid ? folder.GetFile(g) : folder.GetFile(filekey));
		//}

		//public ActionResult CreateNew(int folderid)
		//{
		//	DbFolders storage = new DbFolders(App.DataContext.Session.Connection);
		//	var folder = storage.GetFolder(folderid);
		//	if (folder == null) return new DefaultResult("Error", "Folder not found");
		//	/*bool canUpload = Tango.Web.SPM.ActionSPMContext.Current.Check(parentFolder.SPMActionItemGUID, 3, parentFolder, true);
		//	if (!canUpload)
		//	{
		//		WebFormRenderer.RenderMessage("Нет доступа");
		//		return;
		//	}*/
		//	return View("edit", folder.CreateFile(Guid.NewGuid()));
		//}
	}
}