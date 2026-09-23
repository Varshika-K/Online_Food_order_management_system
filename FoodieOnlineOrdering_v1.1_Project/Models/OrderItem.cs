using System.ComponentModel.DataAnnotations;

namespace FoodieOnlineOrdering.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public int FoodId { get; set; }
    public Food? Food { get; set; }

    [Required, StringLength(120)]
    public string FoodName { get; set; } = "";

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
