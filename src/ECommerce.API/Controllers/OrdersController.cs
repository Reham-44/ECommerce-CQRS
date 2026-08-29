using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _orderService.GetById(id);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<List<OrderDto>>> GetCustomerOrders(
        int customerId)
    {
        var orders = await _orderService.GetCustomerOrders(customerId);

        return Ok(orders);
    }

    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var result = await _orderService.CancelOrder(id);

        if (!result.Success)
        {
            if (result.Error == "Order not found")
                return NotFound(result.Error);

            return BadRequest(result.Error);
        }

        return Ok(new
        {
            message = "Order cancelled successfully"
        });
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(
        [FromBody] CreateOrderDto request)
    {
        var result = await _orderService.Checkout(request);

        if (!result.Success)
        {
            if (result.Error?.Contains("not found") == true)
                return NotFound(result.Error);

            return BadRequest(result.Error);
        }

        return Ok(result.Result);
    }
}