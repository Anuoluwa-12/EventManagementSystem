namespace EventManagementSystem.DTO;
public class CartItemDto
{
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string? EventImageUrl { get; set; }
    public DateTime EventDate { get; set; }
    public decimal UnitPrice { get; set; }
    public int AvailableSeats { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
}
