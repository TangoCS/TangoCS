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
using System.Data.Common;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.SqlTypes;

namespace NHibernate.SqlCommand
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial interface ISqlCommand
	{

		/// <summary>
		/// Bind the appropriate value into the given command.
		/// </summary>
		/// <param name="command">The command into which the value should be bound.</param>
		/// <param name="commandQueryParametersList">The parameter-list of the whole query of the command.</param>
		/// <param name="singleSqlParametersOffset">The offset from where start the list of <see cref="DbParameter"/>, in the given <paramref name="command"/>, for the this <see cref="SqlCommandImpl"/>. </param>
		/// <param name="session">The session against which the current execution is occurring.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <remarks>
		/// Suppose the <paramref name="command"/> is composed by two queries. The <paramref name="singleSqlParametersOffset"/> for the first query is zero.
		/// If the first query in <paramref name="command"/> has 12 parameters (size of its SqlType array) the offset to bind all <see cref="IParameterSpecification"/>s, of the second query in the
		/// <paramref name="command"/>, is 12 (for the first query we are using from 0 to 11).
		/// </remarks>
		Task BindAsync(DbCommand command, IList<Parameter> commandQueryParametersList, int singleSqlParametersOffset, ISessionImplementor session, CancellationToken cancellationToken);

		/// <summary>
		/// Bind the appropriate value into the given command.
		/// </summary>
		/// <param name="command">The command into which the value should be bound.</param>
		/// <param name="session">The session against which the current execution is occurring.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <remarks>
		/// Use this method when the <paramref name="command"/> contains just 'this' instance of <see cref="ISqlCommand"/>.
		/// Use the overload <see cref="Bind(DbCommand, IList{Parameter}, int, ISessionImplementor)"/> when the <paramref name="command"/> contains more instances of <see cref="ISqlCommand"/>.
		/// </remarks>
		Task BindAsync(DbCommand command, ISessionImplementor session, CancellationToken cancellationToken);
	}

	public partial class SqlCommandImpl : ISqlCommand
	{

		/// <summary>
		/// Bind the appropriate value into the given command.
		/// </summary>
		/// <param name="command">The command into which the value should be bound.</param>
		/// <param name="commandQueryParametersList">The parameter-list of the whole query of the command.</param>
		/// <param name="singleSqlParametersOffset">The offset from where start the list of <see cref="DbParameter"/>, in the given <paramref name="command"/>, for the this <see cref="SqlCommandImpl"/>. </param>
		/// <param name="session">The session against which the current execution is occuring.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public async Task BindAsync(DbCommand command, IList<Parameter> commandQueryParametersList, int singleSqlParametersOffset, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			foreach (IParameterSpecification parameterSpecification in Specifications)
			{
				await (parameterSpecification.BindAsync(command, commandQueryParametersList, singleSqlParametersOffset, SqlQueryParametersList, QueryParameters, session, cancellationToken)).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Bind the appropriate value into the given command.
		/// </summary>
		/// <param name="command">The command into which the value should be bound.</param>
		/// <param name="session">The session against which the current execution is occuring.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <remarks>
		/// Use this method when the <paramref name="command"/> contains just 'this' instance of <see cref="ISqlCommand"/>.
		/// Use the overload <see cref="BindAsync(DbCommand, IList{Parameter}, int, ISessionImplementor,CancellationToken)"/> when the <paramref name="command"/> contains more instances of <see cref="ISqlCommand"/>.
		/// </remarks>
		public async Task BindAsync(DbCommand command, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			foreach (IParameterSpecification parameterSpecification in Specifications)
			{
				await (parameterSpecification.BindAsync(command, SqlQueryParametersList, QueryParameters, session, cancellationToken)).ConfigureAwait(false);
			}
		}
	}
}