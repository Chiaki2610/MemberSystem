namespace MemberSystem.Web.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var isAdmin = User.IsInRole("Admin");
            var isAuthenticated = User.Identity.IsAuthenticated;

            var model = new NavbarViewModel
            {
                IsAdmin = isAdmin,
                IsAuthenticated = isAuthenticated
            };

            return View(model);
        }
    }
}
