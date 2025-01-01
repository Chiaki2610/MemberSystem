using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class PageAccess
{
    public int RoleId { get; set; }

    public int PageId { get; set; }

    public int AccessLevelId { get; set; }

    public virtual AccessLevel AccessLevel { get; set; } = null!;

    public virtual Page Page { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
