using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Samaan.API.Data;
using Samaan.API.DTOs;
using Samaan.API.Models;
using Samaan.API.Services;

namespace Samaan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Merchant)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Ok(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                });
            }

            // For demo accounts, check if password is Demo@123
            bool isValidPassword = false;

            if (request.Password == "Demo@123" && user.PasswordHash.Contains("Demo"))
            {
                isValidPassword = true;
            }
            else
            {
                try
                {
                    isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                }
                catch
                {
                    isValidPassword = false;
                }
            }

            if (!isValidPassword)
            {
                return Ok(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                });
            }

            var token = _tokenService.GenerateToken(user, user.Merchant?.Id);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    Phone = user.Phone,
                    MerchantId = user.Merchant?.Id,
                    ShopName = user.Merchant?.ShopName
                }
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Ok(new AuthResponse
                {
                    Success = false,
                    Message = "Email already registered"
                });
            }

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Role = request.Role,
                Phone = request.Phone,
                Address = request.Address,
                City = request.City,
                Pincode = request.Pincode
            };

            _context.Users.Add(user);

            Merchant? merchant = null;

            // If registering as merchant, create merchant profile
            if (request.Role == "Merchant" && !string.IsNullOrEmpty(request.ShopName))
            {
                merchant = new Merchant
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    ShopName = request.ShopName,
                    ShopAddress = request.ShopAddress,
                    City = request.City,
                    Pincode = request.Pincode
                };

                _context.Merchants.Add(merchant);
            }

            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user, merchant?.Id);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    Phone = user.Phone,
                    MerchantId = merchant?.Id,
                    ShopName = merchant?.ShopName
                }
            });
        }
    }
}