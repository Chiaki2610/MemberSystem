namespace MemberSystem.Web.Services
{
    public class LeaveReportViewModelService
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly IRepository<Position> _positionRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<MemberDepartment> _memberDepartmentRepository;
        private readonly IRepository<LeaveType> _leaveTypeRepository;
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        private readonly ITransaction _transaction;
        private readonly ILogger<LeaveReportViewModelService> _logger;

        public LeaveReportViewModelService(IRepository<Member> memberRepository,
                                           IRepository<Position> positionRepository,
                                           IRepository<Department> departmentRepository,
                                           IRepository<MemberDepartment> memberDepartmentRepository,
                                           IRepository<LeaveType> leaveTypeRepository,
                                           IRepository<LeaveRequest> leaveRequestRepository,
                                           ITransaction transaction,
                                           ILogger<LeaveReportViewModelService> logger)
        {
            _memberRepository = memberRepository;
            _positionRepository = positionRepository;
            _departmentRepository = departmentRepository;
            _memberDepartmentRepository = memberDepartmentRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _transaction = transaction;
            _logger = logger;
        }

        // 查詢請假報表(起訖日查總表, 部門、假別或特定人員條件可為空)
        public async Task<List<LeaveReportViewModel>> GetLeaveReportAsync(DateTime startDate,
                                                                    DateTime endDate,
                                                                    int? memberId,
                                                                    string? leaveType,
                                                                    string? departmentName)
        {
            var leaveRequests = await _leaveRequestRepository.ListAsync(lr =>lr.StartDate >= startDate &&lr.EndDate <= endDate);

            var members = await _memberRepository.ListAsync();
            var departments = await _departmentRepository.ListAsync();
            var positions = await _positionRepository.ListAsync();
            var leaveTypes = await _leaveTypeRepository.ListAsync();
            var memberDepartments = await _memberDepartmentRepository.ListAsync();
            var result = (from lr in leaveRequests
                          join m in members on lr.MemberId equals m.MemberId
                          join md in memberDepartments on m.MemberId equals md.MemberId
                          join d in departments on md.DepartmentId equals d.DepartmentId
                          join p in positions on md.PositionId equals p.PositionId
                          join lt in leaveTypes on lr.LeaveType equals lt.LeaveTypeName
                          where (string.IsNullOrEmpty(leaveType) || leaveType == "all" || lt.LeaveTypeName == leaveType) &&
                                (!memberId.HasValue || m.MemberId == memberId.Value) &&
                                (string.IsNullOrEmpty(departmentName) || departmentName == "all" || d.DepartmentName == departmentName)
                          select new LeaveReportViewModel
                          {
                              LeaveRequestId = lr.LeaveRequestId,
                              MemberId = m.MemberId,
                              FullName = m.FullName,
                              DepartmentName = d.DepartmentName,
                              PositionName = p.PositionName,
                              LeaveType = lt.LeaveTypeName,
                              StartDate = lr.StartDate,
                              EndDate = lr.EndDate,
                              LeaveDays = (decimal)(lr.EndDate - lr.StartDate).TotalDays + 1,
                              ApprovalStatus = lr.Status,
                              Reason = lr.Reason,
                          }).OrderBy(lr => lr.StartDate).ToList();

            // 日誌記錄結束
            _logger.LogInformation($"請假報表查詢完成，共查詢到 {result.Count} 條記錄");

            return result;
        }
        public async Task<byte[]> ExportLeaveReportToExcelAsync(List<LeaveReportViewModel> reportData)
        {
            using (var package = new ExcelPackage())
            {
                // 新增Excel
                var worksheet = package.Workbook.Worksheets.Add("Leave Report");

                // 設定標題(Excel的起始欄位A1座標是(1,1)!!!)
                worksheet.Cells[1, 1].Value = "申請編號";
                worksheet.Cells[1, 2].Value = "員工姓名";
                worksheet.Cells[1, 3].Value = "部門";
                worksheet.Cells[1, 4].Value = "職位";
                worksheet.Cells[1, 5].Value = "假別";
                worksheet.Cells[1, 6].Value = "請假起迄";
                worksheet.Cells[1, 7].Value = "請假天數";
                worksheet.Cells[1, 8].Value = "狀態";
                worksheet.Cells[1, 9].Value = "原因";

                // 設置標題樣式
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // 將數據塞進欄位
                for (int i = 0; i < reportData.Count; i++)
                {
                    var row = i + 2;
                    var item = reportData[i];
                    worksheet.Cells[row, 1].Value = item.LeaveRequestId;
                    worksheet.Cells[row, 2].Value = item.FullName;
                    worksheet.Cells[row, 3].Value = item.DepartmentName;
                    worksheet.Cells[row, 4].Value = item.PositionName;
                    worksheet.Cells[row, 5].Value = item.LeaveType;
                    worksheet.Cells[row, 6].Value = $"{item.StartDate:yyyy-MM-dd} - {item.EndDate:yyyy-MM-dd}";
                    worksheet.Cells[row, 7].Value = item.LeaveDays;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "0.0\"天\"";
                    worksheet.Cells[row, 8].Value = item.ApprovalStatus;
                    worksheet.Cells[row, 9].Value = item.Reason;
                }

                // 自動調整欄寬
                worksheet.Cells.AutoFitColumns();

                // 返回Excel檔案內容
                return package.GetAsByteArray();
            }
        }
    }
}
