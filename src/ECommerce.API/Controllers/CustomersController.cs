using ECommerce.Application.Customers.Queries.GetCustomerById;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _mediator.Send(
            new GetCustomerByIdQuery { Id = id }
        );
        if (customer == null)
            return NotFound($"Customer with ID {id} not found.");

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(
        [FromBody] CreateCustomerDto dto)
    {
        var result = await _customerService.Create(dto);

        if (!result.Success)
            return BadRequest(result.Error);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Customer!.Id },
            result.Customer);
    }

    [HttpPost("{id}/upgrade-vip")]
    public async Task<IActionResult> UpgradeToVip(int id)
    {
        var result = await _customerService.UpgradeToVip(id);

        if (!result.Success)
        {
            if (result.Error == "Customer not found.")
                return NotFound(result.Error);

            return BadRequest(result.Error);
        }

        return Ok(new
        {
            message = "Customer upgraded to VIP successfully."
        });
    }
}