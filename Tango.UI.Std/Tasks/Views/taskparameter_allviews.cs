using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using CronExpressionDescriptor;
using Tango;
using Tango.AccessControl;
using Tango.Data;
using Tango.Html;
using Tango.Identity.Std;
using Tango.Localization;
using Tango.LongOperation;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Tasks
{
	[OnAction(typeof(Task), "parameters")]
	public class tm_taskparameters : tm_taskparameters<IdentityUser> { }

	public class tm_taskparameters<TUser> : default_edit_rep<Task, int, ITaskRepository> where TUser : class
	{
		protected override string FormTitle => "Параметры запуска";
		protected Dictionary<string, ParameterData> parameters = new Dictionary<string, ParameterData>();

		public override void OnInit()
		{
			base.OnInit();
			
			var ps = Repository.GetParameters(ViewData.ID).ToDictionary(o => o.SysName.ToLower(), o => o);

			var type = TaskTypeCollection.GetType(ViewData.Class);
			MethodInfo mi = type.GetMethod(ViewData.Method);
			parameters = mi.GetParameters().Where(o => o.ParameterType.Name != typeof(TaskExecutionContext).Name)
										   .ToDictionary(o => o.Name.ToLower(), o => new ParameterData { ParameterInfo = o });

			foreach (var p in ps)
			{
				if (parameters.TryGetValue(p.Key, out var info))
				{
					info.Value = p.Value.Value;
					var defValueAttr = info.ParameterInfo.GetCustomAttribute<DefaultValueAttribute>(false);
					if (defValueAttr != null)
					{
						var providerType = defValueAttr.Value as Type;
						var provider = Activator.CreateInstance(providerType, Context.RequestServices) as ITaskParameterDefaultValueProvider;
						info.Value = provider.GetValue(ViewData, info.ParameterInfo);
					}
				}
			}
		}

		protected override void Form(LayoutWriter w)
		{
			w.FieldsBlock100Percent(() => {
				foreach (var par in parameters)
				{
					var attributes = par.Value.ParameterInfo.GetCustomAttributes<DescriptionAttribute>(false);
					var caption = (attributes != null && attributes.Count() > 0) ? attributes.First().Description : par.Key;

					if (par.Value.ParameterInfo.ParameterType == typeof(DateTime) || par.Value.ParameterInfo.ParameterType == typeof(DateTime?))
						w.FormFieldCalendar(par.Key, caption, par.Value.Value.ToDateTime());
					else
						w.FormFieldTextBox(par.Key, caption, par.Value.Value);
				}
			});
		}

		public sealed override void OnSubmit(ApiResponse response)
        {
            var param = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in parameters)
            {
                param[p.Key] = FormData.Parse<string>(p.Key);
            }

			RunTaskController(param);

			response.RedirectBack(Context, 1);
        }

        protected virtual void RunTaskController(Dictionary<string, string> param)
        {
			var c = new TaskController<TUser> { Context = Context };

			c.InjectProperties(Context.RequestServices);

			c.Run(ViewData, true, param);
		}

        protected class ParameterData
		{
			public ParameterInfo ParameterInfo { get; set; }
			public string Caption { get; set; }
			public string Value { get; set; }
		}

		protected override void ButtonsBar(LayoutWriter w)
		{
			w.ButtonsBar(() => {
				w.ButtonsBarRight(() => {
					w.SubmitButton(a => a.DataReceiver(this).OnClick($"dialog.close(document.getElementById('task_parameters_dialog'))"));
					w.BackButton();
				});
			});
		}
	}

	[OnAction(typeof(TaskParameter), "createnew")]
	[OnAction(typeof(TaskParameter), "edit")]
	public class tm_taskparameter_edit : default_edit_rep<TaskParameter, int, ITaskParameterRepository>
	{
		protected TaskParameterFields.DefaultGroup gr;

		protected override void SetDefaultValues(TaskParameter obj)
		{
			var id = Context.GetIntArg("taskid", 0);
			obj.ParentID = id;
			obj.SeqNo = Repository.MaximumSequenceNumber(id) + 1;
		}

		public override void OnInit()
		{
			gr = AddFieldGroup(new TaskParameterFields.DefaultGroup());
		}

        protected override void Form(LayoutWriter w)
        {
            var type = TaskTypeCollection.GetType(ViewData.ParentClass);
            if (type != null)
            {
                var mi = type.GetMethod(ViewData.ParentMethod);
                var parameter = mi.GetParameters().First(o => o.Name.ToLower() == ViewData.SysName.ToLower());

                w.FieldsBlockStd(() =>
                {
                    w.TextBox(gr.Title);
                    w.TextBox(gr.SysName);
                    if (parameter.ParameterType == typeof(DateTime) || parameter.ParameterType == typeof(DateTime?))
                        w.FormFieldCalendar(gr.Value.ID, gr.Value.Caption, gr.Value.Value.ToDateTime());
                    else
                        w.TextBox(gr.Value);
                });
            }
            else
            {
                w.FieldsBlockStd(() =>
                {
                    w.PlainText("Информация", () => w.Write("Параметры в ручную не создаются."));
                });
            }
        }
    }

	[OnAction(typeof(TaskParameter), "delete")]
	public class tm_taskparameter_delete : default_delete<TaskParameter, int> { }
}