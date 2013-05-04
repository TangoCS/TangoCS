using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nephrite.Web;
using Nephrite.Metamodel;
using Nephrite.CMS.Model;
using System.Web.UI.WebControls;

namespace Nephrite.CMS.Controllers
{
	[ControllerControlsPath("/_controltemplates/Nephrite.CMS/")]
	public class ContentController: BaseController
	{
		public void ViewList(string classID)
		{

			RenderView("list", AppCMS.DataContext.V_SiteSections.Where(o => o.LanguageCode == AppMM.CurrentLanguage.LanguageCode && 
				o.SiteObjectsClasses.Contains(";" + classID) ||		
				o.SiteObjectsClasses.Contains(classID + ";") ||
				o.SiteObjectsClasses == classID));
		}

		public void ElementsList(int sectionID, string returnUrl)
		{
			MM_ObjectType t = AppCMS.DataContext.MM_ObjectTypes.Single(o => o.ObjectTypeID == Query.GetInt("classid", 0));
			RenderView("elementslist", AppCMS.DataContext.V_SiteObjects.Where(o => o.LanguageCode == AppMM.CurrentLanguage.LanguageCode && 
				o.ParentID == sectionID && o.ClassName == t.SysName).OrderByDescending(o => o.SeqNo));
		}
	}

	
}
