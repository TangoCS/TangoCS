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
		bool EnableFixedHeader { get; }

		List<List<IColumnHeader>> HeaderRows { get; }
		List<IColumnHeader> Headers { get; }
		List<IListColumn<TResult>> Cells { get; }
		Action<TagAttributes> ListAttributes { get; set; }
		Action<TagAttributes, TResult, RowInfo<TResult>> RowAttributes { get; set; }
		Action<TagAttributes, TResult, RowInfo<TResult>> GroupRowAttributes { get; set; }
		Action<TagAttributes, int> HeaderRowAttributes { get; set; }
		List<ListGroup<TResult>> Groups { get; }

		Action<LayoutWriter, IEnumerable<TResult>> BeforeHeader { get; set; }
		Action<TResult, RowInfo<TResult>> BeforeRowContent { get; set; }
		Func<TResult, RowInfo<TResult>, IEnumerable<ListColumn<TResult>>> AfterRowContent { get; set; }

		IColumnHeader AddHeader(Action<ThTagAttributes> attrs, Action<LayoutWriter> content);
	}

	public interface IFieldCollection<TEntity, TResult> : IFieldCollection<TResult>
	{
		List<ListGroupSorting> GroupSorting { get; }

		IColumnHeader AddHeader(Action<ThTagAttributes> attrs, string title, HeaderOptions options);
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
        public List<List<IColumnHeader>> HeaderRows { get; } = new List<List<IColumnHeader>> { new List<IColumnHeader>() };
		public List<IColumnHeader> Headers => HeaderRows[HeaderRows.Count - 1];
		public List<IListColumn<TResult>> Cells { get; } = new List<IListColumn<TResult>>();

		public Action<TagAttributes> ListAttributes { get; set; }
		public Action<TagAttributes, int> HeaderRowAttributes { get; set; }
		public Action<TagAttributes, TResult, RowInfo<TResult>> RowAttributes { get; set; }
		public Action<TagAttributes, TResult, RowInfo<TResult>> GroupRowAttributes { get; set; }

		public List<ListGroup<TResult>> Groups { get; } = new List<ListGroup<TResult>>();
		public List<ListGroupSorting> GroupSorting { get; } = new List<ListGroupSorting>();

		public IResourceManager Resources => Context.Resources;
		public ActionContext Context { get; protected set; }

		public bool EnableHeadersMenu { get; set; } = false;
        public bool EnableFixedHeader { get; set; } = false;
		public bool EnableSelect { get; set; }
		public bool AllowSelectAllPages { get; set; } = false;

		public Action<TResult, RowInfo<TResult>> BeforeRowContent { get; set; }
		public Func<TResult, RowInfo<TResult>, IEnumerable<ListColumn<TResult>>> AfterRowContent { get; set; }
		public Action<LayoutWriter, IEnumerable<TResult>> BeforeHeader { get; set; }

		public IColumnHeader AddHeader(Action<ThTagAttributes> attrs, Action<LayoutWriter> content)
		{
			var columnHeader = new ColumnHeader {Attributes = attrs, Content = content};
			
			Headers.Add(new ColumnHeader { Attributes = attrs, Content = content });

			return columnHeader;
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

		public IColumnHeader AddHeader(Action<ThTagAttributes> attrs, string title, HeaderOptions options)
		{
			var columnHeader = AddHeader(attrs, w => {
				if (options.SortSeqNo.HasValue)
					w.SorterLink(_sorter, title, options.SortSeqNo.Value);
				else
					w.Write(title);

				if (EnableHeadersMenu && options.FilterSeqNo.HasValue)
				{
					var id = $"hfilter{options.FilterSeqNo}";
					w.DropDownImage(id, "menu", _filter.RenderHeaderFilter,
						popupAttrs: a => a.DataParm("conditionseqno", options.FilterSeqNo)
							.DataNewContainer(typeof(ChildFormContainer), w.GetID(id)),
						options: new PopupOptions { CloseOnClick = false });
				}
			});

			return columnHeader;
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
			f.HeaderRows.Add(new List<IColumnHeader>());
		}

		#region header only
		public static IColumnHeader AddHeader<TResult>(this IFieldCollection<TResult> f, Action<ThTagAttributes> attrs, string title)
		{
			var columnHeader = f.AddHeader(attrs, w => w.Write(title));
			return columnHeader;
		}
		public static IColumnHeader AddHeader<TResult>(this IFieldCollection<TResult> f, string title)
		{
			var columnHeader = f.AddHeader(null, title);
			return columnHeader;
		}
		public static IColumnHeader AddHeader<TResult, T>(this IFieldCollection<TResult> f, Action<ThTagAttributes> attrs, Expression<Func<TResult, T>> res)
		{
			var columnHeader = f.AddHeader(attrs, f.Resources.CaptionShort(res));
			return columnHeader;
		}
		public static IColumnHeader AddHeader<TResult, T>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res)
		{
			var columnHeader = f.AddHeader(null, res);
			return columnHeader;
		}
		public static IColumnHeader AddHeader<TResult>(this IFieldCollection<TResult> f, Action<LayoutWriter> content)
		{
			var columnHeader = f.AddHeader(null, content);
			return columnHeader;
		}
		#endregion

		#region custom
		public static void AddCustomCell<TResult>(this IFieldCollection<TResult> f, IColumnHeader header, IListColumn<TResult> cell)
		{
			f.Headers.Add(header);
			f.Cells.Add(cell);
		}

		public static void AddCustomCell<TResult>(this IFieldCollection<TResult> f, string title, IListColumn<TResult> cell)
		{
			f.AddHeader(title);
			f.Cells.Add(cell);
		}

		public static IListColumn<TResult> AddCustomCell<TResult>(this IFieldCollection<TResult> f, IListColumn<TResult> cell)
		{
			f.Cells.Add(cell);

			return cell;
		}
		#endregion

		#region cell only
		public static IListColumn<TResult> AddCell<TResult>(this IFieldCollection<TResult> f, RowCellAttributesDelegate<TResult> attrs, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Attributes = attrs, Content = render };
			f.Cells.Add(listColumn);
			return listColumn;
		}

		public static IListColumn<TResult> AddCell<TResult>(this IFieldCollection<TResult> f, RenderRowCellDelegate<TResult> render)
		{
			return f.AddCell((a, o, i) => { }, render);
		}

		public static IListColumn<TResult> AddCell<TResult>(this IFieldCollection<TResult> f, Action<LayoutWriter, TResult> render)
		{
			return f.AddCell((a, o, i) => { }, (w, o, i) => render(w, o));
		}

		public static IListColumn<TResult> AddCell<TResult, T>(this IFieldCollection<TResult> f, Func<TResult, T> value)
		{
			return f.AddCell((a, o, i) => { }, (w, o, i) => w.Write(value(o)?.ToString()));
		}

		//public static void AddCell<TResult>(this IFieldCollection<TResult> f, ListColumn<TResult> listColumn)
		//{
		//	f.Cells.Add(listColumn);
		//}

		public static IListColumn<TResult> AddCellAlignRight<TResult, T>(this IFieldCollection<TResult> f, Func<TResult, T> value)
		{
			var listColumn = new ListColumn<TResult> {Content = (w, o, i) => w.Write(value(o)?.ToString()), Attributes = (a, o, i) => a.Class("r")};
			f.Cells.Add(listColumn);
			return listColumn;
		}
		public static IListColumn<TResult> AddCellAlignRight<TResult>(this IFieldCollection<TResult> f, Action<LayoutWriter, TResult> render)
		{
			var listColumn = new ListColumn<TResult> {Content = (w, o, i) => render(w, o), Attributes = (a, o, i) => a.Class("r")};
			f.Cells.Add(listColumn);
			return listColumn;
		}
		public static IListColumn<TResult> AddCellAlignRight<TResult>(this IFieldCollection<TResult> f, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = render, Attributes = (a, o, i) => a.Class("r") };
			f.Cells.Add(listColumn);
			return listColumn;
		}
		public static IListColumn<TResult> AddCellAlignRight<TResult>(this IFieldCollection<TResult> f, RowCellAttributesDelegate<TResult> attrs, Action<LayoutWriter, TResult> render)
		{
			RowCellAttributesDelegate<TResult> a1 = (a, o, i) => a.Class("r");
			a1 += attrs;
			var listColumn = new ListColumn<TResult> {Content = (w, o, i) => render(w, o), Attributes = a1};
			f.Cells.Add(listColumn);
			return listColumn;
		}
		public static AddCellResult<TResult> AddCellAlignRight<TResult, T>(this IFieldCollection<TResult> f, string title, RowCellAttributesDelegate<TResult> attrs, Func<TResult, T> value)
		{
			return f.AddCellAlignRight(title, attrs, (w, o, i) => w.Write(value(o)?.ToString()));
		}
		public static AddCellResult<TResult> AddCellAlignRight<TResult, T>(this IFieldCollection<TResult> f, string title, Func<TResult, T> value)
        {
			return f.AddCellAlignRight(title, null, (w, o, i) => w.Write(value(o)?.ToString()));
        }
        public static AddCellResult<TResult> AddCellAlignRight<TResult>(this IFieldCollection<TResult> f, string title, Action<LayoutWriter, TResult> render)
        {
			return f.AddCellAlignRight(title, null, (w, o, i) => render(w, o));
        }
		public static AddCellResult<TResult> AddCellAlignRight<TResult>(this IFieldCollection<TResult> f, string title, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = render, Attributes = (a, o, i) => a.Class("r") };
			var columnHeader = f.AddHeader(title);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> { Column = listColumn, Header = columnHeader };
		}
		public static IListColumn<TResult> AddCellAlignCenter<TResult, T>(this IFieldCollection<TResult> f, Func<TResult, T> value)
        {
	        var listColumn = new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()), Attributes = (a, o, i) => a.Class("c") };
            f.Cells.Add(listColumn);
            return listColumn;
        }
        public static IListColumn<TResult> AddCellAlignCenter<TResult>(this IFieldCollection<TResult> f, Action<LayoutWriter, TResult> render)
        {
	        var listColumn = new ListColumn<TResult> { Content = (w, o, i) => render(w, o), Attributes = (a, o, i) => a.Class("c") };
	        f.Cells.Add(listColumn);
            return listColumn;
        }
        public static IListColumn<TResult> AddCellAlignCenter<TResult>(this IFieldCollection<TResult> f, RowCellAttributesDelegate<TResult> attrs, Action<LayoutWriter, TResult> render)
        {
            RowCellAttributesDelegate<TResult> a1 = (a, o, i) => a.Class("c");
            a1 += attrs;
            var listColumn = new ListColumn<TResult> {Content = (w, o, i) => render(w, o), Attributes = a1};
            f.Cells.Add(listColumn);
            return listColumn;
        }
        public static AddCellResult<TResult> AddCellAlignCenter<TResult, T>(this IFieldCollection<TResult> f, string title, Func<TResult, T> value)
        {
			return f.AddCellAlignCenter(title, null, (w, o, i) => w.Write(value(o)?.ToString()));
        }
        public static AddCellResult<TResult> AddCellAlignCenter<TResult>(this IFieldCollection<TResult> f, string title, Action<LayoutWriter, TResult> render)
        {
			return f.AddCellAlignCenter(title, null, (w, o, i) => render(w, o));
        }

		public static AddCellResult<TResult> AddCellAlignCenter<TResult>(this IFieldCollection<TResult> f, string title, RowCellAttributesDelegate<TResult> attrs, RenderRowCellDelegate<TResult> render)
		{
			RowCellAttributesDelegate<TResult> a1 = (a, o, i) => a.Class("c");
			if (attrs != null)
				a1 += attrs;
			var columnHeader = f.AddHeader(title);
			var listColumn = new ListColumn<TResult> { Content = render, Attributes = a1 };
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> { Column = listColumn, Header = columnHeader };
		}

		public static AddCellResult<TResult> AddCellAlignRight<TResult>(this IFieldCollection<TResult> f, string title, RowCellAttributesDelegate<TResult> attrs, RenderRowCellDelegate<TResult> render)
		{
			RowCellAttributesDelegate<TResult> a1 = (a, o, i) => a.Class("r");
			if (attrs != null)
				a1 += attrs;
			var columnHeader = f.AddHeader(title);
			var listColumn = new ListColumn<TResult> { Content = render, Attributes = a1 };
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> { Column = listColumn, Header = columnHeader };
		}

		#endregion

		#region string title
		public static AddCellResult<TResult> AddCell<TResult>(this IFieldCollection<TResult> f, string title, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = render };
			var columnHeader = f.AddHeader(title);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCell<TResult>(this IFieldCollection<TResult> f, string title, Action<LayoutWriter, TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => render(w, o) };
			var columnHeader = f.AddHeader(title);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCell<TResult, T>(this IFieldCollection<TResult> f, string title, Func<TResult, T> value)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) };
			var columnHeader = f.AddHeader(title);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}
		#endregion

		#region string title with attributes
		public static AddCellResult<TResult> AddCell<TResult>(this IFieldCollection<TResult> f, string title, RowCellAttributesDelegate<TResult> attrs, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = render, Attributes = attrs };
			var columnHeader = f.AddHeader(title);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCell<TResult>(this IFieldCollection<TResult> f, string title, RowCellAttributesDelegate<TResult> attrs, Action<LayoutWriter, TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => render(w, o), Attributes = attrs };
			var columnHeader = f.AddHeader(title);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCell<TResult, T>(this IFieldCollection<TResult> f, string title, RowCellAttributesDelegate<TResult> attrs, Func<TResult, T> value)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()), Attributes = attrs };
			var columnHeader = f.AddHeader(title);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}
		#endregion

		#region res title
		public static AddCellResult<TResult> AddCell<TResult, T>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> {Content = render};
			var columnHeader =  f.AddHeader(res);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult>{Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCell<TResult, T>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res, Action<LayoutWriter, TResult> render)
		{
			var listColumn = new ListColumn<TResult> {Content = (w, o, i) => render(w, o)};
			var columnHeader = f.AddHeader(res);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult>{Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCell<TResult, T, T2>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res, Func<TResult, T2> value)
		{
			var listColumn = new ListColumn<TResult> {Content = (w, o, i) => w.Write(value(o)?.ToString())};
			var columnHeader = f.AddHeader(res);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult>{Column = listColumn, Header = columnHeader};
		}

		

		
		#endregion

		#region res title with attributes
		public static AddCellResult<TResult> AddCell<TResult, T>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res,
			RowCellAttributesDelegate<TResult> attrs, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = render, Attributes = attrs };
			var columnHeader = f.AddHeader(res);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult>{Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCell<TResult, T>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res,
			RowCellAttributesDelegate<TResult> attrs, Action<LayoutWriter, TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => render(w, o), Attributes = attrs };
			var columnHeader = f.AddHeader(res);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult>{Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCell<TResult, T, T2>(this IFieldCollection<TResult> f, Expression<Func<TResult, T>> res,
			RowCellAttributesDelegate<TResult> attrs, Func<TResult, T2> value)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()), Attributes = attrs };
			var columnHeader = f.AddHeader(res);
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult>{Column = listColumn, Header = columnHeader};
		}
		#endregion



		public static void AddActionsCell<TResult>(this IFieldCollection<TResult> f, params Func<TResult, Action<ActionLink>>[] actions)
		{
            AddActionsCell(f, f.Resources.Get("Common.Actions"), actions);
        }

        public static IListColumn<TResult> AddActionsCell<TResult>(this IFieldCollection<TResult> f, string title, params Func<TResult, Action<ActionLink>>[] actions)
        {
	        var listColumn = new ListColumn<TResult>(
		        (a, o, i) => a.Style("text-align:center; white-space:nowrap"),
		        (w, o, i) => {
			        foreach (var action in actions.Select(a => a(o)))
				        w.ActionImage(action);
		        }
	        );
	        
            f.AddCustomCell(title, listColumn);
            return listColumn;
        }

        public static IListColumn<TResult> AddActionsCell<TResult>(this IFieldCollection<TResult> f, Action<LayoutWriter, TResult> render)
        {
	        var listColumn = new ListColumn<TResult>(
		        (a, o, i) => a.Style("text-align:center"),
		        (w, o, i) => render(w, o)
	        );
			f.AddCustomCell(f.Resources.Get("Common.Actions"), listColumn);
			return listColumn;
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

	public class AddCellResult<TResult>
	{
		public IListColumn<TResult> Column { get; set; }
		 
		public IColumnHeader Header { get; set; }
	}
	public static class IFieldCollectionExtensions
	{
		static string GetTitle<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, Expression<Func<TEntity, T>> expr)
		{
			return f.Resources.CaptionShort(expr);
		}
		public static IColumnHeader AddHeader<TEntity, TResult>(this IFieldCollection<TEntity, TResult> f, string title, HeaderOptions options)
		{
			var columnHeader = f.AddHeader(null, title, options);

			return columnHeader;
		}

		#region res title = sort expr
		public static AddCellResult<TResult> AddCellWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, Expression<Func<TEntity, T>> res, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = render };
			var columnHeader =f.AddHeader(f.GetTitle(res), new HeaderOptions { SortSeqNo = f.AddSort(res) });
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCellWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, Expression<Func<TEntity, T>> res, Action<LayoutWriter, TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => render(w, o) };
			var columnHeader = f.AddHeader(f.GetTitle(res), new HeaderOptions { SortSeqNo = f.AddSort(res) });
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCellWithSort<TEntity, TResult, T, T2>(this IFieldCollection<TEntity, TResult> f, Expression<Func<TEntity, T>> res, Func<TResult, T2> value)
		{
			var listColumn = new ListColumn<TResult> {Content = (w, o, i) => w.Write(value(o)?.ToString())};
			var columnHeader = f.AddHeader(f.GetTitle(res), new HeaderOptions { SortSeqNo = f.AddSort(res) });
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}
		#endregion

		#region string title, sort expr
		public static AddCellResult<TResult> AddCellWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, string title, Expression<Func<TEntity, T>> sortExpr, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = render };
			var columnHeader = f.AddHeader(title, new HeaderOptions { SortSeqNo = f.AddSort(sortExpr) });
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};

		}

		public static AddCellResult<TResult> AddCellWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, string title, Expression<Func<TEntity, T>> sortExpr, Action<LayoutWriter, TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => render(w, o) };
			var columnHeader = f.AddHeader(title, new HeaderOptions { SortSeqNo = f.AddSort(sortExpr) });
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};

		}

		public static AddCellResult<TResult> AddCellWithSort<TEntity, TResult, T, T2>(this IFieldCollection<TEntity, TResult> f, string title, Expression<Func<TEntity, T>> sortExpr, Func<TResult, T2> value)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) };
			var columnHeader = f.AddHeader(title, new HeaderOptions { SortSeqNo = f.AddSort(sortExpr) });
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};

		}
		#endregion

		#region res title = sort expr = filter expr
		public static AddCellResult<TResult> AddCellWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr, RenderRowCellDelegate<TResult> render)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);
			var listColumn = new ListColumn<TResult> { Content = render };
			var columnHeader = f.AddHeader(titleShort, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};

		}

		public static AddCellResult<TResult> AddCellWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr, Action<LayoutWriter, TResult> render)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => render(w, o) };
			var columnHeader = f.AddHeader(titleShort, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};

		}

		public static AddCellResult<TResult> AddCellWithSortAndFilter<TEntity, TResult, T, T2>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr, Func<TResult, T2> value)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) };
			var columnHeader = f.AddHeader(titleShort, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};

		}
		#endregion

		#region res title = filter expr, no sort
		public static AddCellResult<TResult> AddCellWithFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr, RenderRowCellDelegate<TResult> render)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);
			var listColumn = new ListColumn<TResult> { Content = render };
			var columnHeader = f.AddHeader(titleShort, new HeaderOptions {
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> { Column = listColumn, Header = columnHeader };

		}

		public static AddCellResult<TResult> AddCellWithFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr, Action<LayoutWriter, TResult> render)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => render(w, o) };
			var columnHeader = f.AddHeader(titleShort, new HeaderOptions {
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> { Column = listColumn, Header = columnHeader };

		}

		public static AddCellResult<TResult> AddCellWithFilter<TEntity, TResult, T, T2>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr, Func<TResult, T2> value)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) };
			var columnHeader = f.AddHeader(titleShort, new HeaderOptions {
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> { Column = listColumn, Header = columnHeader };

		}
		#endregion

		#region string title, sort expr = filter expr
		public static AddCellResult<TResult> AddCellWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			string title, Expression<Func<TEntity, T>> expr, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = render };
			var columnHeader = f.AddHeader(title, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCellWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			string title, Expression<Func<TEntity, T>> expr, Action<LayoutWriter, TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => render(w, o) };
			var columnHeader = f.AddHeader(title, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}

		public static AddCellResult<TResult> AddCellWithSortAndFilter<TEntity, TResult, T, T2>(this IFieldCollection<TEntity, TResult> f,
			string title, Expression<Func<TEntity, T>> expr, Func<TResult, T2> value)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) };
			var columnHeader = f.AddHeader(title, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> {Column = listColumn, Header = columnHeader};
		}
		#endregion

		#region string title, filter expr, no sort
		public static AddCellResult<TResult> AddCellWithFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			string title, Expression<Func<TEntity, T>> expr, RenderRowCellDelegate<TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = render };
			var columnHeader = f.AddHeader(title, new HeaderOptions	{
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> { Column = listColumn, Header = columnHeader };
		}

		public static AddCellResult<TResult> AddCellWithFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			string title, Expression<Func<TEntity, T>> expr, Action<LayoutWriter, TResult> render)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => render(w, o) };
			var columnHeader = f.AddHeader(title, new HeaderOptions	{
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> { Column = listColumn, Header = columnHeader };
		}

		public static AddCellResult<TResult> AddCellWithFilter<TEntity, TResult, T, T2>(this IFieldCollection<TEntity, TResult> f,
			string title, Expression<Func<TEntity, T>> expr, Func<TResult, T2> value)
		{
			var listColumn = new ListColumn<TResult> { Content = (w, o, i) => w.Write(value(o)?.ToString()) };
			var columnHeader = f.AddHeader(title, new HeaderOptions	{
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			f.Cells.Add(listColumn);
			return new AddCellResult<TResult> { Column = listColumn, Header = columnHeader };
		}
		#endregion

		public static IColumnHeader AddHeaderWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);

			var columnHeader = f.AddHeader(titleShort, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			return columnHeader;
		}
		public static IColumnHeader AddHeaderWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr)
		{
			var title = f.GetTitle(expr);
			var columnHeader =f.AddHeader(title, new HeaderOptions {
				SortSeqNo = f.AddSort(expr)
			});
			return columnHeader;
		}
		public static IColumnHeader AddHeaderWithFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Expression<Func<TEntity, T>> expr)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);

			var columnHeader = f.AddHeader(titleShort, new HeaderOptions {
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			return columnHeader;
		}

		public static IColumnHeader AddHeaderWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			string title, Expression<Func<TEntity, T>> expr)
		{
			return f.AddHeader(title, new HeaderOptions { SortSeqNo = f.AddSort(expr) });
		}

		public static IColumnHeader AddHeaderWithSortAndFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Action<ThTagAttributes> attrs, Expression<Func<TEntity, T>> expr)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);

			var columnHeader =f.AddHeader(attrs, titleShort, new HeaderOptions {
				SortSeqNo = f.AddSort(expr),
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			return columnHeader;
		}
		public static IColumnHeader AddHeaderWithFilter<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Action<ThTagAttributes> attrs, Expression<Func<TEntity, T>> expr)
		{
			var key = expr.GetResourceKey();
			var titleShort = f.GetTitle(expr);
			var title = f.Resources.Get(key);

			var columnHeader = f.AddHeader(attrs, titleShort, new HeaderOptions	{
				FilterSeqNo = f.AddFilterCondition(title, expr)
			});
			return columnHeader;
		}
		public static IColumnHeader AddHeaderWithSort<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f,
			Action<ThTagAttributes> attrs, Expression<Func<TEntity, T>> expr)
		{
			var title = f.GetTitle(expr);
			var columnHeader =f.AddHeader(attrs, title, new HeaderOptions {
				SortSeqNo = f.AddSort(expr)
			});
			return columnHeader;
		}


		#region groups
		public static ListGroup<TResult> AddGroup<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, Func<TResult, string> value,
			Expression<Func<TEntity, T>> sortExpression, RenderGroupCellDelegate<TResult> cell, bool sortDesc = false)
		{
			var g = f.AddGroup(value, cell);
			f.AddGroupSorting(sortExpression, sortDesc);
			return g;
		}
		
		public static ListGroup<TResult> AddGroupWithCells<TEntity, TResult, T>(this IFieldCollection<TEntity, TResult> f, Func<TResult, string> value,
			Expression<Func<TEntity, T>> sortExpression, 
			Action<TResult, GroupRowDescription<TResult>> cells, bool sortDesc = false)
		{
			var g = new ListGroup<TResult> { ValueFunc = value, Cells = cells };
			f.Groups.Add(g);
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
