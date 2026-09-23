using System.ComponentModel.DataAnnotations;

namespace FoodieOnlineOrdering.Models;

public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    [Required, StringLength(30)]
    public string Method { get; set; } = "Cash on Delivery";

    [Required, StringLength(30)]
    public string Status { get; set; } = "Pending";

    [Required, StringLength(50)]
    public string TransactionReference { get; set; } = "";

    public decimal Amount { get; set; }

    [StringLength(4)]
    public string? CardLast4 { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
