using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public OrderController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            if (order == null || !order.OrderItems.Any())
            {
                return BadRequest("Invalid order.");
            }

            order.OrderDate = DateTime.UtcNow;

            _context.Orders.Add(order);
            _context.SaveChanges();

            return Ok(order);
        }
        [HttpGet("byEmail")]
        public IActionResult GetOrdersByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var orders = _context.Orders
                .Include(o => o.OrderItems) // Включаем элементы заказа
                .Where(o => o.Email == email) // Фильтруем по email
                .ToList();

            if (!orders.Any())
            {
                return NotFound("No orders found for this email.");
            }

            return Ok(orders);
        }
    }
}
