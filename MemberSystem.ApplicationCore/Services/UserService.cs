using MemberSystem.ApplicationCore.Dtos;
using MemberSystem.ApplicationCore.Entities;
using MemberSystem.ApplicationCore.Interfaces;
using MemberSystem.ApplicationCore.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<MemberDepartment> _memberDepartmentRepository;
        private readonly ITransaction _transaction; 
        private readonly ILogger<UserService> _logger;

        public UserService(IRepository<MemberDepartment> memberDepartmentRepository,
                           ITransaction transaction,
                           ILogger<UserService> logger)
        {
            _memberDepartmentRepository = memberDepartmentRepository;
            _transaction = transaction;
            _logger = logger;
        }

        public async Task<MemberDepartmentDto> GetMemberDepartmentAsync(int memberId)
        {
            var memberDepartment = await _memberDepartmentRepository.FirstOrDefaultAsync(m => m.MemberId == memberId);

            var result = new MemberDepartmentDto
            {
                MemberDepartmentId = memberDepartment.MemberId,
                MemberId = memberId,
                DepartmentId = memberDepartment.DepartmentId,
                PositionId = memberDepartment.PositionId,
            };

            return result;
        }
    }

}
