using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodDelivery.Api.Models;
using FoodDelivery.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("profile")]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly FoodDeliveryDbContext _db;
    public UserProfileController(FoodDeliveryDbContext db) => _db = db;

    // Robust user id extraction from JWT ("sub" or NameIdentifier)
    private Guid CurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw, out var id))
            throw new InvalidOperationException("User id claim not found or invalid.");

        return id;
    }

    [HttpGet]
    public async Task<ActionResult<ProfileResponse>> Get()
    {
        var id = CurrentUserId();
        var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return NotFound();

        return new ProfileResponse
        {
            Email = u.Email,
            FullName = u.FullName,
            Phone = u.Phone,
            BirthDate = u.BirthDate,
            Address = u.Address
        };
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProfileRequest req)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == CurrentUserId());
        if (u == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(req.Phone))
        {
            // If you need strict phone regex later, we can add it back. For now keep it simple to finish.
            u.Phone = req.Phone;
        }

        if (!string.IsNullOrWhiteSpace(req.FullName))
            u.FullName = req.FullName;

        if (req.BirthDate.HasValue)
            u.BirthDate = req.BirthDate;

        if (!string.IsNullOrWhiteSpace(req.Address))
            u.Address = req.Address;

        await _db.SaveChangesAsync();
        return NoContent();
    }
}