using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Balancelog
{
    public int Id { get; set; }

    public string Userid { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Transactionnumber { get; set; } = null!;

    public bool Approved { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
