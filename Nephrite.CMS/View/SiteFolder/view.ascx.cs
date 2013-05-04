using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Nephrite.Metamodel;
using Nephrite.CMS.Model;
using Nephrite.Web.Controls;
using Nephrite.CMS.Controllers;
using Nephrite.Web.SPM;
//using Nephrite.CMS.Controllers;

namespace Nephrite.CMS.View
{
    public partial class SiteFolder_view : ViewControl<IMMObject>
    {
		protected MM_ObjectType CurrentMMType;
		protected List<MM_ObjectProperty> Child;
		protected List<MM_ObjectProperty> ChildObj;
        protected IMMObject ParentFolder;
            
        protected void Page_Load(object sender, EventArgs e)
        {
            SetTitle(ViewData.Title);

            folder.SetViewData(ViewData);

            CurrentMMType = AppCMS.DataContext.MM_ObjectTypes.SingleOrDefault(o => o.Guid == ViewData.MetaClass.ID);

            Child = CurrentMMType.MM_ObjectProperties.Where(o => o.RefObjectTypeID.HasValue && 
                o.RefObjectType.SiteInfoobject != null && o.RefObjectType.SiteInfoobject.IsFolder == true
                && o.UpperBound == -1).OrderBy(o => o.RefObjectType.Title).ToList();
            
            ChildObj = CurrentMMType.MM_ObjectProperties.Where(o => o.RefObjectTypeID.HasValue &&
                            (o.RefObjectType.SiteInfoobject == null || o.RefObjectType.SiteInfoobject.IsFolder == false)
                            && o.UpperBound == -1).OrderBy(o => o.RefObjectType.Title).ToList();

			IEnumerable<MM_ObjectType> ChildObjType = null;
			Type vdt = ViewData.GetType();
			if (vdt.GetInterface("ISiteSection", true) != null)
			{
				string s = "";
				object v = vdt.GetProperty("SiteObjectsClasses").GetValue(ViewData, null);
				if (v != null) s = v.ToString();
				List<int> l = s.Split(new char[] { ';' }).Select(o => o.ToInt32(0)).ToList();
				ChildObjType = AppCMS.DataContext.MM_ObjectTypes.Where(o => l.Contains(o.ObjectTypeID));
			}
			else
			{
				ChildObjType = ChildObj.Select(o => o.RefObjectType).SelectMany(o => o.MM_ObjectTypes.DefaultIfEmpty(o));
			}
            ToolbarPopupMenuCompact m = toolbar.AddPopupMenuCompact();
            m.Title = "Создать...";

            Repository r = new Repository();
            
            foreach (var c in Child)
            {
                m.AddItem(c.RefObjectType.Title, String.Format("?mode={0}&action=CreateNew&parent={1}&bgroup={2}&returnurl={3}",
                    c.RefObjectType.SysName, ViewData.ObjectID, Query.GetString("bgroup"), Query.CreateReturnUrl()));

                lists.Controls.Add(new LiteralControl("<div class='tabletitle' style='padding-left:8px'>" + c.Title + "</div>"));

                var t = Nephrite.Metamodel.Model.ObjectTypeRepository.Get(c.RefObjectType.SysName);
                var objects = r.GetList(t);
                var tobj = r.Empty(t) as IChildObject;
                var filtered = objects.Where(tobj.FilterByParentID<IMMObject>(ViewData.ObjectID));

                MView mv = new MView();
                lists.Controls.Add(mv);
                mv.ViewFormSysName = "list";
                mv.SetViewData(c.RefObjectType.SysName, filtered);
            }

            foreach (var c in ChildObjType)
            {
                m.AddItem(c.Title, String.Format("?mode={0}&action=CreateNew&parent={1}&bgroup={2}&returnurl={3}",
                    c.SysName, ViewData.ObjectID, Query.GetString("bgroup"), Query.CreateReturnUrl()));
            }

            foreach (var c in ChildObj)
            {
                lists.Controls.Add(new LiteralControl("<div class='tabletitle' style='padding-left:8px'>" + c.Title + "</div>"));

                var t = Nephrite.Metamodel.Model.ObjectTypeRepository.Get(c.RefObjectType.SysName);
                var objects = r.GetList(t);
                var tobj = r.Empty(t) as IChildObject;
                var filtered = objects.Where(tobj.FilterByParentID<IMMObject>(ViewData.ObjectID));

                MView mv = new MView();
                lists.Controls.Add(mv);
                mv.ViewFormSysName = "list";
                mv.SetViewData(c.RefObjectType.SysName, filtered);
            }

            toolbar.AddItemSeparator();
			var opEdit = ViewData.MetaClass.GetOperation("Edit");
			var opDelete = ViewData.MetaClass.GetOperation("Delete");

            toolbar.AddItem("edit.png", "Редактировать", ActionLink.To(opEdit).With(new HtmlParms { { "oid", ViewData.ObjectID.ToString() } }, true).Href);
			toolbar.AddItem("delete.gif", "Удалить", ActionLink.To(opDelete).With(new HtmlParms { { "oid", ViewData.ObjectID.ToString() } }, true).Href.RemoveQueryParameter("returnurl") + "&returnurl=" +
                (ParentFolder != null ? Query.CreateReturnUrl(Html.ActionUrl<SiteFolderController>(c => c.View(ParentFolder.ObjectID, ParentFolder.MetaClass.Name))) :
                Query.CreateReturnUrl(Html.ActionUrl<SiteFolderController>(c => c.ViewRoot()))));
            toolbar.AddItemSeparator();
            ParentFolder = GetParentSiteFolder(ViewData);
            if (ParentFolder != null)
                toolbar.AddItem<SiteFolderController>("upfolder.gif", "На уровень выше", c => c.View(ParentFolder.ObjectID, ParentFolder.MetaClass.Name));
            else
                toolbar.AddItem<SiteFolderController>("upfolder.gif", "На уровень выше", c => c.ViewRoot());
			
            /*if (vdt.GetInterface("ISiteSection", true) != null)
			{
                bool 
                string sysName = vdt.GetProperty("SysName").GetValue(ViewData, null).ToString();
                SPM_Action a = AppSPM.DataContext.SPM_Actions.SingleOrDefault(o => o.SystemName == sysName && o.Type == 1);
                if (a == null)
                    toolbar.AddItem<SiteFolderController>("enablespm.gif", "Разрешить УПБ", c => c.AddSPM());
                else
                    toolbar.AddItem<SiteFolderController>("disablespm.gif", "Запретить УПБ", c => c.DeleteSPM());
            }*/

        }

        protected IMMObject GetParentSiteFolder(IMMObject obj)
        {
            if (obj is IChildObject)
                return (obj as IChildObject).ParentObject as IMMObject;
            return null;
        }
    }
}