using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Samaan.API.Data;
using Samaan.API.Models;

namespace Samaan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MerchantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MerchantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/merchants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Merchant>>> GetMerchants()
        {
            return await _context.Merchants
                .Where(m => m.IsOpen)
                .Include(m => m.User)
                .ToListAsync();
        }

        // GET: api/merchants/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Merchant>> GetMerchant(Guid id)
        {
            var merchant = await _context.Merchants
                .Include(m => m.User)
                .Include(m => m.Products.Where(p => p.IsAvailable))
                .FirstOrDefaultAsync(m => m.Id == id);

            if (merchant == null)
            {
                return NotFound();
            }

            return merchant;
        }

        // GET: api/merchants/{id}/products
        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetMerchantProducts(Guid id)
        {
            var merchant = await _context.Merchants.FindAsync(id);
            if (merchant == null)
            {
                return NotFound();
            }

            return await _context.Products
                .Where(p => p.MerchantId == id && p.IsAvailable)
                .ToListAsync();
        }

        // GET: api/merchants/nearby?city=Noida
        [HttpGet("nearby")]
        public async Task<ActionResult<IEnumerable<Merchant>>> GetNearbyMerchants([FromQuery] string city)
        {
            return await _context.Merchants
                .Where(m => m.City == city && m.IsOpen)
                .Include(m => m.User)
                .ToListAsync();
        }

        // PUT: api/merchants/{id} (Merchant only - update own profile)
        [HttpPut("{id}")]
        [Authorize(Roles = "Merchant")]
        public async Task<IActionResult> UpdateMerchant(Guid id, Merchant merchant)
        {
            if (id != merchant.Id)
            {
                return BadRequest();
            }

            _context.Entry(merchant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MerchantExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // PUT: api/merchants/{id}/toggle-status
        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "Merchant")]
        public async Task<IActionResult> ToggleShopStatus(Guid id)
        {
            var merchant = await _context.Merchants.FindAsync(id);
            if (merchant == null)
            {
                return NotFound();
            }

            merchant.IsOpen = !merchant.IsOpen;
            await _context.SaveChangesAsync();

            return Ok(new { isOpen = merchant.IsOpen });
        }

        private bool MerchantExists(Guid id)
        {
            return _context.Merchants.Any(e => e.Id == id);
        }
    }
}