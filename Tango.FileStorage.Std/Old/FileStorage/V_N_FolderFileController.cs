using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tango;
using Abc.Model;

namespace Abc.Controllers
{
	public partial class V_N_FolderFileController
	{
		//public ActionResult ViewList()
		//{
		//	if (Url.GetInt("parent", 0) == 0)
		//		return View("list", App.DataContext.Filtered.V_N_FolderFile.Where(o => !o.ParentID.HasValue).OrderBy(o => o.Title).OrderBy(o => o.IsFile));
		//	else
		//		return View("list", App.DataContext.Filtered.V_N_FolderFile.Where(o => o.ParentID == Url.GetInt("parent", 0)).OrderBy(o => o.Title).OrderBy(o => o.IsFile));

		//}
	}
}