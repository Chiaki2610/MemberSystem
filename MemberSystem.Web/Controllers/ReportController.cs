using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MemberSystem.Web.Controllers
{
    public class ReportController : Controller
    {

        private readonly LeaveReportViewModelService _leaveReportViewModelService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(LeaveReportViewModelService leaveReportViewModelService, ILogger<ReportController> logger)
        {
            _leaveReportViewModelService = leaveReportViewModelService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new LeaveReportViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetLeaveReport(DateTime startDate, DateTime endDate, int? memberId, string leaveType = null, string departmentName = null)
        {
            try
            {
                _logger.LogInformation($"開始查詢報表，條件：StartDate={startDate}, EndDate={endDate}, LeaveType={leaveType}, MemberId={memberId}, DepartmentName={departmentName}");

                var reportData = await _leaveReportViewModelService.GetLeaveReportAsync(startDate, endDate, memberId, leaveType, departmentName);
                return View("LeaveReport", reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢請假報表時發生錯誤");
                return RedirectToAction("Index", "Report");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LeaveReport(List<LeaveReportViewModel> model)
        {
            return View(model);
        }
    }
}
