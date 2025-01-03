using MemberSystem.ApplicationCore.Dtos;
using MemberSystem.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Interfaces.Services
{
    public interface IUserService
    {
        Task<MemberDepartmentDto> GetMemberDepartmentAsync(int memberId);
    }
}
