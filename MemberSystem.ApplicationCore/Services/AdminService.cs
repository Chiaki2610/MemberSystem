using MemberSystem.ApplicationCore.Dtos;
using MemberSystem.ApplicationCore.Entities;
using MemberSystem.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<Member> _memberRepository;
        private readonly ITransaction _transaction;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IRepository<Member> memberRepository,
                            ITransaction transaction,
                            ILogger<AdminService> logger)
        {
            _memberRepository = memberRepository;
            _transaction = transaction;
            _logger = logger;
        }

        /// <summary>
        /// 審核更新狀態(通過/駁回)
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> UpdateApprovedAsync(int memberId, bool isApproved)
        {
            try
            {
                _logger.LogInformation("開始進行申請狀態更新：{memberId}", memberId);
                await _transaction.BeginTransactionAsync();

                var member = await _memberRepository.FirstOrDefaultAsync(m => m.MemberId == memberId);
                if (member == null)
                {
                    _logger.LogWarning("找不到指定的會員，ID：{memberId}", memberId);
                    return false;
                }

                member.IsApproved = isApproved;
                await _memberRepository.UpdateAsync(member);

                await _transaction.CommitAsync();
                _logger.LogInformation("申請狀態更新成功：會員ID {memberId}，狀態：{isApproved}", memberId, isApproved ? "通過" : "駁回");

                return true;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "無效操作：{memberId}", memberId);
                await _transaction.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                _logger.LogError(ex, "申請狀態更新時發生錯誤：{memberId}", memberId);
                throw;
            }
        }
    }
}
