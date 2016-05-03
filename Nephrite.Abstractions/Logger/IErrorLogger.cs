using System;

namespace Nephrite.Logger
{
	public interface IErrorLogger
	{
		int Log(Exception exception);
	}
}
