using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tango.Data;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
	public abstract class default_list_rep<TResult> : abstract_list<TResult>
	{
		[Inject]
		protected IDatabase Database { get; set; }

		protected virtual IQueryable<TResult> Data => Enumerable.Empty<TResult>().AsQueryable();
		protected virtual IQueryable<TResult> DefaultOrderBy(IQueryable<TResult> data) { return data; }
		protected virtual Func<IDictionary<string, object>, TResult> Selector => null;

		IRepository<TResult> _repository = null;

		public IRepository<TResult> Repository
		{
			get
			{
				if (_repository == null)
					_repository = GetRepository();
				return _repository;
			}
			set
			{
				_repository = value;
			}
		}

		protected virtual IRepository<TResult> GetRepository() => Context.RequestServices.GetService(typeof(IRepository<TResult>)) as IRepository<TResult> ??
		 	Database.Repository<TResult>();

		protected override int GetCount()
		{
			return Repository.Count(ApplyFilter(Data).Expression);
		}

		//protected IEnumerable<TResult> _pageData = null;

		protected override IEnumerable<TResult> GetPageData()
		{
			if (_result != null)
				return _result;

			var defSort = Sorter.Count == 0;
			foreach (var gs in Fields.GroupSorting)
				Sorter.AddOrderBy(gs.SeqNo, gs.SortDesc, true);

			var q = Sorter.Apply(ApplyFilter(Data));
			if (defSort)
				q = DefaultOrderBy(q);
			if (Sections.RenderPaging)
				q = Paging.Apply(q, true);

			if (Repository.AllObjectsQuery.StartsWith("@"))
			{
				var (filters, parms) = Filter.GetSqlFilters();
				Repository.AllObjectsQuery = EmbeddedResourceManager.GetString(typeof(TResult), Repository.AllObjectsQuery.Substring(1), filters);

				foreach (var pair in parms)
					Repository.Parameters.Add(pair.Key, pair.Value);
			}

			return Repository.List(q.Expression, Selector);
		}
	}
}
