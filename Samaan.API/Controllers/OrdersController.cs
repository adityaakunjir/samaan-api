using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Samaan.API.Data;
using Samaan.API.Models;
using System.Security.Claims;

namespace Samaan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/orders (Customer - their orders)
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Order>>> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            return await _context.Orders
                .Where(o => o.CustomerId == Guid.Parse(userId))
                .Include(o => o.Merchant)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // GET: api/orders/merchant (Merchant - orders for their shop)
        [HttpGet("merchant")]
        [Authorize(Roles = "Merchant")]
        public async Task<ActionResult<IEnumerable<Order>>> GetMerchantOrders()
        {
            var merchantId = User.FindFirst("MerchantId")?.Value;
            if (merchantId == null) return Unauthorized();

            return await _context.Orders
                .Where(o => o.MerchantId == Guid.Parse(merchantId))
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Merchant)
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/orders (Customer creates order)
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"SAM{DateTime.Now:yyMMddHHmmss}",
                CustomerId = Guid.Parse(userId),
                MerchantId = request.MerchantId,
                ItemsTotal = request.ItemsTotal,
                DeliveryFee = request.DeliveryFee,
                Discount = request.Discount,
                GrandTotal = request.ItemsTotal + request.DeliveryFee - request.Discount,
                Status = "Placed",
                PaymentMethod = request.PaymentMethod,
                DeliveryAddress = request.DeliveryAddress,
                DeliveryInstructions = request.DeliveryInstructions,
                EstimatedDelivery = "15-20 mins"
            };

            _context.Orders.Add(order);

            // Add order items
            foreach (var item in request.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        Total = product.Price * item.Quantity
                    };
                    _context.OrderItems.Add(orderItem);
                }
            }

            // Update merchant total orders
            var merchant = await _context.Merchants.FindAsync(request.MerchantId);
            if (merchant != null)
            {
                merchant.TotalOrders += 1;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/orders/{id}/status (Merchant updates status)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Merchant")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = request.Status;
            order.UpdatedAt = DateTime.UtcNow;

            if (request.Status == "Delivered")
            {
                order.PaymentStatus = "Paid";
            }

            await _context.SaveChangesAsync();

            return Ok(new { status = order.Status });
        }
    }

    // DTOs for Orders
    public class CreateOrderRequest
    {
        public Guid MerchantId { get; set; }
        public decimal ItemsTotal { get; set; }
        public decimal DeliveryFee { get; set; } = 25.00m;
        public decimal Discount { get; set; } = 0;
        public string PaymentMethod { get; set; } = "COD";
        public string? DeliveryAddress { get; set; }
        public string? DeliveryInstructions { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}