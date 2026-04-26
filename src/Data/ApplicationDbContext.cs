using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppleStoreWeb.Models;

namespace AppleStoreWeb.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Product entity.
        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.OriginalPrice).HasPrecision(18, 2);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Brand);
            entity.HasIndex(e => e.ProductType);
            entity.HasIndex(e => e.Featured);
        });

        // Configure Order entity
        builder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configure OrderItem entity
        builder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.Items)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.OrderItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).IsRequired().HasDefaultValue("customer");
        });

        // Seed data
        SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
        // Seed Apple technology products
        var products = new[]
        {
            new Product
            {
                Id = "iphone-15-pro",
                Name = "iPhone 15 Pro",
                Brand = "Apple",
                Price = 28990000,
                OriginalPrice = 30990000,
                Description = "iPhone 15 Pro với chip A17 Pro, camera 48MP và khung titan cao cấp",
                Images = "[\"https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=500&h=500&fit=crop&crop=center\"]",
                StorageOptions = "[\"128GB\", \"256GB\", \"512GB\", \"1TB\"]",
                Colors = "[\"Natural Titanium\", \"Blue Titanium\", \"White Titanium\", \"Black Titanium\"]",
                Category = "iPhone",
                ProductType = "iPhone 15 Pro",
                Processor = "A17 Pro",
                DisplaySize = "6.1 inch",
                KeyFeatures = "[\"Camera 48MP\", \"Action Button\", \"USB-C\", \"Titanium Design\"]",
                Featured = true,
                InStock = true,
                Rating = 4.8,
                Reviews = 256,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = "macbook-air-m3",
                Name = "MacBook Air 13-inch M3",
                Brand = "Apple",
                Price = 32990000,
                Description = "MacBook Air 13-inch với chip M3, thiết kế siêu mỏng và pin 18 giờ",
                Images = "[\"https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=500&h=500&fit=crop&crop=center\"]",
                StorageOptions = "[\"256GB\", \"512GB\", \"1TB\", \"2TB\"]",
                Colors = "[\"Midnight\", \"Starlight\", \"Space Gray\", \"Silver\"]",
                Category = "Mac",
                ProductType = "MacBook Air",
                Processor = "Apple M3",
                DisplaySize = "13.6 inch",
                KeyFeatures = "[\"Chip M3\", \"Pin 18 giờ\", \"Liquid Retina Display\", \"MagSafe\"]",
                Featured = true,
                InStock = true,
                Rating = 4.7,
                Reviews = 189,
                CreatedAt = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = "ipad-pro-12-9",
                Name = "iPad Pro 12.9-inch",
                Brand = "Apple",
                Price = 31990000,
                Description = "iPad Pro 12.9-inch với chip M2, màn hình Liquid Retina XDR",
                Images = "[\"https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=500&h=500&fit=crop&crop=center\"]",
                StorageOptions = "[\"128GB\", \"256GB\", \"512GB\", \"1TB\", \"2TB\"]",
                Colors = "[\"Space Gray\", \"Silver\"]",
                Category = "iPad",
                ProductType = "iPad Pro",
                Processor = "Apple M2",
                DisplaySize = "12.9 inch",
                KeyFeatures = "[\"Liquid Retina XDR\", \"Apple Pencil 2\", \"Magic Keyboard\", \"5G\"]",
                Featured = true,
                InStock = true,
                Rating = 4.6,
                Reviews = 142,
                CreatedAt = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        builder.Entity<Product>().HasData(products);
    }
}
