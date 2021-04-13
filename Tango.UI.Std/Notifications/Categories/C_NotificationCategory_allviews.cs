using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tango.AccessControl;
using Tango.Data;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Notifications
{
    [OnAction("notificationCategory", "viewlist")]
    public class C_NotificationCategory_viewlist : default_list_rep<C_NotificationCategory>
    {
        protected override Func<string, Expression<Func<C_NotificationCategory, bool>>> SearchExpression => s => 
            o => o.Title.ToLower().Contains(s.ToLower());

        protected override void FieldsInit(FieldCollection<C_NotificationCategory> fields)
        {
            fields.AddCellWithSortAndFilter(o => o.ID, o => o.ID);
            fields.AddCellWithSortAndFilter(o => o.Title, (w, o) => 
                w.ActionLink(al => al.To("notificationCategory", "view", AccessControl).WithArg(Constants.Id, o.ID).WithTitle(o.Title)));
            fields.AddCellWithSortAndFilter(o => o.Icon, o => o.Icon);
            fields.AddActionsCell(
                o => al => al.To("notificationCategory", "edit", AccessControl).WithArg(Constants.Id, o.ID)
                    .WithImage("edit").WithTitle("Редактировать")//,
                //o => al => al.To("notificationCategory", "delete", AccessControl).WithArg(Constants.Id, o.ID)
                //    .WithImage("delete").WithTitle("Удалить")
            );
        }
        
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemFilter(Filter);
            //t.ToCreateNew("mailCategory", "createnew");
            //ToDeleteBulk(t);
        }
    }

    [OnAction("notificationCategory", "createnew")]
    [OnAction("notificationCategory", "edit")]
    public class C_NotificationCategory_edit : default_edit_rep<C_NotificationCategory, int, INotificationCategoryRepository>
    {
        protected C_NotificationCategoryFields.DefaultGroup Group { get; set; }
                
        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.PlainText("ID", ViewData.NotificationCategoryID);
                w.TextBox(Group.Title);
                w.TextBox(Group.Icon);
            });
        }
    }

    [OnAction("notificationCategory", "view")]
    public class C_NotificationCategory_view : default_view_rep<C_NotificationCategory, int, INotificationCategoryRepository>
    {
        protected C_NotificationCategoryFields.DefaultGroup Group { get; set; }
        
        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.PlainText("ID", ViewData.NotificationCategoryID);
                w.PlainText(Group.Title);
                w.PlainText(Group.Icon);
            });
        }
        
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemBack();
            t.ItemActionImageText(x => x.To("notificationCategory", "edit", AccessControl, null, Context.ReturnUrl.Get(1))
                .WithImage("edit")
                .WithArg(Constants.Id, ViewData.ID));
            //t.ItemSeparator();
            //t.ItemActionImageText(x => x.To("mailCategory", "delete", AccessControl, null, Context.ReturnUrl.Get(1))
            //    .WithImage("delete")
            //    .WithArg(Constants.Id, ViewData.ID));
        }
    }

    [OnAction("notificationCategory", "delete")]
    public class C_NotificationCategory_delete : default_delete<C_NotificationCategory, int>
    {
    }
}