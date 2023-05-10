using System;
using System.Collections.Generic;
using Tango.Data;
using Tango.UI.Controls;

namespace Tango.Mail
{
    public interface IMailCategoryRepository : IRepository<MailCategory>
    {
        IEnumerable<(string title, int id)> GetSystemNames();
        IEnumerable<(string title, int id)> GetMailCategoryTypes();
    }

    public interface IMailTemplateRepository : IRepository<MailTemplate>
    {
    }

    public interface IMailSettingsRepository : IRepository<MailSettings>
    {
        string GetMailTemplateSql();
        string GetMailCategorySql();
    }

    public interface IMailMessageRepository : IRepository<MailMessage>
    {
        string GetMailMessageByIdSql();
    }

    public interface IMailSettingsTemplateRepository : IRepository<MailSettingsTemplate>
    {
        string GetMailTemplateWhereMailSettingsIdSql();
        string GetMailTemplateSql();
    }

    public interface IMailHelperRepository
    {
        MailSettings GetMailSettingsBySystemName(string systemName);
        MailSettingsTemplate GetMailSettingsTemplateByMailSettingsId(int mailSettingsId);
    }
}