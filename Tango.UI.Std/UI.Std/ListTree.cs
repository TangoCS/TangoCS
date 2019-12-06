using System;
using Tango;
using Tango.Html;

namespace Tango.UI.Std
{
	public static class ListTreeExtensions
    {
		public static void InitListTree<T>(this FieldCollectionBase<T> f)
			where T : IListTree
		{
			f.ListAttributes += a => a.Class("tree");
			f.RowAttributes += (a, o, i) => a.Data("level", o.Level);
		}

		static ListColumn<T> ListColumn<T>(Action<LayoutWriter, T> content, TreeCellOptions<T> options = null)
			where T : IListTree
		{
			return new ListColumn<T>((a, o, row) => { }, (w, o, row) => {
				w.Div(a => a.Class($"treerow l{o.Level}"), () => {
					for (int i = 0; i < o.Level; i++)
						w.Div(a => a.Class("level-padding" + (i == o.Level ? " last" : "")), "");

					if (o.HasChildren)
					{
						w.Div(a => a.Class("togglelevel").Class(options?.NodeClass(o)), () => {
							w.Span(a => a.OnClick("listview.togglelevel(this)"), () => w.Icon("right"));
						});
					}
					else
						w.Div(a => a.Class("leaf").Class(options?.NodeClass(o)), () => w.Span("&nbsp;"));

					w.Div(() => {
						content(w, o);
					});
				});
			});
		}

		public static void AddTreeCell<T>(this FieldCollectionBase<T> f, string title, Action<LayoutWriter, T> content, TreeCellOptions<T> options = null)
			where T : IListTree
		{
			var header = new ColumnHeader(a => a.Style("min-width:300px"), w => w.Write(title));
			f.AddCustomCell(header,	ListColumn(content, options));
		}
		public static void AddTreeCell<T>(this FieldCollectionBase<T> f, Action<LayoutWriter, T> content, TreeCellOptions<T> options = null)
			where T : IListTree
		{
			f.AddCustomCell(ListColumn(content, options));
		}

	}

	public class TreeCellOptions<T>
		where T: IListTree
	{
		public Func<T, string> NodeClass { get; set; }
	}

	public interface IListTree
	{
		int Level { get; }
		bool HasChildren { get; }
	}
}
