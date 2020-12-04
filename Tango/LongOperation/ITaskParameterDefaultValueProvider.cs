using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tango.LongOperation
{
	public interface ITaskParameterDefaultValueProvider
	{
		string GetValue(IScheduledTask task, ParameterInfo param);
	}
}
