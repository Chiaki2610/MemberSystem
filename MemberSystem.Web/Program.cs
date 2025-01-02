using MemberSystem.Web.Authorization.MemberSystem.ApplicationCore.Authorization;

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

            // Web的DI移至Configurations資料夾內的兩支檔案
            // ConfigureApplicationCoreService -> for 非Web專案內的DI
            // ConfigureWebService -> for Web專案內的DI
            builder.Services.AddApplicationCoreService().AddWebService();
            builder.Services.AddInfrastructureService(builder.Configuration);

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
