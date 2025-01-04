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
            var data = await _logRepository.ListAsync(d => d.LogTime >= model.StartDate && d.LogTime <= model.EndDate);
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
