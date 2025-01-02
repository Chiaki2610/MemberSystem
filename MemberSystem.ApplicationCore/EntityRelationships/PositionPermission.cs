using MemberSystem.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.EntityRelationships
{
    public class PositionPermission
    {
        public int PositionId { get; set; }
        public int PermissionId { get; set; }
        public Position Position { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }

}
