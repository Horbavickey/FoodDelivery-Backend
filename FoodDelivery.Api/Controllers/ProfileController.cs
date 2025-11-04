using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodDelivery.Api.Models;
using FoodDelivery.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly FoodDeliveryDbContext _db;
    public ProfileController(FoodDeliveryDbContext db) => _db = db;

    private Guid CurrentUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.Parse(sub!);
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
        var id = CurrentUserId();
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(req.Phone))
        {
            // +7 (xxx) xxx-xx-xx-xx
            var ok = Regex.IsMatch(req.Phone, @"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}-\d{2}$");
            if (!ok) return BadRequest("Phone must be in format +7 (xxx) xxx-xx-xx-xx");
            u.Phone = req.Phone;
        }

        u.FullName = req.FullName ?? u.FullName;
        u.BirthDate = req.BirthDate ?? u.BirthDate;
        u.Address = req.Address ?? u.Address;

        await _db.SaveChangesAsync();
        return NoContent();
    }
}