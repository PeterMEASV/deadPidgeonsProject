using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Board
{
    public string Id { get; set; } = null!;

    public string Userid { get; set; } = null!;

    public List<int> Selectednumbers { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public bool Winner { get; set; }

    public virtual User User { get; set; } = null!;
}
