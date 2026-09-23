using FoodieOnlineOrdering.Data;
using FoodieOnlineOrdering.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodieOnlineOrdering.Controllers;

public class ContactController : Controller
{
    private readonly AppDbContext _db;
    public ContactController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactMessage model)
    {
        if (!ModelState.IsValid) return View(model);

        _db.ContactMessages.Add(model);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Thank you! Your message has been sent successfully.";
        return RedirectToAction(nameof(Index));
    }
}
