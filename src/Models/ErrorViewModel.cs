using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AppleStoreWeb.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(15)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Required]
    public string Role { get; set; } = "customer"; // customer or admin.

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Product
{
    [Key]
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? OriginalPrice { get; set; }

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public string Images { get; set; } = string.Empty; // JSON array of image URLs

    public string StorageOptions { get; set; } = string.Empty; // JSON array of storage options (64GB, 128GB, 256GB, etc.)

    public string Colors { get; set; } = string.Empty; // JSON array of colors

    [Required]
    [StringLength(30)]
    public string Category { get; set; } = string.Empty; // iPhone, iPad, Mac, Apple Watch, AirPods, Accessories

    [Required]
    [StringLength(50)]
    public string ProductType { get; set; } = string.Empty; // iPhone 15 Pro, MacBook Air, iPad Pro, Apple Watch Series 9, etc.

    [StringLength(100)]
    public string Processor { get; set; } = string.Empty; // A17 Pro, M3, M3 Pro, S9, etc.

    [StringLength(100)]
    public string DisplaySize { get; set; } = string.Empty; // 6.1", 13.3", 12.9", 45mm, etc.

    [StringLength(200)]
    public string KeyFeatures { get; set; } = string.Empty; // JSON array of key features

    public bool Featured { get; set; }

    public bool InStock { get; set; } = true;

    [Range(0, 5)]
    public double Rating { get; set; }

    public int Reviews { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class Order
{
    [Key]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "pending"; // pending, processing, shipped, delivered, cancelled

    public DateTime CreatedAt { get; set; }

    [Required]
    [StringLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string OrderId { get; set; } = string.Empty;

    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [Required]
    [StringLength(20)]
    public string StorageOption { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Color { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;
}

public class CartItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductBrand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string StorageOption { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class FilterState
{
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public decimal MinPrice { get; set; } = 0;
    public decimal MaxPrice { get; set; } = 50000000; // Updated for Apple product price range
    public string StorageOption { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Search { get; set; } = string.Empty;
}

public class WebsiteStats
{
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TodayOrders { get; set; }
    public decimal TodayRevenue { get; set; }
    public int OnlineUsers { get; set; }
    public double ConversionRate { get; set; }
}
