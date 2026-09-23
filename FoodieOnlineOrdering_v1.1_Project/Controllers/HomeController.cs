using Microsoft.AspNetCore.Mvc;

namespace FoodieOnlineOrdering.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult About() => View();
}
