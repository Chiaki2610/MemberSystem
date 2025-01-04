using MemberSystem.ApplicationCore.Interfaces.Services;

namespace MemberSystem.Web.Controllers
{
    public class LeaveController : Controller
    {
        private readonly LeaveRequesViewModelService _leaveRequesViewModelService;
        private readonly ApprovalListViewModelService _approvalListViewModelService;
        private readonly ILeaveService _leaveService;
        private readonly ILogger<LeaveController> _logger;

        public LeaveController(LeaveRequesViewModelService leaveRequesViewModelService,
                               ApprovalListViewModelService approvalListViewModelService,
                               ILeaveService leaveService,
                               ILogger<LeaveController> logger)
        {
            _leaveRequesViewModelService = leaveRequesViewModelService;
            _approvalListViewModelService = approvalListViewModelService;
            _leaveService = leaveService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var memberId = GetMemberClaim();
            if (memberId < 0) return RedirectToAction("Login", "Account");

            var model = await _leaveRequesViewModelService.GetMemberData(memberId);

            if (model == null)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "找不到會員資料";
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        /// <summary>
        /// 請假申請
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

                if (await _leaveService.SubmitLeaveRequestAsync(dto))
                {
                    TempData["ToastType"] = "success";
                    TempData["ToastMessage"] = "申請成功，請等候審核結果；如欲查詢進度請利用「請假查詢」頁面，謝謝。";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogError("申請失敗");
                    TempData["ToastType"] = "error";
                    TempData["ToastMessage"] = "申請失敗，請確認剩餘天數配額是否足夠。";
                    return RedirectToAction("Index", "Leave");
                }
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

        /// <summary>
        /// 查看申請中的案件
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CheckLeaveRequest(int? page)
        {
            var memberId = GetMemberClaim();
            if (memberId < 0) return RedirectToAction("Login", "Account");

            // 誰登入就是誰的權限來著
            var model = await _leaveRequesViewModelService.GetPendingApprovalData(memberId);
            PagedList(page, model);
            return View(model);
        }

        /// <summary>
        /// 審核申請
        /// </summary>
        /// <param name="approvalId"></param>
        /// <param name="leaveRequestId"></param>
        /// <param name="memberId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditLeaveRequestStatus(int approvalId, int leaveRequestId, int memberId, string status)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "申請資料有誤，請檢查後重新再試一次。";
                return RedirectToAction("CheckLeaveRequest", "Leave");
            }

            try
            {
                var dto = new EditLeaveRequestStatusDto
                {
                    ApproverId = approvalId,
                    LeaveRequestId = leaveRequestId,
                    MemberId = memberId,
                    Status = status
                };

                await _leaveService.SubmitLeaveApprovalAsync(dto);

                TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "審核成功；如欲查詢審核流程請利用「請假查詢」頁面，謝謝。";
                return RedirectToAction("CheckLeaveRequest", "Leave");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = ex.Message;
                return RedirectToAction("CheckLeaveRequest", "Leave");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "申請失敗");
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "發生未預期的錯誤，請稍後重試；如有不便，還請見諒。";
                return RedirectToAction("CheckLeaveRequest", "Leave");
            }
        }

        /// <summary>
        /// 個人查看所有請假申請的頁面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ApprovalList()
        {
            var memberId = GetMemberClaim();
            if (memberId < 0) return RedirectToAction("Login", "Account");

            var model = await _approvalListViewModelService.GetLeaveRequests(memberId);

            return View(model);
        }

        /// <summary>
        /// 查看單一申請的簽核流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ViewApprovalFlow(int id)
        {
            try
            {
                var approvalFlow = await _leaveService.GetApprovalFlowAsync(id);
                return View(approvalFlow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢視簽核流程失敗，LeaveRequestId：{LeaveRequestId}", id);
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetApprovalFlow(int leaveRequestId)
        {
            return RedirectToAction("ViewApprovalFlow", "Leave", new { id = leaveRequestId });
        }

        private void PagedList(int? page, CheckLeaveRequestViewModel model)
        {
            // 資料分頁處理
            var pageSize = 10;
            var pageNumber = page ?? 1;
            var totalRecords = model.CheckLeaveRequestList.Count;
            var pagedData = model.CheckLeaveRequestList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalRecords / pageSize);

            model.CheckLeaveRequestList = pagedData;
        }

        private int GetMemberClaim()
        {
            var memberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (memberIdClaim == null)
            {
                return -1;
            }
            if (!int.TryParse(memberIdClaim.Value, out int memberId))
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "無效的會員";
                return -1;
            }

            return memberId;
        }
    }
}
