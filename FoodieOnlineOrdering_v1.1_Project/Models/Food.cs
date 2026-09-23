using System.ComponentModel.DataAnnotations;

namespace FoodieOnlineOrdering.Models;

public class Food
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [Required, StringLength(60)]
    public string Category { get; set; } = "";

    [Required, StringLength(500)]
    public string Description { get; set; } = "";

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    [Required, StringLength(500)]
    public string ImageUrl { get; set; } = "";

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
