using System;

namespace Tango
{
	public static class ThreadContext
	{
		[ThreadStatic]
		public static string Conn;
	}
}
