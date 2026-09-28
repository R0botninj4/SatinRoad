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