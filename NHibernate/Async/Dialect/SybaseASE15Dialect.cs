//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Environment = NHibernate.Cfg.Environment;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class SybaseASE15Dialect : Dialect
	{
		
		public override Task<DbDataReader> GetResultSetAsync(DbCommand statement, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<DbDataReader>(cancellationToken);
			}
			return statement.ExecuteReaderAsync(cancellationToken);
		}
	}
}
