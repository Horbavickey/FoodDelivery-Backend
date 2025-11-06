using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("ratings")]
[Authorize]
public class RatingsController : ControllerBase
{
    private readonly FoodDeliveryDbContext _db;
    public RatingsController(FoodDeliveryDbContext db) => _db = db;

    private Guid CurrentUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                 User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var guid))
            throw new InvalidOperationException("User id claim not found or invalid.");

        return guid;
    }

    public class RateRequest
    {
        public Guid DishId { get; set; }
        public int Stars { get; set; } // 1..5
    }

    [HttpPost]
    public async Task<IActionResult> Rate([FromBody] RateRequest req)
    {
        if (req.Stars < 1 || req.Stars > 5) return BadRequest("Stars must be 1..5.");

        var dishExists = await _db.Dishes.AsNoTracking().AnyAsync(d => d.Id == req.DishId);
        if (!dishExists) return NotFound("Dish not found.");

        var rating = new DishRating
        {
            Id = Guid.NewGuid(),
            UserId = CurrentUserId(),
            DishId = req.DishId,
            Stars = req.Stars,
            CreatedAt = DateTime.UtcNow
        };

        _db.DishRatings.Add(rating);
        await _db.SaveChangesAsync();

        return Created($"/ratings/{rating.Id}", new { rating.Id, rating.Stars, rating.DishId });
    }
}