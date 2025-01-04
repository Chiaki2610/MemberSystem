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

            // Web的DI移至Configurations資料夾內的兩支檔案
            // ConfigureApplicationCoreService -> for 非Web專案內的DI
            // ConfigureWebService -> for Web專案內的DI
            builder.Services.AddApplicationCoreService().AddWebService();
            builder.Services.AddInfrastructureService(builder.Configuration);

            // 自訂LoggerProvider設定
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new DatabaseLoggerProvider(
                logLevel => logLevel >= LogLevel.Information,
                builder.Services.BuildServiceProvider()
            ));

            // 為了處理報表匯成Excel時的資料量設置Session緩存機制
            builder.Services.AddDistributedMemoryCache(); //需要內存支持功能
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // 設定過期時限
                options.Cookie.HttpOnly = true; // 僅允許HTTP訪問
                options.Cookie.IsEssential = true;
            });

            // 全域設定非商業授權許可
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Cookie驗證+導轉設置
            builder.Services
                   .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       // 登入路徑
                       options.LoginPath = "/Account/Login";
                       // 無權限時導向(HTTP Status Code: 403)
                       options.AccessDeniedPath = "/Account/Login";
                   });

            builder.Services.AddHttpContextAccessor();

            // 註冊自訂授權處理器
            builder.Services.AddAuthorization(options =>
            {
                AuthorizationPolicies.RegisterPolicies(options);
            });

            builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();


            // 1.取得組態中資料庫連線設定
            string connectionString = builder.Configuration.GetConnectionString("MemberSystemContext");

            // 2.註冊EF Core的DbContext
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
            
            // 先確定路由↑設置好才能解析路徑；在驗證與授權↓前要啟用緩存機制
            app.UseSession();

            // 先驗證再授權
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
