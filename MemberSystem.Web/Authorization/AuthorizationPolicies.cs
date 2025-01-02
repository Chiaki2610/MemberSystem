namespace MemberSystem.Web.Authorization
{
    namespace MemberSystem.ApplicationCore.Authorization
    {
        public static class AuthorizationPolicies
        {
            public static void RegisterPolicies(AuthorizationOptions options)
            {
                // 審核通過的使用者策略
                options.AddPolicy("ApprovedUserPolicy", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var isApprovedClaim = context.User.FindFirst("IsApproved");
                        return isApprovedClaim != null && bool.TryParse(isApprovedClaim.Value, out var isApproved) && isApproved;
                    }));

                // Role策略
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

                // Permission策略
                var permissions = new Dictionary<string, string>
                                {
                                    { "ViewProfile", "檢視個人資料" },
                                    { "EditProfile", "編輯個人資料" },
                                    { "SubmitLeaveRequest", "提交請假申請" },
                                    { "ReviewLeaveRequest", "審核請假申請" },
                                    { "EditLeaveRecord", "編輯請假記錄" },
                                    { "ViewMemberList", "檢視會員列表" },
                                    { "EditMemberData", "編輯會員資料" },
                                    { "ViewRegistrationRequest", "檢視註冊申請" },
                                    { "ReviewRegistrationRequest", "審核註冊申請" },
                                    { "ViewLogs", "檢視log日誌" },
                                    { "ViewLeaveReport", "檢視請假報表" },
                                    { "PrintLeaveReport", "列印請假報表" },
                                    { "ViewLeaveRequest", "檢視請假申請" }
                                };

                foreach (var permission in permissions)
                {
                    options.AddPolicy(permission.Key, policy =>
                        policy.Requirements.Add(new PermissionRequirement(permission.Key)));
                }
            }
        }
    }
}
