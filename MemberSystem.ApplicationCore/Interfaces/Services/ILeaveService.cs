using MemberSystem.ApplicationCore.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Interfaces.Services
{
    public interface ILeaveService
    {
        Task<LeaveBalanceDto> ViewLeaveBalanceAsync(int memberId, int leaveTypeId);

        Task<bool> SubmitLeaveRequestAsync(LeaveRequestDto request);
    }
}
