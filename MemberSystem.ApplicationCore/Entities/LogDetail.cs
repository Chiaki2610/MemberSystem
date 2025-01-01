using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class LogDetail
{
    public long DetailId { get; set; }

    public long LogId { get; set; }

    public string? DetailData { get; set; }

    public virtual Log Log { get; set; } = null!;
}
