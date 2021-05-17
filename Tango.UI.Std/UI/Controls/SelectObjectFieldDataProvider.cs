using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tango.Data;

namespace Tango.UI.Controls
{
	public interface ISelectObjectFieldDataProvider<TRef>
	{
		int MaterializeCount(IQueryable<TRef> data);
		IEnumerable<TRef> MaterializeList(IQueryable<TRef> data);
	}

	public class ORMSelectObjectFieldDataProvider<TRef> : ISelectObjectFieldDataProvider<TRef>
	{
		public ORMSelectObjectFieldDataProvider(ISelectObjectField<TRef> field)
		{
		}

		public int MaterializeCount(IQueryable<TRef> data)
		{
			return data.Count();
		}

		public IEnumerable<TRef> MaterializeList(IQueryable<TRef> data)
		{
			return data;
		}
	}

	public class DapperSelectObjectFieldDataProvider<TRef> : ISelectObjectFieldDataProvider<TRef>
	{
		IDatabase database;

		public string AllObjectsSql { get; set; }
		public object AllObjectsSqlParms { get; set; }
		public Func<IQueryable<TRef>, IQueryable<TRef>> PostProcessing { get; set; }

		public IRepository<TRef> Repository => database.Repository<TRef>()
			.WithAllObjectsQuery(AllObjectsSql, AllObjectsSqlParms);

		public DapperSelectObjectFieldDataProvider(ISelectObjectField<TRef> field, IDatabase database)
		{
			this.database = database;
		}

		public int MaterializeCount(IQueryable<TRef> data)
		{
			return Repository.Count(data.Expression);
		}

		public IEnumerable<TRef> MaterializeList(IQueryable<TRef> data)
		{
			if (PostProcessing != null) data = PostProcessing(data);
			return Repository.List(data.Expression);
		}
	}
}
