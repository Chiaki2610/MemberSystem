using MemberSystem.ApplicationCore.EntityRelationships;
using MemberSystem.ApplicationCore.Interfaces;
using MemberSystem.ApplicationCore.Interfaces.Services;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Services
{
    public class RolePermissionService : IPermissionService
    {
        private readonly IRepository<RolePermission> _rolePermissionRepository;

        public RolePermissionService(IRepository<RolePermission> rolePermissionRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<bool> HasPermissionAsync(int roleId, string permissionName)
        {
            return await _rolePermissionRepository.AnyAsync(rp =>
                rp.RoleId == roleId &&
                rp.Permission.PermissionName == permissionName);
        }
    }
}
