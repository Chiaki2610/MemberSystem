namespace MemberSystem.Web.ViewModels
{
    public class ApprovalListViewModel
    { 
        public List<ApprovalViewModel> ApprovalViewModelList { get; set; }
    }
    public class ApprovalViewModel
    {
        public int LeaveRequestId { get; set; }
        public int MemberId { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
    }
}
