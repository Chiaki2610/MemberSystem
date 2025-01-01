using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class ApprovalFlow
{
    public int FlowId { get; set; }

    public int DepartmentId { get; set; }

    public int PositionId { get; set; }

    public int ApprovalOrder { get; set; }

    public string? Description { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<LeaveApproval> LeaveApprovals { get; set; } = new List<LeaveApproval>();

    public virtual Position Position { get; set; } = null!;
}
