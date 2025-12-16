using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Historylog
{
    public string Id { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}
