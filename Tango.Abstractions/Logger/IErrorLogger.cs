using System;

namespace Tango.Logger
{
	public interface IErrorLogger
	{
		int Log(Exception exception);
	}

	public interface IExceptionFilter
	{
		bool Filter(Exception e);
	}
}
