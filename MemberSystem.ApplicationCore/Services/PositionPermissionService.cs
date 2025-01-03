using MemberSystem.ApplicationCore.EntityRelationships;
using MemberSystem.ApplicationCore.Interfaces;
using MemberSystem.ApplicationCore.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Services
{
    public class PositionPermissionService : IPermissionService
    {
        //private readonly IRepository<PositionPermission> _positionPermissionRepository;

        //public PositionPermissionService(IRepository<PositionPermission> positionPermissionRepository)
        //{
        //    _positionPermissionRepository = positionPermissionRepository;
        //}

        //public async Task<bool> HasPermissionAsync(int positionId, string permissionName)
        //{
        //    return await _positionPermissionRepository.AnyAsync(pp =>
        //        pp.PositionId == positionId &&
        //        pp.Permission.PermissionName == permissionName);
        //}

        //public async Task<IEnumerable<string>> GetPermissionsAsync(int id)
        //{
        //    var positionPermissions = await _positionPermissionRepository.ListAsync(x => x.PositionId == id);
        //    return positionPermissions.Select(pp => pp.Permission.PermissionName);
        //}
    }
}
