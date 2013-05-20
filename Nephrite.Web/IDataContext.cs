using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Web;

namespace Nephrite.Web
{
	public interface IDataContext : IDisposable
	{
		IDataContext NewDataContext();
		IDataContext NewDataContext(string connectionString);

		int ExecuteCommand(string command, params object[] parameters);
		IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] parameters);
		IEnumerable ExecuteQuery(Type elementType, string query, params object[] parameters);
		DbCommand GetCommand(IQueryable query);
		IQueryable GetTable(Type type);
		void SubmitChanges();

		TextWriter Log { get; set; }
	}

	public interface IEntity
	{
	}

	public static class IQueryableExtension
	{
		public static void InsertOnSubmit<T>(this IQueryable<T> q, T obj)
			where T : IEntity
		{
			((ITable)q).InsertOnSubmit(obj);
		}

		public static void DeleteOnSubmit<T>(this IQueryable<T> q, T obj)
			where T : IEntity
		{
			((ITable)q).DeleteOnSubmit(obj);
		}

		public static void DeleteAllOnSubmit<T>(this IQueryable<T> q, IEnumerable<T> obj)
			where T : IEntity
		{
			((ITable)q).DeleteAllOnSubmit(obj);
		}
	}
}