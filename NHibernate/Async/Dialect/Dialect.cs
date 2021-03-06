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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Transactions;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Lock;
using NHibernate.Dialect.Schema;
using NHibernate.Exceptions;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class Dialect
	{
		#region Callable statement support

		/// <summary> 
		/// Given a callable statement previously processed by <see cref="RegisterResultSetOutParameter"/>,
		/// extract the <see cref="DbDataReader"/> from the OUT parameter. 
		/// </summary>
		/// <param name="statement">The callable statement. </param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns> The extracted result set. </returns>
		/// <throws>  SQLException Indicates problems extracting the result set. </throws>
		public virtual Task<DbDataReader> GetResultSetAsync(DbCommand statement, CancellationToken cancellationToken)
		{
			throw new NotSupportedException(GetType().FullName + " does not support resultsets via stored procedures");
		}

		#endregion
	}
}
