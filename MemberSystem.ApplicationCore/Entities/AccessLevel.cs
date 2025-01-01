using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class AccessLevel
{
    public int AccessLevelId { get; set; }

    public string LevelName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<PageAccess> PageAccesses { get; set; } = new List<PageAccess>();
}
