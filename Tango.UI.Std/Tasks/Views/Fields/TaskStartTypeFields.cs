﻿using System;
using System.Text.RegularExpressions;
using Tango.Localization;
using Tango.UI;

namespace Tango.Tasks
{
    public static class TaskStartTypeFields
    {
        public class DefaultGroup : FieldGroup
        {
            public CommonFields.Title Title { get; set; }
        }
    }
}
