using FoodieOnlineOrdering.Models;

namespace FoodieOnlineOrdering.ViewModels;

public class AdminDashboardViewModel
{
    public int UserCount { get; set; }
    public int FoodCount { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
    public List<Order> RecentOrders { get; set; } = new();
}
