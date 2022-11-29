using System;
using System.Text.RegularExpressions;
using Tango.Localization;
using Tango.UI;

namespace Tango.Tasks
{
    public static class TaskGroupFields
    {
		public class SeqNo : EntityField<TaskGroup, int> { }

		public class DefaultGroup : FieldGroup
        {
            public CommonFields.Title Title { get; set; }
			public SeqNo SeqNo { get; set; }
		}
    }
}
