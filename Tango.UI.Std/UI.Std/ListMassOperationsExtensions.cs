using System;
using Tango.Html;

namespace Tango.UI.Std.ListMassOperations
{
	public static class ListMassOperationsExtensions
	{
		public static void AddCheckBoxCell<TResult>(this IFieldCollection<TResult> f, int colSeqNo, CheckBoxCellSettings settings = null)
		{
			if(settings == null)
				settings = new CheckBoxCellSettings() { HeadColSeqNo = colSeqNo, BodyColSeqNo = colSeqNo };

			f.AddCheckBoxCell(settings);
		}

		public static void AddCheckBoxCell<TResult>(this IFieldCollection<TResult> f, CheckBoxCellSettings settings)
		{
			f.HeaderRows[settings.HeaderRowNo].Insert(settings.HeadColSeqNo, new ColumnHeader(
				a => a.ID("sel_header").Class("sel_header").RowSpan(settings.RowSpan == 0 ? f.HeaderRows.Count : settings.RowSpan), null,
				w => {
					w.IconThreeStateCheckBox();
					w.Hidden("selectedvalues", null, a => a.DataHasClientState(ClientStateType.Array));
					if (!String.IsNullOrEmpty(settings.Title)) w.Write($" {settings.Title}");
				}
			));

			f.Cells.Insert(settings.BodyColSeqNo,
				new ListColumn<TResult>(
					(a, o, i) => a.Class("sel"),
					(w, o, i) => w.IconCheckBox(settings.Attributes)
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
