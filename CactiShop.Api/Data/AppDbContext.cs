using CactiShop.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CactiShop.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<OrderPoint> OrderPoints => Set<OrderPoint>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderProducts> OrderProducts => Set<OrderProducts>();
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
    public DbSet<CartItems> CartItems => Set<CartItems>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItems>().HasKey(c => new { c.UserId, c.ProductId });
    }
}
