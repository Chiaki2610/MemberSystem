using Microsoft.Data.SqlClient.DataClassification;

namespace MemberSystem.Web.ViewModels
{
    public class CheckLeaveRequestViewModel
    {
        public List<CheckLeaveRequest> CheckLeaveRequestList { get; set; }  
    }

    public class CheckLeaveRequest
    {
        public int ApprovalId { get; set; }

        public int LeaveRequestId { get; set; }

        public int MemberId { get; set; }
        
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public string LeaveType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; }

        public int? ApproverId { get; set; }
    }
}
