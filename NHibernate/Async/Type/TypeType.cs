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
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class TypeType : ImmutableType
	{

		/// <inheritdoc />
		public override Task<object> AssembleAsync(object cached, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return Task.FromResult<object>(Assemble(cached, session, owner));
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <inheritdoc />
		public override Task<object> DisassembleAsync(object value, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return Task.FromResult<object>(Disassemble(value, session, owner));
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
