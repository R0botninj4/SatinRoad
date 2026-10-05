using System.ComponentModel.DataAnnotations;
using Infra;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingController(ListingService listingService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ListingResponseDto>), StatusCodes.Status200OK)]
    public List<ListingResponseDto> GetAll()
    {
        return listingService.GetAll();
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Listing), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Listing> Create(CreateListingRequestDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }

            var listing = listingService.Create(userId, dto);

            return Created($"/api/listings/{listing.Id}", listing);
        }
        catch (ValidationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        return listingService.Delete(userId, id) ? NoContent() : NotFound();
    }
}
