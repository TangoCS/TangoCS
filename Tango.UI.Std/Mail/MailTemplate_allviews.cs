using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tango.AccessControl;
using Tango.Data;
using Tango.Identity;
using Tango.Identity.Std;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Mail
{
    [OnAction(typeof(MailTemplate), "viewlist")]
    public class MailTemplate_viewlist : default_list_rep<MailTemplate>
    {
        protected override Func<string, Expression<Func<MailTemplate, bool>>> SearchExpression => s => 
            o => o.Title.ToLower().Contains(s.ToLower());
        
        protected override void FieldsInit(FieldCollection<MailTemplate> fields)
        {
            fields.AddCellWithSortAndFilter(o => o.ID, o => o.ID);
            fields.AddCellWithSortAndFilter(o => o.Title, (w, o) => 
                w.ActionLink(al => al.ToView<MailTemplate>(AccessControl, o.ID).WithTitle(o.Title)));
            fields.AddCellWithSortAndFilter(o => o.TemplateSubject, o=>o.TemplateSubject);
            fields.AddCellWithSortAndFilter(o => o.TemplateBody, o => o.TemplateBody);
            fields.AddActionsCell(
                o => al => al.ToEdit<MailTemplate>(AccessControl, o.ID)
                    .WithImage("edit").WithTitle("Редактировать"),
                o => al => al.ToDelete<MailTemplate>(AccessControl, o.ID)
                    .WithImage("delete").WithTitle("Удалить")
            );
        }

        // protected override void ToolbarLeft(MenuBuilder t)
        // {
        //     t.ItemFilter(Filter);
        //     t.ToCreateNew("mailTemplate", "createnew");
        //     ToDeleteBulk(t);
        // }
    }
    
    [OnAction(typeof(MailTemplate), "view")]
    public class MailTemplate_view : default_view_rep<MailTemplate, int, IRepository<MailTemplate>>
    {
        [Inject] protected AccessControlOptions AccessControlOptions { get; set; }
        protected MailTemplateFields.DefaultGroup Group { get; set; }
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
    }
    
    [OnAction(typeof(MailTemplate), "createnew")]
    [OnAction(typeof(MailTemplate), "edit")]
    public class MailTemplate_edit : default_edit_rep<MailTemplate, int>
    {
        [Inject] protected AccessControlOptions AccessControlOptions { get; set; }
        [Inject] protected IAccessControl AccessControl { get; set; }
        [Inject] protected IUserIdAccessor<object> UserIdAccessor { get; set; }
        
        protected MailTemplateFields.DefaultGroup Group { get; set; }

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

        protected override MailTemplate GetNewEntity()
        {
            var obj = new MailTemplate();
            SetDefaultValues(obj);
            return obj;
        }

        protected override void SetDefaultValues(MailTemplate obj)
        {
            obj.CreateDate = DateTime.Now;
            obj.LastModifiedDate = DateTime.Now;
            var i = IdentityManager;
            obj.LastModifiedUserID = UserIdAccessor.CurrentUserID;
        }
    }
    
    [OnAction(typeof(MailTemplate), "delete")]
    public class MailTemplate_delete : default_delete<MailTemplate, int>
    {
    }
}