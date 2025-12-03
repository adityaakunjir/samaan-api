using System.ComponentModel.DataAnnotations;

namespace Samaan.API.Models
{
    public class Merchant
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string ShopName { get; set; } = string.Empty;

        public string ShopType { get; set; } = "Kirana";
        public string? Description { get; set; }
        public string? ShopAddress { get; set; }
        public string? City { get; set; }
        public string? Pincode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal DeliveryRadius { get; set; } = 3.0m;
        public decimal MinOrderAmount { get; set; } = 99.00m;
        public decimal DeliveryFee { get; set; } = 25.00m;
        public bool IsOpen { get; set; } = true;
        public TimeSpan OpenTime { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan CloseTime { get; set; } = new TimeSpan(22, 0, 0);
        public decimal Rating { get; set; } = 4.0m;
        public int TotalOrders { get; set; } = 0;
        public bool IsVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}