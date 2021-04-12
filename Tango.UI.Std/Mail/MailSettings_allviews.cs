using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using Tango.Data;
using Tango.Identity;
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
            fields.AddCellWithSortAndFilter(o => o.Title, o => o.Title);
            fields.AddCellWithSortAndFilter(o => o.MailTemplateTitle, o=>o.MailTemplateTitle);
            fields.AddCellWithSortAndFilter(o => o.MailCategoryTitle, o=>o.MailCategoryTitle);
            fields.AddCellWithSortAndFilter(o => o.AttemptsToSendCount, o=>o.AttemptsToSendCount);
            fields.AddCellWithSortAndFilter(o => o.TimeoutValue, o=>o.TimeoutValue);
            fields.AddCellWithSortAndFilter(o => o.CreateAttachmentMethod, o => o.CreateAttachmentMethod);
            fields.AddCellWithSortAndFilter(o => o.PostProcessingMethod, o => o.PostProcessingMethod);
            fields.AddCellWithSortAndFilter(o => o.RecipientsMethod, o => o.RecipientsMethod);
            fields.AddCellWithSortAndFilter(o => o.SystemName, o => o.SystemName);
            fields.AddCell(o => o.SendMailDayInterval, o => o.SendMailDayInterval);
            fields.AddCell(o => o.SendMailStartInterval, o => o.SendMailStartInterval);
            fields.AddCell(o => o.SendMailFinishInterval, o => o.SendMailFinishInterval);
            fields.AddActionsCell(
                o => al => al.ToView<MailSettings>(AccessControl, o.ID)
                    .WithImage("mail").WithTitle("Тема и Текст письма"),
                o => al => al.ToView<MailSettings>(AccessControl, o.ID)
                    .WithImage("hie").WithTitle("Состав письма"),
                o => al => al.ToEdit<MailSettings>(AccessControl, o.ID)
                    .WithImage("edit").WithTitle("Редактировать"),
                o => al => al.ToDelete<MailSettings>(AccessControl, o.ID)
                    .WithImage("delete").WithTitle("Удалить"),
                o =>
                {
                    if (!o.HasTemplate)
                    {
                        return al =>
                        {
                            al.ToView<MailSettings>(AccessControl, o.ID)
                                .WithImage("warning").WithTitle("На текущий момент для письма не определен шаблон");
                        };
                    }

                    return null;
                });
        }
    }
    
    [OnAction(typeof(MailSettings), "view")]
    public class MailSettings_view : default_view_rep<MailSettings, int, IRepository<MailSettings>>
    {
        private MailSettingsTemplate_list _mailSettingsTemplateList;

        public override void OnInit()
        {
            base.OnInit();
            _mailSettingsTemplateList = CreateControl<MailSettingsTemplate_list>("mstlst", c => {
                c.MailSettingsID = ViewData.MailSettingsID;
                c.Sections.RenderContentTitle = false;
            });
        }

        protected MailSettingsFields.DefaultGroup Group { get; set; }
        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.PlainText(Group.Title);
                w.PlainText(Group.MailCategoryTitle);
                w.PlainText(Group.CreateMailMethod);
                w.PlainText(Group.PostProcessingMethod);
                w.PlainText(Group.RecipientsMethod);
                w.PlainText(Group.TimeoutValue);
                w.PlainText(Group.SystemName);
                w.PlainText(Group.SendMailDayInterval);
                w.PlainText(Group.SendMailStartInterval);
                w.PlainText(Group.SendMailFinishInterval);
                w.PlainText(Group.AttemptsToSendCount);
                w.PlainText(Group.LastModifiedDate);
            });
        }

        protected override void LinkedData(LayoutWriter w)
        {
            w.GroupTitle(() =>
            {
                w.Write("Шаблон письма");
                if (!ViewData.HasTemplate)
                {
                    w.Write("&nbsp;");
                    w.ActionImage(al => al.ToCreateNew<MailSettingsTemplate>(AccessControl)
                        .WithArg("s", ViewData.ID)
                        .WithTitle("Добавить шаблон")
                        .WithImage("new2"));
                }
            });
            _mailSettingsTemplateList.Render(w);
        }
    }
    
    [OnAction(typeof(MailSettings), "createnew")]
    [OnAction(typeof(MailSettings), "edit")]
    public class MailSettings_edit : default_edit_rep<MailSettings, int, IMailSettingsRepository>
    {
        [Inject] protected IUserIdAccessor<object> UserIdAccessor { get; set; }
        
        private IEnumerable<SelectListItem> _selectMailTemplate;
        private IEnumerable<SelectListItem> _selectMailCategory;

        public override void OnInit()
        {
            base.OnInit();
            _selectMailTemplate = Database.Connection.Query<MailTemplate>(Repository.GetMailTemplateSql()).ToList()
                .OrderBy(x => x.MailTemplateID)
                .Select(o => new SelectListItem(o.Title, o.MailTemplateID));
            _selectMailCategory = Database.Connection.Query<C_MailCategory>(Repository.GetMailCategorySql()).ToList()
                .OrderBy(x => x.MailCategoryID)
                .Select(o => new SelectListItem(o.Title, o.MailCategoryID));
        }

        protected MailSettingsFields.DefaultGroup Group { get; set; }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.TextBox(Group.Title);
                if(CreateObjectMode)
                    w.DropDownList(Group.MailTemplateID, _selectMailTemplate);
                w.DropDownList(Group.MailCategoryID, _selectMailCategory);
                w.TextBox(Group.CreateMailMethod);
                w.TextBox(Group.PostProcessingMethod);
                w.TextBox(Group.RecipientsMethod);
                w.TextBox(Group.TimeoutValue);
                w.TextBox(Group.SystemName);
                w.TextBox(Group.SendMailDayInterval);
                w.TextBox(Group.SendMailStartInterval);
                w.TextBox(Group.SendMailFinishInterval);
                w.TextBox(Group.AttemptsToSendCount);
            });
        }

        protected override void FieldsPreInit()
        {
            base.FieldsPreInit();
            Group.MailTemplateID.CanRequired = CreateObjectMode;
        }

        public static readonly DateTime StartDate = new DateTime(1900, 1, 1, 0, 0, 0);
        public static readonly DateTime FinishDate = new DateTime(2099, 12, 31, 23, 59, 0);
        
        protected override void AfterSaveEntity()
        {
            base.AfterSaveEntity();
            
            if (CreateObjectMode)
            {
                var rep = Database.Repository<MailSettingsTemplate>();
                var mailSettingsTemplate = new MailSettingsTemplate
                {
                    MailTemplateID = Group.MailTemplateID.Value,
                    MailSettingsID = ViewData.MailSettingsID,
                    StartDate = StartDate,
                    FinishDate = FinishDate
                };
                rep.Create(mailSettingsTemplate);
            }
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
            obj.LastModifiedUserID = UserIdAccessor.CurrentUserID;
        }

        protected override MailSettings GetExistingEntity()
        {
            var obj = base.GetExistingEntity();
            obj.LastModifiedDate = DateTime.Now;
            obj.LastModifiedUserID = UserIdAccessor.CurrentUserID;

            return obj;
        }
    }
    
    [OnAction(typeof(MailSettings), "delete")]
    public class MailSettings_delete : default_delete<MailSettings, int>
    {
    }
}