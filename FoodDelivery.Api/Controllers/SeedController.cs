using FoodDelivery.Domain.Entities;
using FoodDelivery.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("seed")]
public class SeedController : ControllerBase
{
    private readonly FoodDeliveryDbContext _db;
    public SeedController(FoodDeliveryDbContext db) => _db = db;

    [HttpPost]
    public IActionResult Seed()
    {
        if (_db.Dishes.Any()) return Ok("Already seeded");

        var catPizza = new Category { Id = Guid.NewGuid(), Name = "Pizza" };
        var catVegan = new Category { Id = Guid.NewGuid(), Name = "Vegan" };

        var d1 = new Dish { Id = Guid.NewGuid(), Name = "Margherita", Price = 8.50m, IsVegetarian = true };
        var d2 = new Dish { Id = Guid.NewGuid(), Name = "Pepperoni",  Price = 9.90m, IsVegetarian = false };

        _db.Categories.AddRange(catPizza, catVegan);
        _db.Dishes.AddRange(d1, d2);
        _db.DishCategories.AddRange(
            new DishCategory { DishId = d1.Id, CategoryId = catPizza.Id },
            new DishCategory { DishId = d1.Id, CategoryId = catVegan.Id },
            new DishCategory { DishId = d2.Id, CategoryId = catPizza.Id }
        );

        _db.SaveChanges();
        return Ok("Seeded");
    }
}