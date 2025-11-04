namespace FoodDelivery.Domain.Entities;

public class CartItem
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid DishId { get; set; }
    public int Quantity { get; set; }
}