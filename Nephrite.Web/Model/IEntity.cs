using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Nephrite.Meta;

namespace Nephrite.Web
{
	public interface IEntity
	{
		
	}

	public interface IWithTitle
	{
		string GetTitle();
	}

	public interface IChildEntity : IEntity
	{
		string GetPath();
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

	public static class EntityExtensions
	{
		public static MetaClass GetMetaClass(this IEntity obj)
		{
			if (obj is IMMObjectVersion)
				return A.Meta.GetClass(obj.GetType().Name.Substring("HST_".Length));
			if (obj is IMMObjectMLView)
				return A.Meta.GetClass(obj.GetType().Name.Substring("V_".Length));
			return A.Meta.GetClass(obj.GetType().Name);
		}
	}
}