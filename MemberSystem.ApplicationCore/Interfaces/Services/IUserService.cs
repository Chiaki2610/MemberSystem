using MemberSystem.ApplicationCore.Dtos;
using MemberSystem.ApplicationCore.Entities;
using MemberSystem.ApplicationCore.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Interfaces.Services
{
    public interface IUserService
    {
        Task RegisterUserAsync(RegisterDto member);

        Task<(ELoginResult Result, LoginDto LoginInfo)> ValidateUserAsync(string userName, string password);

        Task LoginUserAsync(LoginDto member, bool isPersistent = true);

        Task<(bool IsFound, bool? IsApproved)> CheckProgressAsync(RegisterDto member);

        Task<(bool IsFound, List<Member> Results)> CheckProgressForAdminAsync(RegisterDto member);

        Task<bool> UpdateProfileAsync(RegisterDto model);

        Task<bool> UpdatePasswordAsync(int memberId, string hashedPassword);
    }
}
