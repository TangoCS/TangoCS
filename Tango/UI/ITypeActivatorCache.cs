using System;
using System.Collections.Generic;

namespace Tango.UI
{
	public interface ITypeActivatorCache
	{
		TypeActivatorInfo Get(string key);
        Dictionary<string, TypeActivatorInfo> GetAll();
    }

	public class TypeActivatorInfo
	{
		public Type Type { get; set; }
		public IActionInvoker Invoker { get; set; }
		public Dictionary<string, string> Args { get; set; }
	}
}
