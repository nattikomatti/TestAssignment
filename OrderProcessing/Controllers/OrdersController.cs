using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Models.Requests;
using OrderProcessing.Services.Interfaces;

namespace OrderProcessing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IOrderService orderService, IInventoryService inventoryService) : ControllerBase
    {
        // ดูสินค้าทั้งหมด
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await inventoryService.GetAllProductsAsync();
            return Ok(products);
        }

        // ดูคำสั่งซื้อทั้งหมด
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok( await  orderService.GetAll());
        }

        // ดูคำสั่งซื้อตาม ID
        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            var result = orderService.GetById(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // สร้างคำสั่งซื้อใหม่ (พร้อม idempotency + distributed lock)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var result = await orderService.CreateOrderAsync(request);
            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
                : BadRequest(result);
        }

        // ยกเลิกคำสั่งซื้อ (คืนเงิน + คืน stock)
        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await orderService.CancelOrderAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
