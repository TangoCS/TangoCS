using System;
namespace Nephrite.Identity
{
	public interface IIdentityManager<TKey>
	{
		Func<IAppContext> AppContext { get; }
		Subject<TKey> CurrentSubject { get; }
		Subject<TKey> SystemSubject { get; }
		Func<IDC_Identity<TKey>> DataContext { get; }
		IdentityOptions Options { get; }

		void RunAs(TKey sid, Action action);
	}
}
