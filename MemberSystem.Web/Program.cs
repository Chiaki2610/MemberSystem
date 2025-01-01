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
                options.AddPolicy("ApprovedUserPolicy", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var isApprovedClaim = context.User.FindFirst("IsApproved");
                        return isApprovedClaim != null && bool.TryParse(isApprovedClaim.Value, out var isApproved) && isApproved;
                    }));
                // 針對Role身分組設置
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            });

            builder.Services.AddSingleton<IAuthorizationHandler, ApprovedAndRoleHandler>();


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

            app.Run();
        }
    }
}
