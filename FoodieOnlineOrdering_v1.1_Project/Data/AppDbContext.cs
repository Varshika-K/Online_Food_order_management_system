using FoodieOnlineOrdering.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodieOnlineOrdering.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();

        modelBuilder.Entity<Food>().Property(x => x.Price).HasPrecision(12, 2);

        modelBuilder.Entity<Order>().Property(x => x.TotalAmount).HasPrecision(12, 2);
        modelBuilder.Entity<Order>().HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>().Property(x => x.UnitPrice).HasPrecision(12, 2);
        modelBuilder.Entity<OrderItem>().Property(x => x.LineTotal).HasPrecision(12, 2);

        modelBuilder.Entity<Payment>().Property(x => x.Amount).HasPrecision(12, 2);
        modelBuilder.Entity<Payment>().HasIndex(x => x.TransactionReference).IsUnique();
        modelBuilder.Entity<Payment>()
            .HasOne(x => x.Order)
            .WithOne()
            .HasForeignKey<Payment>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
