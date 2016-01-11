using System;
using System.Security.Principal;

namespace Nephrite.Identity
{
	public interface IIdentityManager<TKey>
	{
		//IHttpContext HttpContext { get; }
		Subject<TKey> CurrentSubject { get; }
		Subject<TKey> SystemSubject { get; }
		IIdentity CurrentIdentity { get; }
		//IDC_Identity<TKey> DataContext { get; }
		IdentityOptions Options { get; }

		void RunAs(TKey sid, Action action);
		void RunAs(Subject<TKey> subject, Action action);
	}
}
