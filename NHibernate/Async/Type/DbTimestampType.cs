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
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class DbTimestampType : AbstractDateTimeType
	{

		/// <inheritdoc />
		public override async Task<object> SeedAsync(ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (session == null)
			{
				log.Debug("incoming session was null; using current application host time");
				return await (base.SeedAsync(null, cancellationToken)).ConfigureAwait(false);
			}
			if (!session.Factory.Dialect.SupportsCurrentTimestampSelection)
			{
				log.Info("falling back to application host based timestamp, as dialect does not support current timestamp selection");
				return await (base.SeedAsync(session, cancellationToken)).ConfigureAwait(false);
			}
			return await (GetCurrentTimestampAsync(session, cancellationToken)).ConfigureAwait(false);
		}

		protected virtual async Task<DateTime> GetCurrentTimestampAsync(ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var dialect = session.Factory.Dialect;
			// Need to round notably for Sql Server DateTime with Odbc, which has a 3.33ms resolution,
			// causing stale data update failure 2/3 of times if not rounded to 10ms.
			return Round(
				await (UsePreparedStatementAsync(dialect.CurrentTimestampSelectString, session, cancellationToken)).ConfigureAwait(false),
				dialect.TimestampResolutionInTicks);
		}

		protected virtual async Task<DateTime> UsePreparedStatementAsync(string timestampSelectString, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var tsSelect = new SqlString(timestampSelectString);
			DbCommand ps = null;
			DbDataReader rs = null;
			using (session.BeginProcess())
			{
				try
				{
					ps = await (session.Batcher.PrepareCommandAsync(CommandType.Text, tsSelect, EmptyParams, cancellationToken)).ConfigureAwait(false);
					rs = await (session.Batcher.ExecuteReaderAsync(ps, cancellationToken)).ConfigureAwait(false);
					await (rs.ReadAsync(cancellationToken)).ConfigureAwait(false);
					var ts = rs.GetDateTime(0);
					log.Debug("current timestamp retreived from db : {0} (ticks={1})", ts, ts.Ticks);
					return ts;
				}
				catch (DbException sqle)
				{
					throw ADOExceptionHelper.Convert(
						session.Factory.SQLExceptionConverter,
						sqle,
						"could not select current db timestamp",
						tsSelect);
				}
				finally
				{
					if (ps != null)
					{
						try
						{
							session.Batcher.CloseCommand(ps, rs);
						}
						catch (DbException sqle)
						{
							log.Warn(sqle, "unable to clean up prepared statement");
						}
					}
				}
			}
		}
	}
}
