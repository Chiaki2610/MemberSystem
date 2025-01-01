using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class LeaveType
{
    public int LeaveTypeId { get; set; }

    public string LeaveTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? RequiresApproval { get; set; }

    public bool? IsPaid { get; set; }

    public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();

    public virtual ICollection<LeavePolicy> LeavePolicies { get; set; } = new List<LeavePolicy>();
}
