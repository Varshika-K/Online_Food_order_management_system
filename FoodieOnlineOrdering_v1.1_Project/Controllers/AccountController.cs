using FoodieOnlineOrdering.Data;
using FoodieOnlineOrdering.Models;
using FoodieOnlineOrdering.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodieOnlineOrdering.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public AccountController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
        => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == model.Username);
        if (user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.Name);
        HttpContext.Session.SetString("Role", user.Role);

        TempData["Success"] = $"Welcome back, {user.Name}!";

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction(user.Role == "Admin" ? "Dashboard" : "Index",
            user.Role == "Admin" ? "Admin" : "Home");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _db.Users.AnyAsync(x => x.Username == model.Username))
            ModelState.AddModelError(nameof(model.Username), "Username already exists.");

        if (await _db.Users.AnyAsync(x => x.Email == model.Email))
            ModelState.AddModelError(nameof(model.Email), "Email already exists.");

        if (!ModelState.IsValid) return View(model);

        var user = new User
        {
            Name = model.Name,
            Username = model.Username,
            Email = model.Email,
            Role = "Customer"
        };
        user.PasswordHash = _hasher.HashPassword(user, model.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Registration successful. Please login.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Profile()
    {
        var id = HttpContext.Session.GetInt32("UserId");
        if (id == null) return RedirectToAction(nameof(Login), new { returnUrl = "/Account/Profile" });

        var user = await _db.Users.FindAsync(id.Value);
        return user == null ? NotFound() : View(user);
    }
}
