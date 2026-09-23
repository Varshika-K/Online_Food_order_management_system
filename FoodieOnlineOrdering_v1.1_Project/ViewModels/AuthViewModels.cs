using System.ComponentModel.DataAnnotations;

namespace FoodieOnlineOrdering.ViewModels;

public class RegisterViewModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, StringLength(50)]
    public string Username { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
        ErrorMessage = "Password must contain letters, numbers and a special character, with at least 8 characters.")]
    public string Password { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = "";
}

public class LoginViewModel
{
    [Required]
    public string Username { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public string? ReturnUrl { get; set; }
}
