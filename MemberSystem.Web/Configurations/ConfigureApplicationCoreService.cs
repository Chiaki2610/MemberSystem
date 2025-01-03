using MemberSystem.ApplicationCore.Factories;
using MemberSystem.ApplicationCore.Interfaces.Services;

namespace MemberSystem.Web.Configurations
{
    [Experimental("SKEXP0020")]
    public static class ConfigureApplicationCoreService
    {
        public static IServiceCollection AddApplicationCoreService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ILeaveService, LeaveService>();
            //services.AddScoped<IPermissionServiceFactory, PermissionServiceFactory>();
            //services.AddScoped<IPermissionService, RolePermissionService>();
            //services.AddScoped<IPermissionService, PositionPermissionService>();

            return services;
        }
    }
}
