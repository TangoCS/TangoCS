using System.Collections.Generic;
using Tango.Data;

namespace Tango.Mail
{
    public interface IMailCategoryRepository : IRepository<C_MailCategory>
    {
        IEnumerable<(string, int)> GetSystemNames();
    }

    public interface IMailTemplateRepository : IRepository<MailTemplate>
    {
    }
}