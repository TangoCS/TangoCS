using System.Linq;
using Tango.UI;

namespace Tango.Mail
{
    public static class C_MailCategoryFields
    {
        public class SystemName : EntityField<C_MailCategory, string>
        {
        }
        public class SystemID : EntityField<C_MailCategory, int>
        {
        }
        public class MailCategoryTypeTitle : EntityField<C_MailCategory, string>
        {
        }

        public class MailCategoryTypeID : EntityField<C_MailCategory, int>
        {
        }

        public class DefaultGroup : FieldGroup
        {
            public CommonFields.Title Title { get; set; }
            public SystemName SystemName { get; set; }
            public SystemID SystemID { get; set; }
            public MailCategoryTypeTitle MailCategoryTypeTitle { get; set; }
            public MailCategoryTypeID MailCategoryTypeID { get; set; }
        }
    }
}
