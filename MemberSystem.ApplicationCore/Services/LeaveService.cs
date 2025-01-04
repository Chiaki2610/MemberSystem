using MemberSystem.ApplicationCore.Dtos;
using MemberSystem.ApplicationCore.Entities;
using MemberSystem.ApplicationCore.Interfaces;
using MemberSystem.ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly IRepository<ApprovalFlow> _approvalFlowRepository;
        private readonly IRepository<LeaveType> _leaveTypeRepository;
        private readonly IRepository<LeaveBalance> _leaveBalanceRepository;
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        private readonly IRepository<LeaveApproval> _leaveApprovalRepository;
        private readonly IRepository<MemberDepartment> _memberDepartmentRepository;
        private readonly IUserService _userService;
        private readonly ITransaction _transaction;
        private readonly ILogger<LeaveService> _logger;

        public LeaveService(IRepository<Member> memberRepository,
                            IRepository<ApprovalFlow> approvalFlowRepository,
                            IRepository<LeaveBalance> leaveBalanceRepository,
                            IRepository<LeaveRequest> leaveRequestRepository,
                            IRepository<LeaveApproval> leaveApprovalRepository,
                            IRepository<MemberDepartment> memberDepartmentRepository,
                            IUserService userService,
                            ITransaction transaction,
                            ILogger<LeaveService> logger,
                            IRepository<LeaveType> leaveTypeRepository)
        {
            _memberRepository = memberRepository;
            _approvalFlowRepository = approvalFlowRepository;
            _leaveBalanceRepository = leaveBalanceRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _leaveApprovalRepository = leaveApprovalRepository;
            _memberDepartmentRepository = memberDepartmentRepository;
            _userService = userService;
            _transaction = transaction;
            _logger = logger;
            _leaveTypeRepository = leaveTypeRepository;
        }

        public async Task<bool> ViewLeaveBalanceAsync(LeaveRequestDto model)
        {
            try
            {
                _logger.LogInformation("開始進行天數驗證：{memberId}", model.MemberId);

                // 計算當次請假天數
                var requestedDays = (model.EndDate - model.StartDate).TotalDays + 1;

                // 取得LeaveTypeID
                var leaveRequest = await _leaveTypeRepository.FirstOrDefaultAsync(t => t.LeaveTypeName == model.LeaveType);

                var leaveBalance = await _leaveBalanceRepository.FirstOrDefaultAsync
                                    (x => x.MemberId == model.MemberId
                                  && x.LeaveTypeId == leaveRequest.LeaveTypeId
                                  && x.Year == DateTime.Now.Year);

                if (leaveBalance == null)
                {
                    _logger.LogInformation($"找不到員工 {model.MemberId} 在假別 {leaveRequest.LeaveTypeId} 下的假別配額記錄");
                }

                _logger.LogInformation($"查得結果為{leaveBalance.MemberId}");

                if (leaveBalance.RemainingDays < (decimal)requestedDays)
                {
                    return false;
                }

                leaveBalance.RemainingDays -= (decimal)requestedDays;
                await _leaveBalanceRepository.UpdateAsync(leaveBalance);

                _logger.LogInformation("使用者驗證成功，已扣除剩餘天數：{Username}", model.MemberId);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SubmitLeaveRequestAsync(LeaveRequestDto model)
        {
            try
            {
                _logger.LogInformation("開始進行申請狀態更新：{memberId}", model.MemberId);
                await _transaction.BeginTransactionAsync();

                // 驗證剩餘天數是否足夠的方法及判斷
                var leaveBalance = await ViewLeaveBalanceAsync(model);
                if (!leaveBalance)
                {
                    _logger.LogWarning("假期餘額不足");
                    return false;
                }

                var leaveRequestEntity = new LeaveRequest
                {
                    MemberId = model.MemberId,
                    LeaveType = model.LeaveType,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Reason = model.Reason,
                    ApproverId = model.ApproverId,
                };

                var entity = await _leaveRequestRepository.AddAsync(leaveRequestEntity);

                var memberDepartment = await _userService.GetMemberDepartmentAsync(model.MemberId);
                var approvalFlows = await _approvalFlowRepository
                                    .ListAsync(d => d.DepartmentId == memberDepartment.DepartmentId);
                var approvalFlowsOrderBy = approvalFlows.OrderBy(d => d.ApprovalOrder);

                foreach (var flow in approvalFlowsOrderBy)
                {
                    var approval = new LeaveApproval
                    {
                        LeaveRequestId = entity.LeaveRequestId,
                        FlowId = flow.FlowId,
                        ApproverId = null,
                        ApprovalStatus = "Pending",
                        ApprovalTime = DateTime.Now,
                    };
                    var approvalAdd = await _leaveApprovalRepository.AddAsync(approval);
                }

                await _transaction.CommitAsync();
                _logger.LogInformation("使用者申請成功：{Username}", model.MemberId);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SubmitLeaveApprovalAsync(EditLeaveRequestStatusDto request)
        {
            try
            {
                _logger.LogInformation("開始進行申請者(非審核者)審核狀態更新：{memberId}", request.MemberId);
                await _transaction.BeginTransactionAsync();

                // 只會有一筆所以使用FirstOrDefaultAsync
                var leaveRequest = await _leaveRequestRepository.FirstOrDefaultAsync
                                  (lr => lr.LeaveRequestId == request.LeaveRequestId);
                if (leaveRequest == null)
                {
                    _logger.LogWarning("找不到指定的LeaveRequest，ID：{request.LeaveRequestId}", request.LeaveRequestId);
                    return false;
                }

                leaveRequest.Status = request.Status;
                leaveRequest.ApproverId = request.ApproverId;
                await _leaveRequestRepository.UpdateAsync(leaveRequest);

                // 會有多筆所以需判斷ApprovalId的FlowID
                var ad = await _memberDepartmentRepository.FirstOrDefaultAsync(m => m.MemberId == request.ApproverId);
                var flowId = await _approvalFlowRepository.FirstOrDefaultAsync
                                                          (f => f.DepartmentId == ad.DepartmentId && f.PositionId == ad.PositionId);
                var leaveApproval = await _leaveApprovalRepository.FirstOrDefaultAsync(lr => lr.FlowId == flowId.FlowId);

                leaveApproval.ApproverId = request.ApproverId;
                leaveApproval.ApprovalStatus = request.Status;
                leaveApproval.ApprovalTime = DateTime.Now;
                await _leaveApprovalRepository.UpdateAsync(leaveApproval);

                await _transaction.CommitAsync();
                _logger.LogInformation("申請狀態更新成功：LeaveRequest {request.LeaveRequestId}，狀態：{request.Status}", request.LeaveRequestId, request.Status);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<ApprovalFlowDto>> GetApprovalFlowAsync(int leaveRequestId)
        {
            // 確認請假申請是否存在
            var leaveRequest = await _leaveRequestRepository.FirstOrDefaultAsync(lr => lr.LeaveRequestId == leaveRequestId);
            if (leaveRequest == null)
            {
                _logger.LogWarning($"找不到指定的請假申請，ID：{leaveRequestId}");
            }

            // 查詢與該筆申請相關的所有簽核記錄
            var leaveApprovals = await _leaveApprovalRepository.ListAsync(la => la.LeaveRequestId == leaveRequestId);

            // 將與簽核紀錄相關的人員資訊找齊
            var approverIds = leaveApprovals.Where(la => la.ApproverId.HasValue).Select(la => la.ApproverId.Value).Distinct();
            var approvers = await _memberRepository.ListAsync(m => approverIds.Contains(m.MemberId));

            var result = leaveApprovals
                .OrderBy(la => la.FlowId)
                .Select(la =>
                {
                    var approver = approvers.FirstOrDefault(m => m.MemberId == la.ApproverId);
                    return new ApprovalFlowDto
                    {
                        ApprovalOrder = la.FlowId, // 設計資料表時FlowId是小→大表示簽核順序
                        FlowDescription = $"簽核順位： {la.FlowId}",
                        ApprovalStatus = la.ApprovalStatus,
                        ApprovalTime = la.ApprovalTime,
                        ApproverName = approver?.FullName
                    };
                })
                .ToList();

            return result;
        }


    }
}
