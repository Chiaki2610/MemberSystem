using MemberSystem.Infrastructure.Logging;
using MemberSystem.Web.Authorization.MemberSystem.ApplicationCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace MemberSystem.Web
{
    public class Program
    {
        [Experimental("SKEXP0020")]
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Web��DI����Configurations��Ƨ���������ɮ�
            // ConfigureApplicationCoreService -> for �DWeb�M�פ���DI
            // ConfigureWebService -> for Web�M�פ���DI
            builder.Services.AddApplicationCoreService().AddWebService();
            builder.Services.AddInfrastructureService(builder.Configuration);

            // �ۭqLoggerProvider�]�w
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new DatabaseLoggerProvider(
                logLevel => logLevel >= LogLevel.Information,
                builder.Services.BuildServiceProvider()
            ));

            // ���F�B�z����צ�Excel�ɪ���ƶq�]�mSession�w�s����
            builder.Services.AddDistributedMemoryCache(); //�ݭn���s����\��
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // �]�w�L���ɭ�
                options.Cookie.HttpOnly = true; // �Ȥ��\HTTP�X��
                options.Cookie.IsEssential = true;
            });

            // ����]�w�D�ӷ~���v�\�i
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Cookie����+����]�m
            builder.Services
                   .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       // �n�J���|
                       options.LoginPath = "/Account/Login";
                       // �L�v���ɾɦV(HTTP Status Code: 403)
                       options.AccessDeniedPath = "/Account/Login";
                   });

            builder.Services.AddHttpContextAccessor();

            // ���U�ۭq���v�B�z��
            builder.Services.AddAuthorization(options =>
            {
                AuthorizationPolicies.RegisterPolicies(options);
            });

            builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();


            // 1.���o�պA����Ʈw�s�u�]�w
            string connectionString = builder.Configuration.GetConnectionString("MemberSystemContext");

            // 2.���UEF Core��DbContext
            builder.Services.AddDbContext<MemberSystemContext>(options => options.UseSqlServer(connectionString));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            // ���T�w���ѡ��]�m�n�~��ѪR���|�F�b���һP���v���e�n�ҥνw�s����
            app.UseSession();

            // �����ҦA���v
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Permission}/{action=CheckPermission}/{id?}");

            app.Run();
        }
    }
}
