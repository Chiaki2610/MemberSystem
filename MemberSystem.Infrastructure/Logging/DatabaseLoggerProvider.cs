using MemberSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace MemberSystem.Infrastructure.Logging
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly Func<LogLevel, bool> _filter;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseLoggerProvider(Func<LogLevel, bool> filter,
                                      IServiceProvider serviceProvider,
                                      IHttpContextAccessor httpContextAccessor)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _httpContextAccessor = httpContextAccessor;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(categoryName, _filter, _serviceProvider, _httpContextAccessor);
        }

        public void Dispose()
        {
            // 因繼承ILoggerProvider必須實作的method，若有需求可進行資源釋放
        }
    }
}
