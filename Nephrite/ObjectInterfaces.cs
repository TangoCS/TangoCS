using System;
using System.Linq.Expressions;
namespace Nephrite
{
	public interface IWithKey<T, TKey>
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
}
