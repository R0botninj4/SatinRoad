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
}