using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Nephrite.Web;
using Nephrite.Metamodel;
using Nephrite.Metamodel.Model;
using Nephrite.Web.Controls;

namespace Nephrite.CMS.View
{
    public partial class SiteFolder_viewroot : ViewControl
    {
		protected List<Nephrite.CMS.Model.MM_ObjectType> RootClasses;
		protected List<Nephrite.CMS.Model.MM_ObjectType> OtherClasses;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetTitle("Корневой раздел");

            RootClasses = AppCMS.DataContext.MM_ObjectTypes.Where(o => o.SiteInfoobject.IsFolder == true && o.SiteInfoobject.IsRoot == true).OrderBy(o => o.Title).ToList();

            ToolbarPopupMenuCompact m = toolbar.AddPopupMenuCompact();
            m.Title = "Создать...";
            
            Repository r = new Repository();
            
            foreach (var c in RootClasses)
            {
                m.AddItem(c.Title, String.Format("?mode={0}&action=CreateNew&bgroup={1}&returnurl={2}",
                    c.SysName, Query.GetString("bgroup"), Query.CreateReturnUrl()));

                lists.Controls.Add(new LiteralControl("<div class='tabletitle' style='padding-left:8px'>" + c.Title + "</div>"));

                var t = Nephrite.Metamodel.Model.ObjectTypeRepository.Get(c.SysName);
                var objects = r.GetList(t);
                var tobj = r.Empty(t) as IChildObject;
                
                MView mv = new MView();
                lists.Controls.Add(mv);
                mv.ViewFormSysName = "list";

                if (tobj != null)
                {
                    var filtered = objects.Where(tobj.FilterByParentID<IMMObject>(0));
                    mv.SetViewData(c.SysName, filtered);
                }
                else
                {
                    mv.SetViewData(c.SysName, objects);
                }
            }

            // Добавить пункты меню от тех классов, которые не являются дочерними
            OtherClasses = AppCMS.DataContext.MM_ObjectTypes.
                Where(o => o.SiteInfoobject.IsFolder == false && !o.MM_ObjectProperties.Any(o1 => o1.SysName == "Parent" && o1.UpperBound == 1 && o1.TypeCode == ObjectPropertyType.Object)).OrderBy(o => o.Title).ToList();
            foreach (var c in OtherClasses)
            {
                m.AddItem(c.Title, String.Format("?mode={0}&action=CreateNew&bgroup={1}&returnurl={2}",
                    c.SysName, Query.GetString("bgroup"), Query.CreateReturnUrl()));
            }
        }
    }
}