using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tango.Html;
using Tango.Localization;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
	public interface IFieldCollection<TResult>
	{
		IResourceManager Resources { get; }

		bool EnableSelect { get; }
		bool AllowSelectAllPages { get; }
		bool EnableHeadersMenu { get; }

		List<List<ColumnHeader>> HeaderRows { get; }
		List<ColumnHeader> Headers { get; }
		List<ListColumn<TResult>> Cells { get; }
		Action<TagAttributes> ListAttributes { get; set; }
		Action<TagAttributes, TResult, RowInfo> RowAttributes { get; set; }
		Action<TagAttributes, TResult, RowInfo> GroupRowAttributes { get; set; }
		Action<TagAttributes, int> HeaderRowAttributes { get; set; }
		List<ListGroup<TResult>> Groups { get; }

		Action<LayoutWriter, TResult, RowInfo> BeforeRowContent { get; set; }
		Action<LayoutWriter, TResult, RowInfo> AfterRowContent { get; set; }

		void AddHeader(Action<ThTagAttributes> attrs, Action<LayoutWriter> content);
	}

	public interface IFieldCollection<TEntity, TResult> : IFieldCollection<TResult>
	{
		List<ListGroupSorting> GroupSorting { get; }

		void AddHeader(Action<ThTagAttributes> attrs, string title, HeaderOptions options);
		void AddGroupSorting(LambdaExpression expr, bool sortDesc = false);

		int AddSort(LambdaExpression expr);
		int AddFilterCondition<T>(string title, Expression<Func<TEntity, T>> expr);
	}

	public class HeaderOptions
	{
		public int? SortSeqNo { get; set; }
		public int? FilterSeqNo { get; set; }
	}

	public class FieldCollectionBase<TResult> : IFieldCollection<TResult>
	{
		public List<List<ColumnHeader>> HeaderRows { get; } = new List<List<ColumnHeader>> { new List<ColumnHeader>() };
		public List<ColumnHeader> Headers => HeaderRows[HeaderRows.Count - 1];
		public List<ListColumn<TResult>> Cells { get; } = new List<ListColumn<TResult>>();

		public Action<TagAttributes> ListAttributes { get; set; }
		public Action<TagAttributes, int> HeaderRowAttributes { get; set; }
		public Action<TagAttributes, TResult, RowInfo> RowAttributes { get; set; }
		public Action<TagAttributes, TResult, RowInfo> GroupRowAttributes { get; set; }

		public List<ListGroup<TResult>> Groups { get; } = new List<ListGroup<TResult>>();
		public List<ListGroupSorting> GroupSorting { get; } = new List<ListGroupSorting>();

		public IResourceManager Resources => Context.Resources;
		public ActionContext Context { get; protected set; }

		public bool EnableHeadersMenu { get; set; } = false;
		public bool EnableSelect { get; set; }
		public bool AllowSelectAllPages { get; set; } = false;

		public Action<LayoutWriter, TResult, RowInfo> BeforeRowContent { get; set; }
		public Action<LayoutWriter, TResult, RowInfo> AfterRowContent { get; set; }

		public void AddHeader(Action<ThTagAttributes> attrs, Action<LayoutWriter> content)
		{
			Headers.Add(new ColumnHeader { Attributes = attrs, Content = content });
		}

		public FieldCollectionBase(ActionContext context)
		{
			Context = context;
		}
	}

	public class FieldCollection<TEntity, TResult> : FieldCollectionBase<TResult>, IFieldCollection<TEntity, TResult>
	{
		ListFilter<TEntity> _filter;
		ISorter<TEntity> _sorter;

		public FieldCollection(ActionContext context, ISorter<TEntity> sorter, ListFilter<TEntity> filter) : base(context)
		{
			Context = context;
			_sorter = sorter;
			_filter = filter;
		}

		public int AddSort(LambdaExpression expr)
		{
			return _sorter.AddColumn(expr);
		}

		public void AddHeader(Action<ThTagAttributes> attrs, string title, HeaderOptions options)
		{
			AddHeader(attrs, w => {
				if (options.SortSeqNo.HasValue)
					w.SorterLink(_sorter, title, options.SortSeqNo.Value);
				else
					w.Write(title);

				if (EnableHeadersMenu && options.FilterSeqNo.HasValue)
				{
					var id = $"hfilter{options.FilterSeqNo}";
					w.DropDownImage(id, "menu", _filter.RenderHeaderFilter,
						popupAttrs: a => a.DataParm("conditionseqno", options.FilterSeqNo)
							.DataContainer(typeof(ChildFormContainer), w.GetID(id)),
						options: new PopupOptions { CloseOnClick = false });
				}
			});
		}

		public void AddGroupSorting(LambdaExpression expr, bool sortDesc = false)
		{
			GroupSorting.Add(new ListGroupSorting { SeqNo = AddSort(expr), SortDesc = sortDesc });
		}

		public int AddFilterCondition<T>(string title, Expression<Func<TEntity, T>> expr)
		{
			return _filter.AddCondition(title, expr);
		}

        public int AddFilterCondition<T>(Expression<Func<TEntity, T>> expr)
        {
            var title = _filter.Resources.CaptionShort(expr.GetResourceKey());
            return _filter.AddCondition(title, expr);
        }

		[Obsolete]
		public void AddCell(Expression<Func<TEntity, object>> res, Action<ListFieldBuilder<TEntity, TResult>> builder)
		{
			var b = new ListFieldBuilder<TEntity, TResult>(res);
			builder(b);

			var title = Resources.CaptionShort(res);
			if (b.Field.SortExpr != null)
				this.AddHeader(title, new HeaderOptions { SortSeqNo = AddSort(res) });
			else
				this.AddHeader(title);

			b.Field.FilterSetup?.Invoke(_filter);
			Cells.Add(new ListColumn<TResult> { Content = b.Field.Render });
		}
	}

	public class FieldCollection<TResult> : FieldCollection<TResult, TResult>
	{
		public FieldCollection(ActionContext context, ISorter<TResult> sorter, ListFilter<TResult> filter) : base(context, sorter, filter)
		{
		}
	}

	public static class IFieldCollectionBaseExtensions
	{
		public static void SetRowID<TResult, T>(this IFieldCollection<TResult> f, Func<TResult, T> rowid)
		{
			f.RowAttributes += (a, o, i) => {
				var id = rowid(o).ToString();
				a.ID("r" + id).Data("rowid", id);
			};
		}

		public static void AddHeaderRow<TResult>(this IFieldCollection<TResult> f)
		{
			f.HeaderRows.Add(new List<ColumnHeader>());
		}

		#region header only
		public static void AddHeader<TResult>(this IFieldCollection<TResult> f, Action<ThTagAttributes> attrs, string title)
		{
			f.AddHeader(attrs, w => w.Write(title));
		}
		public static void AddHeader<TResult>(this IFieldCollection<TResult> f, string title)
		{
			f.AddHeader(null, title);
		}
		public static void AddHeader<TResult, T>(this IFieldCollection<TResult> f, Action<ThTagAttributes> attrs, Expression<Func<TResult, T>> res)
		{
			f.AddHeader(attrs, f.Resources.CaptionShort(res));
		}
		public static void AddHeader<TResult, T>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res)
		{
			f.AddHeader(null, res);
		}
		public static void AddHeader<TResult>(this IFieldCollection<TResult> f, Action<LayoutWriter> content)
		{
			f.AddHeader(null, content);
		}
		#endregion

		#region custom
		public static void AddCustomCell<TResult>(this IFieldCollection<TResult> f, ColumnHeader header, ListColumn<TResult> cell)
		{
			f.Headers.Add(header);
			f.Cells.Add(cell);
		}

		public static void AddCustomCell<TResult>(this IFieldCollection<TResult> f, string title, ListColumn<TResult> cell)
		{
			f.AddHeader(title);
			f.Cells.Add(cell);
		}

		public static void AddCustomCell<TResult>(this IFieldCollection<TResult> f, ListColumn<TResult> cell)
		{
			f.Cells.Add(cell);
		}
		#endregion

		#region cell only
		public static void AddCell<TResult>(this IFieldCollection<TResult> f, RenderRowCellDelegate<TResult> render)
		{
			f.Cells.Add(new ListColumn<TResult> { Content = render });
		}

		public static void AddCell<TResult>(this IFieldCollection<TResult> f, Action<LayoutWriter, TResult> render)
		{
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o) });
		}

		public static void AddCell<TResult, T>(this IFieldCollection<TResult> f, Func<TResult, T> value)
		{
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) });
		}

		//public static void AddCell<TResult>(this IFieldCollection<TResult> f, ListColumn<TResult> listColumn)
		//{
		//	f.Cells.Add(listColumn);
		//}

		public static void AddCellAlignRight<TResult, T>(this IFieldCollection<TResult> f, Func<TResult, T> value)
		{
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()), Attributes = (a, o, i) => a.Class("r") });
		}
		public static void AddCellAlignRight<TResult>(this IFieldCollection<TResult> f, Action<LayoutWriter, TResult> render)
		{
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o), Attributes = (a, o, i) => a.Class("r") });
		}
		public static void AddCellAlignRight<TResult>(this IFieldCollection<TResult> f, RowCellAttributesDelegate<TResult> attrs, Action<LayoutWriter, TResult> render)
		{
			RowCellAttributesDelegate<TResult> a1 = (a, o, i) => a.Class("r");
			a1 += attrs;
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o), Attributes = a1 });
		}
		public static void AddCellAlignRight<TResult, T>(this IFieldCollection<TResult> f, string title, Func<TResult, T> value)
        {
            f.AddHeader(title);
            f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()), Attributes = (a, o, i) => a.Class("r") });
        }
        public static void AddCellAlignRight<TResult>(this IFieldCollection<TResult> f, string title, Action<LayoutWriter, TResult> render)
        {
            f.AddHeader(title);
            f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o), Attributes = (a, o, i) => a.Class("r") });
        }
        public static void AddCellAlignCenter<TResult, T>(this IFieldCollection<TResult> f, Func<TResult, T> value)
        {
            f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()), Attributes = (a, o, i) => a.Class("c") });
        }
        public static void AddCellAlignCenter<TResult>(this IFieldCollection<TResult> f, Action<LayoutWriter, TResult> render)
        {
            f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o), Attributes = (a, o, i) => a.Class("c") });
        }
        public static void AddCellAlignCenter<TResult>(this IFieldCollection<TResult> f, RowCellAttributesDelegate<TResult> attrs, Action<LayoutWriter, TResult> render)
        {
            RowCellAttributesDelegate<TResult> a1 = (a, o, i) => a.Class("c");
            a1 += attrs;
            f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o), Attributes = a1 });
        }
        public static void AddCellAlignCenter<TResult, T>(this IFieldCollection<TResult> f, string title, Func<TResult, T> value)
        {
            f.AddHeader(title);
            f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()), Attributes = (a, o, i) => a.Class("c") });
        }
        public static void AddCellAlignCenter<TResult>(this IFieldCollection<TResult> f, string title, Action<LayoutWriter, TResult> render)
        {
            f.AddHeader(title);
            f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o), Attributes = (a, o, i) => a.Class("c") });
        }

        #endregion

        #region string title
        public static void AddCell<TResult>(this IFieldCollection<TResult> f, string title, RenderRowCellDelegate<TResult> render)
		{
			f.AddHeader(title);
			f.Cells.Add(new ListColumn<TResult> { Content = render });
		}

		public static void AddCell<TResult>(this IFieldCollection<TResult> f, string title, Action<LayoutWriter, TResult> render)
		{
			f.AddHeader(title);
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o) });
		}

		public static void AddCell<TResult, T>(this IFieldCollection<TResult> f, string title, Func<TResult, T> value)
		{
			f.AddHeader(title);
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) });
		}
		#endregion

		#region string title with attributes
		public static void AddCell<TResult>(this IFieldCollection<TResult> f, string title, RowCellAttributesDelegate<TResult> attrs, RenderRowCellDelegate<TResult> render)
		{
			f.AddHeader(title);
			f.Cells.Add(new ListColumn<TResult> { Content = render, Attributes = attrs });
		}

		public static void AddCell<TResult>(this IFieldCollection<TResult> f, string title, RowCellAttributesDelegate<TResult> attrs, Action<LayoutWriter, TResult> render)
		{
			f.AddHeader(title);
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o), Attributes = attrs });
		}

		public static void AddCell<TResult, T>(this IFieldCollection<TResult> f, string title, RowCellAttributesDelegate<TResult> attrs, Func<TResult, T> value)
		{
			f.AddHeader(title);
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()), Attributes = attrs });
		}
		#endregion

		#region res title
		public static void AddCell<TResult, T>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res, RenderRowCellDelegate<TResult> render)
		{
			f.AddHeader(res);
			f.Cells.Add(new ListColumn<TResult> { Content = render });
		}

		public static void AddCell<TResult, T>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res, Action<LayoutWriter, TResult> render)
		{
			f.AddHeader(res);
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o) });
		}

		public static void AddCell<TResult, T, T2>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res, Func<TResult, T2> value)
		{
			f.AddHeader(res);
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) });
		}
		#endregion

		

		public static void AddActionsCell<TResult>(this IFieldCollection<TResult> f, params Func<TResult, Action<ActionLink>>[] actions)
		{
            AddActionsCell(f, f.Resources.Get("Common.Actions"), actions);
        }

        public static void AddActionsCell<TResult>(this IFieldCollection<TResult> f, string title, params Func<TResult, Action<ActionLink>>[] actions)
        {
            f.AddCustomCell(title, new ListColumn<TResult>(
                (a, o, i) => a.Style("text-align:center; white-space:nowrap"),
                (w, o, i) => {
                    foreach (var action in actions.Select(a => a(o)))
                        w.ActionImage(action);
                }
            ));
        }

        public static void AddActionsCell<TResult>(this IFieldCollection<TResult> f, Action<LayoutWriter, TResult> render)
		{
			f.AddCustomCell(f.Resources.Get("Common.Actions"), new ListColumn<TResult>(
				(a, o, i) => a.Style("text-align:center"),
				(w, o, i) => render(w, o)
			));
		}

		public static ListGroup<TResult> AddGroup<TResult>(this IFieldCollection<TResult> f, Func<TResult, string> value, RenderGroupCellDelegate<TResult> cell)
		{
			var g = new ListGroup<TResult> { ValueFunc = value, Cell = cell };
			f.Groups.Add(g);
			return g;
		}

		public static ListGroup<TResult> AddGroup<TResult>(this IFieldCollection<TResult> f, Func<TResult, string> value)
		{
			var g = new ListGroup<TResult> { ValueFunc = value, Cell = (w, o) => w.Write(value(o)) };
			f.Groups.Add(g);
			return g;
		}
	}

	public static class IFieldCollectionExtensions
	{
		static string GetTitle<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, Expression<Func<TEntity, T>> expr)
		{
			return f.Resources.CaptionShort(expr);
		}
		public static void AddHeader<TEntity, TResult>(this IFieldCollection<TEntity, TResult> f, string title, HeaderOptions options)
		{
			f.AddHeader(null, title, options);
		}

		#region res title = sort expr
		public static void AddCellWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, Expression<Func<TEntity, T>> res, RenderRowCellDelegate<TResult> render)
		{
			f.AddHeader(f.GetTitle(res), new HeaderOptions { SortSeqNo = f.AddSort(res) });
			f.Cells.Add(new ListColumn<TResult> { Content = render });
		}

		public static void AddCellWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, Expression<Func<TEntity, T>> res, Action<LayoutWriter, TResult> render)
		{
			f.AddHeader(f.GetTitle(res), new HeaderOptions { SortSeqNo = f.AddSort(res) });
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o) });
		}

		public static void AddCellWithSort<TEntity, TResult, T, T2>(this IFieldCollection<TEntity, TResult> f, Expression<Func<TEntity, T>> res, Func<TResult, T2> value)
		{
			f.AddHeader(f.GetTitle(res), new HeaderOptions { SortSeqNo = f.AddSort(res) });
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) });
		}
		#endregion

		#region string title, sort expr
		public static void AddCellWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, string title, Expression<Func<TEntity, T>> sortExpr, RenderRowCellDelegate<TResult> render)
		{
			f.AddHeader(title, new HeaderOptions { SortSeqNo = f.AddSort(sortExpr) });
			f.Cells.Add(new ListColumn<TResult> { Content = render });
		}

		public static void AddCellWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, string title, Expression<Func<TEntity, T>> sortExpr, Action<LayoutWriter, TResult> render)
		{
			f.AddHeader(title, new HeaderOptions { SortSeqNo = f.AddSort(sortExpr) });
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o) });
		}

		public static void AddCellWithSort<TEntity, TResult, T, T2>(this IFieldCollection<TEntity, TResult> f, string title, Expression<Func<TEntity, T>> sortExpr, Func<TResult, T2> value)
		{
			f.AddHeader(title, new HeaderOptions { SortSeqNo = f.AddSort(sortExpr) });
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) });
		}
		#endregion

		#region res title = sort expr = filter expr
		public static void AddCellWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr, RenderRowCellDelegate<TResult> render)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);

			f.AddHeader(titleShort, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(new ListColumn<TResult> { Content = render });
		}

		public static void AddCellWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr, Action<LayoutWriter, TResult> render)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);

			f.AddHeader(titleShort, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o) });
		}

		public static void AddCellWithSortAndFilter<TEntity, TResult, T, T2>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr, Func<TResult, T2> value)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);

			f.AddHeader(titleShort, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) });
		}
		#endregion

		#region string title, sort expr = filter expr
		public static void AddCellWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			string title, Expression<Func<TEntity, T>> expr, RenderRowCellDelegate<TResult> render)
		{
			f.AddHeader(title, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(new ListColumn<TResult> { Content = render });
		}

		public static void AddCellWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			string title, Expression<Func<TEntity, T>> expr, Action<LayoutWriter, TResult> render)
		{
			f.AddHeader(title, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => render(w, o) });
		}

		public static void AddCellWithSortAndFilter<TEntity, TResult, T, T2>(this IFieldCollection<TEntity, TResult> f,
			string title, Expression<Func<TEntity, T>> expr, Func<TResult, T2> value)
		{
			f.AddHeader(title, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) });
		}
		#endregion

		public static void AddHeaderWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);

			f.AddHeader(titleShort, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
		}
		public static void AddHeaderWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr)
		{
			var title = f.GetTitle(expr);
			f.AddHeader(title, new HeaderOptions {
				SortSeqNo = f.AddSort(expr)
			});
		}

		public static void AddHeaderWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Action<ThTagAttributes> attrs, Expression<Func<TEntity, T>> expr)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);

			f.AddHeader(attrs, titleShort, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
		}
		public static void AddHeaderWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Action<ThTagAttributes> attrs, Expression<Func<TEntity, T>> expr)
		{
			var title = f.GetTitle(expr);
			f.AddHeader(attrs, title, new HeaderOptions {
				SortSeqNo = f.AddSort(expr)
			});
		}


		#region groups
		public static ListGroup<TResult> AddGroup<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, Func<TResult, string> value,
			Expression<Func<TEntity, T>> sortExpression, RenderGroupCellDelegate<TResult> cell, bool sortDesc = false)
		{
			var g = f.AddGroup(value, cell);
			f.AddGroupSorting(sortExpression, sortDesc);
			return g;
		}

		public static ListGroup<TResult> AddGroup<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, Func<TResult, string> value,
			Expression<Func<TEntity, T>> sortExpression, bool sortDesc = false)
		{
			return f.AddGroup(value, sortExpression, (w, o) => w.Write(value(o)), sortDesc);
		}
		#endregion
	}
}
