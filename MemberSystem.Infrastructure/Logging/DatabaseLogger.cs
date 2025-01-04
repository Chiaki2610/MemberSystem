using MemberSystem.ApplicationCore.Entities;
using MemberSystem.ApplicationCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MemberSystem.Infrastructure.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly Func<LogLevel, bool> _filter;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseLogger(string categoryName, Func<LogLevel, bool> filter, IServiceProvider serviceProvider)
        {
            _categoryName = categoryName;
            _filter = filter;
            _serviceProvider = serviceProvider;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => _filter(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            var log = new Log
            {
                LogType = logLevel.ToString(),
                LogTime = DateTime.UtcNow,
                Message = message,
                Severity = logLevel.ToString(),
                RelatedSystem = _categoryName
            };

            using (var scope = _serviceProvider.CreateScope())
            {
                var logRepository = scope.ServiceProvider.GetRequiredService<ILogRepository>();
                logRepository.AddLogAsync(log).Wait();
            }
        }
    }
}
