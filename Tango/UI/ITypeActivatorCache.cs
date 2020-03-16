using System;
using System.Collections.Generic;

namespace Tango.UI
{
	public interface ITypeActivatorCache
	{
		(Type Type, IActionInvoker Invoker)? Get(string key);
        Dictionary<string, (Type Type, IActionInvoker Invoker)> GetAll();
    }
}
