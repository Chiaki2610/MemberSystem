using MemberSystem.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace MemberSystem.Infrastructure.Logging
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly Func<LogLevel, bool> _filter;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseLoggerProvider(Func<LogLevel, bool> filter, IServiceProvider serviceProvider)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(categoryName, _filter, _serviceProvider);
        }

        public void Dispose()
        {
            // 如果有需要可以處理資源釋放
        }
    }
}
