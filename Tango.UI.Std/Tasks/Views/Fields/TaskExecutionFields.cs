using System;
using System.Text.RegularExpressions;
using Tango.Localization;
using Tango.UI;

namespace Tango.Tasks
{
    public static class TaskExecutionFields
    {
        public class StartDate : EntityDateTimeField<TaskExecution> { }

        public class FinishDate : EntityNullableDateTimeField<TaskExecution> { }

        public class Task : EntityField<TaskExecution, int>
        {
            protected override string IDSuffix => "ID";
            public override string StringValue => ViewData.TaskName;
        }

        public class IsSuccessfull : EntityField<TaskExecution, bool> { }

        public class MachineName : EntityField<TaskExecution, string> { }

        public class LastModifiedDate : EntityDateTimeField<TaskExecution> { }

        public class LastModifiedUser : EntityField<TaskExecution, object>
        {
            protected override string IDSuffix => "ID";
            public override string StringValue => ViewData.UserName;
        }

        public class ResultXml : EntityField<TaskExecution, string> { }

        public class ExecutionLog : EntityField<TaskExecution, string> { }

        public class DefaultGroup : FieldGroup
        {
            public StartDate StartDate { get; set; }
            public FinishDate FinishDate { get; set; }
            public Task Task { get; set; }
            public IsSuccessfull IsSuccessfull { get; set; }
            public MachineName MachineName { get; set; }
            public LastModifiedDate LastModifiedDate { get; set; }
            public LastModifiedUser LastModifiedUser { get; set; }
            public ResultXml ResultXml { get; set; }
            public ExecutionLog ExecutionLog { get; set; }
        }
    }
}
