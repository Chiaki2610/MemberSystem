namespace MemberSystem.Web.Services
{
    public class LeaveRequesViewModelService
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly ITransaction _transaction;
        private readonly ILogger<LeaveRequesViewModelService> _logger;

        public LeaveRequesViewModelService(IRepository<Member> memberRepository,
                                           ITransaction transaction,
                                           ILogger<LeaveRequesViewModelService> logger)
        {
            _memberRepository = memberRepository;
            _transaction = transaction;
            _logger = logger;
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
    }
}
