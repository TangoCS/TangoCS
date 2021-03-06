//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Engine;

namespace NHibernate.Dialect.Lock
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial interface ILockingStrategy
	{
		/// <summary> 
		/// Acquire an appropriate type of lock on the underlying data that will
		/// endure until the end of the current transaction.
		/// </summary>
		/// <param name="id">The id of the row to be locked </param>
		/// <param name="version">The current version (or null if not versioned) </param>
		/// <param name="obj">The object logically being locked (currently not used) </param>
		/// <param name="session">The session from which the lock request originated </param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task LockAsync(object id, object version, object obj, ISessionImplementor session, CancellationToken cancellationToken);
	}
}