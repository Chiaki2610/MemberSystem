using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class Member
{
    public int MemberId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? PhoneNumber { get; set; }

    public string? BloodType { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<LeaveApproval> LeaveApprovals { get; set; } = new List<LeaveApproval>();

    public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();

    public virtual ICollection<LeaveRequest> LeaveRequestApprovers { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<LeaveRequest> LeaveRequestMembers { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<MemberDepartment> MemberDepartments { get; set; } = new List<MemberDepartment>();

    public virtual Role Role { get; set; } = null!;
}
