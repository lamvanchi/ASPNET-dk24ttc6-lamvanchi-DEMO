using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleStoreWeb.Data;
using AppleStoreWeb.Models;

namespace AppleStoreWeb.Controllers;
 
[Authorize]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != "admin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var today = DateTime.Today;
        var thisMonth = new DateTime(today.Year, today.Month, 1);
        var lastMonth = thisMonth.AddMonths(-1);

        var stats = new WebsiteStats
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalOrders = await _context.Orders.CountAsync(),
            TotalCustomers = await _context.Users.CountAsync(u => u.Role == "customer"),
            TotalRevenue = await _context.Orders.Where(o => o.Status == "delivered").SumAsync(o => o.Total),
            TodayOrders = await _context.Orders.CountAsync(o => o.CreatedAt.Date == today),
            TodayRevenue = await _context.Orders
                .Where(o => o.CreatedAt.Date == today && o.Status == "delivered")
                .SumAsync(o => o.Total),
            OnlineUsers = await CalculateOnlineUsers(),
            ConversionRate = (double)await CalculateConversionRate()
        };

        // Additional statistics for dashboard
        ViewBag.ThisMonthOrders = 156;
        ViewBag.LastMonthOrders = 142;
        ViewBag.PendingOrders = 8;
        ViewBag.ShippingOrders = 12;
        ViewBag.LowStockProducts = 3;
        ViewBag.FeaturedProducts = await _context.Products.CountAsync(p => p.Featured);

        // Simulate recent orders data
        ViewBag.RecentOrders = new[]
        {
            new { Id = "APL12345", UserName = "Nguyễn Văn A", CreatedAt = DateTime.Now.AddMinutes(-15), Total = 29990000m, Status = "pending" },
            new { Id = "APL67890", UserName = "Trần Thị B", CreatedAt = DateTime.Now.AddHours(-2), Total = 32990000m, Status = "confirmed" },
            new { Id = "APL11111", UserName = "Lê Quang C", CreatedAt = DateTime.Now.AddHours(-4), Total = 31990000m, Status = "shipping" },
            new { Id = "APL22222", UserName = "Phạm Thị D", CreatedAt = DateTime.Now.AddHours(-6), Total = 6990000m, Status = "delivered" },
            new { Id = "APL33333", UserName = "Hoàng Văn E", CreatedAt = DateTime.Now.AddHours(-8), Total = 24990000m, Status = "confirmed" }
        };

        // Simulate top products data
        ViewBag.TopProducts = new[]
        {
            new { ProductName = "iPhone 15 Pro", TotalSold = 45, Revenue = 130455000m },
            new { ProductName = "MacBook Air M3", TotalSold = 38, Revenue = 125362000m },
            new { ProductName = "iPad Pro 12.9-inch", TotalSold = 32, Revenue = 102368000m },
            new { ProductName = "AirPods Pro", TotalSold = 28, Revenue = 69720000m },
            new { ProductName = "Apple Watch Series 9", TotalSold = 25, Revenue = 62475000m }
        };

        return View(stats);
    }

    public async Task<IActionResult> Products()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != "admin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var products = await _context.Products.OrderBy(p => p.Name).ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Orders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != "admin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Customers()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != "admin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var customers = await _context.Users
            .Where(u => u.Role == "customer")
            .OrderBy(u => u.Name)
            .ToListAsync();

        return View(customers);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(string orderId, string status)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != "admin")
        {
            return Json(new { success = false, message = "Không có quyền truy cập" });
        }

        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
        }

        order.Status = status;
        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
    }

    [HttpPost]
    public async Task<IActionResult> ToggleProductStock(string productId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != "admin")
        {
            return Json(new { success = false, message = "Không có quyền truy cập" });
        }

        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
        }

        product.InStock = !product.InStock;
        await _context.SaveChangesAsync();

        return Json(new { success = true, inStock = product.InStock });
    }

    [HttpPost]
    public async Task<IActionResult> LockUser(string userId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser?.Role != "admin")
        {
            return Json(new { success = false, message = "Không có quyền truy cập" });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Json(new { success = false, message = "Không tìm thấy tài khoản" });
        }

        if (user.Role == "admin")
        {
            return Json(new { success = false, message = "Không thể khóa tài khoản quản trị viên" });
        }

        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UnlockUser(string userId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser?.Role != "admin")
        {
            return Json(new { success = false, message = "Không có quyền truy cập" });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Json(new { success = false, message = "Không tìm thấy tài khoản" });
        }

        await _userManager.SetLockoutEndDateAsync(user, null);
        return Json(new { success = true });
    }

    private async Task<int> CalculateOnlineUsers()
    {
        // In a real application, this would track active sessions
        // For demo purposes, we'll simulate based on recent orders
        var recentActivity = await _context.Orders
            .Where(o => o.CreatedAt >= DateTime.UtcNow.AddHours(-1))
            .Select(o => o.UserId)
            .Distinct()
            .CountAsync();

        return Math.Max(recentActivity, new Random().Next(8, 25));
    }

    private async Task<decimal> CalculateConversionRate()
    {
        var totalCustomers = await _context.Users.CountAsync(u => u.Role == "customer");
        var customersWithOrders = await _context.Orders
            .Select(o => o.UserId)
            .Distinct()
            .CountAsync();

        if (totalCustomers == 0) return 0;
        return Math.Round((decimal)customersWithOrders / totalCustomers * 100, 1);
    }
}
