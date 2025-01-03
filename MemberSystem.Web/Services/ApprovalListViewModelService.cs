namespace MemberSystem.Web.Services
{
    public class ApprovalListViewModelService
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        private readonly ITransaction _transaction;
        private readonly ILogger<ApprovalListViewModelService> _logger;

        public ApprovalListViewModelService(IRepository<LeaveRequest> leaveRequestRepository,
                                            ITransaction transaction,
                                            ILogger<ApprovalListViewModelService> logger)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _transaction = transaction;
            _logger = logger;
        }

        public async Task<ApprovalListViewModel> GetLeaveRequests(int memberId)
        {
            var leaveRequests = await _leaveRequestRepository.ListAsync(m => m.MemberId == memberId);

            var model = leaveRequests.Select( m => new ApprovalViewModel
                        {
                            LeaveRequestId = m.LeaveRequestId,
                            MemberId = m.MemberId,
                            LeaveType = m.LeaveType,
                            StartDate = m.StartDate,
                            EndDate = m.EndDate,
                            Reason = m.Reason,
                            Status = m.Status,
                        }).ToList();

            var result = new ApprovalListViewModel
            {
                ApprovalViewModelList = model
            };
            return result;
        }
    }
}
