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
	public partial interface IFlushEventListener
	{
		/// <summary>Handle the given flush event. </summary>
		/// <param name="event">The flush event to be handled.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		Task OnFlushAsync(FlushEvent @event, CancellationToken cancellationToken);
	}
}