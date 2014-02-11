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
		T Get<T, TKey>(TKey id) where T : IEntity, IWithKey<T,TKey>, new();
		void SubmitChanges();

		TextWriter Log { get; set; }

		List<Action> AfterSaveActions { get; }
		List<Action> BeforeSaveActions { get; }
	}
}