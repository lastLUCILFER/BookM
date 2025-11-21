using System.ComponentModel.DataAnnotations.Schema;

namespace BookM.Models
{
    public class TicketType
    {
        public int TicketTypeId { get; set; }

        public int EventId { get; set; }
        public virtual Event Event { get; set; } = null!;

        public string Name { get; set; } = null!; // e.g., "VIP", "Gold", "Student"

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public int QuantityAvailable { get; set; } // How many VIP tickets exist?
    }
}
