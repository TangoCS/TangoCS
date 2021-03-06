//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Connection;

namespace NHibernate.Tool.hbm2ddl
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class ManagedProviderConnectionHelper : IConnectionHelper
	{

		public async Task PrepareAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			connectionProvider = ConnectionProviderFactory.NewConnectionProvider(cfgProperties);
			connection = await (connectionProvider.GetConnectionAsync(cancellationToken)).ConfigureAwait(false);
		}
	}
}
