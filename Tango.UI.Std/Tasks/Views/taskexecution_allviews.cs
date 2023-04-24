using System;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using Tango.Data;
using Tango.Html;
using Tango.Localization;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;

namespace Tango.Tasks
{
    [OnAction(typeof(TaskExecution), "viewlist")]
	public class tm_taskexecution_list : default_list_rep<TaskExecution>
    {
		protected override Func<string, Expression<Func<TaskExecution, bool>>> SearchExpression =>
			s => o => o.UserName.Contains(s) || o.MachineName.Contains(s) || o.TaskName.Contains(s);

        protected override IQueryable<TaskExecution> DefaultOrderBy(IQueryable<TaskExecution> data)
		{
			return data.OrderByDescending(o => o.StartDate);
		}

		protected override void ToolbarLeft(MenuBuilder left)
		{
            left.ItemActionImageText(x => x.ToList<Task>(AccessControl).WithImage("back").WithTitle(Resources.Get("Common.Back")));
            left.ItemSeparator();
			left.ItemFilter(Filter);
            left.ItemSeparator();
            left.ItemActionText(x => x.To<TaskExecution>("clear", AccessControl).AsDialog());
        }

        protected override void FieldsInit(FieldCollection<TaskExecution> f)
		{
			f.AddCellWithSortAndFilter(o => o.StartDate, (w, o) => w.ActionLink(al => al.ToView(AccessControl, o).WithTitle(o.StartDate.DateTimeToString())));
			f.AddCellWithSortAndFilter(o => o.FinishDate, o => o.FinishDate.DateTimeToString());
			f.AddCellWithSortAndFilter(o => o.TaskName, o => o.TaskName);
			f.AddCellWithSortAndFilter(o => o.IsSuccessfull, o => o.IsSuccessfull.Icon());
			f.AddCellWithSortAndFilter(o => o.MachineName, o => o.MachineName);
			f.AddCellWithSortAndFilter(o => o.UserName, o => o.UserName);
			f.AddCellWithSortAndFilter(o => o.LastModifiedDate, o => o.LastModifiedDate.DateTimeToString());
			f.AddCell(o => o.ResultXml, o => o.ResultXml);

            //Filter.AddConditionDDL(Resources.Get<TaskExecution>(o => o.TaskName), o => o.TaskID,
            //    TaskRepository.List().OrderBy(o => o.Title).Select(o => new SelectListItem(o.Title, o.TaskID)));
        }
	}

    public class tm_taskexecution_list2 : default_list_rep<TaskExecution>
    {
        public int TaskID { get; set; }

        protected override Func<string, Expression<Func<TaskExecution, bool>>> SearchExpression =>
            s => o => o.UserName.Contains(s) || o.MachineName.Contains(s);

        protected override IQueryable<TaskExecution> Data => base.Data.Where(o => o.TaskID == TaskID);

        protected override IQueryable<TaskExecution> DefaultOrderBy(IQueryable<TaskExecution> data) => data.OrderByDescending(o => o.StartDate);

        protected override bool EnableQuickSearch => true;
        protected override bool EnableViews => false;

        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemFilter(Filter);
        }

        protected override void FieldsInit(FieldCollection<TaskExecution> f)
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

    [OnAction(typeof(TaskExecution), "view")]
    public class tm_taskexecution_view : default_view_rep<TaskExecution, int, ITaskExecutionRepository>
    {
        TaskExecutionFields.DefaultGroup gr { get; set; }

        protected override void ToolbarLeft(MenuBuilder t)
        {
			t.ItemBack();
		}

        protected override string FormTitle => Resources.Get(typeof(TaskExecution).FullName);

        public override void OnInit()
        {
            base.OnInit();
            gr = AddFieldGroup(new TaskExecutionFields.DefaultGroup());
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
            w.Write(gr.ResultXml.Value);
        }
    }

    [OnAction(typeof(TaskExecution), "clear")]
    public class tm_taskexecution_clear : default_edit_rep<TaskExecution, int, ITaskExecutionRepository>
    {
        protected override string FormTitle => Resources.Get<TaskExecution>("Clear");

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() => {
                w.FormFieldCalendar("date", Resources.Get<TaskExecution>("DeleteAllRecordsBefore"), DateTime.Today.AddMonths(-2));
            });
        }

        public override void OnSubmit(ApiResponse response)
        {
            var date = FormData.Parse<DateTime>("date");
            Repository.Clear(date);

            response.RedirectBack(Context, 1);
        }
    }
}
