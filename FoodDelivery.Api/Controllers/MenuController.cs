using FoodDelivery.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("menu")]
public class MenuController : ControllerBase
{
    private readonly FoodDeliveryDbContext _db;
    public MenuController(FoodDeliveryDbContext db) => _db = db;

    // GET /menu?categories=GUID,GUID&vegetarianOnly=true&sort=price&order=desc
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? categories, [FromQuery] bool vegetarianOnly = false,
                                         [FromQuery] string? sort = "name", [FromQuery] string? order = "asc")
    {
        var q = _db.Dishes.AsQueryable();

        if (vegetarianOnly) q = q.Where(d => d.IsVegetarian);

        if (!string.IsNullOrWhiteSpace(categories))
        {
            var ids = categories.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                .Select(x => Guid.TryParse(x, out var g) ? g : (Guid?)null)
                                .Where(g => g.HasValue).Select(g => g!.Value).ToList();

            if (ids.Count > 0)
            {
                q = from d in _db.Dishes
                    join dc in _db.DishCategories on d.Id equals dc.DishId
                    where ids.Contains(dc.CategoryId)
                    select d;
                q = q.Distinct();
            }
        }

        q = (sort?.ToLower(), order?.ToLower()) switch
        {
            ("price", "desc") => q.OrderByDescending(d => d.Price),
            ("price", _)      => q.OrderBy(d => d.Price),
            ("name", "desc")  => q.OrderByDescending(d => d.Name),
            ("name", _)       => q.OrderBy(d => d.Name),
            _                 => q.OrderBy(d => d.Name)
        };

        var list = await q.Select(d => new { d.Id, d.Name, d.Price, d.IsVegetarian }).ToListAsync();
        return Ok(list);
    }
}