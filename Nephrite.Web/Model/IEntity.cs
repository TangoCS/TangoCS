using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Nephrite.Web
{
	public interface IEntity
	{
	}

	public interface IChildEntity : IEntity
	{
		string GetPath();
	}

	public interface IWithTitle
	{
		string GetTitle();
	}

	public interface IWithDefaultOrder<T> where T : IEntity
	{
		Func<T, string> DefaultOrderBy();
	}

	public interface IWithKey<T, TKey> where T : IEntity
	{
		Expression<Func<T, bool>> KeySelector(TKey id);
	}

	public interface IWithSeqNo
	{
		int SeqNo { get; set; }
	}

	public interface IWithLogicalDelete
	{
		bool IsDeleted { get; set; }
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

	public interface IProperty<TObj, T>
	{
		Expression<Func<TObj, T>> Selector { get; set; }
		Expression<Func<TObj, T>> FilterExpression { get; set; }
		Expression<Func<TObj, T>> SortExpression { get; set; }
	}
}