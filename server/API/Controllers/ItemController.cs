using Infra;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("api/items")]
public class ItemController(ItemService itemService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<Item>), StatusCodes.Status200OK)]
    public List<Item> GetAll()
    {
        return itemService.GetAll();
    }
}