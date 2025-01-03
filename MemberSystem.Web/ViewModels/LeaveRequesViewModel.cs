namespace MemberSystem.Web.ViewModels
{
    public class LeaveRequesViewModel
    {
        public int MemberId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }

        [Required(ErrorMessage = "請假開始日期是必填項目")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "請假結束日期是必填項目")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
    }
}
