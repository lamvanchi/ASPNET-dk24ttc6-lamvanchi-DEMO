# Apple Shop - Hướng dẫn kỹ thuật

## Mục lục
1. Kiến trúc hệ thống  
2. Models và Database  
3. Controllers chi tiết  
4. Views và UI  
5. Authentication & Authorization  
6. Session Management  
7. API Documentation  
8. Performance & Optimization  

---

# Kiến trúc hệ thống

Pattern sử dụng:
- MVC Pattern - Model-View-Controller  
- Repository Pattern - Entity Framework  
- Dependency Injection - ASP.NET Core DI  
- Session State - Giỏ hàng  

Luồng xử lý request:
Request → Middleware → Controller → Service/Repository → Database  
                                 ↓  
Response ← View ← ViewModel ← Controller  

---

# Models và Database

## ApplicationUser
public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = "customer";
    public ICollection<Order> Orders { get; set; }
}

---

## Product
public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string Description { get; set; }
    public string Images { get; set; }
    public string StorageOptions { get; set; }
    public string Colors { get; set; }
    public string Category { get; set; }
    public string ProductType { get; set; }
    public string Processor { get; set; }
    public string DisplaySize { get; set; }
    public string KeyFeatures { get; set; }
    public bool Featured { get; set; }
    public bool InStock { get; set; }
    public double Rating { get; set; }
    public int Reviews { get; set; }
    public DateTime CreatedAt { get; set; }
}

---

## Order & OrderItem
public class Order
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ShippingAddress { get; set; }
    public string PaymentMethod { get; set; }
    public ApplicationUser User { get; set; }
    public ICollection<OrderItem> Items { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public string OrderId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public string StorageOption { get; set; }
    public string Color { get; set; }
    public decimal UnitPrice { get; set; }
    public Order Order { get; set; }
    public Product Product { get; set; }
}

---

## CartItem
public class CartItem
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductBrand { get; set; }
    public decimal Price { get; set; }
    public string Image { get; set; }
    public int Quantity { get; set; }
    public string StorageOption { get; set; }
    public string Color { get; set; }
}

---

# Controllers

## HomeController
public async Task<IActionResult> Index()
{
    var featuredProducts = await _context.Products
        .Where(p => p.Featured && p.InStock)
        .Take(8)
        .ToListAsync();

    return View(featuredProducts);
}

---

## ProductsController
- Lọc theo category  
- Search sản phẩm  
- Filter động  
- AJAX search  

---

## CartController
- Add / update / remove cart  
- Session storage  
- Tính tổng tiền  

---

## AdminController
- Quản lý sản phẩm  
- Quản lý đơn hàng  
- Check role admin  

---

# Views và UI

- Layout `_Layout.cshtml`  
- Bootstrap 5  
- Modal CRUD  
- AJAX update UI  

---

# Authentication & Authorization

- ASP.NET Identity  
- Role: admin / customer  
- Cookie authentication  

---

# Session Management

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

Dùng cho:
- Cart  
- Flash message  
- Filter tạm  

---

# API Documentation

## Add to cart
POST /Cart/AddToCart
{
  "productId": "iphone-15-pro",
  "storageOption": "256GB",
  "color": "Titan",
  "quantity": 1
}

## Get cart count
GET /Cart/GetCartCount
{ "count": 3 }

## Update order status
POST /Admin/UpdateOrderStatus
{
  "orderId": "ORD001",
  "status": "confirmed"
}

## Search product
GET /Products/Search?term=iphone

---

# Performance & Optimization

Backend:
- Async/Await  
- EF Core tối ưu query  
- Select field cần thiết  

Frontend:
- CDN Bootstrap  
- Lazy load image  
- AJAX update  
- Minify JS/CSS  

---

# Debug & Logging

- Console log  
- Debug log  
- Session check  
- Role check  

---

# Deployment

- HTTPS  
- Production DB  
- Custom error pages  
- Logging production  

---

# Security Checklist

- HTTPS  
- CSRF protection  
- SQL Injection safe (EF Core)  
- XSS protection  
- Authorization admin API  

---

Tài liệu kỹ thuật  
System: Apple Shop