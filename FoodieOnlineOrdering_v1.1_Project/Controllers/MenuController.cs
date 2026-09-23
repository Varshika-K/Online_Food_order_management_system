using FoodieOnlineOrdering.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodieOnlineOrdering.Controllers;

public class MenuController : Controller
{
    private readonly AppDbContext _db;
    public MenuController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search, string? category)
    {
        var query = _db.Foods.Where(x => x.IsAvailable).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Name.Contains(search) || x.Description.Contains(search));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(x => x.Category == category);

        ViewBag.Search = search;
        ViewBag.Category = category;
        ViewBag.Categories = await _db.Foods.Select(x => x.Category).Distinct().OrderBy(x => x).ToListAsync();

        return View(await query.OrderBy(x => x.Category).ThenBy(x => x.Name).ToListAsync());
    }
}
