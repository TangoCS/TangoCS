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
	[OnAction(typeof(Task), "viewlist")]
	public class tm_task_bycategories : default_view_rep<Task, int, ITaskRepository>, IHasEmbeddedResources
	{
		Tabs tabs;

		protected override string FormTitle => Resources.Get<Task>("viewlist");
		protected override ContainerWidth FormWidth => ContainerWidth.Width100;
		protected override bool ObjectNotExists => false;
		protected override Task GetExistingEntity() { return null; }

		protected override void ToolbarLeft(MenuBuilder t)
		{
			t.ToCreateNew<Task>();
			t.ItemSeparator();
			t.ItemActionImageText(x => x.ToList<TaskExecution>(AccessControl).WithImage("log"));
		}

		public override void OnInit()
		{
			base.OnInit();

			var groups = Repository.GetGroups().OrderBy(o => o.Title);

			tabs = CreateControl<Tabs>("tasktabs");

			foreach (var group in groups)
			{
				if (AccessControl.Check($"{typeof(TaskGroup).Name}.view_{group.ID}"))
				{
					var list = new tm_task_list {
						ID = "tasks_" + group.ID,
						GroupID = group.ID
					};
					tabs.Pages.Add(new TabPage(group.Title, list));
					tabs.AddControl(list);
				}
			}
			
			if (Repository.Any(o => o.TaskGroupID == null))
			{
				var list_null = new tm_task_list {
					ID = "tasks_null"
				};
				tabs.Pages.Add(new TabPage("Без категории", list_null));
				tabs.AddControl(list_null);
			}
		}

        protected override void Form(LayoutWriter w)
        {
			tabs.RenderTabs(w);
			tabs.RenderPages(w);
		}
    }

	public class tm_task_list : default_list_rep<Task>
	{
		public int? GroupID { get; set; }

		protected override Func<string, Expression<Func<Task, bool>>> SearchExpression => s => o => o.Title.Contains(s);

		protected override IQueryable<Task> Data => GroupID.HasValue ? base.Data.Where(o => o.TaskGroupID == GroupID) : base.Data.Where(o => o.TaskGroupID == null);

		protected override void ToolbarLeft(MenuBuilder t)
		{
			t.ItemFilter(Filter);
            if (Fields.EnableSelect)
            {
                t.ItemSeparator();
                t.ItemActionTextBulk(x => x.To<Task>("deactivation", AccessControl).AsDialog());
                t.ItemSeparator();
                t.ItemActionTextBulk(x => x.ToDeleteBulk<Task>(AccessControl).AsDialog());
            }
		}

		protected override void FieldsInit(FieldCollection<Task> f)
		{
			f.EnableSelect = true;

			f.AddCellWithSortAndFilter(o => o.Title, (w, o) => w.ActionLink(al => al.ToView(AccessControl, o).WithTitle(o.Title)));
			f.AddCellWithSortAndFilter(o => o.StartTypeTitle, o => o.StartTypeTitle);
			f.AddCell(o => o.Interval, o => o.StartTypeID == 2 ? (new TimeSpan(0, o.Interval.ToInt32(0), 0)).ToString(@"hh\:mm") : o.Interval);
			f.AddCellWithSortAndFilter(o => o.LastStartDate, o => o.LastStartDate.DateTimeToString());
			f.AddCell(Resources.Get<Task>("NextTime"), (w, o) => {
				DateTime? nextTime = null;
				if (o.StartTypeID == 1)
				{
					var oldTimeUtc = new DateTimeOffset((o.LastStartDate ?? DateTime.Now));
					var expression = Cronos.CronExpression.Parse(o.Interval);
					var next = expression.GetNextOccurrence(oldTimeUtc, TimeZoneInfo.Local);
					nextTime = next?.DateTime;
				}
				else
				{
					nextTime = (o.LastStartDate ?? DateTime.Now).AddMinutes(o.Interval.ToInt32(3000000));
				}

				if (nextTime < DateTime.Now)
					nextTime = DateTime.Now.AddMinutes(1);

				w.Write(o.IsActive ? nextTime.DateTimeToString() : "");
			});
			f.AddCellWithSortAndFilter(o => o.Status, o => Enumerations.GetEnumDescription((TaskStatusType)o.Status));
			f.AddCellWithSortAndFilter(o => o.IsActive, o => o.IsActive.Icon());
		}
	}

	[OnAction(typeof(Task), "createnew")]
	[OnAction(typeof(Task), "edit")]
	public class tm_task_edit : default_edit_rep<Task, int, ITaskRepository>
	{
		[Inject]
		protected IAccessControl AccessControl { get; set; }

		TaskFields.DefaultGroup gr { get; set; }

		IEnumerable<SelectListItem> Types() => Repository.GetStartTypes().OrderBy(o => o.Title).Select(o => new SelectListItem(o.Title, o.TaskStartTypeID));
		IEnumerable<SelectListItem> Groups() => Repository.GetGroups().OrderBy(o => o.Title).ToList()
			.Where(o => AccessControl.Check($"{typeof(TaskGroup).Name}.view_{o.TaskGroupID}"))
			.Select(o => new SelectListItem(o.Title, o.TaskGroupID)).AddEmptyItem();

		protected override void SetDefaultValues(Task obj)
		{
			base.SetDefaultValues(obj);
			obj.StartTypeID = 1;
			obj.StartFromService = true;
		}

		public override void OnInit()
		{
			gr = AddFieldGroup(new TaskFields.DefaultGroup(ViewData.StartTypeID));
		}

		protected override void Form(LayoutWriter w)
		{
			w.FieldsBlockStd(() => {
				w.TextBox(gr.Title);
				w.TextBox(gr.SystemName);
				w.DropDownList(gr.TaskGroup, Groups());
				w.DropDownList(gr.StartType, Types(), attrs: a => a.OnChangePostEvent(OnStartTypeChanged));
				w.TextBox(gr.Interval);
				w.TextBox(gr.Class);
				w.TextBox(gr.Method);
				w.TextBox(gr.ExecutionTimeout);
				w.ToggleSwitch(gr.IsActive);
			});
		}

		public void OnStartTypeChanged(ApiResponse response)
		{
			response.WithNamesFor(this).SetElementValue(gr.Interval.ID + "_fieldcaption", Resources.Get<Task>(o => o.Interval, gr.StartType.FormValue.ToString()));
			response.WithNamesFor(this).SetElementValue(gr.Interval.ID + "_fielddescription", gr.Interval.Description);
		}

		protected override void ValidateFormData(ValidationMessageCollection val)
		{
			base.ValidateFormData(val);

            var value = gr.Title.Value?.ToLower();
            if (Repository.Any(o => o.TaskID != ViewData.ID && o.Title.ToLower() == value))
			{
				val.Add(gr.Title, "Задача с указанным названием уже существует");
			}
            value = gr.SystemName.Value?.ToLower();
            if (!gr.SystemName.Value.IsEmpty() && Repository.Any(o => o.TaskID != ViewData.ID && o.SystemName.ToLower() == value))
			{
				val.Add(gr.SystemName, "Задача с указанным системным именем уже существует");
			}
		}
	}

	[OnAction(typeof(Task), "view")]
	public class tm_task_view : default_view_rep<Task, int, ITaskRepository>
	{
		tm_taskexecution_list2 taskexecution;
		protected virtual bool ShowBaseTaskExecutionList => true;

		TaskFields.DefaultGroup gr { get; set; }
		bool isParam = false;

		protected override Task GetExistingEntity()
		{
			var id = Context.GetArg<int>(Constants.Id);
			var obj = Repository.GetById(id);
			if (obj != null)
				setTaskParamerers(obj);
			return obj;
		}

		void setTaskParamerers(Task task)
		{
			var type = TaskTypeCollection.GetType(task.Class);
			if (type != null)
			{
				ParameterInfo[] newpars = new ParameterInfo[0];
				var mi = type.GetMethod(task.Method);
				if (mi != null)
				{
					newpars = mi.GetParameters().Where(o => !o.ParameterType.IsInterface && o.ParameterType.Name != typeof(TaskExecutionContext).Name).ToArray();
					isParam = newpars.Length > 0;
				}
				if (isParam)
				{
					bool ischange = false;

					var oldpars = Repository.GetParameters(task.ID); 
					var seqno = oldpars.Max(o => (int?)o.SeqNo) ?? 0;

					using (var tran = Database.BeginTransaction())
					{
						foreach (var newpar in newpars)
						{
							if (!oldpars.Any(o => o.SysName.ToLower() == newpar.Name.ToLower()))
							{
								string title = "";
								seqno++;

								var attr = newpar.GetCustomAttribute<DescriptionAttribute>(false);
								if (attr != null)
									title = attr.Description;

								var par = new TaskParameter { ParentID = task.ID, SysName = newpar.Name, Title = title, SeqNo = seqno };
								Repository.CreateParameter(par);
								ischange = true;
							}
						}
						foreach (var oldpar in oldpars)
						{
							if (!newpars.Any(o => o.Name.ToLower() == oldpar.SysName.ToLower()))
							{
								Repository.DeleteParameter(oldpar.ID);
								ischange = true;
							}
						}
						if (ischange)
							tran.Commit();
					}
				}
                else
                {
                    var oldpars = Repository.GetParameters(task.ID);
                    foreach (var oldpar in oldpars)
                    {
                            Repository.DeleteParameter(oldpar.ID);
                    }
                }    
			}
		}

		protected override void ToolbarLeft(MenuBuilder t)
		{
            t.ItemBack();
            t.ItemSeparator();
            t.ItemActionImageText(x => x.ToEdit(AccessControl, ViewData));
            t.ItemSeparator();
            t.ItemActionImageText(x => x.ToDelete(AccessControl, ViewData, Context.ReturnUrl.Get(1))
				.WithArg(Constants.ReturnUrl + "_0", Context.CreateReturnUrl(1)).AsDialog());

            if (AccessControl.Check("task.start"))
			{
				t.ItemSeparator();
				if (isParam)
					t.Item(w => w.ActionImageTextButton(al => al.To<Task>("parameters", AccessControl).WithArg(Constants.Id, ViewData.ID).WithTitle("Старт").WithImage("settings2").AsNoCloseIconDialog()));
				else
					t.Item(w => w.ActionImageTextButton(al => al.ToCurrent().KeepTheSameUrl().PostEvent(OnRunTask).WithTitle("Старт").WithImage("settings2")));
			}
		}

		public override void OnInit()
		{
			base.OnInit();
			gr = AddFieldGroup(new TaskFields.DefaultGroup(ViewData.StartTypeID));

			if (ShowBaseTaskExecutionList)
			{
				taskexecution = CreateControl<tm_taskexecution_list2>("taskexecution", c =>
				{
					c.TaskID = ViewData.ID;
					c.Sections.RenderContentTitle = false;
					c.Sections.SetPageTitle = false;
				});
			}
		}

		public override void OnLoad(ApiResponse response)
		{
			base.OnLoad(response);

			if (ShowBaseTaskExecutionList)
			{
				response.WithNamesAndWritersFor(taskexecution);
				taskexecution.OnLoad(response);
			}
		}

		protected override void Form(LayoutWriter w)
		{
			w.FieldsBlockStd(() => {
				w.PlainText(gr.Title);
				w.PlainText(gr.SystemName);
				w.PlainText(gr.TaskGroup);
				w.PlainText(gr.StartType);
				void content()
				{
					if (gr.StartType.Value == 1)
					{
						w.Write(gr.Interval.Value);
						var text = ExpressionDescriptor.GetDescription(gr.Interval.Value, new Options() {
							DayOfWeekStartIndexZero = false,
							Use24HourTimeFormat = true,
							Locale = "ru",
							ThrowExceptionOnParseError = false
						});
						w.Div(a => a.Class("descriptiontext"), text);
					}
					else
						w.Write(gr.Interval.StringValue);
				}
				w.PlainText(gr.Interval, content);

				DateTime? nextTime = null;

				if (gr.StartType.Value == 1)
				{
					var oldTimeUtc = new DateTimeOffset((ViewData.LastStartDate ?? DateTime.Now));
					var expression = Cronos.CronExpression.Parse(gr.Interval.Value);
					var next = expression.GetNextOccurrence(oldTimeUtc, TimeZoneInfo.Local);
					nextTime = next?.DateTime;
				}
				else
					nextTime = (ViewData.LastStartDate ?? DateTime.Now).AddMinutes(gr.Interval.Value.ToInt32(3000000));

				if (nextTime < DateTime.Now)
					nextTime = DateTime.Now.AddMinutes(1);

				w.PlainText(Resources.Get<Task>("NextTime"), ViewData.IsActive ? nextTime?.ToString("dd.MM.yyyy HH:mm:ss") : "");

				w.PlainText(Resources.Get<Task>(o => o.Method), ViewData.Class + "." + ViewData.Method);
				w.PlainText(gr.ExecutionTimeout);
				w.PlainText(gr.IsActive);
				w.PlainText(Resources.Get<Task>(o => o.LastStartDate), ViewData.LastStartDate?.ToString("dd.MM.yyyy HH:mm:ss"));
				w.PlainText(gr.Status, () => w.Write(Enumerations.GetEnumDescription((TaskStatusType)gr.Status.Value)));
			});
		}

		public void OnRunTask(ApiResponse response)
		{
            var exec = Repository.IsExecuteTask(ViewData.ID);

            if (exec)
            {
				RunTaskController();

			}
			response.RedirectTo(Context, a => a.ToCurrent());
		}

		protected virtual void RunTaskController()
		{
			var c = new TaskController { Context = Context };

			c.InjectProperties(Context.RequestServices);

			c.Run(ViewData, true);
		}

		protected override void LinkedData(LayoutWriter w)
		{
			w.Br();
			w.GroupTitle("Параметры");
			ParamList(w);

			if (ShowBaseTaskExecutionList)
			{
				w.Br();
				w.GroupTitle("Лог запуска задачи");
				taskexecution.RenderPlaceHolder(w);
			}
		}

		void ParamList(LayoutWriter w)
		{
			var f = new FieldCollectionBase<TaskParameter>(Context);

			f.AddCell(o => o.SeqNo, o => o.SeqNo);
			f.AddCell(o => o.Title, (cw, o) => cw.ActionLink(al => al.ToEdit(AccessControl, o).WithTitle(o.Title).AsDialog()));
			f.AddCell(o => o.SysName, o => o.SysName);
			f.AddCell(o => o.Value, o => o.Value);
			f.AddActionsCell(o => al => al.ToDelete(AccessControl, o).AsDialog());

			var result = Repository.GetParameters(ViewData.ID).OrderBy(o => o.SeqNo);
			new ListRenderer<TaskParameter>("param").Render(w, result, f);
		}
	}

	[OnAction(typeof(Task), "delete")]
	public class tm_task_delete : default_delete<Task, int> { }

	[OnAction(typeof(Task), "deactivation")]
	public class tm_task_deactivation : default_edit_rep<Task, int, ITaskRepository>
	{
		protected override string Title => Resources.Get<Task>(BulkMode ? "Deactivation.Bulk.Title" : "Deactivation.Title");
		protected override bool BulkMode => Context.GetListArg<int>(Constants.SelectedValues)?.Count > 1;
		protected override bool ObjectNotExists => false;
		protected override Task GetNewEntity() { return null; }
		protected override Task GetExistingEntity()	{ return null; }

        protected override void Form(LayoutWriter w)
        {
			var cnt = Context.GetListArg<int>(Constants.SelectedValues)?.Count ?? 0;
			var confirm = BulkMode ?
				string.Format(Resources.Get<Task>("Deactivation.Bulk.Confirm"), cnt) : Resources.Get<Task>("Deactivation.Confirm");

			w.P(() => {
				w.Write(confirm);
			});
		}

		public override void OnSubmit(ApiResponse response)
		{
			var sel = Context.GetListArg<int>(Constants.SelectedValues);
			Repository.Deactivation(sel);

			response.RedirectBack(Context, 1);
		}
	}
}