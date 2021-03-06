//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace NHibernate.Id.Enhanced
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial interface IOptimizer
	{

		/// <summary>
		/// Generate an identifier value accounting for this specific optimization. 
		/// </summary>
		/// <param name="callback">Callback to access the underlying value source. </param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>The generated identifier value.</returns>
		Task<object> GenerateAsync(IAccessCallback callback, CancellationToken cancellationToken);
	}
}