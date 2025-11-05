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
    private Guid CurrentUserId() => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    public record RateRequest(Guid DishId, int Stars);

    [HttpPost]
    public async Task<IActionResult> Rate([FromBody] RateRequest req)
    {
        if (req.Stars < 1 || req.Stars > 5) return BadRequest("Stars 1..5");

        var userId = CurrentUserId();

        // can rate only if ordered before (simple check via OrderItems)
        var ordered = await _db.OrderItems.AnyAsync(oi => oi.DishId == req.DishId && _db.Orders.Any(o => o.Id == oi.OrderId && o.UserId == userId));
        if (!ordered) return BadRequest("Rate only dishes you have ordered.");

        var rating = new DishRating { Id = Guid.NewGuid(), UserId = userId, DishId = req.DishId, Stars = req.Stars, CreatedAt = DateTime.UtcNow };
        _db.DishRatings.Add(rating);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}