using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Tango.UI
{
	public interface ITypeActivatorCache
	{
		Tuple<Type, IActionInvoker> Get(string key);
    }
}
