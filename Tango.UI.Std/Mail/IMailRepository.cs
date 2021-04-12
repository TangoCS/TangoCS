using System;
using System.Collections.Generic;
using Tango.Data;
using Tango.UI.Controls;

namespace Tango.Mail
{
    public interface IMailCategoryRepository : IRepository<C_MailCategory>
    {
        IEnumerable<(string, int)> GetSystemNames();
    }

    public interface IMailTemplateRepository : IRepository<MailTemplate>
    {
    }

    public interface IMailSettingsRepository : IRepository<MailSettings>
    {
        string GetMailTemplateSql();
        string GetMailCategorySql();
    }

    public interface IMailSettingsTemplateRepository : IRepository<MailTemplate>
    {
        string GetMailTemplateWhereMailSettingsIdSql();
        //string DeleteSql();
    }
}