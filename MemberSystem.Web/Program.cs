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
                options.AddPolicy("ApprovedUserPolicy", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var isApprovedClaim = context.User.FindFirst("IsApproved");
                        return isApprovedClaim != null && bool.TryParse(isApprovedClaim.Value, out var isApproved) && isApproved;
                    }));
                // �w��Role�����ճ]�m
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            });

            builder.Services.AddSingleton<IAuthorizationHandler, ApprovedAndRoleHandler>();


            // 1.���o�պA����Ʈw�s�u�]�w
            string connectionString = builder.Configuration.GetConnectionString("MemberSystemContext");

            // 2.���UEF Core��DbContext
            builder.Services.AddDbContext<MemberSystemContext>(options => options.UseSqlServer(connectionString));

            // Web��DI����Configurations��Ƨ���������ɮ�
            // ConfigureApplicationCoreService -> for �DWeb�M�פ���DI
            // ConfigureWebService -> for Web�M�פ���DI
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

            // �����ҦA���v
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
