using MemberSystem.ApplicationCore.Entities;
using MemberSystem.ApplicationCore.Interfaces.Services;
using X.PagedList;

namespace MemberSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly CheckViewModelService _checkViewModelService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, CheckViewModelService checkViewModelService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _checkViewModelService = checkViewModelService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            var model = await _checkViewModelService.GetMembersAsync();
            PagedList(page, model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ApproveMember(int? page)
        {
            var model = await _checkViewModelService.GetUnapprovedMembersAsync();
            PagedList(page, model);
            return View(model);
        }

        private void PagedList(int? page, CheckMemberDataViewModel model)
        {
            // 資料分頁處理
            var pageSize = 10;
            var pageNumber = page ?? 1;
            var totalRecords = model.CheckMemberDataList.Count;
            var pagedData = model.CheckMemberDataList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalRecords / pageSize);

            model.CheckMemberDataList = pagedData;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateApproved(int memberId, bool isApproved)
        {
            try
            {
                if (!await _adminService.UpdateApprovedAsync(memberId, isApproved))
                {
                    _logger.LogError("審核失敗：會員ID {MemberId}，狀態 {IsApproved}", memberId, isApproved);
                    TempData["ToastType"] = "error";
                    TempData["ToastMessage"] = "審核失敗，請重新進行審核，或洽系統管理員";
                    return RedirectToAction("ApproveMember");
                }
                TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "審核完成，請進行下一筆審核申請";
                return RedirectToAction("ApproveMember");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "審核更新時發生異常");
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "系統錯誤，請洽系統管理員";
                return RedirectToAction("ApproveMember");
            }
        }
    }
}
