using FoodieOnlineOrdering.Data;
using FoodieOnlineOrdering.Models;
using FoodieOnlineOrdering.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FoodieOnlineOrdering.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public AdminController(AppDbContext db) => _db = db;

    private bool IsAdmin => HttpContext.Session.GetString("Role") == "Admin";

    private IActionResult Denied()
        => RedirectToAction("Login", "Account", new { returnUrl = "/Admin/Dashboard" });

    public async Task<IActionResult> Dashboard()
    {
        if (!IsAdmin) return Denied();

        var vm = new AdminDashboardViewModel
        {
            UserCount = await _db.Users.CountAsync(),
            FoodCount = await _db.Foods.CountAsync(),
            OrderCount = await _db.Orders.CountAsync(),
            Revenue = await _db.Orders.Where(x => x.Status != "Cancelled").Select(x => (decimal?)x.TotalAmount).SumAsync() ?? 0,
            RecentOrders = await _db.Orders.Include(x => x.User).OrderByDescending(x => x.CreatedAt).Take(8).ToListAsync()
        };
        return View(vm);
    }

    public async Task<IActionResult> Foods()
    {
        if (!IsAdmin) return Denied();
        return View(await _db.Foods.OrderByDescending(x => x.Id).ToListAsync());
    }

    [HttpGet]
    public IActionResult CreateFood()
    {
        if (!IsAdmin) return Denied();
        return View(new Food());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFood(Food model)
    {
        if (!IsAdmin) return Denied();
        if (!ModelState.IsValid) return View(model);
        _db.Foods.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Food created successfully.";
        return RedirectToAction(nameof(Foods));
    }

    [HttpGet]
    public async Task<IActionResult> EditFood(int id)
    {
        if (!IsAdmin) return Denied();
        var food = await _db.Foods.FindAsync(id);
        return food == null ? NotFound() : View(food);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFood(Food model)
    {
        if (!IsAdmin) return Denied();
        if (!ModelState.IsValid) return View(model);
        _db.Foods.Update(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Food updated successfully.";
        return RedirectToAction(nameof(Foods));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFood(int id)
    {
        if (!IsAdmin) return Denied();
        var food = await _db.Foods.FindAsync(id);
        if (food != null)
        {
            var usedInOrders = await _db.OrderItems.AnyAsync(x => x.FoodId == id);
            if (usedInOrders)
            {
                food.IsAvailable = false;
                await _db.SaveChangesAsync();
                TempData["Success"] = "Food is already part of an order, so it was safely disabled instead of deleting its history.";
            }
            else
            {
                _db.Foods.Remove(food);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Food deleted successfully.";
            }
        }
        return RedirectToAction(nameof(Foods));
    }

    public async Task<IActionResult> Orders()
    {
        if (!IsAdmin) return Denied();
        return View(await _db.Orders.Include(x => x.User).Include(x => x.Items).OrderByDescending(x => x.CreatedAt).ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int id, string status)
    {
        if (!IsAdmin) return Denied();
        var order = await _db.Orders.FindAsync(id);
        if (order != null)
        {
            order.Status = status;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Order #{id} status updated.";
        }
        return RedirectToAction(nameof(Orders));
    }

    public async Task<IActionResult> Users()
    {
        if (!IsAdmin) return Denied();
        return View(await _db.Users.OrderByDescending(x => x.Id).ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(int id, string role)
    {
        if (!IsAdmin) return Denied();
        if (role is not ("Admin" or "Customer")) return BadRequest();

        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            user.Role = role;
            await _db.SaveChangesAsync();
            TempData["Success"] = "User role updated.";
        }
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Reports()
    {
        if (!IsAdmin) return Denied();
        var orders = await _db.Orders.Include(x => x.User).Include(x => x.Items)
            .OrderByDescending(x => x.CreatedAt).ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> ExportCsv()
    {
        if (!IsAdmin) return Denied();

        var orders = await _db.Orders.Include(x => x.User).Include(x => x.Items).OrderByDescending(x => x.CreatedAt).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("OrderId,Customer,Status,PaymentMethod,TotalAmount,CreatedAt,DeliveryAddress");

        foreach (var o in orders)
        {
            static string Csv(string? value) => "\"" + (value ?? "").Replace("\"", "\"\"") + "\"";
            sb.AppendLine($"{o.Id},{Csv(o.User?.Name)},{Csv(o.Status)},{Csv(o.PaymentMethod)},{o.TotalAmount:F2},{o.CreatedAt:yyyy-MM-dd HH:mm},{Csv(o.DeliveryAddress)}");
        }

        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"foodie-orders-{DateTime.Now:yyyyMMddHHmm}.csv");
    }
}
