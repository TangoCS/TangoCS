using System.Collections.Generic;

namespace Nephrite.Logger
{
	public interface IRequestLoggerProvider
	{
		void RegisterLogger<T>(string categoryName) where T : IRequestLogger, new();
		IRequestLogger GetLogger(string categoryName);
		IReadOnlyDictionary<string, IRequestLogger> GetLoggers();
	}
}
