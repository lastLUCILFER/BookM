using System;
using System.Collections.Generic;

namespace BookM.Models;

public partial class Booking
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public int SeatsBooked { get; set; }
    public DateTime BookingDate { get; set; }


    public int TicketTypeId { get; set; }
    public virtual TicketType TicketType { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
