using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tango.AccessControl;
using Tango.Data;
using Tango.Identity.Std;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Mail
{
    [OnAction("mailTemplate", "viewlist")]
    public class expl_mailTemplate_viewlist : default_list_rep<MailTemplate>
    {
        protected override Func<string, Expression<Func<MailTemplate, bool>>> SearchExpression => s => 
            o => o.Title.ToLower().Contains(s.ToLower());
        
        protected override void FieldsInit(FieldCollection<MailTemplate> fields)
        {
            fields.AddCellWithSortAndFilter(o => o.ID, o => o.ID);
            fields.AddCellWithSortAndFilter(o => o.Title, (w, o) => 
                w.ActionLink(al => al.To("mailTemplate", "view", AccessControl).WithArg(Constants.Id, o.ID).WithTitle(o.Title)));
            fields.AddCellWithSortAndFilter(o => o.TemplateSubject, o=>o.TemplateSubject);
            fields.AddCellWithSortAndFilter(o => o.TemplateBody, o => o.TemplateBody);
            fields.AddActionsCell(
                o => al => al.To("mailTemplate", "edit", AccessControl).WithArg(Constants.Id, o.ID)
                    .WithImage("edit").WithTitle("Редактировать"),
                o => al => al.To("mailTemplate", "delete", AccessControl).WithArg(Constants.Id, o.ID)
                    .WithImage("delete").WithTitle("Удалить")
            );
        }
        
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemFilter(Filter);
            t.ToCreateNew("mailTemplate", "createnew");
            ToDeleteBulk(t);
        }
    }
    
    [OnAction("mailTemplate", "view")]
    public class expl_mailTemplate_view : default_view_rep<MailTemplate, int, IRepository<MailTemplate>>
    {
        [Inject] protected AccessControlOptions AccessControlOptions { get; set; }
        protected DTO_MailTemplateFields.DefaultGroup Group { get; set; }
        protected override void Form(LayoutWriter w)
        {
            var devMode = AccessControlOptions.DeveloperAccess(AccessControl);
            w.FieldsBlockStd(() =>
            {
                w.PlainText(Group.Title);
                w.PlainText(Group.TemplateSubject);
                w.PlainText(Group.TemplateBody);
                w.PlainText(Group.Comment);
                if(devMode)
                    w.PlainText(Group.IsSystem);
                w.PlainText(Group.LastModifiedDate);
            });
        }
        
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemBack();
            t.ItemActionImageText(x => x.To("mailTemplate", "edit", AccessControl, null, Context.ReturnUrl.Get(1))
                .WithImage("edit")
                .WithArg(Constants.Id, ViewData.ID));
            t.ItemSeparator();
            t.ItemActionImageText(x => x.To("mailTemplate", "delete", AccessControl, null, Context.ReturnUrl.Get(1))
                .WithImage("delete")
                .WithArg(Constants.Id, ViewData.ID));
        }
    }
    
    [OnAction("mailTemplate", "createnew")]
    [OnAction("mailTemplate", "edit")]
    public class expl_mailTemplate_edit : default_edit_rep<MailTemplate, int>
    {
        [Inject] protected AccessControlOptions AccessControlOptions { get; set; }
        [Inject] protected IAccessControl AccessControl { get; set; }
        
        protected DTO_MailTemplateFields.DefaultGroup Group { get; set; }

        protected override void Form(LayoutWriter w)
        {
            var devMode = AccessControlOptions.DeveloperAccess(AccessControl);
            w.FieldsBlockStd(() =>
            {
                w.TextBox(Group.Title);
                w.TextBox(Group.TemplateSubject);
                w.TextArea(Group.TemplateBody);
                w.TextArea(Group.Comment);
                if(devMode)
                    w.ToggleSwitch(Group.IsSystem);
                if(!CreateObjectMode)
                    w.PlainText(Group.LastModifiedDate);
            });
        }
    }
    
    [OnAction("mailTemplate", "delete")]
    public class expl_mailTemplate_delete : default_delete<MailTemplate, int>
    {
    }
}