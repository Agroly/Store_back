using Microsoft.AspNetCore.Mvc;
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
    }
}
