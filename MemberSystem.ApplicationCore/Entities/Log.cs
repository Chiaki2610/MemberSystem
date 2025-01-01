using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class Log
{
    public long LogId { get; set; }

    public string LogType { get; set; } = null!;

    public DateTime? LogTime { get; set; }

    public int? MemberId { get; set; }

    public string? RelatedSystem { get; set; }

    public string? Severity { get; set; }

    public string Message { get; set; } = null!;

    public virtual ICollection<LogDetail> LogDetails { get; set; } = new List<LogDetail>();

    public virtual Member? Member { get; set; }
}
