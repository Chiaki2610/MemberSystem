using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class LeavePolicy
{
    public int PolicyId { get; set; }

    public int LeaveTypeId { get; set; }

    public int? MinYearsOfService { get; set; }

    public int? MaxYearsOfService { get; set; }

    public int LeaveDays { get; set; }

    public bool? CarryOver { get; set; }

    public int? ExpiryInMonths { get; set; }

    public virtual LeaveType LeaveType { get; set; } = null!;
}
