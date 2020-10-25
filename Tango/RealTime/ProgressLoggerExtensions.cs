using System;
using System.Collections.Concurrent;
using Tango.Cache;
using Tango.Logger;
using Tango.UI;

namespace Tango.RealTime
{
	public static class ProgressLoggerExtensions
	{
		public static IRealTimeProgressLogger AddOrGetRealTimeProgressLogger(this IServiceProvider provider, string loggerKey)
		{
			var cache = provider.GetService(typeof(ICache)) as ICache;

			var loggercollection = cache.GetOrAdd("RealTimeLoggers", () => new ConcurrentDictionary<string, IRealTimeProgressLogger>());

			if (loggercollection.TryGetValue(loggerKey, out IRealTimeProgressLogger logger))
				return logger;
			else
			{
				logger = provider.GetService(typeof(IRealTimeProgressLogger)) as IRealTimeProgressLogger;
				loggercollection.AddIfNotExists(loggerKey, logger);
				return logger;
			}
		}
	}
}
