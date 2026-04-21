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
     
    public async Task<IActionResult> Index(FilterState? filter)
    {
        filter ??= new FilterState();

        var query = _context.Products.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(filter.Category))
        {
            query = query.Where(p => p.Category == filter.Category);
        }

        if (!string.IsNullOrEmpty(filter.Brand))
        {
            query = query.Where(p => p.Brand == filter.Brand);
        }

        if (!string.IsNullOrEmpty(filter.ProductType))
        {
            query = query.Where(p => p.ProductType == filter.ProductType);
        }

        if (filter.MinPrice > 0 || filter.MaxPrice < 6000000)
        {
            query = query.Where(p => p.Price >= filter.MinPrice && p.Price <= filter.MaxPrice);
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(p => p.Name.Contains(filter.Search) || 
                                   p.Brand.Contains(filter.Search) || 
                                   p.Description.Contains(filter.Search));
        }

        if (!string.IsNullOrEmpty(filter.StorageOption))
        {
            query = query.Where(p => p.StorageOptions.Contains($"\"{filter.StorageOption}\""));
        }

        if (!string.IsNullOrEmpty(filter.Color))
        {
            query = query.Where(p => p.Colors.Contains($"\"{filter.Color}\""));
        }

        var products = await query.Where(p => p.InStock).ToListAsync();

        // Get filter options
        ViewBag.Categories = await _context.Products.Select(p => p.Category).Distinct().ToListAsync();
        ViewBag.Brands = await _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
        ViewBag.ProductTypes = await _context.Products.Select(p => p.ProductType).Distinct().ToListAsync();
        
        // Get all storage options and colors from all products
        var allStorageOptions = new List<string>();
        var allColors = new List<string>();

        var allProducts = await _context.Products.ToListAsync();
        foreach (var product in allProducts)
        {
            try
            {
                var storageOptions = JsonSerializer.Deserialize<string[]>(product.StorageOptions);
                var colors = JsonSerializer.Deserialize<string[]>(product.Colors);

                if (storageOptions != null) allStorageOptions.AddRange(storageOptions);
                if (colors != null) allColors.AddRange(colors);
            }
            catch
            {
                // Handle JSON parsing errors
            }
        }

        ViewBag.StorageOptions = allStorageOptions.Distinct().ToList();
        ViewBag.Colors = allColors.Distinct().ToList();
        ViewBag.Filter = filter;

        return View(products);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term)
    {
        if (string.IsNullOrEmpty(term))
        {
            return Json(new List<object>());
        }

        var products = await _context.Products
            .Where(p => p.Name.Contains(term) || p.Brand.Contains(term))
            .Take(10)
            .Select(p => new { 
                id = p.Id, 
                name = p.Name, 
                brand = p.Brand, 
                price = p.Price,
                image = GetFirstImage(p.Images)
            })
            .ToListAsync();

        return Json(products);
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
