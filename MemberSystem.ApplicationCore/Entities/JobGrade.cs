using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class JobGrade
{
    public int JobGradeId { get; set; }

    public int GradeLevel { get; set; }

    public string? GradeName { get; set; }

    public decimal? BaseSalary { get; set; }

    public decimal? MaxSalary { get; set; }

    public string? Description { get; set; }
}
