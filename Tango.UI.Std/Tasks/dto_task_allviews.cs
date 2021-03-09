using System;
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
	[OnAction(typeof(DTO_Task), "viewlist")]
	public class tm_task_bycategories : ViewPagePart, IHasEmbeddedResources
	{
		[Inject]
		protected IAccessControl AccessControl { get; set; }
		[Inject]
		protected ITaskRepository TaskRepository { get; set; }

		Tabs tabs;

		protected void ToolbarLeft(MenuBuilder t)
		{
			t.ToCreateNew<DTO_Task>();
			t.ItemSeparator();
			t.ItemActionImageText(x => x.To<DTO_TaskExecution>("viewlist", AccessControl).WithImage("log"));
		}

		protected void Toolbar(LayoutWriter w)
		{
			w.Toolbar(t => ToolbarLeft(t), t => { });
		}

		protected string FormTitle => Resources.Get<DTO_Task>("viewlist");


		public override void OnInit()
		{
			base.OnInit();
			
			var groups = TaskRepository.GetTaskGroups().OrderBy(o => o.Title);

			tabs = CreateControl<Tabs>("tasktabs");

			foreach (var group in groups)
			{
				if (AccessControl.Check($"{typeof(DTO_TaskGroup).Name}.view_{group.ID}"))
				{
					var list = new tm_task_list {
						ID = "tasks_" + group.ID,
						GroupID = group.ID
					};
					tabs.Pages.Add(new TabPage(group.Title, list));
					tabs.AddControl(list);
				}
			}
			
			if (TaskRepository.GetTasks().Any(o => o.TaskGroupID == null))
			{
				var list_null = new tm_task_list {
					ID = "tasks_null"
				};
				tabs.Pages.Add(new TabPage("Без категории", list_null));
				tabs.AddControl(list_null);
			}
		}

		public override void OnLoad(ApiResponse response)
		{
			response.AddWidget(Sections.ContentToolbar, w => Toolbar(w));
			response.AddWidget(Sections.ContentBody, w => {
				tabs.RenderTabs(w);
				tabs.RenderPages(w);
			});

			response.AddWidget(Sections.ContentTitle, FormTitle);
			response.AddWidget("#title", FormTitle);

			foreach (var r in Context.EventReceivers)
				if (r.ParentElement.ClientID == this.ClientID && r is Tabs tabs)
					tabs.OnPageSelect(response);
		}

		public ViewSections Sections { get; set; } = new ViewSections();
		public class ViewSections
		{
			public string ContentBody { get; set; } = "contentbody";
			public string ContentToolbar { get; set; } = "contenttoolbar";
			public string ContentTitle { get; set; } = "contenttitle";
			public bool SetPageTitle { get; set; } = true;
			public bool RenderToolbar { get; set; } = true;
			public bool RenderContentTitle { get; set; } = true;
		}
	}

	public class tm_task_list : default_list_rep<DTO_Task>
	{
		[Inject]
		protected ITaskRepository TaskRepository { get; set; }

		public int? GroupID { get; set; }

		protected override Func<string, Expression<Func<DTO_Task, bool>>> SearchExpression => s => o => o.Title.Contains(s);

		protected override IRepository<DTO_Task> GetRepository() => TaskRepository.GetTasks();

		protected override IQueryable<DTO_Task> Data => GroupID.HasValue ? base.Data.Where(o => o.TaskGroupID == GroupID) : base.Data.Where(o => o.TaskGroupID == null);

		protected override void ToolbarLeft(MenuBuilder t)
		{
			t.ItemFilter(Filter);
			t.ItemSeparator();
			t.ItemActionTextBulk(x => x.To<DTO_Task>("Deactivation", AccessControl).AsDialog());
			t.ItemSeparator();
			ToDeleteBulk(t);
		}

		protected override void FieldsInit(FieldCollection<DTO_Task> f)
		{
			f.EnableSelect = true;

			f.AddCellWithSortAndFilter(o => o.Title, (w, o) => w.ActionLink(al => al.ToView(AccessControl, o).WithTitle(o.Title)));
			f.AddCellWithSortAndFilter(o => o.StartTypeTitle, o => o.StartTypeTitle);
			f.AddCell(o => o.Interval, o => o.StartTypeID == 2 ? (new TimeSpan(0, o.Interval.ToInt32(0), 0)).ToString(@"hh\:mm") : o.Interval);
			f.AddCellWithSortAndFilter(o => o.LastStartDate, o => o.LastStartDate.DateTimeToString());
			f.AddCell(Resources.Get<DTO_Task>("NextTime"), (w, o) => {
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

	[OnAction(typeof(DTO_Task), "createnew")]
	[OnAction(typeof(DTO_Task), "edit")]
	public class tm_task_edit : default_edit_rep<DTO_Task, int>
	{
		[Inject]
		protected IAccessControl AccessControl { get; set; }
		[Inject]
		protected ITaskRepository TaskRepository { get; set; }

		DTO_TaskFields.DefaultGroup gr { get; set; }

		IEnumerable<SelectListItem> Types() => TaskRepository.GetTaskStartTypes().OrderBy(o => o.Title).Select(o => new SelectListItem(o.Title, o.TaskStartTypeID));
		IEnumerable<SelectListItem> Groups() => TaskRepository.GetTaskGroups().OrderBy(o => o.Title).ToList()
			.Where(o => AccessControl.Check($"{typeof(DTO_TaskGroup).Name}.view_{o.TaskGroupID}"))
			.Select(o => new SelectListItem(o.Title, o.TaskGroupID));

		protected override DTO_Task GetExistingEntity()
		{
			var id = Context.GetArg<int>(Constants.Id);
			var obj = TaskRepository.GetTasks().GetById(id);
			Tracker?.StartTracking(obj);
			return obj;
		}

		protected override void SetDefaultValues(DTO_Task obj)
		{
			base.SetDefaultValues(obj);
			obj.StartTypeID = 1;
			obj.StartFromService = true;
		}

		public override void OnInit()
		{
			gr = AddFieldGroup(new DTO_TaskFields.DefaultGroup(ViewData.StartTypeID));
		}

		protected override void Form(LayoutWriter w)
		{
			w.FieldsBlockStd(() => {
				w.TextBox(gr.Title);
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
			response.WithNamesFor(this).SetElementValue(gr.Interval.ID + "_fieldcaption", Resources.Get<DTO_Task>(o => o.Interval, gr.StartType.FormValue.ToString()));
			response.WithNamesFor(this).SetElementValue(gr.Interval.ID + "_fielddescription", gr.Interval.Description);
		}

		protected override void ValidateFormData(ValidationMessageCollection val)
		{
			base.ValidateFormData(val);

			if (TaskRepository.GetTasks().List().Any(o => o.TaskID != ViewData.ID && o.Title == gr.Title.Value))
			{
				val.Add(gr.Title, "Задача с указанным названием уже существует");
			}
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
					TaskRepository.CreateTask(ViewData);
				});
			else
			{
				InTransaction(() =>
				{
					TaskRepository.UpdateTask(ViewData);
				});
			}
		}
	}

	[OnAction(typeof(DTO_Task), "view")]
	public class tm_task_view : default_view_rep<DTO_Task, int>
	{
		[Inject]
		protected ITaskRepository TaskRepository { get; set; }

		DTO_TaskFields.DefaultGroup gr { get; set; }
		tm_taskexecution_list2 taskexecution;
		bool isParam = false;

		protected override DTO_Task GetExistingEntity()
		{
			var id = Context.GetArg<int>(Constants.Id);
			var obj = TaskRepository.GetTasks().GetById(id);
			if (obj != null)
				setTaskParamerers(obj);
			return obj;
		}
		void setTaskParamerers(DTO_Task task)
		{
			Type type = Type.GetType(task.Class, true);
			if (type != null)
			{
				ParameterInfo[] newpars = new ParameterInfo[0];
				var mi = type.GetMethod(task.Method);
				if (mi != null)
				{
					newpars = mi.GetParameters().Where(o => o.ParameterType.Name != typeof(TaskExecutionContext).Name).ToArray();
					isParam = newpars.Length > 0;
				}
				if (isParam)
				{
					bool ischange = false;

					var oldpars = TaskRepository.GetTaskParameters().List().Where(o => o.ParentID == task.ID); 
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

								var par = new DTO_TaskParameter { ParentID = task.ID, SysName = newpar.Name, Title = title, SeqNo = seqno };
								TaskRepository.CreateTaskParameter(par);
								ischange = true;
							}
						}
						foreach (var oldpar in oldpars)
						{
							if (!newpars.Any(o => o.Name.ToLower() == oldpar.SysName.ToLower()))
							{
								TaskRepository.DeleteTaskParameter(new[] { oldpar.ID });
								ischange = true;
							}
						}
						if (ischange)
							tran.Commit();
					}
				}
			}
		}

		public override void OnInit()
		{
			gr = AddFieldGroup(new DTO_TaskFields.DefaultGroup(ViewData.StartTypeID));
			taskexecution = CreateControl<tm_taskexecution_list2>("taskexecution", c => {
				c.TaskID = ViewData.ID;
				c.Sections.RenderContentTitle = false;
				c.Sections.SetPageTitle = false;
			});
		}

		protected override void Form(LayoutWriter w)
		{
			w.FieldsBlockStd(() => {
				w.PlainText(gr.Title);
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

				w.PlainText(Resources.Get<DTO_Task>("NextTime"), ViewData.IsActive ? nextTime?.ToString("dd.MM.yyyy HH:mm:ss") : "");

				w.PlainText(Resources.Get<DTO_Task>(o => o.Method), ViewData.Class + "." + ViewData.Method);
				w.PlainText(gr.ExecutionTimeout);
				w.PlainText(gr.IsActive);
				w.PlainText(Resources.Get<DTO_Task>(o => o.LastStartDate), ViewData.LastStartDate?.ToString("dd.MM.yyyy HH:mm:ss"));
				w.PlainText(gr.Status, () => w.Write(Enumerations.GetEnumDescription((TaskStatusType)gr.Status.Value)));
			});
		}

		protected override void ToolbarLeft(MenuBuilder t)
		{
			base.ToolbarLeft(t);

			if (AccessControl.Check($"{typeof(DTO_Task).Name}.start"))
			{
				t.ItemSeparator();
				if (isParam)
					t.Item(w => w.ActionImageTextButton(al => al.To<DTO_Task>("Parameters").WithArg(Constants.Id, ViewData.ID).WithTitle("Старт").WithImage("settings2").AsNoCloseIconDialog()));
				else
					t.Item(w => w.ActionImageTextButton(al => al.ToCurrent().KeepTheSameUrl().PostEvent(OnRunTask).WithTitle("Старт").WithImage("settings2")));
			}
		}

		public void OnRunTask(ApiResponse response)
		{
            var exec = TaskRepository.TasksForExecute().Where(o => o.TaskID == ViewData.ID).Any();

            if (exec)
            {
                var c = new TaskController { Context = Context };

                c.InjectProperties(Context.RequestServices);

                c.Run(ViewData, true);
            }
			response.RedirectTo(Context, a => a.ToCurrent());
		}

		protected override void LinkedData(LayoutWriter w)
		{
			w.Br();
			w.GroupTitle("Параметры");
			ParamList(w);
			w.Br();
			w.GroupTitle("Лог запуска задачи");
			taskexecution.RenderPlaceHolder(w);
		}

		public override void OnLoad(ApiResponse response)
		{
			base.OnLoad(response);
			response.WithNamesAndWritersFor(taskexecution);
			taskexecution.OnLoad(response);

		}

		void ParamList(LayoutWriter w)
		{
			var f = new FieldCollectionBase<DTO_TaskParameter>(Context);

			f.AddCell(o => o.SeqNo, o => o.SeqNo);
			f.AddCell(o => o.Title, (cw, o) => cw.ActionLink(al => al.ToEdit(AccessControl, o).WithTitle(o.Title).AsDialog()));
			f.AddCell(o => o.SysName, o => o.SysName);
			f.AddCell(o => o.Value, o => o.Value);
			f.AddActionsCell(o => al => al.ToDelete(AccessControl, o).AsDialog());

			var result = TaskRepository.GetTaskParameters().List().Where(o => o.ParentID == ViewData.ID).OrderBy(o => o.SeqNo);
			new ListRenderer<DTO_TaskParameter>("param").Render(w, result, f);
		}
	}

	[OnAction(typeof(DTO_Task), "delete")]
	public class tm_task_delete : default_delete<DTO_Task, int> 
	{
		[Inject]
		protected ITaskRepository TaskRepository { get; set; }
		protected override void Delete(IEnumerable<int> ids)
        {
			TaskRepository.DeleteTask(ids);
		}
    }

	[OnAction(typeof(DTO_Task), "deactivation")]
	public class tm_task_deactivation : ViewPagePart
	{
		[Inject]
		protected ITaskRepository TaskRepository { get; set; }

		public override ViewContainer GetContainer() => new EditEntityContainer();

		public override void OnLoad(ApiResponse response)
		{
			var sel = GetArg(Constants.SelectedValues);
			if (sel == null) sel = GetArg(Constants.Id);
			var cnt = sel?.Split(',').Count() ?? 0;
			var bulk = cnt > 1;

			var confirm = bulk ?
				string.Format(Resources.Get<DTO_Task>("Deactivation.Bulk.Confirm"), cnt) : Resources.Get<DTO_Task>("Deactivation.Confirm");

			response.AddWidget("form", w => {
				w.P(confirm);
				if (cnt > 0) w.Hidden(Constants.SelectedValues, sel);
				w.FormValidationBlock();
			});

			var title = Resources.Get<DTO_Task>(bulk ? "Deactivation.Bulk.Title" : "Deactivation.Title");
			response.AddWidget("contenttitle", title);
			response.AddWidget("#title", title);

			response.AddAdjacentWidget("form", "buttonsbar", AdjacentHTMLPosition.BeforeEnd, w => {
				w.ButtonsBar(() => {
					w.ButtonsBarRight(() => {
						w.SubmitButton();
						w.BackButton();
					});
				});
			});
		}

		public void OnSubmit(ApiResponse response)
		{
			var sel = Context.GetListArg<int>(Constants.SelectedValues);
			TaskRepository.DeactivationTasks(sel);

			response.RedirectBack(Context, 1);
		}
	}
}