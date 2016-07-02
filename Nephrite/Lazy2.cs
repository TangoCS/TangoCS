using System;

namespace Nephrite
{
	public class Lazy2<T> : Lazy<T>
	{
		public static explicit operator T(Lazy2<T> obj)
		{
			return obj.Value;
		}
	}
}
