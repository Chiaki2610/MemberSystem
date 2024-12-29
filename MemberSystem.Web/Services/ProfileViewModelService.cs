namespace MemberSystem.Web.Services
{
    public class ProfileViewModelService
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly ITransaction _transaction;
        private readonly ILogger<ProfileViewModelService> _logger;

        public ProfileViewModelService(IRepository<Member> memberRepository,
                                       ITransaction transaction,
                                       ILogger<ProfileViewModelService> logger)
        {
            _memberRepository = memberRepository;
            _transaction = transaction;
            _logger = logger;
        }

        public async Task<ProfileViewModel> GetMemberData(int memberId)
        {
            var member = await _memberRepository.FirstOrDefaultAsync(m => m.MemberId == memberId);

            var result = new ProfileViewModel
            {
                MemberId = memberId,
                UserName = member.Username,
                Password = member.Password,
                FullName = member.FullName,
                DateOfBirth = (DateOnly)member.DateOfBirth,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                BloodType = member.BloodType,
            };

            return result;
        }

    }
}
