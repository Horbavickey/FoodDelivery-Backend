namespace FoodDelivery.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";

    // profile
    public string? FullName { get; set; }
    public string? Phone { get; set; }        // format: +7 (xxx) xxx-xx-xx-xx
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
}