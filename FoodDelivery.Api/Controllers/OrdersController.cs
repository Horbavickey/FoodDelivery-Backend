using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodDelivery.Domain.Entities;
using FoodDelivery.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly FoodDeliveryDbContext _db;
    public OrdersController(FoodDeliveryDbContext db) => _db = db;
    private Guid CurrentUserId() => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var userId = CurrentUserId();
        var lead = await _db.ServerSettings.AsNoTracking()
                     .Select(s => (int?)s.DeliveryLeadMinutes).FirstOrDefaultAsync() ?? 30;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.InProcess,
            DeliveryAt = DateTime.UtcNow.AddMinutes(lead)
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return Ok(new { order.Id, order.Status, order.DeliveryAt });
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var userId = CurrentUserId();
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        if (order == null) return NotFound();
        if (order.Status != OrderStatus.InProcess) return BadRequest("Only 'In Process' can be confirmed.");

        order.Status = OrderStatus.Delivered;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}