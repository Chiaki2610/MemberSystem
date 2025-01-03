using MemberSystem.ApplicationCore.Entities;

namespace MemberSystem.Web.Services
{
    public class CheckViewModelService
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly ITransaction _transaction;
        private readonly ILogger<CheckViewModelService> _logger;

        public CheckViewModelService(IRepository<Member> memberRepository,
                                     ITransaction transaction,
                                     ILogger<CheckViewModelService> logger)
        {
            _memberRepository = memberRepository;
            _transaction = transaction;
            _logger = logger;
        }

        public async Task<CheckMemberDataViewModel> GetUnapprovedMembersAsync()
        {
            var unapprovedMembers = await _memberRepository.ListAsync(m => m.IsApproved == null);
            List<RegisterDto> checkList = ConvertToViewModel(unapprovedMembers);

            return new CheckMemberDataViewModel
            {
                CheckMemberDataList = checkList,
            };
        }

        public async Task<CheckMemberDataViewModel> GetMembersAsync()
        {
            var members = await _memberRepository.ListAsync(m => m.IsApproved != null);
            List<RegisterDto> checkList = ConvertToViewModel(members);

            return new CheckMemberDataViewModel
            {
                CheckMemberDataList = checkList,
            };
        }

        private static List<RegisterDto> ConvertToViewModel(List<Member> members)
        {
            return members.Select(member => new RegisterDto
                   {
                       MemberId = member.MemberId,
                       UserName = member.Username,
                       Email = member.Email,
                       FullName = member.FullName,
                       DateOfBirth = (DateOnly)member.DateOfBirth,
                       PhoneNumber = member.PhoneNumber,
                       BloodType = member.BloodType,
                       RoleId = member.RoleId,
                       IsApproved = member.IsApproved,
                   })
                   .ToList();
        }
    }
}
