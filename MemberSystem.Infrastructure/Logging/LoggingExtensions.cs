using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MemberSystem.Infrastructure.Logging
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder loggingBuilder, IServiceCollection services)
        {
            services.AddSingleton<ILoggerProvider>(serviceProvider =>
            {
                return new DatabaseLoggerProvider(
                    logLevel => logLevel >= LogLevel.Information,
                    serviceProvider,
                    serviceProvider.GetRequiredService<IHttpContextAccessor>()
                );
            });

            return loggingBuilder;
        }
    }

}
