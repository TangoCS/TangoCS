using System.Linq;
using Tango.UI;

namespace Tango.Notifications
{
    public static class C_NotificationCategoryFields
    {
        public class Icon : EntityField<C_NotificationCategory, string>
        {            
        }
        
        public class DefaultGroup : FieldGroup
        {
            public CommonFields.Title Title { get; set; }
            public Icon Icon { get; set; }
        }
    }
}