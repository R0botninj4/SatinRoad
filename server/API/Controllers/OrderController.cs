using System.ComponentModel.DataAnnotations;
using Infra;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(OrderService orderService)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Order> Create(CreateOrderRequestDto dto)
    {
        try
        {
            var order = orderService.Create(dto);

            return Created($"/api/orders/{order.Id}", order);
        }
        catch (ValidationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}