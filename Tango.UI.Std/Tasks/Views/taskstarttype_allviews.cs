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
    [OnAction(typeof(TaskStartType), "viewlist")]
    public class tm_taskstarttype_list : default_list_rep<TaskStartType>
    {
        protected override Func<string, Expression<Func<TaskStartType, bool>>> SearchExpression =>
            s => o => o.Title.Contains(s);

        protected override void ToolbarLeft(MenuBuilder left)
        {
            left.ItemFilter(Filter);
        }

        protected override void FieldsInit(FieldCollection<TaskStartType> f)
        {
            f.AddCellWithSortAndFilter(o => o.TaskTypeID, (w, o) => w.ActionLink(al => al.ToView(AccessControl, o).WithTitle(o.TaskTypeID)));
            f.AddCellWithSortAndFilter(o => o.Title, o => o.Title);
        }
    }

    [OnAction(typeof(TaskStartType), "view")]
    public class tm_taskstarttype_view : default_view_rep<TaskStartType, int>
    {
        protected override void ToolbarLeft(MenuBuilder t)
        {
            t.ItemBack();
            t.ItemSeparator();
        }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() => {
                w.PlainText(Resources.Get<TaskStartType>("TaskTypeID"), ViewData.TaskTypeID);
                w.PlainText(Resources.Get<TaskStartType>("Title"), ViewData.Title);
            });
        }
    }

    [OnAction(typeof(TaskStartType), "createnew")]
    [OnAction(typeof(TaskStartType), "edit")]
    public class tm_taskstarttype_edit : default_edit_rep<TaskStartType, int>
    {
        protected TaskStartTypeFields.DefaultGroup gr { get; set; }

        protected override void Form(LayoutWriter w)
        {
            w.FieldsBlockStd(() =>
            {
                w.TextBox(gr.Title);
            });
        }
    }

    [OnAction(typeof(TaskStartType), "delete")]
    public class tm_taskstarttype_delete : default_delete<TaskStartType, int> { }
}
