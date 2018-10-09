using NHibernate.Event;
using System;

namespace NHibernate.Impl
{
	public sealed partial class SessionFactoryImpl
	{
		internal partial class SessionBuilderImpl<T>
		{
			public ISession OpenSession(IServiceProvider services, EventListeners listeners = null)
			{
				var session = new SessionImpl(_sessionFactory, this, services, listeners);
				if (_interceptor != null)
				{
					// NH specific feature
					// _interceptor may be the shared accros threads EmptyInterceptor.Instance, but that is
					// not an issue, SetSession is no-op on it.
					_interceptor.SetSession(session);
				}
				return session;
			}
		}
	}
}
