using System.Linq;

namespace EventManagementSystem.DTO;

public class CartPageViewModel
{
    public List<CartItemDto> Items { get; set; } = [];
    public int TotalQuantity => Items.Sum(item => item.Quantity);
    public decimal TotalAmount => Items.Sum(item => item.Subtotal);
}
