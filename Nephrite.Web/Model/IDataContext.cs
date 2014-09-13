using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Nephrite.Web
{
	public interface IDataContext : IDisposable
	{
		IDataContext NewDataContext();
		IDataContext NewDataContext(string connectionString);

		int ExecuteCommand(string command, params object[] parameters);
		IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] parameters);
		//IEnumerable ExecuteQuery(Type elementType, string query, params object[] parameters);
		IDbCommand GetCommand(IQueryable query);
		IQueryable<T> GetTable<T>();
		ITable GetTable(Type t);
		T Get<T, TKey>(TKey id);
		void SubmitChanges();

		TextWriter Log { get; set; }

		List<Action> AfterSaveActions { get; }
		List<Action> BeforeSaveActions { get; }

		IDataContext All { get; }
		IDataContext Filtered { get; }
	}

	public static class DefaultTableFilters
	{
		static Dictionary<string, object> _filters = new Dictionary<string, object>();

		public static void AddFor<T>(string name, Func<IQueryable<T>, IQueryable<T>> filter)
		{
			string t = typeof(T).Name;
			List<TableFilter<T>> tableFilters = null;
			if (_filters.ContainsKey(t))
				tableFilters = _filters[t] as List<TableFilter<T>>;
			else
			{
				tableFilters = new List<TableFilter<T>>();
				_filters.Add(t, tableFilters);
			}
			var item = tableFilters.FirstOrDefault(o => o.Name == name);
			if (item != null)
				item.Func = filter;
			else
				tableFilters.Add(new TableFilter<T>(name, filter));
		}

		public static IQueryable<T> ApplyFor<T>(IQueryable<T> table)
		{
			string t = typeof(T).Name;
			if (!_filters.ContainsKey(t)) return table;
			var tableFilters = _filters[t] as List<TableFilter<T>>;
			foreach (var filter in tableFilters)
				table = filter.Func(table);
			return table;
		}
	}

	internal class TableFilter<T>
	{
		public string Name { get; set; }
		public Func<IQueryable<T>, IQueryable<T>> Func { get; set; }

		public TableFilter(string name, Func<IQueryable<T>, IQueryable<T>> func)
		{
			Name = name;
			Func = func;
		}
	}
}