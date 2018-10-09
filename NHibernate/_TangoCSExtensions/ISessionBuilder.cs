using NHibernate.Event;
using System;

namespace NHibernate
{
	public partial interface ISessionBuilder<T>
	{
		ISession OpenSession(IServiceProvider services, EventListeners listeners = null);
	}
}
