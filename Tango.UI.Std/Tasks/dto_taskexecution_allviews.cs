using System;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using Tango.Data;
using Tango.Localization;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Tasks
{
    [OnAction(typeof(DTO_TaskExecution), "viewlist")]
	public class tm_taskexecution_list : default_list_rep<DTO_TaskExecution>
    {
		[Inject]
		protected ITaskRepository TaskRepository { get; set; }

		protected override Func<string, Expression<Func<DTO_TaskExecution, bool>>> SearchExpression =>
			s => o => o.UserName.Contains(s) || o.MachineName.Contains(s) || o.TaskName.Contains(s);

		protected override IRepository<DTO_TaskExecution> GetRepository() => TaskRepository.GetTaskExecutions();

        protected override IQueryable<DTO_TaskExecution> DefaultOrderBy(IQueryable<DTO_TaskExecution> data)
		{
			return data.OrderByDescending(o => o.StartDate);
		}

		protected override void ToolbarLeft(MenuBuilder left)
		{
			left.ItemBack();
			left.ItemSeparator();
			left.ItemFilter(Filter);
            left.ItemSeparator();
            left.ItemActionText(x => x.To<DTO_TaskExecution>("Clear", AccessControl));
        }

        protected override void FieldsInit(FieldCollection<DTO_TaskExecution> f)
		{
			f.AddCellWithSortAndFilter(o => o.StartDate, (w, o) => w.ActionLink(al => al.ToView(AccessControl, o).WithTitle(o.StartDate.DateTimeToString())));
			f.AddCellWithSortAndFilter(o => o.FinishDate, o => o.FinishDate.DateTimeToString());
			f.AddCellWithSortAndFilter(o => o.TaskName, o => o.TaskName);
			f.AddCellWithSortAndFilter(o => o.IsSuccessfull, o => o.IsSuccessfull.Icon());
			f.AddCellWithSortAndFilter(o => o.MachineName, o => o.MachineName);
			f.AddCellWithSortAndFilter(o => o.UserName, o => o.UserName);
			f.AddCellWithSortAndFilter(o => o.LastModifiedDate, o => o.LastModifiedDate.DateTimeToString());
			f.AddCell(o => o.ResultXml, o => o.ResultXml);
		}

		protected override void FilterInit(ListFilter<DTO_TaskExecution> filter)
		{
			filter.AddConditionDDL(Resources.Get<DTO_TaskExecution>(o => o.TaskName), o => o.TaskID,
                TaskRepository.GetTasks().List().OrderBy(o => o.Title).Select(o => new SelectListItem(o.Title, o.TaskID)));
		}
	}

    public class tm_taskexecution_list2 : default_list_rep<DTO_TaskExecution>
    {
        [Inject]
        protected ITaskRepository TaskRepository { get; set; }

        public int TaskID { get; set; }

        protected override Func<string, Expression<Func<DTO_TaskExecution, bool>>> SearchExpression =>
            s => o => o.UserName.Contains(s) || o.MachineName.Contains(s);

        protected override IRepository<DTO_TaskExecution> GetRepository() => TaskRepository.GetTaskExecutions();

        protected override IQueryable<DTO_TaskExecution> Data => base.Data.Where(o => o.TaskID == TaskID);

        protected override IQueryable<DTO_TaskExecution> DefaultOrderBy(IQueryable<DTO_TaskExecution> data)
        {
            return data.OrderByDescending(o => o.StartDate);
        }

        protected override bool EnableQuickSearch => true;
        protected override bool EnableViews => false;

        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemFilter(Filter);
        }

        protected override void FieldsInit(FieldCollection<DTO_TaskExecution> f)
        {
            f.AddCellWithSortAndFilter(o => o.StartDate, (w, o) => w.ActionLink(al => al.ToView(AccessControl, o).WithTitle(o.StartDate.DateTimeToString())));
            f.AddCellWithSortAndFilter(o => o.FinishDate, o => o.FinishDate.DateTimeToString());
            f.AddCellWithSortAndFilter(o => o.IsSuccessfull, o => o.IsSuccessfull.Icon());
            f.AddCellWithSortAndFilter(o => o.MachineName, o => o.MachineName);
            f.AddCellWithSortAndFilter(o => o.UserName, o => o.UserName);
            f.AddCellWithSortAndFilter(o => o.LastModifiedDate, o => o.LastModifiedDate.DateTimeToString());
            f.AddCell(o => o.ResultXml, o => o.ResultXml);
        }
    }

    [OnAction(typeof(DTO_TaskExecution), "view")]
    public class tm_taskexecution_view : default_view_rep<DTO_TaskExecution, int>
    {
        [Inject]
        protected ITaskRepository TaskRepository { get; set; }

        DTO_TaskExecutionFields.DefaultGroup gr { get; set; }

        protected override DTO_TaskExecution GetExistingEntity()
        {
            var id = Context.GetArg<int>(Constants.Id);
            var obj = TaskRepository.GetTaskExecutions().GetById(id);
            return obj;
        }
        protected override void ToolbarLeft(MenuBuilder t)
        {
			t.ItemBack();
		}

        protected override string FormTitle => Resources.Get(typeof(DTO_TaskExecution).FullName);

        public override void OnInit()
        {
            base.OnInit();
            gr = AddFieldGroup(new DTO_TaskExecutionFields.DefaultGroup());
        }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() => {
                w.PlainText(gr.StartDate);
                w.PlainText(gr.FinishDate);
                w.PlainText(gr.IsSuccessfull);
                w.PlainText(gr.MachineName);
                w.PlainText(gr.ExecutionLog);
                w.PlainText(gr.Task);
                w.PlainText(gr.ResultXml, () => ResultXmlField(w));
                w.PlainText(gr.LastModifiedUser);
                w.PlainText(gr.LastModifiedDate);
            });
        }

        protected virtual void ResultXmlField(LayoutWriter w)
        {
            w.Write(gr.ResultXml);
        }
    }

    [OnAction(typeof(DTO_TaskExecution), "clear")]
    public class tm_taskexecution_delete : default_edit_rep<DTO_TaskExecution, int>
    {
        [Inject]
        protected ITaskRepository TaskRepository { get; set; }

        protected override string Title => "Очистить";

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() => {
                w.FormFieldCalendar("date", Resources.Get<DTO_TaskExecution>("DeleteAllRecordsBefore"), DateTime.Today.AddMonths(-2));
            });
        }

        public override void OnSubmit(ApiResponse response)
        {
            var date = FormData.Parse<DateTime>("date");
            TaskRepository.ClearTasksExecution(date);
            response.RedirectBack(Context, 1);
        }
    }
}
