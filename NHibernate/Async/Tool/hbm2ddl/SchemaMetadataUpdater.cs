//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using System.Collections.Generic;

namespace NHibernate.Tool.hbm2ddl
{
	using System.Threading.Tasks;
	using System.Threading;
	public static partial class SchemaMetadataUpdater
	{
		public static Task UpdateAsync(ISessionFactoryImplementor sessionFactory, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return UpdateDialectKeywordsAsync(
				sessionFactory.Dialect,
				new SuppliedConnectionProviderConnectionHelper(sessionFactory.ConnectionProvider), cancellationToken);
		}

		public static Task UpdateAsync(Configuration configuration, Dialect.Dialect dialect, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return UpdateDialectKeywordsAsync(
				dialect,
				new ManagedProviderConnectionHelper(configuration.GetDerivedProperties()), cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		static async Task UpdateDialectKeywordsAsync(Dialect.Dialect dialect, IConnectionHelper connectionHelper, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			dialect.RegisterKeywords(await (GetReservedWordsAsync(dialect, connectionHelper, cancellationToken)).ConfigureAwait(false));
		}

		static async Task<IEnumerable<string>> GetReservedWordsAsync(Dialect.Dialect dialect, IConnectionHelper connectionHelper, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await (connectionHelper.PrepareAsync(cancellationToken)).ConfigureAwait(false);
			try
			{
				var metaData = dialect.GetDataBaseSchema(connectionHelper.Connection);
				return metaData.GetReservedWords();
			}
			finally
			{
				connectionHelper.Release();
			}
		}
	}
}
