using System;

namespace NHibernate
{
	public partial interface ISession
	{
		IServiceProvider RequestServices { get; }
	}
}
