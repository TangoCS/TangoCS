using System.Collections.Generic;
using Tango.Data;

namespace Tango.Mail
{
    public interface IMailCategoryRepository : IRepository<DTO_C_MailCategory>
    {
        IEnumerable<(string, int)> GetSystemNames();
    }

    public interface IMailTemplateRepository : IRepository<DTO_MailTemplate>
    {
    }
}