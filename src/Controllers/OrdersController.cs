using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleStoreWeb.Data;
using AppleStoreWeb.Models;
using AppleStoreWeb.ViewModels;
using System.Text.Json;

namespace AppleStoreWeb.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private const string CartSessionKey = "ShoppingCart";

    public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ================= LIST ORDERS =================
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var orders = await _context.Orders
            .Where(o => o.UserId == user.Id)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    // ================= ORDER DETAILS =================
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

        if (order == null) return NotFound();

        return View(order);
    }

    // ================= CHECKOUT GET =================
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = GetCart();
        if (!cart.Any())
        {
            TempData["Error"] = "Giỏ hàng trống";
            return RedirectToAction("Index", "Cart");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var model = new CheckoutViewModel
        {
            Name = user.Name,
            Email = user.Email ?? "",
            Phone = user.Phone ?? "",
            Address = user.Address ?? "",
            CartItems = cart,
            Total = cart.Sum(x => x.Price * x.Quantity)
        };

        return View(model);
    }

    // ================= CHECKOUT POST (FIXED FULL) =================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var cart = GetCart();

        if (!cart.Any())
        {
            TempData["Error"] = "Giỏ hàng trống";
            return RedirectToAction("Index", "Cart");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            model.CartItems = cart;
            model.Total = cart.Sum(x => x.Price * x.Quantity);
            return View(model);
        }

        try
        {
            // ================= CREATE ORDER =================
            var order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                UserName = user.Name,
                Total = cart.Sum(x => x.Price * x.Quantity),
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                ShippingAddress = model.Address,
                PaymentMethod = model.PaymentMethod
            };

            _context.Orders.Add(order);

            // ================= ORDER ITEMS =================
            foreach (var item in cart)
            {
                if (string.IsNullOrEmpty(item.ProductId) || item.Quantity <= 0)
                    continue;

                // CHECK PRODUCT EXISTS (QUAN TRỌNG)
                var productExists = await _context.Products
                    .AnyAsync(p => p.Id == item.ProductId);

                if (!productExists)
                    continue;

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    StorageOption = item.StorageOption ?? "",
                    Color = item.Color ?? "",
                    UnitPrice = item.Price
                };

                _context.OrderItems.Add(orderItem);
            }

            // ================= SAVE =================
            await _context.SaveChangesAsync();

            // Clear cart
            HttpContext.Session.Remove(CartSessionKey);

            TempData["Success"] = "Đặt hàng thành công! Mã: " + order.Id;

            return RedirectToAction("Details", new { id = order.Id });
        }
        catch (Exception ex)
        {
            var error = ex.InnerException?.Message ?? ex.Message;
            return Content("Lỗi đặt hàng: " + error);
        }
    }

    // ================= GET CART =================
    private List<CartItem> GetCart()
    {
        var cartJson = HttpContext.Session.GetString(CartSessionKey);

        if (string.IsNullOrEmpty(cartJson))
            return new List<CartItem>();

        try
        {
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson)
                   ?? new List<CartItem>();
        }
        catch
        {
            return new List<CartItem>();
        }
    }
}