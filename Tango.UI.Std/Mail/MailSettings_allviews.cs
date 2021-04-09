using System;
using System.Linq.Expressions;
using Tango.Data;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Mail
{
    [OnAction(typeof(MailSettings), "viewlist")]
    public class MailSettings_viewlist : default_list_rep<MailSettings>
    {
        protected override Func<string, Expression<Func<MailSettings, bool>>> SearchExpression => s => 
            o => o.Title.ToLower().Contains(s.ToLower());
        
        protected override void FieldsInit(FieldCollection<MailSettings> fields)
        {
            fields.AddCellWithSortAndFilter(o => o.ID, o => o.ID);
            fields.AddCellWithSortAndFilter(o => o.Title, (w, o) => 
                w.ActionLink(al => al.To("mailSettings", "view", AccessControl).WithArg(Constants.Id, o.ID).WithTitle(o.Title)));
            fields.AddCellWithSortAndFilter(o => o.MailTemplateTitle, o=>o.MailTemplateTitle);
            fields.AddCellWithSortAndFilter(o => o.AttemptsToSendCount, o=>o.AttemptsToSendCount);
            fields.AddCellWithSortAndFilter(o => o.TimeoutValue, o=>o.TimeoutValue);
            fields.AddCellWithSortAndFilter(o => o.MailCategoryTitle, o=>o.MailCategoryTitle);
            fields.AddCellWithSortAndFilter(o => o.CreateMailMethod, o => o.CreateMailMethod);
            fields.AddCellWithSortAndFilter(o => o.PostProcessingMethod, o => o.PostProcessingMethod);
            fields.AddCellWithSortAndFilter(o => o.RecipientsMethod, o => o.RecipientsMethod);
            fields.AddCell(o => o.SendMailStartInterval, o => o.SendMailStartInterval);
            fields.AddCell(o => o.SendMailFinishInterval, o => o.SendMailFinishInterval);
            fields.AddActionsCell(
                o => al => al.To("mailSettings", "edit", AccessControl).WithArg(Constants.Id, o.ID)
                    .WithImage("edit").WithTitle("Редактировать"),
                o => al => al.To("mailSettings", "delete", AccessControl).WithArg(Constants.Id, o.ID)
                    .WithImage("delete").WithTitle("Удалить")
            );
        }
        
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemFilter(Filter);
            t.ToCreateNew("mailSettings", "createnew");
            ToDeleteBulk(t);
        }
    }
    
    [OnAction(typeof(MailSettings), "view")]
    public class MailSettings_view : default_view_rep<MailSettings, int, IRepository<MailSettings>>
    {
        protected MailSettingsFields.DefaultGroup Group { get; set; }
        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.PlainText(Group.Title);
                w.PlainText(Group.MailTemplateTitle);
                w.PlainText(Group.CreateMailMethod);
                w.PlainText(Group.PostProcessingMethod);
                w.PlainText(Group.RecipientsMethod);
                w.PlainText(Group.TimeoutValue);
                w.PlainText(Group.SendMailStartInterval);
                w.PlainText(Group.SendMailFinishInterval);
                w.PlainText(Group.AttemptsToSendCount);
                w.PlainText(Group.MailCategoryTitle);
                w.PlainText(Group.LastModifiedDate);
            });
        }
    }
    
    [OnAction(typeof(MailSettings), "createnew")]
    [OnAction(typeof(MailSettings), "edit")]
    public class MailSettings_edit : default_edit_rep<MailSettings, int, IMailSettingsRepository>
    {
        // [Inject] protected AccessControlOptions AccessControlOptions { get; set; }
        // [Inject] protected IAccessControl AccessControl { get; set; }
        // [Inject] protected IUserIdAccessor<object> UserIdAccessor { get; set; }
        
        private SelectSingleObjectField<MailTemplate, int> _selectMailTemplate;
        private SelectSingleObjectField<C_MailCategory, int> _selectMailCategory;

        public override void OnInit()
        {
            base.OnInit();
            _selectMailTemplate = CreateControl("mailtemplate", Repository.GetMailTemplateObjectField());
        }

        protected MailSettingsFields.DefaultGroup Group { get; set; }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.TextBox(Group.Title);
                w.SelectSingleObject(Group.MailTemplate, _selectMailTemplate);
                //w.PlainText(Group.MailTemplateTitle);
                w.TextBox(Group.CreateMailMethod);
                w.TextBox(Group.PostProcessingMethod);
                w.TextBox(Group.RecipientsMethod);
                w.TextBox(Group.TimeoutValue);
                w.TextBox(Group.SendMailStartInterval);
                w.TextBox(Group.SendMailFinishInterval);
                w.TextBox(Group.AttemptsToSendCount);
                w.PlainText(Group.MailCategoryTitle);
                w.TextBox(Group.LastModifiedDate);
            });
            // var devMode = AccessControlOptions.DeveloperAccess(AccessControl);
            // w.FieldsBlockStd(() =>
            // {
            //     w.TextBox(Group.Title);
            //     w.TextBox(Group.TemplateSubject);
            //     w.TextArea(Group.TemplateBody);
            //     w.TextArea(Group.Comment);
            //     if(devMode)
            //         w.ToggleSwitch(Group.IsSystem);
            //     if(!CreateObjectMode)
            //         w.PlainText(Group.LastModifiedDate);
            // });
        }

        protected override MailSettings GetNewEntity()
        {
            var obj = new MailSettings();
            SetDefaultValues(obj);
            return obj;
        }

        protected override void SetDefaultValues(MailSettings obj)
        {
            obj.CreateDate = DateTime.Now;
            obj.LastModifiedDate = DateTime.Now;
            var i = IdentityManager;
            //obj.LastModifiedUserID = UserIdAccessor.CurrentUserID;
        }
    }
    
    [OnAction(typeof(MailSettings), "delete")]
    public class MailSettings_delete : default_delete<MailSettings, int>
    {
    }
}