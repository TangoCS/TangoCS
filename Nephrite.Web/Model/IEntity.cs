using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Nephrite.Meta;
using Nephrite.Web.SPM;

namespace Nephrite.Web
{
	public interface IEntity
	{
		//MetaClass MetaClass { get; }
	}

	public interface IWithKey : IEntity
	{
	}

	public interface IWithKey<TKey> : IWithKey
	{
		//TKey ID { get; }
	}

	public interface IWithKey<T, TKey> : IWithKey<TKey> where T : IEntity
	{
		Expression<Func<T, bool>> KeySelector(TKey id);
	}

	public interface IWithTitle
	{
		string Title { get; }
	}

	public interface IChildEntity : IEntity
	{
		string GetPath();
	}

	public interface IWithDefaultOrder<T> where T : IEntity
	{
		Func<T, string> DefaultOrderBy();
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

			if (obj is IWithTimeStamp)
			{
				var obj2 = obj as IWithTimeStamp;
				obj2.LastModifiedDate = DateTime.Now;
				obj2.LastModifiedUserID = Subject.Current.ID;
			}
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

		public static void AttachOnSubmit<T>(this IQueryable<T> q, T obj)
		where T : IEntity
		{
			((ITable)q).Attach(obj);

			if (obj is IWithTimeStamp)
			{
				var obj2 = obj as IWithTimeStamp;
				obj2.LastModifiedDate = DateTime.Now;
				obj2.LastModifiedUserID = Subject.Current.ID;
			}
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

		public static object GetID<T>(this T obj) where T : IWithKey, IEntity
		{
			return (obj.GetMetaClass().Key.GetValue as Func<T, object>)(obj);
		}

		//public static TKey GetTypedID<T, TKey>(this IWithKey<T, TKey> obj) where T : class
		//{
		//	return (obj.GetMetaClass().Key.GetValue as Func<T, TKey>)(obj as T);
		//}
	}
}