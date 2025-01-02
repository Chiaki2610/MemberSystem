namespace MemberSystem.Web.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var isApprovedClaim = context.User.FindFirst("IsApproved");
            if (isApprovedClaim == null || !bool.TryParse(isApprovedClaim.Value, out var isApproved) || !isApproved)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var permissionsClaim = context.User.FindFirst("Permissions");
            if (permissionsClaim != null && permissionsClaim.Value.Split(',').Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }

}
