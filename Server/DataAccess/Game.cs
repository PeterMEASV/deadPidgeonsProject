using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Game
{
    public string Id { get; set; } = null!;

    public string Weeknumber { get; set; } = null!;

    public List<int> Winningnumbers { get; set; } = null!;

    public DateTime Drawdate { get; set; }

    public bool Isactive { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();
}
