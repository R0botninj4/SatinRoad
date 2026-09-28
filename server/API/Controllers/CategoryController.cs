using System.ComponentModel.DataAnnotations;
using Infra;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController(CategoryService categoryService)
    : ControllerBase
{
    [HttpGet]
    public List<Category> GetAll()
    {
        return categoryService.GetAll();
    }
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Category> GetById(string id)
    {
        var category = categoryService.GetById(id);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost]
    public ActionResult<Category> Create(CreateCategoryRequestDto dto)
    {
        try
        {
            var category = categoryService.Create(dto);

            return Created($"/api/categories/{category.Id}", category);
        }
        catch (ValidationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}