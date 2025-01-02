using MemberSystem.ApplicationCore.EntityRelationships;
using MemberSystem.ApplicationCore.Interfaces;
using MemberSystem.ApplicationCore.Interfaces.Services;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Services
{
    public class PositionPermissionService : IPermissionService
    {
        private readonly IRepository<PositionPermission> _positionPermissionRepository;

        public PositionPermissionService(IRepository<PositionPermission> positionPermissionRepository)
        {
            _positionPermissionRepository = positionPermissionRepository;
        }

        public async Task<bool> HasPermissionAsync(int positionId, string permissionName)
        {
            return await _positionPermissionRepository.AnyAsync(pp =>
                pp.PositionId == positionId &&
                pp.Permission.PermissionName == permissionName);
        }
    }
}
