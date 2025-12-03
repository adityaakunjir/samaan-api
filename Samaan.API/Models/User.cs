using System.ComponentModel.DataAnnotations;

namespace Samaan.API.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Customer";

        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Pincode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation
        public Merchant? Merchant { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}