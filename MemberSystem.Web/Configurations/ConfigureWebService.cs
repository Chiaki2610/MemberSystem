using MemberSystem.Web.Services;

namespace MemberSystem.Web.Configurations
{
    public static class ConfigureWebService
    {
        public static IServiceCollection AddWebService(this IServiceCollection services)
        {
            services.AddScoped<CheckViewModelService>();
            services.AddScoped<ProfileViewModelService>();
            services.AddScoped<LeaveRequesViewModelService>();
            services.AddScoped<LeaveReportViewModelService>();
            services.AddScoped<ApprovalListViewModelService>();
            return services;
        }
    }
}
