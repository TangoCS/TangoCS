//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Data.Common;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class AbstractBinaryType : MutableType, IVersionType, IComparer
	{

		#region IVersionType Members

		//      Note : simply returns null for seed() and next() as the only known
		//      application of binary types for versioning is for use with the
		//      TIMESTAMP datatype supported by Sybase and SQL Server, which
		//      are completely db-generated values...

		public Task<object> NextAsync(object current, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return Task.FromResult<object>(Next(current, session));
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public Task<object> SeedAsync(ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return Task.FromResult<object>(Seed(session));
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		#endregion
	}
}
