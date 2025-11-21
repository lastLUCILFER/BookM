using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookM.Models;

public partial class BookMContext : DbContext
{
    public BookMContext()
    {
    }

    public BookMContext(DbContextOptions<BookMContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<TicketType> TicketType { get; set; }

    public virtual DbSet<Payment> Payment { get; set; }
    public virtual DbSet<Category> Category { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // We leave this empty because the connection is handled in Program.cs
        // using builder.Services.AddDbContext...
        if (!optionsBuilder.IsConfigured)
        {
            // You can keep the warning here if you want, or delete it.
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951AED82D804E4");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingDate).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Event");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_User");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__7944C810E36BADA9");

            entity.ToTable("Event");

            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Ltype)
                .HasMaxLength(50)
                .HasColumnName("LType");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CD64AA4CC");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534F52A5985").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
