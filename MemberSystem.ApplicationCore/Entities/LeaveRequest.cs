using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class LeaveRequest
{
    public int LeaveRequestId { get; set; }

    public int MemberId { get; set; }

    public string LeaveType { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public int? ApproverId { get; set; }

    public virtual Member? Approver { get; set; }

    public virtual ICollection<LeaveApproval> LeaveApprovals { get; set; } = new List<LeaveApproval>();

    public virtual Member Member { get; set; } = null!;
}
