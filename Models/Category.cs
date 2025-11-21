namespace BookM.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }

}
