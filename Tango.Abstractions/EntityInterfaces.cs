using System;
using System.Linq.Expressions;
namespace Tango
{
	public interface IWithKey<TKey>
	{
		TKey ID { get; }
	}

	public interface IWithKey<T, TKey> : IWithKey<TKey>
	{
		Expression<Func<T, bool>> KeySelector(TKey id);		
	}

	public interface IWithTitle
	{
		string Title { get; }
	}

	public interface IWithDefaultOrder<T>
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

	public interface IWithTimeStamp<TUser> : IWithTimeStamp
		where TUser : IWithTitle
	{
		TUser LastModifiedUser { get; set; }
	}
	
	public interface IWithTimeStampEx<TUser> where TUser : IWithTitle
	{
		DateTime LastModifiedDate { get; set; }
		TUser LastModifiedUser { get; set; }
		DateTime CreateDate { get; set; }
		TUser Creator { get; set; }
	}

	public interface IChildEntity<TParentKey>
	{
		TParentKey ParentID { get; set; }
	}

	public interface IChildEntity<T, TParentKey> : IChildEntity<TParentKey>
	{
		Expression<Func<T, bool>> ByParentKeySelector(TParentKey id);
	}

	public interface IEnum
	{
	}
}
