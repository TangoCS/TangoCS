using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web.Controls;

using Nephrite.Web;
using Nephrite.Metamodel;
using Nephrite.Metamodel.Model;
using Nephrite.CMS.Controllers;


namespace Nephrite.CMS.View
{
	public partial class MainTree : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Page.ClientScript.RegisterClientScriptInclude("nt-listtoolbar", Settings.JSPath + "nt_listtoolbar_menu.js");

			if (ScriptManager.GetCurrent(Page) == null || !ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
			{
				NodeTemplateQueryable t = new NodeTemplateQueryable();
                Repository r = new Repository();
                ObjectTypeRepository otr = new ObjectTypeRepository();
                int[] folderClasses = AppCMS.DataContext.MM_ObjectTypes.Where(o => o.SiteInfoobject.IsFolder == true).Select(o => o.ObjectTypeID).ToArray();
				var rootclasses = AppCMS.DataContext.MM_ObjectTypes.Where(o => o.SiteInfoobject.IsFolder == true && o.SiteInfoobject.IsRoot == true).Select(o => o.ObjectTypeID).ToList();
                List<IMMObject> items = new List<IMMObject>();
                List<IMMObject> rootitems = new List<IMMObject>();
                foreach (var rc in rootclasses)
                {
                    var ot = otr.Get(rc);
                    var list = r.GetList(ot).Where(o => !o.IsDeleted);
                    var cls = r.Empty(ot);
                    if (cls is IMovableObject)
                        list = list.Cast<IMovableObject>().OrderBy(o => o.SeqNo).Cast<IMMObject>();
                    // Если класс не иерархический, добавить все объекты в список корневых
                    IChildObject tobj = cls as IChildObject;
                    if (tobj == null)
                    {
                        rootitems.AddRange(list);
                    }
                    else
                    {
                        items.AddRange(list);
                        // Добавим в список корневых только те объекты, у которых указанное свойство не задано

                        rootitems.AddRange(list.Where(tobj.FilterByParentID<IMMObject>(0)));
                    }
                }

                // В корне должен быть список объектов, свойство которых

                t.DataSource = p => (from o in rootitems
							    select new Node { ID = o.ObjectID, ClassName = o.MetaClass.Name, Title = o.Title }).AsQueryable();
                t.DefaultMethodFunc = n => HtmlHelperBase.Instance.ActionLink<SiteFolderController>(c => c.View(n.ID, n.ClassName), n.Title);

                NodeTemplateStatic ntroot = new NodeTemplateStatic { Title = HtmlHelperBase.Instance.ActionLink<SiteFolderController>(c => c.ViewRoot(), "Корневой раздел") };
                ntroot.Children.Add(t);
                tv.Template.Add(ntroot);

                NodeTemplateQueryable t2 = new NodeTemplateQueryable();
                t2.DataSource = p => ObjectTypeRepository.Get(p.ClassName).
                                 MM_ObjectProperties.Where(o => o.RefObjectTypeID.HasValue &&
                                 folderClasses.Contains(o.RefObjectTypeID.Value) &&
                                 o.RefObjectPropertyID.HasValue &&
                                 o.RefObjectProperty.UpperBound == 1).
                                 Select(o => o.RefObjectType).
                                 Select(o => new { List = r.GetList(o), Obj = r.Empty(o) }).
                                 SelectMany(o => o.List.Where((o.Obj as IChildObject).FilterByParentID<IMMObject>(p.ID))).
								 OrderBy(o => (o as IMovableObject).SeqNo).
                                 Select(o2 => new Node
                                 {
                                     ClassName = o2.MetaClass.Name,
                                     ID = o2.ObjectID,
                                     Title = o2.Title
                                 });
                t2.DefaultMethodFunc = n => HtmlHelperBase.Instance.ActionLink<SiteFolderController>(c => c.View(n.ID, n.ClassName), n.Title);
                //t.MethodFuncs.Add(n => "<a href='?mode=" + n.ClassName + "&action=edit&oid=" + n.ID.ToString() + "&bgroup=" + Query.GetString("bgroup") + "&returnurl=" + Query.CreateReturnUrl(HtmlHelperBase.Instance.ActionUrl<SiteFolderController>(c => c.View(n.ID, n.ClassName))) + "'>" + HtmlHelperBase.Instance.Image("edit.png", "Редактировать") + "</a>");
                //t2.MethodFuncs.Add(n => "<a href='?mode=" + n.ClassName + "&action=edit&oid=" + n.ID.ToString() + "&bgroup=" + Query.GetString("bgroup") + "&returnurl=" + Query.CreateReturnUrl(HtmlHelperBase.Instance.ActionUrl<SiteFolderController>(c => c.View(n.ID, n.ClassName))) + "'>" + HtmlHelperBase.Instance.Image("edit.png", "Редактировать") + "</a>");
				
                t.Children.Add(t2);
                t2.Children.Add(t2);

                foreach (var o in AppMM.DataContext.MM_ObjectTypes.Where(o => rootclasses.Contains(o.ObjectTypeID)))
                {
                    createMenu.AddItem(o.Title, "?mode=" + o.SysName + "&action=createnew&returnurl=" + Query.CreateReturnUrl() + "&bgroup=" + Query.GetString("bgroup"));
                }
                
			}
		}
	}
}