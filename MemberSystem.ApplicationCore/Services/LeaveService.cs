using MemberSystem.ApplicationCore.Dtos;
using MemberSystem.ApplicationCore.Entities;
using MemberSystem.ApplicationCore.Interfaces;
using MemberSystem.ApplicationCore.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly IRepository<ApprovalFlow> _approvalFlowRepository;
        private readonly IRepository<LeaveBalance> _leaveBalanceRepository;
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        //private readonly INotificationService _notificationService;
        private readonly ITransaction _transaction;
        private readonly ILogger<LeaveService> _logger;



        public async Task<LeaveBalanceDto> ViewLeaveBalanceAsync(int memberId, int leaveTypeId)
        {
            var leaveBalances = await _leaveBalanceRepository.FirstOrDefaultAsync
                                (x => x.MemberId == memberId && x.LeaveTypeId == leaveTypeId);

            _logger.LogInformation($"查得結果為{leaveBalances}");

            var result = new LeaveBalanceDto
            {
                MemberId = leaveBalances.MemberId,
                LeaveTypeId = leaveBalances.LeaveTypeId,
                Year = leaveBalances.Year,
                RemainingDays = leaveBalances.RemainingDays,
            };
            return result;
        }

        public async Task<bool> SubmitLeaveRequestAsync(LeaveRequestDto request)
        {
            try
            {
                _logger.LogInformation("開始進行申請狀態更新：{memberId}", request.MemberId);
                await _transaction.BeginTransactionAsync();

                var leaveBalance = await ViewLeaveBalanceAsync(request.MemberId, request.LeaveTypeId);

                if (leaveBalance == null ||
                    leaveBalance.RemainingDays < Math.Round((decimal)(request.EndDate - request.StartDate).TotalDays, 2))
                {
                    _logger.LogWarning("假期餘額不足");
                }

                

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
