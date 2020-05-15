using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Server.Envir
{
    public class ApiServerLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ApiServerLogger> _loggers = new ConcurrentDictionary<string, ApiServerLogger>();

        public ApiServerLoggerProvider()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ApiServerLogger(name));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
