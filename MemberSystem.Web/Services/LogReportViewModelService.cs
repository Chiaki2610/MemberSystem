namespace MemberSystem.Web.Services
{
    public class LogReportViewModelService
    {
        private readonly IRepository<Log> _logRepository;
        private readonly ILogger<LogReportViewModelService> _logger;

        public LogReportViewModelService(IRepository<Log> logRepository,
                                         ILogger<LogReportViewModelService> logger)
        {
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<List<LogReportViewModel>> GetLogReportAsync(LogReportViewModel model)
        {
            // 先把查詢條件描述出來但不會觸發執行，所以這邊不用加上await
            var query = _logRepository.ListAsync(d =>
                       d.LogTime >= model.StartDate &&
                       d.LogTime <= model.EndDate &&
                       (model.LogType == "all" || string.IsNullOrEmpty(model.LogType) || d.LogType == model.LogType)
    );

            var data = await query;

            var result = data.Select
                (item => new LogReportViewModel
                {
                    LogId = item.LogId,
                    LogType = item.LogType,
                    LogTime = item.LogTime.GetValueOrDefault(DateTime.MinValue),
                    RelatedSystem = item.RelatedSystem,
                    Severity = item.Severity,
                    Message = item.Message,
                    MemberId = item.MemberId,
                }
                ).OrderBy(d => d.LogTime).ToList();

            return result;
        }
    }
}
