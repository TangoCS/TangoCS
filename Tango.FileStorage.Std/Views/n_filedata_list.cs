using System;
using System.Linq.Expressions;
using Tango.UI;
using Tango.UI.Controls;
using Tango.UI.Std;
using Tango.FileStorage.Std.Model;
using System.Linq;

namespace Tango.FileStorage.Std.Views
{
	[OnAction(typeof(N_FileData), "viewlist")]
	public class n_filedata_list : default_list<N_FileData>
	{
		MetaN_FileData meta = new MetaN_FileData();

		protected override void ToolbarLeft(MenuBuilder left)
		{
			left.ItemFilter(Filter);
			left.ItemSeparator();
			left.ItemActionImageText(x => x.ToN_FileData_Upload(AccessControl));
		}

		protected override void FieldsInit(FieldCollection<N_FileData, N_FileData> f)
		{
			f.AddCellWithSortAndFilter(o => o.Title, (w, o) => w.ActionLink(al => al.ToEdit(AccessControl, o).WithTitle(o.Title)));
			f.AddCellWithSortAndFilter(o => o.LastModifiedDate, o => o.LastModifiedDate.DateTimeToString());
			f.AddCellWithSortAndFilter(o => o.Size, o => o.Size);
			f.AddCell(o => o.Owner, o => o.Owner);
            f.AddFilterCondition(o => o.Owner);
			f.AddActionsCell(o => al => al.ToDelete(AccessControl, o));
		}

		//protected override Expression<Func<N_FileData, N_FileData>> SelectExpression()
		protected override IQueryable<N_FileData> Selector(IQueryable<N_FileData> data)
		{
			return data.Select(o => new N_FileData {
				Title = o.Title,
				Owner = o.Owner,
				Size = o.Size,
				LastModifiedDate = o.LastModifiedDate,
				FileGUID = o.FileGUID
			});
		}
	}
}
