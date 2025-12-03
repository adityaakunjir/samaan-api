using System.ComponentModel.DataAnnotations;

namespace Samaan.API.DTOs
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Pincode { get; set; }
        public string Role { get; set; } = "Customer";

        // For Merchant registration
        public string? ShopName { get; set; }
        public string? ShopAddress { get; set; }
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public UserInfo? User { get; set; }
    }

    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public Guid? MerchantId { get; set; }
        public string? ShopName { get; set; }
    }
}