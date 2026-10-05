using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(OrderService orderService)
    : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<Order> Create(CreateOrderRequestDto dto)
    {
        try
        {
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (buyerId is null)
            {
                return Unauthorized();
            }

            var order = orderService.Create(buyerId, dto);

            return Created($"/api/orders/{order.Id}", order);
        }
        catch (ValidationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
    [Authorize]
    [HttpGet("my-orders")]
    [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<List<Order>> GetMyOrders()
    {
        var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (buyerId is null)
        {
            return Unauthorized();
        }

        return Ok(orderService.GetMyOrders(buyerId));
    }
    
    [Authorize]
    [HttpGet("quote")]
    [ProducesResponseType(typeof(OrderQuoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<OrderQuoteResponseDto> GetQuote(
        [FromQuery] CreateOrderRequestDto dto)
    {
        var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (buyerId is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(orderService.GetQuote(buyerId, dto));
        }
        catch (ValidationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}