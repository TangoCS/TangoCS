using System.Linq;
using Tango.UI;

namespace Tango.Mail
{
    public static class DTO_C_MailCategoryFields
    {
        public class SystemName : EntityField<DTO_C_MailCategory, string>
        {
            public override string Hint { get; set; } = string.Empty;
        }
        public class SystemID : EntityField<DTO_C_MailCategory, int>
        {
            public override bool IsRequired { get; set; } = true;
            public override string Hint { get; set; } = string.Empty;
        }
        public class MailType : EntityField<DTO_C_MailCategory, int>
        {
            public override bool IsRequired { get; set; } = true;
            public override string Hint { get; set; } = string.Empty;
            public override string Caption { get; } = "Тип";
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