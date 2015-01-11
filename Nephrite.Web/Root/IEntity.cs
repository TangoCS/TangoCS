using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Nephrite
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

	public interface IWithTimeStamp
	{
		DateTime LastModifiedDate { get; set; }
		int LastModifiedUserID { get; set; }
	}

}