﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CronExpressionDescriptor;
using Tango;
using Tango.AccessControl;
using Tango.Data;
using Tango.Html;
using Tango.Localization;
using Tango.LongOperation;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Tasks
{
	[OnAction(typeof(DTO_Task), "Parameters")]
	public class tm_taskparameters : default_edit_rep<DTO_Task, int>
	{
		[Inject]
		protected ITaskRepository TaskRepository { get; set; }

		protected override string Title => "Параметры запуска";
		Dictionary<string, ParameterData> parameters = new Dictionary<string, ParameterData>();

		protected override DTO_Task GetExistingEntity()
		{
			var id = Context.GetArg<int>(Constants.Id);
			var obj = TaskRepository.GetTasks().GetById(id);
			return obj;
		}

		public override void OnInit()
		{
			base.OnInit();
			
			var ps = TaskRepository.GetTaskParameters().List().Where(o => o.ParentID == ViewData.TaskID).ToDictionary(o => o.SysName, o => o);
			Type type = Type.GetType(ViewData.Class, true);
			MethodInfo mi = type.GetMethod(ViewData.Method);
			parameters = mi.GetParameters().Where(o => o.ParameterType.Name != typeof(TaskExecutionContext).Name)
										   .ToDictionary(o => o.Name, o => new ParameterData { ParameterInfo = o });

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

		public override void OnSubmit(ApiResponse response)
		{
			var param = new Dictionary<string, string>();
			foreach (var p in parameters)
			{
				param[p.Key] = FormData.Parse<string>(p.Key);
			}

			var c = new TaskController { Context = Context };

			c.InjectProperties(Context.RequestServices);

			c.Run(ViewData, true, param);

			response.RedirectBack(Context, 1);
		}

		public class ParameterData
		{
			public ParameterInfo ParameterInfo { get; set; }
			public string Caption { get; set; }
			public string Value { get; set; }
		}
	}

	[OnAction(typeof(DTO_TaskParameter), "createnew")]
	[OnAction(typeof(DTO_TaskParameter), "edit")]
	public class tm_taskparameter_edit : default_edit_rep<DTO_TaskParameter, int>
	{
		[Inject]
		protected ITaskRepository TaskRepository { get; set; }

		DTO_TaskParameterFields.DefaultGroup gr;

		protected override DTO_TaskParameter GetExistingEntity()
		{
			var id = Context.GetArg<int>(Constants.Id);
			var obj = TaskRepository.GetTaskParameters().GetById(id);
			Tracker?.StartTracking(obj);
			return obj;
		}

		protected override void SetDefaultValues(DTO_TaskParameter obj)
		{
			var id = Context.GetIntArg("taskid", ViewData.ParentID);
			obj.ParentID = id;
			obj.SeqNo = (TaskRepository.GetTaskParameters().List().Where(o => o.ParentID == id).Max(o => (int?)o.SeqNo) ?? 0) + 1;
		}

		public override void OnInit()
		{
			gr = AddFieldGroup(new DTO_TaskParameterFields.DefaultGroup());
		}

		protected override void Form(LayoutWriter w)
		{
			var type = Type.GetType(ViewData.ParentClass, true);
			var mi = type.GetMethod(ViewData.ParentMethod);
			var parameter = mi.GetParameters().First(o => o.Name == ViewData.SysName);

			w.FieldsBlockStd(() => {
				w.TextBox(gr.Title);
				w.TextBox(gr.SysName);
				if (parameter.ParameterType == typeof(DateTime) || parameter.ParameterType == typeof(DateTime?))
					w.FormFieldCalendar(gr.Value.ID, gr.Value.Caption, gr.Value.Value.ToDateTime());
				else
					w.TextBox(gr.Value);
			});
		}

		protected override void Submit(ApiResponse response)
		{
			if (EntityAudit != null && ViewData != null)
			{
				if (!CreateObjectMode)
				{
					if (EntityAudit != null)
						EntityAudit.PrimaryObject.PropertyChanges = Tracker?.GetChanges(ViewData);
				}
			}

			if (CreateObjectMode)
				InTransaction(() =>
				{
					TaskRepository.CreateTaskParameter(ViewData);
				});
			else
			{
				InTransaction(() =>
				{
					TaskRepository.UpdateTaskParameter(ViewData);
				});
			}
		}
	}

	[OnAction(typeof(DTO_TaskParameter), "delete")]
	public class tm_taskparameter_delete : default_delete<DTO_TaskParameter, int> 
	{
		[Inject]
		protected ITaskRepository TaskRepository { get; set; }
		protected override void Delete(IEnumerable<int> ids)
        {
			TaskRepository.DeleteTaskParameter(ids);
        }
    }
}