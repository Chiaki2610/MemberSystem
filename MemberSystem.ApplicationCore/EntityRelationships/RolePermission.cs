using MemberSystem.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.EntityRelationships
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }

}
