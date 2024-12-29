namespace MemberSystem.Web.Authorization
{
    public class ApprovedAndRoleRequirement : IAuthorizationRequirement
    {
        public string RequiredRole { get; }
        public ApprovedAndRoleRequirement()
        {
        }
        public ApprovedAndRoleRequirement(string requiredRole)
        {
            RequiredRole = requiredRole;
        }
    }

    public class ApprovedAndRoleHandler : AuthorizationHandler<ApprovedAndRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApprovedAndRoleRequirement requirement)
        {
            // 檢查登入時設置在Claims裡的IsApproved值及Role值
            var isApprovedClaim = context.User.FindFirst("IsApproved");
            var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (isApprovedClaim != null && bool.TryParse(isApprovedClaim.Value, out var isApproved) && isApproved && roleClaim == requirement.RequiredRole)
            {
                context.Succeed(requirement);
            }
            else if (isApprovedClaim == null)
            {
                // 若IsApproved為null(未審核)時亦禁止訪問有權限限制的頁面
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }

}
