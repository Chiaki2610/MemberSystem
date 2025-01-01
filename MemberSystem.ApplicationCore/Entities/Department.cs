using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int? ParentDepartmentId { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ApprovalFlow> ApprovalFlows { get; set; } = new List<ApprovalFlow>();

    public virtual ICollection<Department> InverseParentDepartment { get; set; } = new List<Department>();

    public virtual ICollection<MemberDepartment> MemberDepartments { get; set; } = new List<MemberDepartment>();

    public virtual Department? ParentDepartment { get; set; }
}
