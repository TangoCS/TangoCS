using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tango.AccessControl;
using Tango.Data;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Mail
{
    [OnAction("mailCategory", "viewlist")]
    public class C_MailCategory_viewlist : default_list_rep<C_MailCategory>
    {
        protected override Func<string, Expression<Func<C_MailCategory, bool>>> SearchExpression => s => 
            o => o.Title.ToLower().Contains(s.ToLower());

        protected override void FieldsInit(FieldCollection<C_MailCategory> fields)
        {
            fields.AddCellWithSortAndFilter(o => o.MailCategoryID, o => o.MailCategoryID);
            fields.AddCellWithSortAndFilter(o => o.Title, (w, o) => 
                w.ActionLink(al => al.To("mailCategory", "view", AccessControl).WithArg(Constants.Id, o.ID).WithTitle(o.Title)));
            fields.AddCellWithSortAndFilter(o => o.SystemName, o=>o.SystemName);
            fields.AddCellWithSortAndFilter(o => o.MailCategoryTypeTitle, o => o.MailCategoryTypeTitle);
            fields.AddActionsCell(
                o => al => al.To("mailCategory", "edit", AccessControl).WithArg(Constants.Id, o.ID)
                    .WithImage("edit").WithTitle("Редактировать"),
                o => al => al.To("mailCategory", "delete", AccessControl).WithArg(Constants.Id, o.ID)
                    .WithImage("delete").WithTitle("Удалить")
            );
        }
        
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemFilter(Filter);
            t.ToCreateNew("mailCategory", "createnew");
            ToDeleteBulk(t);
        }
    }

    [OnAction("mailCategory", "createnew")]
    [OnAction("mailCategory", "edit")]
    public class C_MailCategory_edit : default_edit_rep<C_MailCategory, int, IMailCategoryRepository>
    {
        protected C_MailCategoryFields.DefaultGroup Group { get; set; }

        private IEnumerable<SelectListItem> GetSystemNames() => Repository.GetSystemNames().OrderBy(x => x.title)
            .Select(o => new SelectListItem(o.title, o.id));
        private IEnumerable<SelectListItem> GetMailTypes() => Repository.GetMailCategoryTypes().OrderBy(x => x.title)
            .Select(o => new SelectListItem(o.title, o.id));
        
        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.TextBox(Group.Title);
                w.DropDownList(Group.SystemID, GetSystemNames());
                w.DropDownList(Group.MailCategoryTypeTitle, GetMailTypes());
            });
        }
    }

    [OnAction("mailCategory", "view")]
    public class C_MailCategory_view : default_view_rep<C_MailCategory, int, IMailCategoryRepository>
    {
        protected C_MailCategoryFields.DefaultGroup Group { get; set; }
        
        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.PlainText(Group.Title);
                w.PlainText(Group.SystemName);
                w.PlainText(Group.MailCategoryTypeTitle);
            });
        }
        
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemBack();
            t.ItemActionImageText(x => x.To("mailCategory", "edit", AccessControl, null, Context.ReturnUrl.Get(1))
                .WithImage("edit")
                .WithArg(Constants.Id, ViewData.ID));
            t.ItemSeparator();
            t.ItemActionImageText(x => x.To("mailCategory", "delete", AccessControl, null, Context.ReturnUrl.Get(1))
                .WithImage("delete")
                .WithArg(Constants.Id, ViewData.ID));
        }
    }

    [OnAction("mailCategory", "delete")]
    public class C_MailCategory_delete : default_delete<C_MailCategory, int>
    {
    }
}