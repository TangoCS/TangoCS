//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace NHibernate.Event
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial interface IReplicateEventListener
	{
		/// <summary>Handle the given replicate event. </summary>
		/// <param name="event">The replicate event to be handled.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task OnReplicateAsync(ReplicateEvent @event, CancellationToken cancellationToken);
	}
}