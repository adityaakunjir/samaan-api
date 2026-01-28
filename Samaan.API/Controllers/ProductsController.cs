using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Samaan.API.Data;
using Samaan.API.Models;

namespace Samaan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products
                .Where(p => p.IsAvailable)
                .Include(p => p.Merchant)
                .ToListAsync();
        }

        // GET: api/products/merchant/{merchantId}
        [HttpGet("merchant/{merchantId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByMerchant(Guid merchantId)
        {
            return await _context.Products
                .Where(p => p.MerchantId == merchantId && p.IsAvailable)
                .ToListAsync();
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Merchant)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // GET: api/products/category/{category}
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
        {
            return await _context.Products
                .Where(p => p.Category == category && p.IsAvailable)
                .Include(p => p.Merchant)
                .ToListAsync();
        }

        // GET: api/products/search?q=rice
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts([FromQuery] string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return await _context.Products
                    .Where(p => p.IsAvailable)
                    .Include(p => p.Merchant)
                    .ToListAsync();
            }

            return await _context.Products
                .Where(p => p.IsAvailable &&
                    (p.Name.Contains(q) ||
                     p.Brand!.Contains(q) ||
                     p.Category!.Contains(q)))
                .Include(p => p.Merchant)
                .ToListAsync();
        }

        // POST: api/products (Merchant only)
        [HttpPost]
        [Authorize(Roles = "Merchant")]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/products/{id} (Merchant only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Merchant")]
        public async Task<IActionResult> UpdateProduct(Guid id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/products/{id} (Merchant only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Merchant")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.IsAvailable = false; // Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}