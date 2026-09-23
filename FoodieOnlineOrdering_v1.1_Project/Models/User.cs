using System.ComponentModel.DataAnnotations;

namespace FoodieOnlineOrdering.Models;

public class User
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, StringLength(50)]
    public string Username { get; set; } = "";

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    [Required, StringLength(20)]
    public string Role { get; set; } = "Customer";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
