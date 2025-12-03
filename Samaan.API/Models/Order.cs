using System.ComponentModel.DataAnnotations;

namespace Samaan.API.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        [Required]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid MerchantId { get; set; }

        public decimal ItemsTotal { get; set; }
        public decimal DeliveryFee { get; set; } = 25.00m;
        public decimal Discount { get; set; } = 0;
        public decimal GrandTotal { get; set; }
        public string Status { get; set; } = "Placed";
        public string PaymentMethod { get; set; } = "COD";
        public string PaymentStatus { get; set; } = "Pending";
        public string? DeliveryAddress { get; set; }
        public string? DeliveryInstructions { get; set; }
        public string? EstimatedDelivery { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User Customer { get; set; } = null!;
        public Merchant Merchant { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}