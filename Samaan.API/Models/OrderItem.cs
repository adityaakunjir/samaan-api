using System.ComponentModel.DataAnnotations;

namespace Samaan.API.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}