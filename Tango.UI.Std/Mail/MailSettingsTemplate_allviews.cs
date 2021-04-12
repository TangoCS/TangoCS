using System.Collections.Generic;
using System.Linq;
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