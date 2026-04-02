namespace EcommerseEscalavel.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<OrderItem> Items { get; set; }
    }
}