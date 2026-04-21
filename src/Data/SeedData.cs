using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppleStoreWeb.Models;

namespace AppleStoreWeb.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Create roles if they don't exist
        string[] roles = { "admin", "customer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create admin user
        var adminEmail = "admin@applestore.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Admin Apple Store",
                Phone = "0123456789",
                Address = "123 Admin Street",
                Role = "admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "AppleAdmin@2025");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "admin");
            }
        }
        else
        {
            adminUser.Role = "admin";
            await userManager.UpdateAsync(adminUser);
            if (!await userManager.IsInRoleAsync(adminUser, "admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "admin");
            }
        }

        // Create a sample customer user
        var customerEmail = "customer@example.com";
        var customerUser = await userManager.FindByEmailAsync(customerEmail);
        
        if (customerUser == null)
        {
            customerUser = new ApplicationUser
            {
                UserName = customerEmail,
                Email = customerEmail,
                Name = "Khách hàng mẫu",
                Phone = "0987654321",
                Address = "456 Đường XYZ, Quận 2, TP.HCM",
                Role = "customer",
                EmailConfirmed = true
            };

            var customerResult = await userManager.CreateAsync(customerUser, "Customer@123456");
            if (customerResult.Succeeded)
            {
                await userManager.AddToRoleAsync(customerUser, "customer");
            }
        }

        // Create additional sample customers
        await CreateSampleCustomersAsync(userManager);

        // Create sample orders
        await CreateSampleOrdersAsync(serviceProvider);
    }

    private static async Task CreateSampleCustomersAsync(UserManager<ApplicationUser> userManager)
    {
        var sampleCustomers = new[]
        {
            new { Email = "nguyenvana@gmail.com", Name = "Nguyễn Văn A", Phone = "0901234567", Address = "123 Lê Lợi, Q1, TP.HCM" },
            new { Email = "tranthib@gmail.com", Name = "Trần Thị B", Phone = "0912345678", Address = "456 Nguyễn Huệ, Q1, TP.HCM" },
            new { Email = "lequangc@gmail.com", Name = "Lê Quang C", Phone = "0923456789", Address = "789 Đồng Khởi, Q1, TP.HCM" },
            new { Email = "phamthid@gmail.com", Name = "Phạm Thị D", Phone = "0934567890", Address = "321 Hai Bà Trưng, Q3, TP.HCM" },
            new { Email = "hoangvane@gmail.com", Name = "Hoàng Văn E", Phone = "0945678901", Address = "654 Cách Mạng Tháng 8, Q10, TP.HCM" }
        };

        foreach (var customer in sampleCustomers)
        {
            var existingUser = await userManager.FindByEmailAsync(customer.Email);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = customer.Email,
                    Email = customer.Email,
                    Name = customer.Name,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    Role = "customer",
                    EmailConfirmed = true
                };

                var userResult = await userManager.CreateAsync(user, "Customer@123456");
                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "customer");
                }
            }
        }
    }

    private static async Task CreateSampleOrdersAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Check if orders already exist
        if (await context.Orders.AnyAsync())
        {
            return; // Orders already seeded
        }

        // Get sample customers
        var customers = await userManager.Users.Where(u => u.Role == "customer").ToListAsync();
        if (!customers.Any())
        {
            return; // No customers to create orders for
        }

        // Get sample products
        var products = await context.Products.Take(5).ToListAsync();
        if (!products.Any())
        {
            return; // No products to create orders for
        }

        var random = new Random();
        var statuses = new[] { "pending", "confirmed", "shipping", "delivered", "cancelled" };
        var paymentMethods = new[] { "COD", "Bank", "Card" };

        // Create sample orders
        var sampleOrders = new List<Order>();

        for (int i = 1; i <= 15; i++)
        {
            var customer = customers[random.Next(customers.Count)];
            var orderDate = DateTime.UtcNow.AddDays(-random.Next(30)); // Orders from last 30 days

            var order = new Order
            {
                Id = $"ORD{i:D3}",
                UserId = customer.Id,
                UserName = customer.Name,
                Status = statuses[random.Next(statuses.Length)],
                PaymentMethod = paymentMethods[random.Next(paymentMethods.Length)],
                CreatedAt = orderDate,
                ShippingAddress = customer.Address ?? "123 Đường ABC, Quận 1, TP.HCM",
                Total = 0 // Will be calculated after adding items
            };

            // Add 1-3 random products to each order
            var itemCount = random.Next(1, 4);
            var orderItems = new List<OrderItem>();
            decimal total = 0;

            for (int j = 0; j < itemCount; j++)
            {
                var product = products[random.Next(products.Count)];
                var quantity = random.Next(1, 3);
                var storageOptions = new[] { "64GB", "128GB", "256GB", "512GB", "1TB" };
                var colors = new[] { "Đen", "Trắng", "Xám", "Đỏ", "Xanh" };

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = quantity,
                    StorageOption = storageOptions[random.Next(storageOptions.Length)],
                    Color = colors[random.Next(colors.Length)],
                    UnitPrice = product.Price
                };

                orderItems.Add(orderItem);
                total += product.Price * quantity;
            }

            order.Total = total;
            sampleOrders.Add(order);

            context.Orders.Add(order);
            context.OrderItems.AddRange(orderItems);
        }

        await context.SaveChangesAsync();
    }
}
