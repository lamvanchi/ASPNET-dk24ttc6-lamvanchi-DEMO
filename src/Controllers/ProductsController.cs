using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleStoreWeb.Data;
using AppleStoreWeb.Models;
using System.Text.Json;

namespace AppleStoreWeb.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ===================== INDEX =====================
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);
    }

    // ===================== DETAILS =====================
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    // ===================== CREATE =====================

    // GET
    public IActionResult Create()
    {
        return View();
    }

    // POST (UPLOAD + JSON)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Product model,
        List<IFormFile> ImageFiles,
        List<string> StorageOptions,
        List<string> Colors)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.Id = Guid.NewGuid().ToString();
        model.CreatedAt = DateTime.UtcNow;

        // ===== UPLOAD ẢNH =====
        var imagePaths = new List<string>();

        if (ImageFiles != null && ImageFiles.Count > 0)
        {
            var uploadPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/images"
            );

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            foreach (var file in ImageFiles)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imagePaths.Add("/images/" + fileName);
                }
            }
        }

        model.Images = JsonSerializer.Serialize(imagePaths);

        // ===== STORAGE =====
        model.StorageOptions = JsonSerializer.Serialize(StorageOptions ?? new List<string>());

        // ===== COLORS =====
        model.Colors = JsonSerializer.Serialize(Colors ?? new List<string>());

        _context.Products.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // ===================== DELETE =====================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        // 🔥 FIX LỖI: XÓA ORDERITEM TRƯỚC
        var orderItems = _context.OrderItems
            .Where(x => x.ProductId == id);

        _context.OrderItems.RemoveRange(orderItems);

        // ===== XÓA ẢNH =====
        if (!string.IsNullOrEmpty(product.Images))
        {
            try
            {
                var images = JsonSerializer.Deserialize<string[]>(product.Images);

                if (images != null)
                {
                    foreach (var img in images)
                    {
                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            img.TrimStart('/')
                        );

                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }
            }
            catch
            {
                // bỏ qua lỗi JSON
            }
        }

        // ===== XÓA PRODUCT =====
        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // ===================== SEARCH (OPTIONAL) =====================
    [HttpGet]
    public async Task<IActionResult> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var products = await _context.Products
            .Where(p => p.Name.Contains(term) || p.Brand.Contains(term))
            .Take(10)
            .Select(p => new
            {
                id = p.Id,
                name = p.Name,
                brand = p.Brand,
                price = p.Price,
                image = GetFirstImage(p.Images)
            })
            .ToListAsync();

        return Json(products);
    }

    // ===================== HELPER =====================
    private string GetFirstImage(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return "/images/no-image.png";

        try
        {
            var arr = JsonSerializer.Deserialize<string[]>(json);
            return arr?.FirstOrDefault() ?? "/images/no-image.png";
        }
        catch
        {
            return "/images/no-image.png";
        }
    }
}