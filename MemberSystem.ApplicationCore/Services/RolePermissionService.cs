using MemberSystem.ApplicationCore.EntityRelationships;
using MemberSystem.ApplicationCore.Interfaces;
using MemberSystem.ApplicationCore.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Services
{
    public class RolePermissionService : IPermissionService
    {
        //private readonly IRepository<RolePermission> _rolePermissionRepository;

        //public RolePermissionService(IRepository<RolePermission> rolePermissionRepository)
        //{
        //    _rolePermissionRepository = rolePermissionRepository;
        //}

        //public async Task<bool> HasPermissionAsync(int roleId, string permissionName)
        //{
        //    return await _rolePermissionRepository.AnyAsync(rp =>
        //        rp.RoleId == roleId &&
        //        rp.Permission.PermissionName == permissionName);
        //}

        //public async Task<IEnumerable<string>> GetPermissionsAsync(int id)
        //{
        //    var rolePermissions = await _rolePermissionRepository.ListAsync(x => x.RoleId == id);
        //    return rolePermissions.Select(rp => rp.Permission.PermissionName);
        //}
    }
}
