namespace MemberSystem.Web.ViewModels
{

    public class LeaveReportViewModel
    {
        public int   LeaveRequestId { get; set; }

        public int? MemberId { get; set; }

        public string FullName { get; set; }

        public string DepartmentName {  get; set; }

        public string PositionName { get; set; }

        public int LeaveTypeId { get; set; }

        public string LeaveType { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }

        public decimal LeaveDays { get; set; }

        public string ApprovalStatus { get; set; }

        public string Reason { get; set; }
    }
}
