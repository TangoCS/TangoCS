using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Tango.Html;

namespace Tango.UI.Controls
{
	public interface ISorter : IViewElement
	{
		string ParameterName { get; set; }

		(string parm, bool? sortState, int? n) CreateParm(int seqNo);
		int AddColumn(LambdaExpression column);
		void AddOrderBy(int seqNo, bool sortDesc = false, bool isHidden = false);
		void InsertOrderBy(int seqNo, bool sortDesc = false, bool isHidden = false);

		Action<ApiResponse> OnSort { get; }
	}

	public interface ISorter<T> : ISorter
	{
		IQueryable<T> Apply(IQueryable<T> query);	
	}

	public class Sorter<T> : ViewComponent, ISorter<T>
	{
        List<Column> _columns = new List<Column>();
		List<(int, bool, bool)> _currentSort = new List<(int, bool, bool)>();

		public Action<ApiResponse> OnSort { get; set; }

		public override void OnInit()
		{
			var orderByColumns = GetArg(ClientID)?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (orderByColumns == null) return;

			foreach (var c in orderByColumns)
			{
				string[] s = c.Split(new char[] { '_' });
				_currentSort.Add((s[0].ToInt32(0), s.Length != 1, false));
			}
		}

		public int AddColumn(LambdaExpression expr)
		{
			if (expr.Body is UnaryExpression)
				expr = Expression.Lambda((expr.Body as UnaryExpression).Operand, expr.Parameters);
			if (expr.ReturnType == typeof(object))
				expr = Expression.Lambda(expr.Body, expr.Parameters);

			var sc = new Column
			{
				SeqNo = _columns.Count,
				OrderAsc = q => {
					MethodInfo genericMethod = OrderByMethod.MakeGenericMethod(typeof(T), expr.Body.Type);
					return genericMethod.Invoke(null, new object[] { q, expr }) as IQueryable<T>;
				},
				OrderDesc = q => {
					MethodInfo genericMethod = OrderByDescendingMethod.MakeGenericMethod(typeof(T), expr.Body.Type);
					return genericMethod.Invoke(null, new object[] { q, expr }) as IQueryable<T>;
				},
			};
			_columns.Add(sc);
			return sc.SeqNo;
		}

		public void AddOrderBy(int seqNo, bool sortDesc = false, bool isHidden = false)
		{
			_currentSort.Add((seqNo, sortDesc, isHidden));
		}

		public void InsertOrderBy(int seqNo, bool sortDesc = false, bool isHidden = false)
		{
			_currentSort.Insert(0, (seqNo, sortDesc, isHidden));
		}

		public (string parm, bool? sortState, int? n) CreateParm(int seqNo)
		{
			var sb = new StringBuilder();
			bool f = true;
			bool? b = null;
			int i = 1;
			int? n = null;

			foreach (var (key, sortDesc, isHidden) in _currentSort)
			{
				if (isHidden) continue;
				if (key != seqNo)
				{
					sb.Append(f ? "" : ",").Append(key).Append(sortDesc ? "_desc" : "");
					f = false;
					i++;
				}
				else
				{
					b = sortDesc;
					n = i;
				}
			}

			if (b == null)
				sb.Append(f ? "" : ",").Append(seqNo);
			else if (!b.Value)
				sb.Append(f ? "" : ",").Append(seqNo).Append("_desc");

			return (sb.ToString(), b, n);
		}

		public IQueryable<T> Apply(IQueryable<T> query)
		{
			foreach (var (key, value, isHidden) in _currentSort)
			{
				if (value)
					query = _columns[key].OrderDesc(query);
				else
					query = _columns[key].OrderAsc(query);
			}
			return query;
		}

		public int Count => _currentSort.Count;

		class Column
		{
			public int SeqNo { get; set; }
			public Func<IQueryable<T>, IQueryable<T>> OrderAsc { get; set; }
			public Func<IQueryable<T>, IQueryable<T>> OrderDesc { get; set; }
		}

		static readonly MethodInfo OrderByMethod = typeof(Queryable).GetMethods()
			.Where(method => method.Name == "OrderBy")
			.Where(method => method.GetParameters().Length == 2)
			.Single();

		static readonly MethodInfo OrderByDescendingMethod = typeof(Queryable).GetMethods()
			.Where(method => method.Name == "OrderByDescending")
			.Where(method => method.GetParameters().Length == 2)
			.Single();

		public string ParameterName { get; set; }
	}

	public static class SorterExtensions
	{
		public static void SorterLink(this LayoutWriter w, ISorter sorter, string title, int seqNo)
		{
			var (parm, sortState, n) = sorter.CreateParm(seqNo);
			var icon = sortState.HasValue ? (sortState.Value ? "SortDesc" : "SortAsc") : "";
			var pname = sorter.ParameterName.IsEmpty() ? sorter.ClientID : sorter.ParameterName;

			w.ActionLink(a => a.ToCurrent().WithArg(pname, parm).RunEvent(sorter.OnSort).WithTitle(title));

			if (!icon.IsEmpty()) w.Icon(icon);
			if (n.HasValue) w.Write(n);
		}

		public static void Th<T>(this LayoutWriter w, ISorter<T> sorter, string title, Expression<Func<T, object>> expr)
		{
			w.Th(() => w.SorterLink(sorter, title, sorter.AddColumn(expr)));
		}
	}
}
