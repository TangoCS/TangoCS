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
        public class MailType : EntityField<C_MailCategory, int>
        {
            public override int DefaultValue => 0;
            public override void ValidateFormValue(ValidationMessageCollection val) { }

            public override string StringValue => ViewData.GetMailTypes().FirstOrDefault(i => i.Value == Value.ToString())?.Text;
        }
        
        public class DefaultGroup : FieldGroup
        {
            public CommonFields.Title Title { get; set; }
            public SystemName SystemName { get; set; }
            public SystemID SystemID { get; set; }
            public MailType MailType { get; set; }
        }
    }
}