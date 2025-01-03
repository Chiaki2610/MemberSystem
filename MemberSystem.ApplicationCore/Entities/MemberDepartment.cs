using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class MemberDepartment
{
    public int MemberDepartmentId { get; set; }

    public int MemberId { get; set; }

    public int DepartmentId { get; set; }

    public int PositionId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;

    public virtual Position Position { get; set; } = null!;
}
