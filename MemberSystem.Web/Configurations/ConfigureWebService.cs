using MemberSystem.Web.Services;

namespace MemberSystem.Web.Configurations
{
    public static class ConfigureWebService
    {
        public static IServiceCollection AddWebService(this IServiceCollection services)
        {
            services.AddScoped<CheckViewModelService>();
            services.AddScoped<ProfileViewModelService>();
            return services;
        }
    }
}
