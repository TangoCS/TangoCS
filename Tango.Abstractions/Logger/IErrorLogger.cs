using System;

namespace Tango.Logger
{
	public interface IErrorLogger
	{
		int Log(Exception exception);
	}
}
