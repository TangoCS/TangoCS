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
    [OnAction(typeof(TaskGroup), "viewlist")]
    public class tm_taskgroup_list : default_list_rep<TaskGroup>
    {
        protected override Func<string, Expression<Func<TaskGroup, bool>>> SearchExpression =>
            s => o => o.Title.Contains(s);

        protected override void ToolbarLeft(MenuBuilder left)
        {
            left.ItemFilter(Filter);
        }

        protected override void FieldsInit(FieldCollection<TaskGroup> f)
        {
            f.AddCellWithSortAndFilter(o => o.TaskGroupID, (w, o) => w.ActionLink(al => al.ToView(AccessControl, o).WithTitle(o.TaskGroupID)));
            f.AddCellWithSortAndFilter(o => o.Title, o => o.Title);
			f.AddCellWithSortAndFilter(o => o.SeqNo, o => o.SeqNo);
		}
    }

    [OnAction(typeof(TaskGroup), "view")]
    public class tm_taskgroup_view : default_view_rep<TaskGroup, int>
    {
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemBack();
            t.ItemSeparator();
        }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() => {
                w.PlainText(Resources.Get<TaskGroup>("TaskGroupID"), ViewData.TaskGroupID);
                w.PlainText(Resources.Get<TaskGroup>("Title"), ViewData.Title);
				w.PlainText(Resources.Get<TaskGroup>("SeqNo"), ViewData.SeqNo);
			});
        }
    }

    [OnAction(typeof(TaskGroup), "createnew")]
    [OnAction(typeof(TaskGroup), "edit")]
    public class tm_taskgroup_edit : default_edit_rep<TaskGroup, int>
    {
        protected TaskGroupFields.DefaultGroup gr { get; set; }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.TextBox(gr.Title);
				w.TextBox(gr.SeqNo);
			});
        }
    }

    [OnAction(typeof(TaskGroup), "delete")]
    public class tm_taskgroup_delete : default_delete<TaskGroup, int> { }
}
