using System.Collections.Generic;
using Tango.Data;

namespace Tango.Mail
{
    public interface IMailRepository
    {
        #region Mail Category

        IRepository<DTO_C_MailCategory> GetMailCategories();
        void DeleteMailCategory(IEnumerable<int> ids);
        IEnumerable<(string, int)> GetSystemNames();
        int CreateMailCategory(DTO_C_MailCategory mailCategory);
        void UpdateMailCategory(DTO_C_MailCategory mailCategory);
        
        #endregion

        #region Mail Template

        IRepository<DTO_MailTemplate> GetMailTemplates();
        void DeleteMailTemplate(IEnumerable<int> ids);
        void UpdateMailTemplate(DTO_MailTemplate viewData);
        int CreateMailTemplate(DTO_MailTemplate viewData);
        
        #endregion
    }
}