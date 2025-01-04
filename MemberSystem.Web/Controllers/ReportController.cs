using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

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

                // 將查到的資料以Json形式存儲在Session
                HttpContext.Session.SetString("LeaveReportData", JsonConvert.SerializeObject(reportData));

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

        // 將報表匯出Excel的方法
        [HttpPost]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                _logger.LogInformation("開始執行匯出Excel檔案流程");

                var jsonData = HttpContext.Session.GetString("LeaveReportData");
                if (string.IsNullOrEmpty(jsonData))
                {
                    return RedirectToAction("Index");
                }
                var reportData = JsonConvert.DeserializeObject<List<LeaveReportViewModel>>(jsonData);

                // 產生Excel
                var excelBytes = await _leaveReportViewModelService.ExportLeaveReportToExcelAsync(reportData);

                // 返回檔案下載
                var fileName = $"LeaveReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "匯出 Excel 時發生錯誤");
                return RedirectToAction("Index", "Report");
            }
        }

    }
}
