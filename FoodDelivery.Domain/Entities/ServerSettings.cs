namespace FoodDelivery.Domain.Entities;

public class ServerSettings
{
    public Guid Id { get; set; }
    public int DeliveryLeadMinutes { get; set; }
}