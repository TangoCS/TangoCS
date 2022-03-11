using System.Collections.Generic;
using System.Linq;

namespace Tango.Logger.Std
{
	public class RequestLoggerProvider : IRequestLoggerProvider
	{
		Dictionary<string, IRequestLogger> loggers = new Dictionary<string, IRequestLogger>();

		public void RegisterLogger<T>(string categoryName)	where T : IRequestLogger, new()
		{
			loggers[categoryName] = new T();
		}

		public IRequestLogger GetLogger(string categoryName)
		{
			IRequestLogger logger = null;
			if (!loggers.TryGetValue(categoryName, out logger))
			{
				logger = new EmptyLogger();
				loggers[categoryName] = logger;
			}
			return logger;
		}

		public IReadOnlyDictionary<string, IRequestLogger> GetLoggers()
		{
			return loggers;
		}
	}
}
