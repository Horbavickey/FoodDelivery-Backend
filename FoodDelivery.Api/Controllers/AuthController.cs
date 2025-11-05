using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using FoodDelivery.Api.Models;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly FoodDeliveryDbContext _db;
    private readonly IConfiguration _cfg;
    public AuthController(FoodDeliveryDbContext db, IConfiguration cfg) { _db = db; _cfg = cfg; }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Email and password are required.");

        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            return BadRequest("Email already used.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { token = Token(user) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized();

        return Ok(new { token = Token(user) });
    }

    private string Token(User user)
    {
        var jwt = _cfg.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        var t = new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresMinutes"]!)),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(t);
    }
}