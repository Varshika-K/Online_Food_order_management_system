using FoodieOnlineOrdering.Data;
using FoodieOnlineOrdering.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodieOnlineOrdering.Controllers;

public class ContactController : Controller
{
    private readonly AppDbContext _db;

    public ContactController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactMessage contactMessage)
    {
        if (!ModelState.IsValid)
        {
            return View(contactMessage);
        }

        contactMessage.CreatedAt = DateTime.UtcNow;

        _db.ContactMessages.Add(contactMessage);

        await _db.SaveChangesAsync();

        TempData["Success"] =
            "Thank you for contacting us! Your message has been sent successfully.";

        return RedirectToAction(nameof(Index));
    }
}