using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Needed for database links

namespace BookM.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public string? Location { get; set; }

    public int Capacity { get; set; }

    public string? Ltype { get; set; }

    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;

    public string? ImageUrl { get; set; }


    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
}