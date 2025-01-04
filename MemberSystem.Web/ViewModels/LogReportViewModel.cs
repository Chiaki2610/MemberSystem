namespace MemberSystem.Web.ViewModels
{
    public class LogReportViewModel
    {
        public long LogId { get; set; }

        public string LogType { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }

        public DateTime LogTime { get; set; }

        public string RelatedSystem { get; set; }
        
        public string Severity { get; set; }
        
        public string Message { get; set; }

        public int? MemberId { get; set; }
    }
}
