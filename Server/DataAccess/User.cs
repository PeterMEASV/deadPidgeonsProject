using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class User
{
    public string Id { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phonenumber { get; set; } = null!;

    public bool Isactive { get; set; }

    public bool Isadmin { get; set; }

    public decimal Balance { get; set; }

    public string Password { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual ICollection<Balancelog> Balancelogs { get; set; } = new List<Balancelog>();

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();
}
