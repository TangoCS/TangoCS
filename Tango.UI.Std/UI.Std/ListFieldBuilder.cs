using System;
using System.Linq.Expressions;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
	public class ListFieldBuilder<TEntity, TResult>
	{
		public ListField<TEntity, TResult> Field { get; } = new ListField<TEntity, TResult>();

		Expression<Func<TEntity, object>> _def;

		public ListFieldBuilder(Expression<Func<TEntity, object>> def)
		{
			_def = def;
			Field.SortExpr = def;
		}

		public ListFieldBuilder<TEntity, TResult> WithSort(Expression<Func<TEntity, object>> sortExpr)
		{
			Field.SortExpr = sortExpr;
			return this;
		}

		public ListFieldBuilder<TEntity, TResult> WithoutSort()
		{
			Field.SortExpr = null;
			return this;
		}

		public ListFieldBuilder<TEntity, TResult> AddToFilter(Expression<Func<TEntity, object>> filterExpr = null)
		{
			Field.FilterSetup = f => f.AddCondition(filterExpr ?? _def);
			return this;
		}

		public ListFieldBuilder<TEntity, TResult> AddToFilter(Action<ListFilter<TEntity>> filterSetup)
		{
			Field.FilterSetup = filterSetup;
			return this;
		}

		public ListFieldBuilder<TEntity, TResult> Render(RenderRowCellDelegate<TResult> action)
		{
			Field.Render = action;
			return this;
		}

		public ListFieldBuilder<TEntity, TResult> Render(Action<LayoutWriter, TResult> action)
		{
			Field.Render = (w, o, i) => action(w, o);
			return this;
		}
	}

	public class ListField<TEntity, TResult>
	{
		public Expression<Func<TEntity, object>> SortExpr { get; set; }
		public Action<ListFilter<TEntity>> FilterSetup { get; set; }
		public RenderRowCellDelegate<TResult> Render { get; set; }
	}

	public static class ListFieldBuilderExtensions
	{
		public static ListFieldBuilder<TEntity, TResult> RenderCell<TEntity, TResult>(this ListFieldBuilder<TEntity, TResult> b, Func<TResult, object> value)
		{
			b.Field.Render = (w, o, i) => w.Write(value(o)?.ToString());
			return b;
		}

		public static ListFieldBuilder<TEntity, TResult> RenderActionLinkCell<TEntity, TResult>(this ListFieldBuilder<TEntity, TResult> b, Func<TResult, Action<ActionLink>> action, Func<TResult, object> value)
		{
			b.Field.Render = (w, o, i) => w.CellActionLink(action(o), value(o));
			return b;
		}
	}
}
