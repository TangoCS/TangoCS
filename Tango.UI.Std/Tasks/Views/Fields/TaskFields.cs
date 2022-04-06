using System;
using System.Text.RegularExpressions;
using Tango.Localization;
using Tango.UI;

namespace Tango.Tasks
{
	public static class TaskFields
	{
		public class StartFromService : EntityField<Task, bool>
        {
            public override bool DefaultValue => true;
            public override void SubmitProperty(ValidationMessageCollection val) { }
            public override void ValidateFormValue(ValidationMessageCollection val) { }
        }

		public class StartType : EntityField<Task, int>
		{
            protected override string IDSuffix => "ID";
            public override string StringValue => ViewData.StartTypeTitle;
		}

        public class Interval : EntityField<Task, string>
        {
            public override bool IsRequired => true;

            int _startType;
            public Interval(int startType)
            {
                _startType = startType;
            }

            public override string Caption => Resources.Get<Task>(o => o.Interval, Context.GetArg<int>("StartType") != 0 ? Context.GetArg("StartType") : _startType.ToString());
            public override bool ShowDescription => true;
            public override string Description
            {
                get
                {
					var cron = @"Отправка задается форматом<br/>
<b>м Ч Д М Н</b><br/>
м - минута (0-59)<br />
Ч - час в сутках (0-23)<br />
Д - день в месяце (1-31)<br />
М - месяц (1-12)<br />
Н - день недели (пн=1, ... вс=7)<br />
""Для всех"" = *<br />
""Каждые N (м, Ч, Д)"" = "" /N""<br />
допускается использование<br />
"","" и ""-"" при перечислении";

					/*var cron = @"* * * * *&nbsp;&nbsp;&nbsp;&nbsp;cron формат <br/>
                                 * * * * --- день недели (0-6)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;* , - / # L ?&nbsp;&nbsp;(Вс 0 или 7) <br/>
                                 * * * ----- месяц (1-12)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;* , - / <br/>
                                 * * ------- день месяца (1-31)&nbsp;&nbsp;* , - / L W ? <br/>
                                 * --------- час (0-23)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;* , - / <br/>
                                 ----------- минута (0-59)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;* , - /";*/
                    return Context.GetIntArg("StartType").HasValue ? (Context.GetIntArg("StartType") == 1 ? cron : "HH:mm") : (_startType == 1 ? cron : "HH:mm");
                }
            }
			public override string StringValue
            {
                get
                {
                    string res = "";
                    if (_startType == 1)
                    {
                        res = Value;
                    }
                    else
                    {
                        TimeSpan ts = TimeSpan.FromMinutes(Value.ToInt32(0));
                        res = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00");
                    }
                    return res;
                }
            }

			public override void ValidateFormValue(ValidationMessageCollection val)
			{
                if (Context.GetIntArg("StartType") == 1)
                {
                    try
                    {
                        var expression = Cronos.CronExpression.Parse(Context.GetArg(ID));
                    }
                    catch
                    {
                        val.Add("entitycheck", Caption, "Неверный формат данных в поле Время запуска");
                    }
                }
                else
                {
                    if (!Regex.IsMatch(Context.GetArg(ID), @"\d{2}:\d{2}"))
                        val.Add("entitycheck", Caption, "Неверный формат данных в поле Интервал запуска");
                }
			}

            public override string GetFormValue()
            {
                string res = "";
                if (Context.GetIntArg("StartType") == 1)
                {
                    res = Context.GetArg(ID);
                }
                else
                {
                    string[] arr = Context.GetArg(ID).Split(':');
                    res = (arr.Length == 1 ? arr[0].ToInt32(0) : arr[0].ToInt32(0) * 60 + arr[1].ToInt32(0)).ToString();
                }
                return res;
            }
		}

		public class ExecutionTimeout : EntityField<Task, int>
		{
			public override int DefaultValue => 60;
		}

		public class Class : EntityField<Task, string>
		{
			public override bool IsRequired => true;
		}

		public class Method : EntityField<Task, string>
		{
			public override bool IsRequired => true;
		}

		public class IsActive : EntityField<Task, bool> { }

		public class Status : EntityField<Task, int>
        {
            public override int DefaultValue => 0;
            public override void SubmitProperty(ValidationMessageCollection val) { }
            public override void ValidateFormValue(ValidationMessageCollection val) { }
        }

		public class TaskGroup : EntityField<Task, int?>
		{
            protected override string IDSuffix => "ID";
            public override string StringValue => ViewData.GroupTitle;
		}

        public class SystemName : EntityField<Task, string> { }
        public class Priority : EntityField<Task, int>
        {
            public override bool IsRequired => false;
        }
        public class DefaultGroup : FieldGroup
		{
			public CommonFields.Title Title { get; set; }
			public StartFromService StartFromService { get; set; }
			public StartType StartType { get; set; }
			public Interval Interval { get; set; }
			public ExecutionTimeout ExecutionTimeout { get; set; }
			public Class Class { get; set; }
			public Method Method { get; set; }
			public IsActive IsActive { get; set; }
			public Status Status { get; set; }
			public TaskGroup TaskGroup { get; set; }
            public SystemName SystemName { get; set; }
            public Priority Priority { get; set; }

            public DefaultGroup(int startType)
            {
                Title = AddField(new CommonFields.Title());
                StartFromService = AddField(new StartFromService());
                StartType = AddField(new StartType());
                Interval = AddField(new Interval(startType));
                ExecutionTimeout = AddField(new ExecutionTimeout());
                Class = AddField(new Class());
                Method = AddField(new Method());
                IsActive = AddField(new IsActive());
                Status = AddField(new Status());
				TaskGroup = AddField(new TaskGroup());
                SystemName = AddField(new SystemName());
                Priority = AddField(new Priority());
            }
        }
    }
}
