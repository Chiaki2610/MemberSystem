using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class LeaveApproval
{
    public int ApprovalId { get; set; }

    public int LeaveRequestId { get; set; }

    public int FlowId { get; set; }

    public int? ApproverId { get; set; }

    public string? ApprovalStatus { get; set; }

    public DateTime? ApprovalTime { get; set; }

    public virtual Member? Approver { get; set; }

    public virtual ApprovalFlow Flow { get; set; } = null!;

    public virtual LeaveRequest LeaveRequest { get; set; } = null!;
}
