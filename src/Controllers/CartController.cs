using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleStoreWeb.Data;
using AppleStoreWeb.Models;
using System.Text.Json;

namespace AppleStoreWeb.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private const string CartSessionKey = "ShoppingCart";

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================= VIEW CART =================
    public IActionResult Index()
    {
        var cart = GetCart();
        return View(cart);
    }

    // ================= ADD TO CART =================
    [HttpPost]
    public async Task<IActionResult> AddToCart(string productId, string storageOption, string color, int quantity = 1)
    {
        var product = await _context.Products.FindAsync(productId);

        if (product == null)
            return Json(new { success = false, message = "Sản phẩm không tồn tại" });

        var cart = GetCart();

        var existingItem = cart.FirstOrDefault(c =>
            c.ProductId == productId &&
            c.StorageOption == storageOption &&
            c.Color == color);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var firstImage = GetFirstImage(product.Images);

            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductBrand = product.Brand,
                Price = product.Price,
                Image = firstImage,
                Quantity = quantity,
                StorageOption = storageOption,
                Color = color
            });
        }

        SaveCart(cart);

        return Json(new
        {
            success = true,
            message = "Đã thêm vào giỏ hàng",
            cartCount = cart.Sum(x => x.Quantity)
        });
    }

    // ================= UPDATE QUANTITY =================
    [HttpPost]
    public IActionResult UpdateQuantity(string productId, string storageOption, string color, int quantity)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(c =>
            c.ProductId == productId &&
            c.StorageOption == storageOption &&
            c.Color == color);

        if (item != null)
        {
            if (quantity <= 0)
                cart.Remove(item);
            else
                item.Quantity = quantity;

            SaveCart(cart);
        }

        return Json(new { success = true, cartCount = cart.Sum(x => x.Quantity) });
    }

    // ================= REMOVE ITEM =================
    [HttpPost]
    public IActionResult RemoveFromCart(string productId, string storageOption, string color)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(c =>
            c.ProductId == productId &&
            c.StorageOption == storageOption &&
            c.Color == color);

        if (item != null)
        {
            cart.Remove(item);
            SaveCart(cart);
        }

        return Json(new { success = true, cartCount = cart.Sum(x => x.Quantity) });
    }

    // ================= CLEAR CART =================
    [HttpPost]
    public IActionResult ClearCart()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return Json(new { success = true });
    }

    // ================= GET CART COUNT (FIXED) =================
    [HttpGet]
    public IActionResult GetCartCount()
    {
        var cart = GetCart();
        return Json(new { count = cart.Sum(x => x.Quantity) });
    }

    // ================= GET CART ITEMS =================
    [HttpGet]
    public IActionResult GetCartItems()
    {
        var cart = GetCart();

        return Json(new
        {
            items = cart,
            total = cart.Sum(x => x.Price * x.Quantity),
            count = cart.Sum(x => x.Quantity)
        });
    }

    // ================= HELPER =================
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

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetString(
            CartSessionKey,
            JsonSerializer.Serialize(cart)
        );
    }

    private string GetFirstImage(string imagesJson)
    {
        try
        {
            var images = JsonSerializer.Deserialize<string[]>(imagesJson);
            return images?.FirstOrDefault() ?? "/images/placeholder.jpg";
        }
        catch
        {
            return "/images/placeholder.jpg";
        }
    }
}