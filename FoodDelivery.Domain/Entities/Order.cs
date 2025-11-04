namespace FoodDelivery.Domain.Entities;

public enum OrderStatus { Draft, InProcess, Delivered, Cancelled }

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime DeliveryAt { get; set; }
}