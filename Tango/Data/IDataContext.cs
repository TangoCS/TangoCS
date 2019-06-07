using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Tango.Data
{
	public interface IDataContextActivator
	{
		IDataContext CreateInstance(string connectionString = null);
	}

	public static class DataContextActivatorExtensions
	{
		public static T CreateInstance<T>(this IDataContextActivator activator, string connectionString = null)
			where T : class, IDataContext
		{
			return activator.CreateInstance(connectionString) as T;
		}
	}

	public interface IDataContext : IDisposable
	{
		DbConnection Connection { get; }
		IDbTransaction Transaction { get; }

		int ExecuteCommand(string command, params object[] parameters);
		IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] parameters);
		IDbCommand GetCommand(IQueryable query);
		IQueryable<T> GetTable<T>();
		IQueryable GetTable(Type t);
		T Get<T, TKey>(TKey id);

		void InsertOnSubmit<T>(T obj, int? index = null) where T : class;
		void DeleteOnSubmit<T>(T obj, int? index = null) where T : class;
		void DeleteAllOnSubmit<T>(IEnumerable<T> obj, int? index = null) where T : class;
		void AttachOnSubmit<T>(T obj, int? index = null) where T : class;
		void CommandOnSubmit(string query, params object[] parms);
		void CommandOnSubmit(string query, int index, params object[] parms);

		void SubmitChanges();

		List<Action> AfterSaveActions { get; }
		List<Action> BeforeSaveActions { get; }

		void ClearCache();
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

		public static IQueryable<T> Filtered<T>(this IQueryable<T> table)
		{
			return ApplyFor(table);
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

	public enum DBType
	{
		MSSQL, DB2, ORACLE, POSTGRESQL
	}
}