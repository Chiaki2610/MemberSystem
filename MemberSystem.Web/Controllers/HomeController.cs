namespace MemberSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "User")]
        [Authorize(Policy = "ApprovedUserPolicy")]
        public IActionResult Privacy()
        {
            var user = HttpContext.User.Identity.Name;
            ViewData["Message"] = "���ߧA�����q�L�f�֪��i�̡I";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
