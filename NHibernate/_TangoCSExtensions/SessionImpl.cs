using NHibernate.Event;
using System;

namespace NHibernate.Impl
{
	public sealed partial class SessionImpl
	{
		public IServiceProvider RequestServices { get; private set; }

		internal SessionImpl(SessionFactoryImpl factory, ISessionCreationOptions options, IServiceProvider services, EventListeners listeners = null)
			: this(factory, options)
		{
			RequestServices = services;
			if (listeners != null)
				this.listeners = listeners;
		}
	}
}
