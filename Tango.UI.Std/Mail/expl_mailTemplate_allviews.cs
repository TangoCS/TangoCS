using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tango.AccessControl;
using Tango.Data;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Mail
{
    [OnAction("mailTemplate", "viewlist")]
    public class expl_mailTemplate_viewlist : default_list_rep<DTO_MailTemplate>
    {
        [Inject] protected IMailRepository MailRepository { get; set; }
        protected override Func<string, Expression<Func<DTO_MailTemplate, bool>>> SearchExpression => s => 
            o => o.Title.ToLower().Contains(s.ToLower());
        
        protected override IRepository<DTO_MailTemplate> GetRepository() => MailRepository.GetMailTemplates();
        
        protected override void FieldsInit(FieldCollection<DTO_MailTemplate> fields)
        {
            fields.AddCellWithSortAndFilter(o => o.ID, o => o.ID);
            fields.AddCellWithSortAndFilter(o => o.Title, (w, o) => 
                w.ActionLink(al => al.To("mailTemplate", "view", AccessControl).WithArg(Constants.Id, o.ID).WithTitle(o.Title)));
            // fields.AddCellWithSortAndFilter(o => o.Subject, o=>o.Subject);
            // fields.AddCellWithSortAndFilter(o => o.Body, o => o.Body);
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
    public class expl_mailTemplate_view : default_view_rep<DTO_MailTemplate, int>
    {
        [Inject] protected IMailRepository MailRepository { get; set; }
        [Inject] protected AccessControlOptions AccessControlOptions { get; set; }
        [Inject] protected IAccessControl AccessControl { get; set; }
        private DTO_MailTemplateFields.DefaultGroup _group;
        protected override void Form(LayoutWriter w)
        {
            var devMode = AccessControlOptions.DeveloperAccess(AccessControl);
            w.FieldsBlockStd(() =>
            {
                w.PlainText(_group.Title);
                w.PlainText(_group.TemplateSubject);
                w.PlainText(_group.TemplateBody);
                w.PlainText(_group.Comment);
                if(devMode)
                    w.PlainText(_group.IsSystem);
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

        public override void OnInit()
        {
            _group = AddFieldGroup(new DTO_MailTemplateFields.DefaultGroup());
        }

        protected override DTO_MailTemplate GetExistingEntity()
        {
            var id = Context.GetArg<int>(Constants.Id);
            var obj = MailRepository.GetMailTemplates().GetById(id);
            return obj;
        }
    }
    
    [OnAction("mailTemplate", "createnew")]
    [OnAction("mailTemplate", "edit")]
    public class expl_mailTemplate_edit : default_edit_rep<DTO_MailTemplate, int>
    {
        [Inject] protected IMailRepository MailRepository { get; set; }
        [Inject] protected AccessControlOptions AccessControlOptions { get; set; }
        [Inject] protected IAccessControl AccessControl { get; set; }
        private DTO_MailTemplateFields.DefaultGroup _group;

        protected override void Form(LayoutWriter w)
        {
            var devMode = AccessControlOptions.DeveloperAccess(AccessControl);
            w.FieldsBlockStd(() =>
            {
                w.TextBox(_group.Title);
                w.TextBox(_group.TemplateSubject);
                w.TextArea(_group.TemplateBody);
                w.TextArea(_group.Comment);
                if(devMode)
                    w.ToggleSwitch(_group.IsSystem);
            });
        }
        
        public override void OnInit()
        {
            _group = AddFieldGroup(new DTO_MailTemplateFields.DefaultGroup());
        }

        protected override DTO_MailTemplate GetExistingEntity()
        {
            var id = Context.GetArg<int>(Constants.Id);
            var obj = MailRepository.GetMailTemplates().GetById(id);
            return obj;
        }
        
        protected override void Submit(ApiResponse response)
        {
            if (EntityAudit != null && ViewData != null)
            {
                if (!CreateObjectMode)
                {
                    if (EntityAudit != null)
                        EntityAudit.PrimaryObject.PropertyChanges = Tracker?.GetChanges(ViewData);
                }
            }

            if (CreateObjectMode)
                InTransaction(() =>
                {
                    MailRepository.CreateMailTemplate(ViewData);
                });
            else
            {
                InTransaction(() =>
                {
                    MailRepository.UpdateMailTemplate(ViewData);
                });
            }
        }
    }
    
    [OnAction("mailTemplate", "delete")]
    public class expl_mailTemplate_delete : default_delete<DTO_MailTemplate, int>
    {
        [Inject] protected IMailRepository MailRepository { get; set; }
        protected override void Delete(IEnumerable<int> ids)
        {
            MailRepository.DeleteMailTemplate(ids);
        }
    }
}