using System.Text.Json;
using FoodieOnlineOrdering.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodieOnlineOrdering.Controllers;

public class CartController : Controller
{
    private readonly AppDbContext _db;
    private const string CartKey = "Cart";

    public CartController(AppDbContext db) => _db = db;

    private bool LoggedIn => HttpContext.Session.GetInt32("UserId") != null;

    private Dictionary<int, int> GetCart()
    {
        var json = HttpContext.Session.GetString(CartKey);
        return string.IsNullOrWhiteSpace(json)
            ? new Dictionary<int, int>()
            : JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new();
    }

    private void SaveCart(Dictionary<int, int> cart)
        => HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(cart));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id, int quantity = 1)
    {
        if (!LoggedIn)
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Menu") });

        var food = await _db.Foods.FindAsync(id);
        if (food == null || !food.IsAvailable)
        {
            TempData["Error"] = "This food is not available.";
            return RedirectToAction("Index", "Menu");
        }

        quantity = Math.Clamp(quantity, 1, 20);
        var cart = GetCart();
        cart[id] = cart.TryGetValue(id, out var old) ? old + quantity : quantity;
        SaveCart(cart);

        TempData["Success"] = "Product added successfully";
        return Redirect(Request.Headers.Referer.ToString());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int id, int quantity)
    {
        var cart = GetCart();
        if (quantity <= 0) cart.Remove(id);
        else cart[id] = Math.Min(quantity, 20);
        SaveCart(cart);
        TempData["Success"] = "Cart updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int id)
    {
        var cart = GetCart();
        cart.Remove(id);
        SaveCart(cart);
        TempData["Success"] = "Product removed from cart.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index()
    {
        var cart = GetCart();
        var ids = cart.Keys.ToList();
        var foods = await _db.Foods.Where(x => ids.Contains(x.Id)).ToListAsync();
        var items = foods.Select(f => new
        {
            Food = f,
            Quantity = cart.TryGetValue(f.Id, out var q) ? q : 0
        }).Where(x => x.Quantity > 0).ToList();

        ViewBag.Total = items.Sum(x => x.Food.Price * x.Quantity);
        return View(items);
    }
}
