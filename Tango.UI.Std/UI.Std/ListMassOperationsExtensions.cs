using System;
using Tango.Html;

namespace Tango.UI.Std.ListMassOperations
{
	public static class ListMassOperationsExtensions
	{
		public static void AddCheckBoxCell<TResult>(this IFieldCollection<TResult> f, int colSeqNo)
		{
			f.HeaderRows[0].Insert(colSeqNo, new ColumnHeader(
				a => a.ID("sel_header").Class("sel_header").RowSpan(f.HeaderRows.Count), 
				w => {
					w.IconThreeStateCheckBox();
					w.Hidden("selectedvalues", null, a => a.DataHasClientState(ClientStateType.Array));
				}
			));

			f.Cells.Insert(colSeqNo,
				new ListColumn<TResult>(
					(a, o, i) => a.Class("sel"),
					(w, o, i) => w.IconCheckBox()
				)
			);
		}

		public static void InfoRow(this LayoutWriter w, int colSpan)
		{
			w.Tr(a => a.ID("sel_info").Class("sel_info").Style("display:none"), () => w.Td(a => a.ColSpan(colSpan), () => {
				w.Write("Rows selected&nbsp;&mdash;&nbsp;");
				w.Span(a => a.ID("sel_info_cnt"), "0");
				w.Span(a => a.ID("sel_info_all").Style("display:none"), "all");

				w.Write("&nbsp;&nbsp;&nbsp;");
				w.A(a => a.OnClick($"listview.selectall('{w.IDPrefix}')"), "Select all rows in this list");
				w.Write("&nbsp;&#183;&nbsp;");
				w.A(a => a.OnClick($"listview.clearselection('{w.IDPrefix}')"), "Clear selection");
			}));
		}
	}
}
