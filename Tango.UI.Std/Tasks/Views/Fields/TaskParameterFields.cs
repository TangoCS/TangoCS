using System;
using System.Text.RegularExpressions;
using Tango.Localization;
using Tango.UI;

namespace Tango.Tasks
{
    public static class TaskParameterFields
    {
        public class SysName : EntityField<TaskParameter, string>
        {
            public override bool IsRequired => true;
        }

        public class Value : EntityField<TaskParameter, string> { }

        public class DefaultGroup : FieldGroup
        {
            public CommonFields.Title Title { get; set; }
            public SysName SysName { get; set; }
            public Value Value { get; set; }
        }
    }
}
