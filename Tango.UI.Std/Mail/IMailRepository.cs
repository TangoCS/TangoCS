using System.Collections.Generic;
using Tango.Data;

namespace Tango.Mail
{
    public interface IMailRepository
    {
        IRepository<DTO_C_MailCategory> GetMailCategories();
        void DeleteMailCategory(IEnumerable<int> ids);
        IEnumerable<(string, int)> GetSystemNames();
        int CreateMailCategory(DTO_C_MailCategory mailCategory);
        void UpdateMailCategory(DTO_C_MailCategory mailCategory);
    }
}