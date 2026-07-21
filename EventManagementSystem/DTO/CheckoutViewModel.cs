using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.DTO;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } =
        string.Empty;

    public List<CartItemDto> Items { get; set; } =
        [];

    public int TotalQuantity =>
        Items.Sum(item => item.Quantity);

    public decimal TotalAmount =>
        Items.Sum(item => item.Subtotal);
}