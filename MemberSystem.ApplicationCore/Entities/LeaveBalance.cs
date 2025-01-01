using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class LeaveBalance
{
    public int BalanceId { get; set; }

    public int MemberId { get; set; }

    public int LeaveTypeId { get; set; }

    public int Year { get; set; }

    public decimal RemainingDays { get; set; }

    public virtual LeaveType LeaveType { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;
}
