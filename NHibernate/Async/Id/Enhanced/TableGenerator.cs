//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.SqlCommand;
using NHibernate.AdoNet.Util;

namespace NHibernate.Id.Enhanced
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class TableGenerator : TransactionHelper, IPersistentIdentifierGenerator, IConfigurable
	{
		private readonly NHibernate.Util.AsyncLock _generate = new NHibernate.Util.AsyncLock();


		[MethodImpl()]
		public virtual async Task<object> GenerateAsync(ISessionImplementor session, object obj, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			using (await _generate.LockAsync())
			{
				return await (Optimizer.GenerateAsync(new TableAccessCallback(session, this), cancellationToken)).ConfigureAwait(false);
			}
		}


		private partial class TableAccessCallback : IAccessCallback
		{

			#region IAccessCallback Members

			public async Task<long> GetNextValueAsync(CancellationToken cancellationToken)
			{
				cancellationToken.ThrowIfCancellationRequested();
				return Convert.ToInt64(await (owner.DoWorkInNewTransactionAsync(session, cancellationToken)).ConfigureAwait(false));
			}

			#endregion
		}


		public override async Task<object> DoWorkInCurrentTransactionAsync(ISessionImplementor session, DbConnection conn, DbTransaction transaction, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			long result;
			int updatedRows;

			do
			{
				object selectedValue;

				try
				{
					var selectCmd = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, selectQuery, selectParameterTypes);
					using (selectCmd)
					{
						selectCmd.Connection = conn;
						selectCmd.Transaction = transaction;
						string s = selectCmd.CommandText;
						selectCmd.Parameters[0].Value = SegmentValue;
						PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand(selectCmd, FormatStyle.Basic);

						selectedValue = await (selectCmd.ExecuteScalarAsync(cancellationToken)).ConfigureAwait(false);
					}

					if (selectedValue == null)
					{
						result = InitialValue;

						var insertCmd = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, insertQuery, insertParameterTypes);
						using (insertCmd)
						{
							insertCmd.Connection = conn;
							insertCmd.Transaction = transaction;

							insertCmd.Parameters[0].Value = SegmentValue;
							insertCmd.Parameters[1].Value = result;

							PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand(insertCmd, FormatStyle.Basic);
							await (insertCmd.ExecuteNonQueryAsync(cancellationToken)).ConfigureAwait(false);
						}
					}
					else
					{
						result = Convert.ToInt64(selectedValue);
					}
				}
				catch (OperationCanceledException) { throw; }
				catch (Exception ex)
				{
					log.Error(ex, "Unable to read or initialize hi value in {0}", TableName);
					throw;
				}


				try
				{
					var updateCmd = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, updateQuery, updateParameterTypes);
					using (updateCmd)
					{
						updateCmd.Connection = conn;
						updateCmd.Transaction = transaction;

						int increment = Optimizer.ApplyIncrementSizeToSourceValues ? IncrementSize : 1;
						updateCmd.Parameters[0].Value = result + increment;
						updateCmd.Parameters[1].Value = result;
						updateCmd.Parameters[2].Value = SegmentValue;
						PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand(updateCmd, FormatStyle.Basic);
						updatedRows = await (updateCmd.ExecuteNonQueryAsync(cancellationToken)).ConfigureAwait(false);
					}
				}
				catch (OperationCanceledException) { throw; }
				catch (Exception ex)
				{
					log.Error(ex, "Unable to update hi value in {0}", TableName);
					throw;
				}
			}
			while (updatedRows == 0);

			TableAccessCount++;

			return result;
		}
	}
}
