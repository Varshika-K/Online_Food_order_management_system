using FoodieOnlineOrdering.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodieOnlineOrdering.Controllers;

public class OrdersController : Controller
{
    private readonly AppDbContext _db;

    public OrdersController(AppDbContext db)
    {
        _db = db;
    }


    // ==========================================
    // CUSTOMER ORDER HISTORY
    // ==========================================

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var orders = await _db.Orders
            .Include(x => x.Items)
            .Where(x => x.UserId == userId.Value)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(orders);
    }


    // ==========================================
    // CUSTOMER ORDER INVOICE
    // ==========================================

    public async Task<IActionResult> Invoice(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var order = await _db.Orders
            .Include(x => x.Items)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userId.Value);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }


    // ==========================================
    // CUSTOMER ORDER STATUS NOTIFICATION
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> StatusUpdates()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        var role = HttpContext.Session.GetString("Role");


        // Customer must be logged in.
        // Admin should not receive customer notifications.
        if (userId == null || role == "Admin")
        {
            return Unauthorized();
        }


        // Get only this customer's orders.
        var orders = await _db.Orders
            .Where(o => o.UserId == userId.Value)
            .Select(o => new
            {
                id = o.Id,
                status = o.Status
            })
            .ToListAsync();


        return Json(orders);
    }
}