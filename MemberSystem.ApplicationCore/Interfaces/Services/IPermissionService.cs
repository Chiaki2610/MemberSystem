using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Interfaces.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(int id, string permissionName);

        Task<IEnumerable<string>> GetPermissionsAsync(int id);

    }
}
