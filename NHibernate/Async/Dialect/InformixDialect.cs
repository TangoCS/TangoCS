//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Dialect.Function;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

//using NHibernate.Dialect.Schema;

namespace NHibernate.Dialect
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class InformixDialect : Dialect
	{

		public override Task<DbDataReader> GetResultSetAsync(DbCommand statement, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<DbDataReader>(cancellationToken);
			}
			return statement.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken);
		}
	}
}
