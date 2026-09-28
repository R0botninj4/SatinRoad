using System.ComponentModel.DataAnnotations;
using Infra;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingController(ListingService listingService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<Listing>), StatusCodes.Status200OK)]
    public List<Listing> GetAll()
    {
        return listingService.GetAll();
    }

    [HttpPost]
    [ProducesResponseType(typeof(Listing), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Listing> Create(CreateListingRequestDto dto)
    {
        try
        {
            var listing = listingService.Create(dto);

            return Created($"/api/listings/{listing.Id}", listing);
        }
        catch (ValidationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}