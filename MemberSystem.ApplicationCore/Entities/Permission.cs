﻿using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();
}
