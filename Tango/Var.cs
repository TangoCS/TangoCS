using System;

namespace Tango
{
	public class Var<T>
	{
		T obj;
		bool hasValue = false;
		public T GetValue(Func<T> func)
		{
			if (!hasValue)
			{
				obj = func();
				hasValue = true;
			}
			return obj;
		}
	}
}
