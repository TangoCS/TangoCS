using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tango.Data;
using Tango.UI.Controls;

namespace Tango.UI.Std
{
	public abstract class default_list_rep<TResult> : abstract_list<TResult, TResult>
	{
		[Inject]
		protected IDatabase Database { get; set; }

		protected virtual IQueryable<TResult> Data => Enumerable.Empty<TResult>().AsQueryable();
		protected virtual IQueryable<TResult> DefaultOrderBy(IQueryable<TResult> data) { return data; }

		IRepository<TResult> _repository = null;

		IRepository<TResult> Repository
		{
			get
			{
				if (_repository == null)
					_repository = GetRepository();
				return _repository;
			}
		}

		protected virtual IRepository<TResult> GetRepository() => Database.Repository<TResult>();

		protected override int GetCount()
		{
			return Repository.Count(ApplyFilter(Data).Expression);
		}

		IEnumerable<TResult> _pageData = null;

		protected override IEnumerable<TResult> GetPageData()
		{
			if (_pageData != null)
				return _pageData;

			foreach (var gs in _fields.GroupSorting)
				Sorter.AddOrderBy(gs.SeqNo, gs.SortDesc, true);
			var filtered = ApplyFilter(Data);
			var q = Paging.Apply(Sorter.Count > 0 ? Sorter.Apply(filtered) : DefaultOrderBy(filtered), true);

			if (Repository.AllObjectsQuery.StartsWith("@"))
			{
				var (filters, parms) = Filter.GetSqlFilters();
				Repository.AllObjectsQuery = EmbeddedResourceManager.GetString(typeof(TResult), Repository.AllObjectsQuery.Substring(1), filters);

				foreach (var pair in parms)
					Repository.Parameters.Add(pair.Key, pair.Value);
			}

			_pageData = Repository.List(q.Expression);

			return _pageData;
		}

		protected override IFieldCollection<TResult, TResult> FieldsConstructor()
		{
			var f = new FieldCollection<TResult>(Context, Sorter, Filter);
			f.RowAttributes += (a, o, i) => a.ZebraStripping(i.RowNum);
			FieldsInit(f);
			return f;
		}

		protected abstract void FieldsInit(FieldCollection<TResult> fields);
	}
}
