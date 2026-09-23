using System.ComponentModel.DataAnnotations;

namespace FoodieOnlineOrdering.ViewModels;

public class CheckoutViewModel
{
    [Required, StringLength(200)]
    public string DeliveryAddress { get; set; } = "";

    [Required]
    public string PaymentMethod { get; set; } = "Cash on Delivery";

    // Demo online payment fields. Full card numbers are never stored.
    public string? CardHolderName { get; set; }
    public string? CardNumber { get; set; }
    public string? Expiry { get; set; }
    public string? Cvv { get; set; }
}
