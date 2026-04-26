using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleStoreWeb.Data;
using AppleStoreWeb.Models;
using System.Text;
using System.Globalization;

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

    // ================= DASHBOARD =================
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != "admin")
            return RedirectToAction("AccessDenied", "Account");

        var today = DateTime.Today;

        var stats = new WebsiteStats
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalOrders = await _context.Orders.CountAsync(),
            TotalCustomers = await _context.Users.CountAsync(u => u.Role == "customer"),

            TotalRevenue = await _context.Orders
                .Where(o => o.Status == "delivered")
                .SumAsync(o => (decimal?)o.Total) ?? 0,

            TodayOrders = await _context.Orders
                .CountAsync(o => o.CreatedAt.Date == today),

            TodayRevenue = await _context.Orders
                .Where(o => o.CreatedAt.Date == today && o.Status == "delivered")
                .SumAsync(o => (decimal?)o.Total) ?? 0
        };

        ViewBag.RecentOrders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new
            {
                o.Id,
                o.UserName,
                o.CreatedAt,
                o.Total,
                o.Status
            })
            .ToListAsync();

        return View(stats);
    }

    // ================= CUSTOMERS =================
    public async Task<IActionResult> Customers()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != "admin")
            return RedirectToAction("AccessDenied", "Account");

        var users = await _context.Users
            .OrderByDescending(u => u.UserName)
            .ToListAsync();

        return View(users);
    }

    // ================= PRODUCTS =================
    public async Task<IActionResult> Products()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != "admin")
            return RedirectToAction("AccessDenied", "Account");

        var products = await _context.Products
            .OrderByDescending(p => p.Name)
            .ToListAsync();

        return View(products);
    }

    // ================= ORDERS =================
    public async Task<IActionResult> Orders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.Role != "admin")
            return RedirectToAction("AccessDenied", "Account");

        var orders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    // ================= LOCK USER =================
    [HttpPost]
    public async Task<IActionResult> LockUser(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return Json(new { success = false });

        user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    // ================= UNLOCK USER =================
    [HttpPost]
    public async Task<IActionResult> UnlockUser(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return Json(new { success = false });

        user.LockoutEnd = null;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    // ================= EXPORT DATABASE =================
    [HttpGet]
    public async Task<IActionResult> ExportDatabase()
    {
        var sql = new StringBuilder();

        sql.AppendLine("SET NOCOUNT ON;");
        sql.AppendLine("BEGIN TRY");
        sql.AppendLine("BEGIN TRANSACTION;");
        sql.AppendLine("");

        sql.AppendLine("DELETE FROM OrderItems;");
        sql.AppendLine("DELETE FROM Orders;");
        sql.AppendLine("DELETE FROM Products;");
        sql.AppendLine("DELETE FROM Users;");
        sql.AppendLine("");

        // PRODUCTS
        var products = await _context.Products.ToListAsync();
        foreach (var p in products)
        {
            sql.AppendLine($@"
INSERT INTO Products (Id, Name, Price, Brand, Category, InStock)
VALUES (
{p.Id},
N'{Escape(p.Name)}',
{p.Price.ToString(CultureInfo.InvariantCulture)},
N'{Escape(p.Brand)}',
N'{Escape(p.Category)}',
{(p.InStock ? 1 : 0)}
);");
        }

        // ORDERS
        var orders = await _context.Orders.ToListAsync();
        foreach (var o in orders)
        {
            sql.AppendLine($@"
INSERT INTO Orders (Id, UserId, UserName, Total, Status, CreatedAt)
VALUES (
'{Escape(o.Id)}',
'{Escape(o.UserId)}',
N'{Escape(o.UserName)}',
{o.Total.ToString(CultureInfo.InvariantCulture)},
'{Escape(o.Status)}',
'{o.CreatedAt:yyyy-MM-dd HH:mm:ss}'
);");
        }

        // ORDER ITEMS
        var items = await _context.OrderItems.ToListAsync();
        foreach (var i in items)
        {
            sql.AppendLine($@"
INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity, UnitPrice)
VALUES (
{i.Id},
'{Escape(i.OrderId)}',
'{Escape(i.ProductId)}',
{i.Quantity},
{i.UnitPrice.ToString(CultureInfo.InvariantCulture)}
);");
        }

        // USERS (không export admin)
        var users = await _context.Users.Where(u => u.Role != "admin").ToListAsync();
        foreach (var u in users)
        {
            sql.AppendLine($@"
INSERT INTO Users (Id, UserName, Name, Role)
VALUES (
'{Escape(u.Id)}',
'{Escape(u.UserName)}',
N'{Escape(u.Name)}',
'{Escape(u.Role)}'
);");
        }

        sql.AppendLine("");
        sql.AppendLine("COMMIT;");
        sql.AppendLine("END TRY");
        sql.AppendLine("BEGIN CATCH");
        sql.AppendLine("ROLLBACK;");
        sql.AppendLine("PRINT ERROR_MESSAGE();");
        sql.AppendLine("END CATCH;");

        return File(
            Encoding.UTF8.GetBytes(sql.ToString()),
            "application/sql",
            "database-export.sql"
        );
    }

    // ================= ESCAPE =================
    private string Escape(string? input)
    {
        if (string.IsNullOrEmpty(input)) return "";

        return input
            .Replace("'", "''")
            .Replace("\n", " ")
            .Replace("\r", " ");
    }
}