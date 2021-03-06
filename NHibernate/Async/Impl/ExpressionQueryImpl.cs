//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	using System.Threading.Tasks;
	using System.Threading;

	internal partial class ExpressionFilterImpl : ExpressionQueryImpl
	{

		public override async Task<IList> ListAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			VerifyParameters();
			var namedParams = NamedParams;
			Before();
			try
			{
				return await (Session.ListFilterAsync(collection, ExpandParameters(namedParams), GetQueryParameters(namedParams), cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				After();
			}
		}
	}
}
