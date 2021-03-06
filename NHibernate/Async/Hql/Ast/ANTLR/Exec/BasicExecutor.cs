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
using System.Linq;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Param;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Hql.Ast.ANTLR.Exec
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class BasicExecutor : AbstractStatementExecutor
	{

		public override async Task<int> ExecuteAsync(QueryParameters parameters, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			CoordinateSharedCacheCleanup(session);

			DbCommand st = null;
			RowSelection selection = parameters.RowSelection;

			try
			{
				try
				{
					CheckParametersExpectedType(parameters); // NH Different behavior (NH-1898)

					var sqlQueryParametersList = sql.GetParameters().ToList();
					SqlType[] parameterTypes = Parameters.GetQueryParameterTypes(sqlQueryParametersList, session.Factory);

					st = await (session.Batcher.PrepareCommandAsync(CommandType.Text, sql, parameterTypes, cancellationToken)).ConfigureAwait(false);
					foreach (var parameterSpecification in Parameters)
					{
						await (parameterSpecification.BindAsync(st, sqlQueryParametersList, parameters, session, cancellationToken)).ConfigureAwait(false);
					}

					if (selection != null)
					{
						if (selection.Timeout != RowSelection.NoValue)
						{
							st.CommandTimeout = selection.Timeout;
						}
					}
					return await (session.Batcher.ExecuteNonQueryAsync(st, cancellationToken)).ConfigureAwait(false);
				}
				finally
				{
					if (st != null)
					{
						session.Batcher.CloseCommand(st, null);
					}
				}
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle,
																 "could not execute update query", sql);
			}
		}
	}
}