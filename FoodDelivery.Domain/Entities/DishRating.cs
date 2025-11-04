namespace FoodDelivery.Domain.Entities;

public class DishRating
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DishId { get; set; }
    public int Stars { get; set; }  // 1..5
    public DateTime CreatedAt { get; set; }
}