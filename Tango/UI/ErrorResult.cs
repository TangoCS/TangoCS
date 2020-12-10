using System;
using Tango.Logger;

namespace Tango.UI
{
    public interface IErrorResult
    {
        string OnError(Exception e);
    }

	public interface IErrorResultXml : IErrorResult { }

	public class ErrorResult : IErrorResultXml
	{
		readonly IErrorLogger _logger;

		public ErrorResult(IErrorLogger logger)
		{
			_logger = logger;
		}
        public string OnError(Exception e)
        {
			_logger?.Log(e);

			return e.ToString().Replace(Environment.NewLine, "<br/>");
        }
    }
}
