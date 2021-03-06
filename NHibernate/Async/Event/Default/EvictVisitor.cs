//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Event.Default
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class EvictVisitor : AbstractVisitor
	{

		internal override Task<object> ProcessCollectionAsync(object collection, CollectionType type, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return Task.FromResult<object>(ProcessCollection(collection, type));
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
