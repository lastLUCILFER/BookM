using System.ComponentModel.DataAnnotations.Schema;

namespace BookM.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;

        public DateTime PaymentDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = null!; // e.g., "CMI", "PayPal", "Cash"
        public string Status { get; set; } = null!; // "Pending", "Completed", "Failed"
        public string? TransactionId { get; set; } // The ID returned by the payment gateway
    }
}
