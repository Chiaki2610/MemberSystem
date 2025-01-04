using MemberSystem.ApplicationCore.Dtos;
using MemberSystem.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Interfaces.Services
{
    public interface ILeaveService
    {
        Task<bool> ViewLeaveBalanceAsync(LeaveRequestDto model);

        Task<bool> SubmitLeaveRequestAsync(LeaveRequestDto request);

        Task<bool> SubmitLeaveApprovalAsync(EditLeaveRequestStatusDto request);

        Task<List<ApprovalFlowDto>> GetApprovalFlowAsync(int leaveRequestId);
    }
}
