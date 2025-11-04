namespace FoodDelivery.Domain.Entities;

public class Dish
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public bool IsVegetarian { get; set; }
}