using FoodieOnlineOrdering.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodieOnlineOrdering.Controllers;

public class OrdersController : Controller
{
    private readonly AppDbContext _db;
    public OrdersController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");

        var orders = await _db.Orders
            .Include(x => x.Items)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Invoice(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");

        var order = await _db.Orders
            .Include(x => x.Items)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        return order == null ? NotFound() : View(order);
    }
}
