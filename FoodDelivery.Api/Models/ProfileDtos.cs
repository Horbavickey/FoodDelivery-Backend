namespace FoodDelivery.Api.Models;

public class ProfileResponse
{
    public string Email { get; set; } = "";
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
}

public class UpdateProfileRequest
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
}