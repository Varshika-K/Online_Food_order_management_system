using System.ComponentModel.DataAnnotations;

namespace FoodieOnlineOrdering.Models;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    [Required, StringLength(200)]
    public string DeliveryAddress { get; set; } = "";

    [Required, StringLength(30)]
    public string PaymentMethod { get; set; } = "Cash on Delivery";

    [Required, StringLength(30)]
    public string Status { get; set; } = "Pending";

    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<OrderItem> Items { get; set; } = new();
}
