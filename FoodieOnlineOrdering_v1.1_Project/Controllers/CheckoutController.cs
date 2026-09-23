using System.Security.Cryptography;
using System.Text.Json;
using FoodieOnlineOrdering.Data;
using FoodieOnlineOrdering.Models;
using FoodieOnlineOrdering.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodieOnlineOrdering.Controllers;

public class CheckoutController : Controller
{
    private readonly AppDbContext _db;
    private const string CartKey = "Cart";

    public CheckoutController(AppDbContext db) => _db = db;

    private Dictionary<int, int> GetCart()
    {
        var json = HttpContext.Session.GetString(CartKey);
        return string.IsNullOrWhiteSpace(json)
            ? new Dictionary<int, int>()
            : JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new();
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToAction("Login", "Account", new { returnUrl = "/Checkout" });

        var cart = GetCart();
        if (cart.Count == 0)
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Menu");
        }

        var foods = await _db.Foods.Where(x => cart.Keys.Contains(x.Id)).ToListAsync();
        ViewBag.Total = foods.Sum(f => f.Price * cart[f.Id]);
        return View(new CheckoutViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");

        var cart = GetCart();
        if (cart.Count == 0)
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Menu");
        }

        if (model.PaymentMethod == "Online Payment")
        {
            if (string.IsNullOrWhiteSpace(model.CardHolderName) ||
                string.IsNullOrWhiteSpace(model.CardNumber) ||
                string.IsNullOrWhiteSpace(model.Expiry) ||
                string.IsNullOrWhiteSpace(model.Cvv))
            {
                ModelState.AddModelError("", "Please complete the demo online payment fields.");
            }
            else if (!model.CardNumber.All(char.IsDigit) || model.CardNumber.Length != 16)
            {
                ModelState.AddModelError("", "Card number must contain exactly 16 digits for the demo payment.");
            }
            else if (!model.Cvv.All(char.IsDigit) || model.Cvv.Length != 3)
            {
                ModelState.AddModelError("", "CVV must contain exactly 3 digits for the demo payment.");
            }
        }

        if (!ModelState.IsValid)
        {
            var invalidFoods = await _db.Foods.Where(x => cart.Keys.Contains(x.Id) && x.IsAvailable).ToListAsync();
            ViewBag.Total = invalidFoods.Sum(f => f.Price * cart[f.Id]);
            return View("Index", model);
        }

        var foods = await _db.Foods.Where(x => cart.Keys.Contains(x.Id) && x.IsAvailable).ToListAsync();
        if (foods.Count != cart.Count)
        {
            TempData["Error"] = "One or more selected foods are no longer available. Please review your cart.";
            return RedirectToAction("Index", "Cart");
        }

        var order = new Order
        {
            UserId = userId.Value,
            DeliveryAddress = model.DeliveryAddress.Trim(),
            PaymentMethod = model.PaymentMethod,
            Status = "Pending"
        };

        foreach (var food in foods)
        {
            var qty = cart[food.Id];
            if (qty < 1) continue;

            order.Items.Add(new OrderItem
            {
                FoodId = food.Id,
                FoodName = food.Name,
                Quantity = qty,
                UnitPrice = food.Price,
                LineTotal = food.Price * qty
            });
        }

        if (order.Items.Count == 0)
        {
            TempData["Error"] = "Your cart contains no valid food items.";
            return RedirectToAction("Index", "Cart");
        }

        order.TotalAmount = order.Items.Sum(x => x.LineTotal);
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        var payment = new Payment
        {
            OrderId = order.Id,
            Method = model.PaymentMethod,
            Status = model.PaymentMethod == "Online Payment" ? "Paid" : "Pending",
            TransactionReference = model.PaymentMethod == "Online Payment"
                ? $"DEMO-{RandomNumberGenerator.GetInt32(100000, 999999)}"
                : $"COD-{order.Id:000000}",
            Amount = order.TotalAmount,
            CardLast4 = model.PaymentMethod == "Online Payment" ? model.CardNumber![^4..] : null
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        HttpContext.Session.Remove(CartKey);
        TempData["Success"] = model.PaymentMethod == "Online Payment"
            ? $"Payment successful. Order #{order.Id} placed successfully!"
            : $"Order #{order.Id} placed successfully!";

        return RedirectToAction("Invoice", "Orders", new { id = order.Id });
    }
}
