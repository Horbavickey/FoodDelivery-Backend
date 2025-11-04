using Microsoft.EntityFrameworkCore;
using FoodDelivery.Domain.Entities;

namespace FoodDelivery.Infrastructure;

public class FoodDeliveryDbContext : DbContext
{
    public FoodDeliveryDbContext(DbContextOptions<FoodDeliveryDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<DishCategory> DishCategories => Set<DishCategory>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<DishRating> DishRatings => Set<DishRating>();
    public DbSet<ServerSettings> ServerSettings => Set<ServerSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite PK for the join table
        modelBuilder.Entity<DishCategory>()
            .HasKey(x => new { x.DishId, x.CategoryId });

        // (Optional but recommended) unique email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();
    }
}