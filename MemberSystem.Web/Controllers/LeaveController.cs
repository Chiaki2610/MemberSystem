using MemberSystem.ApplicationCore.Interfaces.Services;
using MemberSystem.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MemberSystem.Web.Controllers
{
    public class LeaveController : Controller
    {
        private readonly LeaveRequesViewModelService _leaveRequesViewModelService;
        private readonly ILeaveService _leaveService;
        private readonly ILogger<LeaveController> _logger;

        public LeaveController(LeaveRequesViewModelService leaveRequesViewModelService,
                               ILeaveService leaveService,
                               ILogger<LeaveController> logger)
        {
            _leaveRequesViewModelService = leaveRequesViewModelService;
            _leaveService = leaveService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var memberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (memberIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (!int.TryParse(memberIdClaim.Value, out int memberId))
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "無效的會員";
                return RedirectToAction("Login", "Account");
            }
            var model = await _leaveRequesViewModelService.GetMemberData(memberId);

            if (model == null)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "找不到會員資料";
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitLeaveRequest()
        {
            


            return Ok();
        }
    }
}
