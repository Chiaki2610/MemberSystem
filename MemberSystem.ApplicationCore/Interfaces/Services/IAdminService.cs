using MemberSystem.ApplicationCore.Dtos;
using MemberSystem.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Interfaces
{
    public interface IAdminService
    {
        Task<bool> UpdateApprovedAsync(int memberId, bool isApproved);
    }
}
