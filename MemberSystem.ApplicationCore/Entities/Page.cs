using System;
using System.Collections.Generic;

namespace MemberSystem.ApplicationCore.Entities;

public partial class Page
{
    public int PageId { get; set; }

    public string PageName { get; set; } = null!;

    public string? Description { get; set; }
}
