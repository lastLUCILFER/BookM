using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Add this for [Required] attributes

namespace BookM.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public bool IsAdmin { get; set; } = false; 


    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}