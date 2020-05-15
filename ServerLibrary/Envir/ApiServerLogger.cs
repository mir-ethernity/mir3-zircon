using Microsoft.Extensions.Logging;
using System;

namespace Server.Envir
{
    public class ApiServerLogger : ILogger
    {
        private string _name;

        public ApiServerLogger(string name)
        {
            _name = name;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var logLevelStr = Config.ApiServerLogLevel;
            LogLevel logLevelConfig = LogLevel.Warning;
            if (Enum.TryParse(logLevelStr, out LogLevel result))
                logLevelConfig = result;

            if (logLevelConfig == LogLevel.None)
                return false;

            return (int)logLevel >= (int)logLevelConfig;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            SEnvir.Log($"{logLevel} - {eventId.Id} - {_name} - {formatter(state, exception)}");
        }
    }
}
