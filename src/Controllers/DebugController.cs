using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleStoreWeb.Data;
using AppleStoreWeb.Models;

namespace AppleStoreWeb.Controllers;

public class DebugController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DebugController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
     
    public async Task<IActionResult> CheckAdmin()
    {
        var adminEmail = "admin@appleshop.com";
        var admin = await _userManager.FindByEmailAsync(adminEmail);
        
        var info = new
        {
            AdminExists = admin != null,
            AdminRole = admin?.Role,
            AdminName = admin?.Name,
            AdminEmail = admin?.Email,
            IsInAdminRole = admin != null ? await _userManager.IsInRoleAsync(admin, "admin") : false,
            TotalUsers = await _context.Users.CountAsync(),
            AllUsers = await _context.Users.Select(u => new { u.Email, u.Role, u.Name }).ToListAsync()
        };

        return Json(info);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdmin()
    {
        try
        {
            await AppleStoreWeb.Data.SeedData.InitializeAsync(HttpContext.RequestServices);
            return Json(new { success = true, message = "Admin account created successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}