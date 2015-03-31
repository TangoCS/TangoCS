using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace Nephrite.Linq2Sql
{
    public class Linq2SqlDataContext : DataContext, IDataContext
    {
		public Linq2SqlDataContext()
			: base("")
		{
		}

		public IDataContext NewDataContext()
		{
			throw new NotImplementedException();
		}

		public IDataContext NewDataContext(string connectionString)
		{
			throw new NotImplementedException();
		}

		public new IDbCommand GetCommand(IQueryable query)
		{
			return base.GetCommand(query);
		}

		public new ITable<T> GetTable<T>()
		{
			throw new NotImplementedException();
		}

		public new ITable GetTable(Type t)
		{
			throw new NotImplementedException();
		}

		public T Get<T, TKey>(TKey id)
		{
			throw new NotSupportedException();
		}

		public List<Action> AfterSaveActions
		{
			get { throw new NotImplementedException(); }
		}

		public List<Action> BeforeSaveActions
		{
			get { throw new NotImplementedException(); }
		}

		public IDataContext All
		{
			get { throw new NotImplementedException(); }
		}

		public IDataContext Filtered
		{
			get { throw new NotImplementedException(); }
		}
	}


}
