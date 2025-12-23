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
        // --- 1. EXISTING CONFIGURATIONS ---
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
            entity.Property(e => e.Ltype).HasMaxLength(50).HasColumnName("LType");
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

        // --- 2. SEED DATA (HasData) ---

        // A. Seed Categories First (So we have IDs to use in Events)
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Cinema" },
            new Category { CategoryId = 2, Name = "Concert" },
            new Category { CategoryId = 3, Name = "Sport" },
            new Category { CategoryId = 4, Name = "Theater" }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1, 
                Name = "System Admin",
                Email = "admin@bookm.com",
                // This is the SHA256 hash for the password: "admin123"
                PasswordHash = "pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=",
                IsAdmin = true 
            }
        );

        // B. Seed Events (Using the Category IDs from above)
        // Note: Dates here are calculated when you create the MIGRATION, not when the app runs.
        modelBuilder.Entity<Event>().HasData(
            new Event
            {
                EventId = 1, // REQUIRED
                Title = "Mawazine Festival - Opening Night",
                Description = "The biggest music festival in Morocco returns.",
                EventDate = DateTime.Parse("2025-06-20"), // Better to use fixed dates for migrations
                Location = "OLM Souissi, Rabat",
                Capacity = 50000,
                Ltype = "Outdoor",
                CategoryId = 2, // Concert
                ImageUrl = "https://images.unsplash.com/photo-1459749411177-2a296581dca0?auto=format&fit=crop&w=600&q=80"
            },
            new Event
            {
                EventId = 2, // REQUIRED
                Title = "Wydad vs Raja - The Derby",
                Description = "The most anticipated football match of the season.",
                EventDate = DateTime.Parse("2025-05-15"),
                Location = "Complexe Mohammed V, Casablanca",
                Capacity = 45000,
                Ltype = "Stadium",
                CategoryId = 3, // Sport
                ImageUrl = "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=600&q=80"
            },
            new Event
            {
                EventId = 3, // REQUIRED
                Title = "Inception - Special Screening",
                Description = "Watch the Christopher Nolan masterpiece in IMAX.",
                EventDate = DateTime.Parse("2025-04-10"),
                Location = "Megarama, Casablanca",
                Capacity = 200,
                Ltype = "Indoor",
                CategoryId = 1, // Cinema
                ImageUrl = "https://images.unsplash.com/photo-1536440136628-849c177e76a1?auto=format&fit=crop&w=600&q=80"
            },
            new Event
            {
                EventId = 4, // REQUIRED
                Title = "Marrakech du Rire",
                Description = "An evening of laughter with the best comedians.",
                EventDate = DateTime.Parse("2025-07-01"),
                Location = "Palais El Badi, Marrakech",
                Capacity = 1500,
                Ltype = "Outdoor",
                CategoryId = 4, // Theater
                ImageUrl = "https://images.unsplash.com/photo-1585699324551-f6c309eedeca?auto=format&fit=crop&w=600&q=80"
            }
        );

        modelBuilder.Entity<TicketType>(entity =>
        {
            // 1. Force EF to see the new column
            entity.Property(e => e.QuantityAvailable)
                  .IsRequired()
                  .HasDefaultValue(0);

            // 2. Add your Seed Data here
            entity.HasData(
                new TicketType { TicketTypeId = 1, Name = "Standard", Price = 450.00m, QuantityAvailable = 1000, EventId = 1 },
                new TicketType { TicketTypeId = 2, Name = "VIP", Price = 800.00m, QuantityAvailable = 100, EventId = 1 },
                new TicketType { TicketTypeId = 3, Name = "Golden Circle", Price = 1200.00m, QuantityAvailable = 50, EventId = 1 }
            );
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
