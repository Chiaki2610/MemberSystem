namespace MemberSystem.Web.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var isAuthenticated = User.Identity.IsAuthenticated;

            // User類型為IPrincipal，但在ASP.NET Core中，它通常是為ClaimsPrincipal的實例。
            var claimsPrincipal = User as ClaimsPrincipal;
            var roleName = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Guest";

            var model = new NavbarViewModel
            {
                RoleName = roleName,
                IsAuthenticated = isAuthenticated
            };

            return View(model);
        }
    }
}
