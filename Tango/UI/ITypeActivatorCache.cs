using System;

namespace Tango.UI
{
	public interface ITypeActivatorCache
	{
		(Type Type, IActionInvoker Invoker)? Get(string key);
    }
}
