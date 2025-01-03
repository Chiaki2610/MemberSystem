namespace MemberSystem.Web.Services
{
    public class LeaveRequesViewModelService
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly IRepository<MemberDepartment> _memberDepartmentRepository;
        private readonly IRepository<LeaveApproval> _leaveApprovalRepository;
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        private readonly IRepository<ApprovalFlow> _approvalFlowRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<Position> _positionRepository;
        private readonly ITransaction _transaction;
        private readonly ILogger<LeaveRequesViewModelService> _logger;

        public LeaveRequesViewModelService(IRepository<Member> memberRepository,
                                           IRepository<LeaveApproval> leaveApprovalRepository,
                                           ITransaction transaction,
                                           ILogger<LeaveRequesViewModelService> logger,
                                           IRepository<MemberDepartment> memberDepartmentRepository,
                                           IRepository<ApprovalFlow> approvalFlowRepository,
                                           IRepository<LeaveRequest> leaveRequestRepository,
                                           IRepository<Department> departmentRepository,
                                           IRepository<Position> positionRepository)
        {
            _memberRepository = memberRepository;
            _leaveApprovalRepository = leaveApprovalRepository;
            _transaction = transaction;
            _logger = logger;
            _memberDepartmentRepository = memberDepartmentRepository;
            _approvalFlowRepository = approvalFlowRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
        }

        public async Task<LeaveRequesViewModel> GetMemberData(int memberId)
        {
            var member = await _memberRepository.FirstOrDefaultAsync(m => m.MemberId == memberId);

            var result = new LeaveRequesViewModel
            {
                MemberId = memberId,
                UserName = member.Username,
                FullName = member.FullName,
            };

            return result;
        }

        public async Task<CheckLeaveRequestViewModel> GetPendingApprovalData(int memberId)
        {
            // 取得使用者的Department
            var department = await _memberDepartmentRepository.FirstOrDefaultAsync(d => d.MemberId == memberId);
            if (department == null)
            {
                _logger.LogWarning("找不到使用者的部門資訊");
            }

            // 取得使用者的ApprovalFlow為哪個規則
            var approvalFlow = await _approvalFlowRepository
                              .FirstOrDefaultAsync(af => af.DepartmentId == department.DepartmentId && af.PositionId == department.PositionId);
            if (approvalFlow == null)
            {
                _logger.LogWarning("找不到對應的簽核流程規則");
            }

            // 取得使用者能檢視的LeaveApprovals (確定有東西)
            var pendingApprovals = await _leaveApprovalRepository
                                  .ListAsync(m => m.FlowId == approvalFlow.FlowId && m.ApprovalStatus == "Pending");

            var model = new List<CheckLeaveRequest>();

            var members = await _memberRepository.ListAsync();
            var memberDepartments = await _memberDepartmentRepository.ListAsync();
            var departments = await _departmentRepository.ListAsync();
            var positions = await _positionRepository.ListAsync();

            foreach (var item in pendingApprovals)
            {
                var leaveRequest = await _leaveRequestRepository.FirstOrDefaultAsync(lr => lr.LeaveRequestId == item.LeaveRequestId);
                if (leaveRequest == null) continue;


                // 判斷上一層簽核狀態
                var approvalFlowForCurrent = await _approvalFlowRepository.FirstOrDefaultAsync(af => af.FlowId == item.FlowId);
                if (approvalFlowForCurrent == null) continue;

                var previousApprovalFlow = await _approvalFlowRepository.FirstOrDefaultAsync(af =>
                    af.DepartmentId == approvalFlowForCurrent.DepartmentId &&
                    af.ApprovalOrder == approvalFlowForCurrent.ApprovalOrder - 1); // 找出上一層簽核節點

                if (previousApprovalFlow != null)
                {
                    var previousApproval = await _leaveApprovalRepository.FirstOrDefaultAsync(la =>
                        la.LeaveRequestId == leaveRequest.LeaveRequestId &&
                        la.FlowId == previousApprovalFlow.FlowId);

                    // 如果上一層為Pending，則當前節點不可見
                    if (previousApproval != null && previousApproval.ApprovalStatus == "Pending")
                    {
                        continue;
                    }

                    // 如果上一層為Rejected，則後續節點不顯示
                    if (previousApproval != null && previousApproval.ApprovalStatus == "Rejected")
                    {
                        continue;
                    }
                }

                // 如果沒有上一層則直接顯示
                var member = members.FirstOrDefault(m => m.MemberId == leaveRequest.MemberId);
                if (member == null) continue;
                var memberDepartment = memberDepartments.FirstOrDefault(m => m.MemberId == member.MemberId);
                if (memberDepartment == null) continue;

                var departmentName = departments.FirstOrDefault(d => d.DepartmentId == memberDepartment.DepartmentId);
                var positionName = positions.FirstOrDefault(p => p.PositionId == memberDepartment.PositionId);

                var checkLeaveRequest = new CheckLeaveRequest
                {
                    ApprovalId = memberId,
                    LeaveRequestId = leaveRequest.LeaveRequestId,
                    MemberId = member.MemberId,
                    UserName = member.Username,
                    FullName = member.FullName,
                    Department = departmentName.DepartmentName,
                    Position = positionName.PositionName,
                    LeaveType = leaveRequest.LeaveType,
                    StartDate = leaveRequest.StartDate,
                    EndDate = leaveRequest.EndDate,
                    Reason = leaveRequest.Reason,
                    Status = leaveRequest.Status,
                    ApproverId = leaveRequest.ApproverId,
                };
                model.Add(checkLeaveRequest);
            }


            var result = new CheckLeaveRequestViewModel
            { 
                CheckLeaveRequestList = model
            };

            return result;
        }
    }
}
