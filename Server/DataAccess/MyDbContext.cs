using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Balancelog> Balancelogs { get; set; }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Balancelog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("balancelog_pkey");

            entity.ToTable("balancelog", "deadpidgeons");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Approved)
                .HasDefaultValue(false)
                .HasColumnName("approved");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.Transactionnumber).HasColumnName("transactionnumber");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Balancelogs)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("balancelog_userid_fkey");
        });

        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("boards_pkey");

            entity.ToTable("boards", "deadpidgeons");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Gameid).HasColumnName("gameid");
            entity.Property(e => e.Selectednumbers).HasColumnName("selectednumbers");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Winner)
                .HasDefaultValue(false)
                .HasColumnName("winner");

            entity.HasOne(d => d.Game).WithMany(p => p.Boards)
                .HasForeignKey(d => d.Gameid)
                .HasConstraintName("boards_gameid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Boards)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("boards_userid_fkey");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("games_pkey");

            entity.ToTable("games", "deadpidgeons");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Drawdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("drawdate");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.Weeknumber).HasColumnName("weeknumber");
            entity.Property(e => e.Winningnumbers).HasColumnName("winningnumbers");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "deadpidgeons");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Balance)
                .HasPrecision(10, 2)
                .HasColumnName("balance");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Firstname).HasColumnName("firstname");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(false)
                .HasColumnName("isactive");
            entity.Property(e => e.Isadmin)
                .HasDefaultValue(false)
                .HasColumnName("isadmin");
            entity.Property(e => e.Lastname).HasColumnName("lastname");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Phonenumber).HasColumnName("phonenumber");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
