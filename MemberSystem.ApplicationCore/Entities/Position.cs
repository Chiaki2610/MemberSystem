using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class Position
{
    public int PositionId { get; set; }

    public string PositionName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<ApprovalFlow> ApprovalFlows { get; set; } = new List<ApprovalFlow>();

    public virtual ICollection<MemberDepartment> MemberDepartments { get; set; } = new List<MemberDepartment>();
}
