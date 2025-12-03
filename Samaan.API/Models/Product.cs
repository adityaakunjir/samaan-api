using System.ComponentModel.DataAnnotations;

namespace Samaan.API.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        [Required]
        public Guid MerchantId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Brand { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public decimal MRP { get; set; }

        public string? Unit { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; } = 0;
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Merchant Merchant { get; set; } = null!;
    }
}