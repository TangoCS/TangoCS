using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Nephrite
{
	public interface IEntity
	{
	}

	public interface IWithKey : IEntity
	{
	}

	public interface IWithKey<TKey> : IWithKey
	{
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