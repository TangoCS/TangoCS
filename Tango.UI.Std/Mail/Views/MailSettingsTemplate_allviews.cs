using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Tango.Data;
using Tango.UI;
using Tango.UI.Std;

namespace Tango.Mail
{
    public class MailSettingsTemplate_list : default_list_rep<MailTemplate>
    {
        public int MailSettingsID { get; set; }
        
        [Inject] protected IMailSettingsTemplateRepository MailSettingsTemplateRepository { get; set; }

        protected override IRepository<MailTemplate> GetRepository() => Database.Repository<MailTemplate>()
            .WithAllObjectsQuery(MailSettingsTemplateRepository.GetMailTemplateWhereMailSettingsIdSql(), 
                new {@mailSettingsId = MailSettingsID});

        protected override void FieldsInit(FieldCollection<MailTemplate> fields)
        {
            fields.AddCellWithSortAndFilter(o => o.ID, o => o.ID);
            fields.AddCellWithSortAndFilter(o => o.Title, o => o.Title);
            fields.AddCellWithSortAndFilter(o => o.TemplateSubject, o=>o.TemplateSubject);
            fields.AddCellWithSortAndFilter(o => o.TemplateBody, o => o.TemplateBody);
            fields.AddActionsCell(
                o => al => al.ToDelete<MailSettingsTemplate>(AccessControl)
                    .WithArg("s", MailSettingsID)
                    .WithArg("t", o.ID)
                    .WithImage("delete").WithTitle("Удалить")
            );
        }
    }

    [OnAction(typeof(MailSettingsTemplate), "createnew")]
    public class MailSettingsTemplate_new : default_edit_rep<MailSettingsTemplate, int, IMailSettingsTemplateRepository>
    {
        public static readonly DateTime StartDate = new DateTime(1900, 1, 1, 0, 0, 0);
        public static readonly DateTime FinishDate = new DateTime(2099, 12, 31, 23, 59, 0);
        
        private IEnumerable<SelectListItem> _selectMailTemplate;
        
        protected MailSettingsTemplateFields.DefaultGroup Group { get; set; }

        public override void OnInit()
        {
            _selectMailTemplate = Database.Connection.Query<MailTemplate>(Repository.GetMailTemplateSql()).ToList()
                .OrderBy(x => x.MailTemplateID)
                .Select(o => new SelectListItem(o.Title, o.MailTemplateID));
        }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                if (CreateObjectMode)
                    w.DropDownList(Group.MailTemplateID, _selectMailTemplate);
            });
        }
        
        protected override MailSettingsTemplate GetNewEntity()
        {
            var obj = new MailSettingsTemplate();
            SetDefaultValues(obj);
            return obj;
        }

        protected override void SetDefaultValues(MailSettingsTemplate obj)
        {
            obj.FinishDate = FinishDate;
            obj.StartDate = StartDate;
            obj.MailSettingsID = Context.GetIntArg("s", 0);
        }
    }

    [OnAction(typeof(MailSettingsTemplate), "delete")]
    public class MailSettingsTemplate_delete : default_delete<MailSettingsTemplate, int>
    {
        protected override void Delete(IEnumerable<int> ids)
        {
            var settingId = Context.GetIntArg("s");
            var templateId = Context.GetIntArg("t");
            var mst = Database.Repository<MailSettingsTemplate>().List()
                .FirstOrDefault(i => i.MailSettingsID == settingId && i.MailTemplateID == templateId);
            Database.Repository<MailSettingsTemplate>().Delete(new[] {mst.MailSettingsTemplateID});
        }
    }
}