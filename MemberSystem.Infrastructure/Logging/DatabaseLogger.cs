using MemberSystem.ApplicationCore.Entities;
using MemberSystem.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace MemberSystem.Infrastructure.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly Func<LogLevel, bool> _filter;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseLogger(string categoryName,
                              Func<LogLevel, bool> filter,
                              IServiceProvider serviceProvider,
                              IHttpContextAccessor httpContextAccessor)
        {
            _categoryName = categoryName;
            _filter = filter;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            if (_categoryName.Contains("Microsoft.EntityFrameworkCore.Database.Command"))
            {
                return false;
            }

            return _filter(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var message = formatter(state, exception);

            if (_categoryName.Contains("Microsoft.EntityFrameworkCore.Database.Command"))
            {
                return;
            }

            // 透過HttpContextAccessor獲得自訂Claim資訊
            var context = _httpContextAccessor.HttpContext;
            var memberId = context?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var department = context?.User.FindFirstValue("Department");

            var log = new Log
            {
                LogType = logLevel.ToString(),
                LogTime = DateTime.UtcNow,
                Message = message,
                Severity = logLevel.ToString(),
                MemberId = string.IsNullOrEmpty(memberId) ? (int?)null : int.Parse(memberId),
                RelatedSystem = department ?? _categoryName, // 若department為null則改存取相關system紀錄
            };

            using (var scope = _serviceProvider.CreateScope())
            {
                var logRepository = scope.ServiceProvider.GetRequiredService<ILogRepository>();
                logRepository.AddLogAsync(log).Wait();
            }
        }
    }
}
