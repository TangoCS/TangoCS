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
		readonly IServiceProvider _provider;

		public ErrorResult(IServiceProvider provider)
		{
			_provider = provider;
		}
        public string OnError(Exception e)
        {
			var s = e.ToString().Replace(Environment.NewLine, "<br/>");
			try
			{
				var logger = _provider.GetService(typeof(IErrorLogger)) as IErrorLogger;
				logger?.Log(e);
			}
			catch (Exception e2)
			{
				s += "<br/><br/>" + e2.ToString().Replace(Environment.NewLine, "<br/>");
			}

			return s;
		}
    }
}
