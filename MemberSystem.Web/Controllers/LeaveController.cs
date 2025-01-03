using MemberSystem.ApplicationCore.Interfaces.Services;

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

        [HttpGet]
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
        public async Task<IActionResult> SubmitLeaveRequest(LeaveRequesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "申請資料有誤，請檢查後重新再試一次。";
                return View(model);
            }

            try
            {
                var dto = new LeaveRequestDto
                {
                    MemberId = model.MemberId,
                    LeaveType = model.LeaveType,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Reason = model.Reason
                };

                await _leaveService.SubmitLeaveRequestAsync(dto);

                TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "申請成功，請等候審核結果；如欲查詢進度請利用「請假查詢」頁面，謝謝。";
                return RedirectToAction("Index", "Home");

            }
            catch (InvalidOperationException ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = ex.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "申請失敗");
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "發生未預期的錯誤，請稍後重試；如有不便，還請見諒。";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckLeaveRequest()
        { 
        
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditLeaveRequestStatus()
        {



            return RedirectToAction("Index", "Home");
        }
    }
}
