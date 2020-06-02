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

	public interface IWithName
	{
		string Name { get; }
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
	}

	public interface IWithUserTimeStamp : IWithTimeStamp
	{
		int LastModifiedUserID { get; set; }
	}

	public interface IWithUserTimeStamp<TUser> : IWithTimeStamp
		where TUser : IWithTitle
	{
		TUser LastModifiedUser { get; set; }
	}
	
	public interface IWithUserTimeStampEx<TUser> : IWithUserTimeStamp<TUser>
		where TUser : IWithTitle
	{
		DateTime CreateDate { get; }
		TUser Creator { get; }
	}

    public interface IWithUserTimeStampEx : IWithTimeStamp
    {
        DateTime CreateDate { get; }
        String Creator { get; }
        String LastModifiedUser { get; }
    }

    public interface IChildEntity<TParentKey>
	{
		TParentKey ParentID { get; set; }
	}

	public interface IChildEntity<T, TParentKey> : IChildEntity<TParentKey>
	{
		Expression<Func<T, bool>> ByParentKeySelector(TParentKey id);
	}

	public interface IWithoutEntityAudit
	{

	}

	public interface IWithPropertyAudit
	{

	}

	public interface IEnum
	{
	}
}
